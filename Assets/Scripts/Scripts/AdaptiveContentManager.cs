using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AdaptiveContentManager : MonoBehaviour
{
    public static AdaptiveContentManager Instance { get; private set; }
    
    [Header("Adaptive Settings")]
    public float difficultyAdjustmentRate = 0.1f;
    public int minQuestionsPerSession = 3;
    public int maxQuestionsPerSession = 10;
    public float masteryThreshold = 80f;
    
    [System.Serializable]
    public class LearningPath
    {
        public string module;
        public List<QuestionData> questions;
        public float difficulty;
        public float mastery;
        public List<string> weakAreas;
        public List<string> strongAreas;
        public DateTime lastUpdated;
        
        public LearningPath(string module)
        {
            this.module = module;
            this.questions = new List<QuestionData>();
            this.difficulty = 0f;
            this.mastery = 0f;
            this.weakAreas = new List<string>();
            this.strongAreas = new List<string>();
            this.lastUpdated = DateTime.Now;
        }
    }
    
    [System.Serializable]
    public class ContentRecommendation
    {
        public string module;
        public List<QuestionData> recommendedQuestions;
        public string reason;
        public float priority;
        public string learningTip;
        
        public ContentRecommendation(string module, string reason, float priority)
        {
            this.module = module;
            this.recommendedQuestions = new List<QuestionData>();
            this.reason = reason;
            this.priority = priority;
            this.learningTip = "";
        }
    }
    
    private Dictionary<string, LearningPath> learningPaths;
    private List<ContentRecommendation> currentRecommendations;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAdaptiveContent();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAdaptiveContent()
    {
        learningPaths = new Dictionary<string, LearningPath>();
        currentRecommendations = new List<ContentRecommendation>();
        
        // Initialize learning paths for each module
        learningPaths["Nouns"] = new LearningPath("Nouns");
        learningPaths["Verbs"] = new LearningPath("Verbs");
        learningPaths["Numbers"] = new LearningPath("Numbers");
    }
    
    public List<QuestionData> GetPersonalizedQuestions(string module, int count = 5)
    {
        if (SM2Algorithm.Instance == null) return new List<QuestionData>();
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return new List<QuestionData>();
        
        // Get all questions for the module
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        
        if (moduleQuestions.Count == 0) return new List<QuestionData>();
        
        // Update learning path
        UpdateLearningPath(module, moduleQuestions);
        
        // Generate personalized question selection
        var personalizedQuestions = GeneratePersonalizedSelection(module, moduleQuestions, count);
        
        return personalizedQuestions;
    }
    
    private void UpdateLearningPath(string module, List<QuestionData> questions)
    {
        if (!learningPaths.ContainsKey(module))
        {
            learningPaths[module] = new LearningPath(module);
        }
        
        var learningPath = learningPaths[module];
        learningPath.questions = questions;
        
        // Calculate current mastery
        learningPath.mastery = (float)questions.Average(q => q.mastery);
        
        // Calculate average difficulty
        learningPath.difficulty = (float)questions.Average(q => q.difficulty);
        
        // Identify weak and strong areas
        learningPath.weakAreas = IdentifyWeakAreas(questions);
        learningPath.strongAreas = IdentifyStrongAreas(questions);
        
        learningPath.lastUpdated = DateTime.Now;
    }
    
    private List<string> IdentifyWeakAreas(List<QuestionData> questions)
    {
        var weakAreas = new List<string>();
        
        // Find questions with low mastery
        var weakQuestions = questions.Where(q => q.mastery < 50f).ToList();
        
        if (weakQuestions.Count > 0)
        {
            // Group by difficulty level
            var difficultyGroups = weakQuestions.GroupBy(q => q.difficulty).ToList();
            
            foreach (var group in difficultyGroups)
            {
                if (group.Count() > 1) // Multiple weak questions at this difficulty
                {
                    weakAreas.Add($"Difficulty Level {group.Key}");
                }
            }
        }
        
        return weakAreas;
    }
    
    private List<string> IdentifyStrongAreas(List<QuestionData> questions)
    {
        var strongAreas = new List<string>();
        
        // Find questions with high mastery
        var strongQuestions = questions.Where(q => q.mastery >= 80f).ToList();
        
        if (strongQuestions.Count > 0)
        {
            // Group by difficulty level
            var difficultyGroups = strongQuestions.GroupBy(q => q.difficulty).ToList();
            
            foreach (var group in difficultyGroups)
            {
                if (group.Count() > 1) // Multiple strong questions at this difficulty
                {
                    strongAreas.Add($"Difficulty Level {group.Key}");
                }
            }
        }
        
        return strongAreas;
    }
    
    private List<QuestionData> GeneratePersonalizedSelection(string module, List<QuestionData> questions, int count)
    {
        var selectedQuestions = new List<QuestionData>();
        
        // Priority 1: Questions due for review
        var reviewQuestions = questions.Where(q => q.nextReview <= DateTime.Now).ToList();
        selectedQuestions.AddRange(reviewQuestions.Take(count));
        
        // Priority 2: Weak questions (low mastery)
        if (selectedQuestions.Count < count)
        {
            var weakQuestions = questions
                .Where(q => q.mastery < 50f && !selectedQuestions.Contains(q))
                .OrderBy(q => q.mastery)
                .ToList();
            
            int remaining = count - selectedQuestions.Count;
            selectedQuestions.AddRange(weakQuestions.Take(remaining));
        }
        
        // Priority 3: Questions that need more practice
        if (selectedQuestions.Count < count)
        {
            var practiceQuestions = questions
                .Where(q => q.repetitions < 3 && !selectedQuestions.Contains(q))
                .OrderBy(q => q.repetitions)
                .ToList();
            
            int remaining = count - selectedQuestions.Count;
            selectedQuestions.AddRange(practiceQuestions.Take(remaining));
        }
        
        // Priority 4: Fill with random questions if needed
        if (selectedQuestions.Count < count)
        {
            var remainingQuestions = questions
                .Where(q => !selectedQuestions.Contains(q))
                .OrderBy(q => q.nextReview)
                .ToList();
            
            int remaining = count - selectedQuestions.Count;
            selectedQuestions.AddRange(remainingQuestions.Take(remaining));
        }
        
        return selectedQuestions;
    }
    
    public List<ContentRecommendation> GetContentRecommendations()
    {
        currentRecommendations.Clear();
        
        if (SM2Algorithm.Instance == null) return currentRecommendations;
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return currentRecommendations;
        
        // Generate recommendations for each module
        foreach (var module in learningPaths.Keys)
        {
            var learningPath = learningPaths[module];
            
            // Check if module needs attention
            if (learningPath.mastery < masteryThreshold)
            {
                var recommendation = new ContentRecommendation(
                    module,
                    $"Module mastery is {learningPath.mastery:F1}% (below {masteryThreshold}% threshold)",
                    0.8f
                );
                
                // Add specific recommendations
                if (learningPath.weakAreas.Count > 0)
                {
                    recommendation.learningTip = $"Focus on: {string.Join(", ", learningPath.weakAreas)}";
                }
                
                currentRecommendations.Add(recommendation);
            }
            
            // Check for review needs
            var reviewQuestions = userProgress.questions
                .Where(q => q.module == module && q.nextReview <= DateTime.Now)
                .ToList();
            
            if (reviewQuestions.Count > 0)
            {
                var recommendation = new ContentRecommendation(
                    module,
                    $"{reviewQuestions.Count} questions need review",
                    0.9f
                );
                
                recommendation.recommendedQuestions = reviewQuestions;
                recommendation.learningTip = "Review these questions to maintain your progress";
                
                currentRecommendations.Add(recommendation);
            }
        }
        
        // Sort by priority
        currentRecommendations.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        return currentRecommendations;
    }
    
    public string GetPersonalizedStudyTip(string module)
    {
        if (!learningPaths.ContainsKey(module)) return "Keep practicing!";
        
        var learningPath = learningPaths[module];
        
        if (learningPath.mastery < 30f)
        {
            return "Start with easier questions to build confidence";
        }
        else if (learningPath.mastery < 60f)
        {
            return "Focus on accuracy over speed";
        }
        else if (learningPath.mastery < 80f)
        {
            return "Try to answer questions more quickly";
        }
        else
        {
            return "Great job! Try more challenging questions";
        }
    }
    
    public float GetRecommendedSessionLength(string module)
    {
        if (!learningPaths.ContainsKey(module)) return 15f;
        
        var learningPath = learningPaths[module];
        
        // Adjust session length based on mastery
        if (learningPath.mastery < 50f)
        {
            return 20f; // Longer sessions for struggling areas
        }
        else if (learningPath.mastery < 80f)
        {
            return 15f; // Standard sessions
        }
        else
        {
            return 10f; // Shorter sessions for mastered content
        }
    }
    
    public int GetRecommendedQuestionCount(string module)
    {
        if (!learningPaths.ContainsKey(module)) return 5;
        
        var learningPath = learningPaths[module];
        
        // Adjust question count based on mastery
        if (learningPath.mastery < 50f)
        {
            return Mathf.Min(maxQuestionsPerSession, 8); // More questions for practice
        }
        else if (learningPath.mastery < 80f)
        {
            return 5; // Standard count
        }
        else
        {
            return Mathf.Max(minQuestionsPerSession, 3); // Fewer questions for review
        }
    }
    
    public string GetLearningStatus(string module)
    {
        if (!learningPaths.ContainsKey(module)) return "Unknown";
        
        var learningPath = learningPaths[module];
        
        if (learningPath.mastery >= 90f) return "Expert";
        if (learningPath.mastery >= 70f) return "Advanced";
        if (learningPath.mastery >= 50f) return "Intermediate";
        if (learningPath.mastery >= 30f) return "Beginner";
        return "Novice";
    }
    
    public List<QuestionData> GetWeakQuestions(string module, int count = 3)
    {
        if (SM2Algorithm.Instance == null) return new List<QuestionData>();
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return new List<QuestionData>();
        
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        
        return moduleQuestions
            .Where(q => q.mastery < 50f)
            .OrderBy(q => q.mastery)
            .Take(count)
            .ToList();
    }
    
    public List<QuestionData> GetStrongQuestions(string module, int count = 3)
    {
        if (SM2Algorithm.Instance == null) return new List<QuestionData>();
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return new List<QuestionData>();
        
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        
        return moduleQuestions
            .Where(q => q.mastery >= 80f)
            .OrderByDescending(q => q.mastery)
            .Take(count)
            .ToList();
    }
    
    public void UpdateContentDifficulty(string module, bool isCorrect, float responseTime)
    {
        if (!learningPaths.ContainsKey(module)) return;
        
        var learningPath = learningPaths[module];
        
        // Adjust difficulty based on performance
        if (isCorrect && responseTime < 3f)
        {
            // Good performance - increase difficulty
            learningPath.difficulty = Mathf.Min(5f, learningPath.difficulty + difficultyAdjustmentRate);
        }
        else if (!isCorrect || responseTime > 6f)
        {
            // Poor performance - decrease difficulty
            learningPath.difficulty = Mathf.Max(0f, learningPath.difficulty - difficultyAdjustmentRate);
        }
    }
    
    // Public getters
    public Dictionary<string, LearningPath> GetLearningPaths()
    {
        return learningPaths;
    }
    
    public LearningPath GetLearningPath(string module)
    {
        if (learningPaths.ContainsKey(module))
            return learningPaths[module];
        return null;
    }
    
    public List<ContentRecommendation> GetCurrentRecommendations()
    {
        return currentRecommendations;
    }
    
    public float GetModuleMastery(string module)
    {
        if (learningPaths.ContainsKey(module))
            return learningPaths[module].mastery;
        return 0f;
    }
    
    public float GetModuleDifficulty(string module)
    {
        if (learningPaths.ContainsKey(module))
            return learningPaths[module].difficulty;
        return 0f;
    }
}
