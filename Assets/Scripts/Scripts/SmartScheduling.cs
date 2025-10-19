using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class SmartScheduling : MonoBehaviour
{
    public static SmartScheduling Instance { get; private set; }
    
    [Header("Scheduling Settings")]
    public float optimalReviewTime = 0.8f; // 80% of forgetting curve
    public float reviewPriorityThreshold = 0.7f;
    public int maxReviewsPerDay = 20;
    public float sessionSpacing = 2f; // Hours between sessions
    
    [System.Serializable]
    public class ReviewSchedule
    {
        public QuestionData question;
        public DateTime optimalReviewTime;
        public float priority;
        public string reason;
        public float forgettingProbability;
        public TimeSpan timeSinceLastReview;
        
        public ReviewSchedule(QuestionData question, DateTime optimalTime, float priority, string reason)
        {
            this.question = question;
            this.optimalReviewTime = optimalTime;
            this.priority = priority;
            this.reason = reason;
            this.forgettingProbability = 0f;
            this.timeSinceLastReview = TimeSpan.Zero;
        }
    }
    
    [System.Serializable]
    public class SessionPlan
    {
        public DateTime scheduledTime;
        public List<ReviewSchedule> reviews;
        public string module;
        public int estimatedDuration; // minutes
        public float expectedAccuracy;
        public string focusArea;
        
        public SessionPlan(DateTime scheduledTime, string module)
        {
            this.scheduledTime = scheduledTime;
            this.reviews = new List<ReviewSchedule>();
            this.module = module;
            this.estimatedDuration = 15;
            this.expectedAccuracy = 0f;
            this.focusArea = "";
        }
    }
    
    private List<ReviewSchedule> reviewSchedule;
    private List<SessionPlan> sessionPlans;
    private UserProgress userProgress;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSmartScheduling();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeSmartScheduling()
    {
        reviewSchedule = new List<ReviewSchedule>();
        sessionPlans = new List<SessionPlan>();
    }
    
    public List<ReviewSchedule> GetOptimalReviews(string module = null, int maxCount = 10)
    {
        if (SM2Algorithm.Instance == null) return new List<ReviewSchedule>();
        
        userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return new List<ReviewSchedule>();
        
        // Generate review schedule
        GenerateReviewSchedule(module);
        
        // Filter and prioritize reviews
        var optimalReviews = reviewSchedule
            .Where(r => r.priority >= reviewPriorityThreshold)
            .OrderByDescending(r => r.priority)
            .Take(maxCount)
            .ToList();
        
        return optimalReviews;
    }
    
    private void GenerateReviewSchedule(string module = null)
    {
        reviewSchedule.Clear();
        
        var questions = userProgress.questions;
        if (module != null)
        {
            questions = questions.Where(q => q.module == module).ToList();
        }
        
        foreach (var question in questions)
        {
            var review = CalculateOptimalReview(question);
            if (review != null)
            {
                reviewSchedule.Add(review);
            }
        }
    }
    
    private ReviewSchedule CalculateOptimalReview(QuestionData question)
    {
        DateTime now = DateTime.Now;
        TimeSpan timeSinceLastReview = now - question.lastReviewed;
        
        // Calculate forgetting probability using Ebbinghaus forgetting curve
        float forgettingProbability = CalculateForgettingProbability(question, timeSinceLastReview);
        
        // Calculate priority based on forgetting probability and question importance
        float priority = CalculateReviewPriority(question, forgettingProbability, timeSinceLastReview);
        
        // Determine optimal review time
        DateTime optimalTime = CalculateOptimalReviewTime(question, forgettingProbability);
        
        // Generate reason for review
        string reason = GenerateReviewReason(question, forgettingProbability, timeSinceLastReview);
        
        var review = new ReviewSchedule(question, optimalTime, priority, reason);
        review.forgettingProbability = forgettingProbability;
        review.timeSinceLastReview = timeSinceLastReview;
        
        return review;
    }
    
    private float CalculateForgettingProbability(QuestionData question, TimeSpan timeSinceLastReview)
    {
        // Ebbinghaus forgetting curve: R = e^(-t/S)
        // Where R is retention, t is time, S is strength of memory
        
        float memoryStrength = CalculateMemoryStrength(question);
        float timeInHours = (float)timeSinceLastReview.TotalHours;
        
        // Adjust for question difficulty and mastery
        float difficultyFactor = 1f + (question.difficulty * 0.2f);
        float masteryFactor = 1f - (question.mastery / 100f * 0.5f);
        
        float adjustedStrength = memoryStrength * difficultyFactor * masteryFactor;
        
        // Calculate forgetting probability (1 - retention)
        float retention = Mathf.Exp(-timeInHours / adjustedStrength);
        float forgettingProbability = 1f - retention;
        
        return Mathf.Clamp01(forgettingProbability);
    }
    
    private float CalculateMemoryStrength(QuestionData question)
    {
        // Memory strength based on repetitions and ease factor
        float baseStrength = 24f; // 24 hours base strength
        
        // Increase strength with repetitions
        float repetitionBonus = question.repetitions * 12f;
        
        // Increase strength with ease factor
        float easeBonus = (question.easeFactor - 1.3f) * 20f;
        
        return baseStrength + repetitionBonus + easeBonus;
    }
    
    private float CalculateReviewPriority(QuestionData question, float forgettingProbability, TimeSpan timeSinceLastReview)
    {
        float priority = 0f;
        
        // Base priority on forgetting probability
        priority += forgettingProbability * 0.4f;
        
        // Priority for overdue reviews
        if (question.nextReview < DateTime.Now)
        {
            TimeSpan overdue = DateTime.Now - question.nextReview;
            priority += Mathf.Min(0.3f, (float)overdue.TotalDays * 0.1f);
        }
        
        // Priority for weak questions
        if (question.mastery < 50f)
        {
            priority += 0.2f;
        }
        
        // Priority for questions with low repetitions
        if (question.repetitions < 3)
        {
            priority += 0.1f;
        }
        
        // Priority for questions with poor recent performance
        if (question.qualityHistory.Count > 0)
        {
            float recentQuality = (float)question.qualityHistory.TakeLast(3).Average();
            if (recentQuality < 3f)
            {
                priority += 0.1f;
            }
        }
        
        return Mathf.Clamp01(priority);
    }
    
    private DateTime CalculateOptimalReviewTime(QuestionData question, float forgettingProbability)
    {
        // Optimal review time is when forgetting probability reaches optimal threshold
        float memoryStrength = CalculateMemoryStrength(question);
        
        // Calculate time when forgetting probability reaches optimal threshold
        float optimalTimeInHours = -memoryStrength * Mathf.Log(1f - optimalReviewTime);
        
        return question.lastReviewed.AddHours(optimalTimeInHours);
    }
    
    private string GenerateReviewReason(QuestionData question, float forgettingProbability, TimeSpan timeSinceLastReview)
    {
        if (forgettingProbability > 0.8f)
        {
            return "High risk of forgetting";
        }
        else if (forgettingProbability > 0.6f)
        {
            return "Moderate risk of forgetting";
        }
        else if (question.nextReview < DateTime.Now)
        {
            return "Overdue for review";
        }
        else if (question.mastery < 50f)
        {
            return "Low mastery level";
        }
        else if (question.repetitions < 3)
        {
            return "Needs more practice";
        }
        else
        {
            return "Regular review";
        }
    }
    
    public List<SessionPlan> GenerateSessionPlans(int daysAhead = 7)
    {
        sessionPlans.Clear();
        
        DateTime startDate = DateTime.Now.Date;
        
        for (int day = 0; day < daysAhead; day++)
        {
            DateTime sessionDate = startDate.AddDays(day);
            
            // Generate morning session
            var morningSession = new SessionPlan(sessionDate.AddHours(9), "Mixed");
            GenerateSessionContent(morningSession);
            if (morningSession.reviews.Count > 0)
            {
                sessionPlans.Add(morningSession);
            }
            
            // Generate evening session
            var eveningSession = new SessionPlan(sessionDate.AddHours(18), "Mixed");
            GenerateSessionContent(eveningSession);
            if (eveningSession.reviews.Count > 0)
            {
                sessionPlans.Add(eveningSession);
            }
        }
        
        return sessionPlans;
    }
    
    private void GenerateSessionContent(SessionPlan session)
    {
        // Get optimal reviews for the session
        var reviews = GetOptimalReviews(null, maxReviewsPerDay / 2);
        
        // Filter reviews that are optimal for this session time
        var sessionReviews = reviews.Where(r => 
            Math.Abs((r.optimalReviewTime - session.scheduledTime).TotalHours) < 2f
        ).ToList();
        
        session.reviews = sessionReviews;
        
        // Calculate session metrics
        if (sessionReviews.Count > 0)
        {
            session.estimatedDuration = Mathf.Max(10, sessionReviews.Count * 2);
            session.expectedAccuracy = CalculateExpectedAccuracy(sessionReviews);
            session.focusArea = DetermineFocusArea(sessionReviews);
        }
    }
    
    private float CalculateExpectedAccuracy(List<ReviewSchedule> reviews)
    {
        if (reviews.Count == 0) return 0f;
        
        float totalExpectedAccuracy = 0f;
        
        foreach (var review in reviews)
        {
            // Expected accuracy based on question mastery and forgetting probability
            float baseAccuracy = review.question.mastery / 100f;
            float forgettingPenalty = review.forgettingProbability * 0.3f;
            
            totalExpectedAccuracy += Mathf.Clamp01(baseAccuracy - forgettingPenalty);
        }
        
        return totalExpectedAccuracy / reviews.Count;
    }
    
    private string DetermineFocusArea(List<ReviewSchedule> reviews)
    {
        if (reviews.Count == 0) return "General Review";
        
        // Group by module
        var moduleGroups = reviews.GroupBy(r => r.question.module).ToList();
        
        if (moduleGroups.Count == 1)
        {
            return moduleGroups[0].Key;
        }
        
        // Find most common reason
        var reasonGroups = reviews.GroupBy(r => r.reason).ToList();
        var mostCommonReason = reasonGroups.OrderByDescending(g => g.Count()).First().Key;
        
        return mostCommonReason;
    }
    
    public DateTime GetNextOptimalSessionTime()
    {
        if (sessionPlans.Count == 0)
        {
            GenerateSessionPlans();
        }
        
        var nextSession = sessionPlans
            .Where(s => s.scheduledTime > DateTime.Now)
            .OrderBy(s => s.scheduledTime)
            .FirstOrDefault();
        
        if (nextSession != null)
        {
            return nextSession.scheduledTime;
        }
        
        // Default to next day at 9 AM
        return DateTime.Now.Date.AddDays(1).AddHours(9);
    }
    
    public List<ReviewSchedule> GetReviewsForSession(DateTime sessionTime, int maxCount = 10)
    {
        var reviews = GetOptimalReviews(null, maxCount * 2);
        
        // Filter reviews optimal for this session time
        var sessionReviews = reviews.Where(r => 
            Math.Abs((r.optimalReviewTime - sessionTime).TotalHours) < 2f
        ).Take(maxCount).ToList();
        
        return sessionReviews;
    }
    
    public string GetSchedulingInsights()
    {
        if (reviewSchedule.Count == 0)
        {
            GenerateReviewSchedule();
        }
        
        var insights = new List<string>();
        
        // High priority reviews
        var highPriorityReviews = reviewSchedule.Where(r => r.priority > 0.8f).ToList();
        if (highPriorityReviews.Count > 0)
        {
            insights.Add($"{highPriorityReviews.Count} questions need immediate review");
        }
        
        // Overdue reviews
        var overdueReviews = reviewSchedule.Where(r => r.question.nextReview < DateTime.Now).ToList();
        if (overdueReviews.Count > 0)
        {
            insights.Add($"{overdueReviews.Count} questions are overdue for review");
        }
        
        // Weak areas
        var weakReviews = reviewSchedule.Where(r => r.question.mastery < 50f).ToList();
        if (weakReviews.Count > 0)
        {
            insights.Add($"{weakReviews.Count} questions need more practice");
        }
        
        if (insights.Count == 0)
        {
            insights.Add("All questions are on track for optimal learning");
        }
        
        return string.Join("\n", insights);
    }
    
    // Public getters
    public List<ReviewSchedule> GetReviewSchedule()
    {
        return reviewSchedule;
    }
    
    public List<SessionPlan> GetSessionPlans()
    {
        return sessionPlans;
    }
    
    public ReviewSchedule GetReviewSchedule(QuestionData question)
    {
        return reviewSchedule.FirstOrDefault(r => r.question.questionId == question.questionId);
    }
    
    public float GetForgettingProbability(QuestionData question)
    {
        var review = GetReviewSchedule(question);
        return review?.forgettingProbability ?? 0f;
    }
    
    public float GetReviewPriority(QuestionData question)
    {
        var review = GetReviewSchedule(question);
        return review?.priority ?? 0f;
    }
}
