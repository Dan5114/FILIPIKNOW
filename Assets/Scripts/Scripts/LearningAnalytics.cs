using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class LearningAnalytics : MonoBehaviour
{
    public static LearningAnalytics Instance { get; private set; }
    
    [Header("Analytics Settings")]
    public bool enableDetailedLogging = true;
    public float updateInterval = 1f; // Update analytics every second
    
    private UserProgress userProgress;
    private float lastUpdateTime;
    
    // Analytics Data
    [System.Serializable]
    public class LearningInsights
    {
        public string learningStyle; // Visual, Auditory, Kinesthetic
        public float optimalSessionLength; // Best session duration in minutes
        public List<string> studyTips; // Personalized study advice
        public float predictedMastery; // When user will master module
        public List<string> weakAreas; // Areas needing improvement
        public List<string> strongAreas; // User's strengths
        public float learningVelocity; // Questions learned per day
        public float retentionRate; // How well user remembers
        public float learningEfficiency; // Questions learned per hour
        public string recommendedFocus; // What to focus on next
    }
    
    [System.Serializable]
    public class PerformanceMetrics
    {
        public float dailyProgress;
        public float weeklyProgress;
        public float monthlyProgress;
        public float averageResponseTime;
        public float accuracyTrend; // Improving or declining
        public int streakTrend; // Streak getting better or worse
        public float difficultyProgression; // How difficulty increases
        public float timeToMastery; // Estimated time to master module
    }
    
    private LearningInsights currentInsights;
    private PerformanceMetrics currentMetrics;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAnalytics();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAnalytics()
    {
        currentInsights = new LearningInsights();
        currentMetrics = new PerformanceMetrics();
        
        // Initialize study tips
        currentInsights.studyTips = new List<string>();
        currentInsights.weakAreas = new List<string>();
        currentInsights.strongAreas = new List<string>();
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAnalytics();
            lastUpdateTime = Time.time;
        }
    }
    
    public void UpdateAnalytics()
    {
        if (SM2Algorithm.Instance == null) return;
        
        userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // Update performance metrics
        UpdatePerformanceMetrics();
        
        // Update learning insights
        UpdateLearningInsights();
        
        if (enableDetailedLogging)
        {
            LogAnalytics();
        }
    }
    
    private void UpdatePerformanceMetrics()
    {
        // Calculate daily progress
        var todaySessions = userProgress.sessionHistory.Where(s => 
            s.sessionDate.Date == DateTime.Now.Date).ToList();
        currentMetrics.dailyProgress = todaySessions.Sum(s => s.questionsAnswered);
        
        // Calculate weekly progress
        var weekAgo = DateTime.Now.AddDays(-7);
        var weeklySessions = userProgress.sessionHistory.Where(s => 
            s.sessionDate >= weekAgo).ToList();
        currentMetrics.weeklyProgress = weeklySessions.Sum(s => s.questionsAnswered);
        
        // Calculate monthly progress
        var monthAgo = DateTime.Now.AddDays(-30);
        var monthlySessions = userProgress.sessionHistory.Where(s => 
            s.sessionDate >= monthAgo).ToList();
        currentMetrics.monthlyProgress = monthlySessions.Sum(s => s.questionsAnswered);
        
        // Calculate average response time
        if (userProgress.sessionHistory.Count > 0)
        {
            var validSessions = userProgress.sessionHistory.Where(s => s.averageResponseTime > 0);
            if (validSessions.Any())
            {
                currentMetrics.averageResponseTime = validSessions.Average(s => s.averageResponseTime);
            }
            else
            {
                currentMetrics.averageResponseTime = 0f;
            }
        }
        
        // Calculate accuracy trend
        if (userProgress.sessionHistory.Count >= 2)
        {
            var recentSessions = userProgress.sessionHistory
                .OrderByDescending(s => s.sessionDate)
                .Take(5).ToList();
            
            if (recentSessions.Count >= 2)
            {
                var recentSessionsList = recentSessions.Take(3).ToList();
                var olderSessionsList = recentSessions.Skip(3).ToList();
                
                if (recentSessionsList.Any() && olderSessionsList.Any())
                {
                    float recentAccuracy = recentSessionsList.Average(s => s.accuracy);
                    float olderAccuracy = olderSessionsList.Average(s => s.accuracy);
                    currentMetrics.accuracyTrend = recentAccuracy - olderAccuracy;
                }
                else
                {
                    currentMetrics.accuracyTrend = 0f;
                }
            }
        }
        
        // Calculate learning velocity
        currentMetrics.difficultyProgression = CalculateDifficultyProgression();
        currentMetrics.timeToMastery = CalculateTimeToMastery();
    }
    
    private void UpdateLearningInsights()
    {
        // Determine learning style based on response patterns
        currentInsights.learningStyle = DetermineLearningStyle();
        
        // Calculate optimal session length
        currentInsights.optimalSessionLength = CalculateOptimalSessionLength();
        
        // Generate study tips
        GenerateStudyTips();
        
        // Identify weak and strong areas
        IdentifyWeakAndStrongAreas();
        
        // Predict mastery time
        currentInsights.predictedMastery = PredictMasteryTime();
        
        // Recommend focus area
        currentInsights.recommendedFocus = RecommendFocusArea();
        
        // Update learning metrics
        currentInsights.learningVelocity = userProgress.learningVelocity;
        currentInsights.retentionRate = userProgress.retentionRate;
        currentInsights.learningEfficiency = userProgress.learningEfficiency;
    }
    
    private string DetermineLearningStyle()
    {
        // Analyze response patterns to determine learning style
        var recentSessions = userProgress.sessionHistory
            .OrderByDescending(s => s.sessionDate)
            .Take(10).ToList();
        
        if (recentSessions.Count < 3) return "Mixed";
        
        // Calculate average response time
        float avgResponseTime = recentSessions.Average(s => s.averageResponseTime);
        
        // Calculate accuracy consistency
        float accuracyVariance = CalculateVariance(recentSessions.Select(s => s.accuracy).ToList());
        
        if (avgResponseTime < 3f && accuracyVariance < 10f)
            return "Visual"; // Fast and consistent
        else if (avgResponseTime > 5f && accuracyVariance > 20f)
            return "Auditory"; // Slower but improving
        else
            return "Kinesthetic"; // Mixed patterns
    }
    
    private float CalculateOptimalSessionLength()
    {
        // Analyze session data to find optimal length
        var sessions = userProgress.sessionHistory.Where(s => s.questionsAnswered > 0).ToList();
        
        if (sessions.Count < 3) return 15f; // Default 15 minutes
        
        // Find sessions with highest efficiency
        var efficientSessions = sessions
            .Where(s => s.accuracy > 70f)
            .OrderByDescending(s => s.questionsAnswered / Mathf.Max(1f, (float)(DateTime.Now - s.sessionDate).TotalMinutes))
            .Take(5).ToList();
        
        if (efficientSessions.Count > 0)
        {
            return (float)efficientSessions.Average(s => (DateTime.Now - s.sessionDate).TotalMinutes);
        }
        
        return 20f; // Default 20 minutes
    }
    
    private void GenerateStudyTips()
    {
        currentInsights.studyTips.Clear();
        
        // Generate tips based on performance
        if (userProgress.retentionRate < 0.7f)
        {
            currentInsights.studyTips.Add("Practice more frequently to improve retention");
        }
        
        if (currentMetrics.averageResponseTime > 6f)
        {
            currentInsights.studyTips.Add("Try to answer questions more quickly");
        }
        
        if (userProgress.currentStreak < 3)
        {
            currentInsights.studyTips.Add("Focus on accuracy over speed");
        }
        
        if (userProgress.learningVelocity < 5f)
        {
            currentInsights.studyTips.Add("Study for longer sessions to increase learning velocity");
        }
        
        // Add learning style specific tips
        switch (currentInsights.learningStyle)
        {
            case "Visual":
                currentInsights.studyTips.Add("Use visual aids and diagrams to reinforce learning");
                break;
            case "Auditory":
                currentInsights.studyTips.Add("Read questions aloud to improve comprehension");
                break;
            case "Kinesthetic":
                currentInsights.studyTips.Add("Take breaks between questions to process information");
                break;
        }
    }
    
    private void IdentifyWeakAndStrongAreas()
    {
        currentInsights.weakAreas.Clear();
        currentInsights.strongAreas.Clear();
        
        // Analyze module mastery
        foreach (var module in userProgress.moduleMastery.Keys)
        {
            float mastery = userProgress.moduleMastery[module];
            
            if (mastery < 50f)
            {
                currentInsights.weakAreas.Add(module);
            }
            else if (mastery > 80f)
            {
                currentInsights.strongAreas.Add(module);
            }
        }
        
        // Analyze question difficulty
        var difficultQuestions = userProgress.questions
            .Where(q => q.mastery < 30f)
            .GroupBy(q => q.module)
            .OrderByDescending(g => g.Count())
            .Take(2);
        
        foreach (var group in difficultQuestions)
        {
            if (!currentInsights.weakAreas.Contains(group.Key))
            {
                currentInsights.weakAreas.Add(group.Key);
            }
        }
    }
    
    private float PredictMasteryTime()
    {
        // Predict when user will achieve 90% mastery
        if (userProgress.moduleMastery == null || userProgress.moduleMastery.Count == 0)
            return 365f; // Default 1 year if no data
        
        float currentMastery = userProgress.moduleMastery.Values.Average();
        float learningRate = userProgress.learningVelocity * 0.1f; // Questions per day * mastery per question
        
        if (learningRate <= 0) return 365f; // Default 1 year
        
        float remainingMastery = 90f - currentMastery;
        return remainingMastery / learningRate;
    }
    
    private string RecommendFocusArea()
    {
        if (currentInsights.weakAreas.Count > 0)
        {
            return $"Focus on {currentInsights.weakAreas[0]} - your weakest area";
        }
        
        if (userProgress.currentStreak < 5)
        {
            return "Build a longer streak by focusing on accuracy";
        }
        
        return "Continue practicing to maintain your progress";
    }
    
    private float CalculateDifficultyProgression()
    {
        // Calculate how difficulty increases over time
        var questions = userProgress.questions.OrderBy(q => q.firstSeen).ToList();
        
        if (questions.Count < 5) return 0f;
        
        var earlyQuestions = questions.Take(questions.Count / 3);
        var recentQuestions = questions.Skip(questions.Count * 2 / 3);
        
        if (!earlyQuestions.Any() || !recentQuestions.Any()) return 0f;
        
        float earlyDifficulty = (float)earlyQuestions.Average(q => q.difficulty);
        float recentDifficulty = (float)recentQuestions.Average(q => q.difficulty);
        
        return recentDifficulty - earlyDifficulty;
    }
    
    private float CalculateTimeToMastery()
    {
        // Estimate time to reach 90% mastery across all modules
        if (userProgress.moduleMastery == null || userProgress.moduleMastery.Count == 0)
            return 180f; // Default 6 months if no data
        
        float averageMastery = userProgress.moduleMastery.Values.Average();
        float learningRate = userProgress.learningVelocity * 0.05f; // Conservative estimate
        
        if (learningRate <= 0) return 180f; // Default 6 months
        
        float remainingMastery = 90f - averageMastery;
        return Mathf.Max(30f, remainingMastery / learningRate); // Minimum 30 days
    }
    
    private float CalculateVariance(List<float> values)
    {
        if (values.Count < 2) return 0f;
        
        float mean = values.Average();
        float variance = values.Sum(v => Mathf.Pow(v - mean, 2)) / values.Count;
        return Mathf.Sqrt(variance);
    }
    
    private void LogAnalytics()
    {
        Debug.Log($"=== LEARNING ANALYTICS ===");
        Debug.Log($"Learning Style: {currentInsights.learningStyle}");
        Debug.Log($"Optimal Session Length: {currentInsights.optimalSessionLength:F1} minutes");
        Debug.Log($"Learning Velocity: {currentInsights.learningVelocity:F2} questions/day");
        Debug.Log($"Retention Rate: {currentInsights.retentionRate:P1}");
        Debug.Log($"Predicted Mastery: {currentInsights.predictedMastery:F1} days");
        Debug.Log($"Recommended Focus: {currentInsights.recommendedFocus}");
        
        if (currentInsights.weakAreas.Count > 0)
        {
            Debug.Log($"Weak Areas: {string.Join(", ", currentInsights.weakAreas)}");
        }
        
        if (currentInsights.strongAreas.Count > 0)
        {
            Debug.Log($"Strong Areas: {string.Join(", ", currentInsights.strongAreas)}");
        }
    }
    
    // Public getters
    public LearningInsights GetLearningInsights()
    {
        return currentInsights;
    }
    
    public PerformanceMetrics GetPerformanceMetrics()
    {
        return currentMetrics;
    }
    
    public string GetPersonalizedStudyTip()
    {
        if (currentInsights.studyTips.Count > 0)
        {
            return currentInsights.studyTips[UnityEngine.Random.Range(0, currentInsights.studyTips.Count)];
        }
        return "Keep practicing to improve your skills!";
    }
    
    public float GetLearningProgress()
    {
        if (userProgress == null || userProgress.moduleMastery == null || userProgress.moduleMastery.Count == 0) 
            return 0f;
        return userProgress.moduleMastery.Values.Average();
    }
    
    public bool IsLearningImproving()
    {
        return currentMetrics.accuracyTrend > 0f;
    }
    
    public string GetLearningStatus()
    {
        float progress = GetLearningProgress();
        
        if (progress >= 90f) return "Expert";
        if (progress >= 70f) return "Advanced";
        if (progress >= 50f) return "Intermediate";
        if (progress >= 30f) return "Beginner";
        return "Novice";
    }
}
