using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class QuestionData
{
    public int questionId;
    public string module;
    public string question;
    public string[] choices;
    public int correctAnswer;
    public string explanation; // Added for compatibility with other scripts
    public int difficulty; // 0-5 scale (0 = easiest, 5 = hardest)
    public int interval; // Days until next review
    public int repetitions; // Number of times reviewed
    public float easeFactor; // Difficulty multiplier (default 2.5)
    public DateTime lastReviewed;
    public DateTime nextReview;
    
    // Advanced SM2 Features
    public int consecutiveCorrectAnswers; // Streak of correct answers
    public int totalAttempts; // Total times attempted
    public float averageResponseTime; // Average time to answer
    public float mastery; // Mastery level (0-100%)
    public int quality; // Last answer quality (0-5)
    public DateTime firstSeen; // When first encountered
    public List<float> responseTimes; // History of response times
    public List<int> qualityHistory; // History of answer quality
    
    // Parameterless constructor for object initializer syntax
    public QuestionData()
    {
        questionId = 0;
        module = "";
        question = "";
        choices = new string[0];
        correctAnswer = 0;
        explanation = "";
        difficulty = 0;
        interval = 1;
        repetitions = 0;
        easeFactor = 2.5f;
        lastReviewed = DateTime.Now;
        nextReview = DateTime.Now.AddDays(1);
        
        // Initialize advanced features
        consecutiveCorrectAnswers = 0;
        totalAttempts = 0;
        averageResponseTime = 0f;
        mastery = 0f;
        quality = 0;
        firstSeen = DateTime.Now;
        responseTimes = new List<float>();
        qualityHistory = new List<int>();
    }
    
    public QuestionData(int id, string mod, string q, string[] c, int correct, string exp = "")
    {
        questionId = id;
        module = mod;
        question = q;
        choices = c;
        correctAnswer = correct;
        explanation = exp;
        difficulty = 0;
        interval = 1; // Start with 1 day
        repetitions = 0;
        easeFactor = 2.5f;
        lastReviewed = DateTime.Now;
        nextReview = DateTime.Now.AddDays(1);
        
        // Initialize advanced features
        consecutiveCorrectAnswers = 0;
        totalAttempts = 0;
        averageResponseTime = 0f;
        mastery = 0f;
        quality = 0;
        firstSeen = DateTime.Now;
        responseTimes = new List<float>();
        qualityHistory = new List<int>();
    }
}

public class UserProgress
{
    public List<QuestionData> questions = new List<QuestionData>();
    public Dictionary<string, int> moduleScores = new Dictionary<string, int>();
    public int totalQuestionsAnswered;
    public int totalCorrectAnswers;
    public DateTime lastSession;
    
    // Advanced Analytics
    public int currentStreak; // Current correct answer streak
    public int longestStreak; // Best streak ever achieved
    public int level; // User level based on progress
    public int experience; // Total XP earned
    public float learningVelocity; // Questions learned per day
    public float retentionRate; // How well user remembers
    public float learningEfficiency; // Questions learned per hour
    public DateTime firstSession; // When user started learning
    public List<string> achievements; // Unlocked achievements
    public Dictionary<string, float> moduleMastery; // Mastery per module
    public List<SessionData> sessionHistory; // Historical session data
    
    public UserProgress()
    {
        totalQuestionsAnswered = 0;
        totalCorrectAnswers = 0;
        lastSession = DateTime.Now;
        firstSession = DateTime.Now;
        currentStreak = 0;
        longestStreak = 0;
        level = 1;
        experience = 0;
        learningVelocity = 0f;
        retentionRate = 0f;
        learningEfficiency = 0f;
        achievements = new List<string>();
        moduleMastery = new Dictionary<string, float>();
        sessionHistory = new List<SessionData>();
    }
}

public class SessionData
{
    public DateTime sessionDate;
    public string module;
    public int questionsAnswered;
    public int correctAnswers;
    public float accuracy;
    public float averageResponseTime;
    public int experienceGained;
    public List<string> achievementsUnlocked;
    
    public SessionData()
    {
        sessionDate = DateTime.Now;
        questionsAnswered = 0;
        correctAnswers = 0;
        accuracy = 0f;
        averageResponseTime = 0f;
        experienceGained = 0;
        achievementsUnlocked = new List<string>();
    }
}

public class SM2Algorithm : MonoBehaviour
{
    public static SM2Algorithm Instance { get; private set; }
    
    [Header("SM2 Settings")]
    public float initialEaseFactor = 2.5f;
    public int initialInterval = 1;
    public int maxInterval = 365; // Maximum interval in days
    public float minEaseFactor = 1.3f;
    public float maxEaseFactor = 2.5f;
    
    [Header("Gamification Settings")]
    public int xpPerCorrectAnswer = 10;
    public int xpPerStreak = 5;
    public int xpPerLevel = 100;
    
    private UserProgress userProgress;
    private const string PROGRESS_KEY = "UserProgress";
    
    // Events for UI updates
    public System.Action<int> OnLevelUp;
    public System.Action<string> OnAchievementUnlocked;
    public System.Action<int> OnStreakUpdated;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadProgress()
    {
        string json = PlayerPrefs.GetString(PROGRESS_KEY, "");
        if (!string.IsNullOrEmpty(json))
        {
            userProgress = JsonUtility.FromJson<UserProgress>(json);
        }
        else
        {
            userProgress = new UserProgress();
        }
        
        // Initialize module mastery if not set
        if (userProgress.moduleMastery == null)
            userProgress.moduleMastery = new Dictionary<string, float>();
    }
    
    void SaveProgress()
    {
        string json = JsonUtility.ToJson(userProgress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
        PlayerPrefs.Save();
    }
    
    public void AddQuestion(int questionId, string module, string question, string[] choices, int correctAnswer)
    {
        // Check if question already exists
        if (userProgress.questions.Exists(q => q.questionId == questionId))
            return;
            
        QuestionData newQuestion = new QuestionData(questionId, module, question, choices, correctAnswer);
        userProgress.questions.Add(newQuestion);
        SaveProgress();
    }
    
    public List<QuestionData> GetQuestionsForReview(string module = null, int maxQuestions = 5)
    {
        List<QuestionData> reviewQuestions = new List<QuestionData>();
        DateTime now = DateTime.Now;
        
        foreach (QuestionData question in userProgress.questions)
        {
            // If module is specified, only get questions from that module
            if (module != null && question.module != module)
                continue;
                
            // If it's time for review
            if (question.nextReview <= now)
            {
                reviewQuestions.Add(question);
            }
        }
        
        // Sort by priority (earliest next review first, then by difficulty)
        reviewQuestions.Sort((a, b) => {
            int timeComparison = a.nextReview.CompareTo(b.nextReview);
            if (timeComparison != 0) return timeComparison;
            return b.difficulty.CompareTo(a.difficulty); // Harder questions first
        });
        
        // Limit to max questions
        if (reviewQuestions.Count > maxQuestions)
        {
            reviewQuestions = reviewQuestions.GetRange(0, maxQuestions);
        }
        
        return reviewQuestions;
    }
    
    public void ProcessAnswer(QuestionData question, bool isCorrect, float responseTime = 0f)
    {
        userProgress.questions.Add(question);
        
        // Update basic statistics
        userProgress.totalQuestionsAnswered++;
        question.totalAttempts++;
        
        // Calculate quality based on correctness and response time
        int quality = CalculateQuality(isCorrect, responseTime);
        question.quality = quality;
        question.qualityHistory.Add(quality);
        
        // Update response time tracking
        question.responseTimes.Add(responseTime);
        question.averageResponseTime = question.responseTimes.Average();
        
        // Update streak
        if (isCorrect)
        {
            userProgress.totalCorrectAnswers++;
            userProgress.currentStreak++;
            question.consecutiveCorrectAnswers++;
            
            if (userProgress.currentStreak > userProgress.longestStreak)
            {
                userProgress.longestStreak = userProgress.currentStreak;
            }
        }
        else
        {
            userProgress.currentStreak = 0;
            question.consecutiveCorrectAnswers = 0;
        }
        
        // Update module score
        if (!userProgress.moduleScores.ContainsKey(question.module))
            userProgress.moduleScores[question.module] = 0;
        if (isCorrect)
            userProgress.moduleScores[question.module]++;
            
        // Enhanced SM2 Algorithm with quality-based scheduling
        ProcessAnswerWithQuality(question, quality);
        
        // Update mastery
        UpdateQuestionMastery(question);
        UpdateModuleMastery(question.module);
        
        // Award experience and check for level up
        AwardExperience(question, isCorrect, responseTime);
        
        // Check for achievements
        CheckAchievements();
        
        // Update learning analytics
        UpdateLearningAnalytics();
        
        SaveProgress();
    }
    
    private int CalculateQuality(bool isCorrect, float responseTime)
    {
        if (!isCorrect) return 0; // Complete blackout
        
        // Quality based on response time (faster = higher quality)
        if (responseTime < 2f) return 5; // Perfect response
        if (responseTime < 4f) return 4; // Good response
        if (responseTime < 6f) return 3; // Correct response
        return 2; // Correct but slow
    }
    
    private void ProcessAnswerWithQuality(QuestionData question, int quality)
    {
        if (quality < 3)
        {
            // Reset on poor performance
            question.repetitions = 0;
            question.interval = 1;
            question.easeFactor = Mathf.Max(minEaseFactor, question.easeFactor - 0.2f);
        }
        else
        {
            // Good performance - advance
            question.repetitions++;
            
            if (question.repetitions == 1)
            {
                question.interval = 1;
            }
            else if (question.repetitions == 2)
            {
                question.interval = 6;
            }
            else
            {
                question.interval = Mathf.RoundToInt(question.interval * question.easeFactor);
            }
            
            // Adjust ease factor based on quality
            if (quality >= 4)
            {
                question.easeFactor = Mathf.Min(maxEaseFactor, question.easeFactor + 0.1f);
            }
            else if (quality == 3)
            {
                // No change to ease factor
            }
            
            // Cap the interval
            question.interval = Mathf.Min(question.interval, maxInterval);
        }
        
        // Update review dates
        question.lastReviewed = DateTime.Now;
        question.nextReview = DateTime.Now.AddDays(question.interval);
    }
    
    private void UpdateQuestionMastery(QuestionData question)
    {
        // Calculate mastery based on consecutive correct answers and ease factor
        float mastery = Mathf.Min(100f, (question.consecutiveCorrectAnswers * 20f) + (question.easeFactor - 1.3f) * 50f);
        question.mastery = mastery;
    }
    
    private void UpdateModuleMastery(string module)
    {
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        if (moduleQuestions.Count > 0)
        {
            float totalMastery = moduleQuestions.Sum(q => q.mastery);
            float averageMastery = totalMastery / moduleQuestions.Count;
            userProgress.moduleMastery[module] = averageMastery;
        }
    }
    
    private void AwardExperience(QuestionData question, bool isCorrect, float responseTime)
    {
        int xpGained = 0;
        
        if (isCorrect)
        {
            xpGained += xpPerCorrectAnswer;
            
            // Speed bonus
            if (responseTime < 3f)
                xpGained += 5;
            
            // Streak bonus
            if (userProgress.currentStreak > 1)
                xpGained += xpPerStreak * (userProgress.currentStreak - 1);
            
            // Difficulty bonus
            xpGained += Mathf.RoundToInt(question.difficulty * 2);
        }
        
        userProgress.experience += xpGained;
        
        // Check for level up
        int newLevel = CalculateLevel(userProgress.experience);
        if (newLevel > userProgress.level)
        {
            userProgress.level = newLevel;
            OnLevelUp?.Invoke(newLevel);
        }
    }
    
    private int CalculateLevel(int experience)
    {
        // Level formula: level = sqrt(experience / 100) + 1
        return Mathf.FloorToInt(Mathf.Sqrt(experience / 100f)) + 1;
    }
    
    private void CheckAchievements()
    {
        List<string> newAchievements = new List<string>();
        
        // Streak achievements
        if (userProgress.currentStreak >= 5 && !userProgress.achievements.Contains("Streak5"))
        {
            newAchievements.Add("Streak5");
        }
        if (userProgress.currentStreak >= 10 && !userProgress.achievements.Contains("Streak10"))
        {
            newAchievements.Add("Streak10");
        }
        
        // Level achievements
        if (userProgress.level >= 5 && !userProgress.achievements.Contains("Level5"))
        {
            newAchievements.Add("Level5");
        }
        
        // Accuracy achievements
        if (GetOverallAccuracy() >= 90f && !userProgress.achievements.Contains("Accuracy90"))
        {
            newAchievements.Add("Accuracy90");
        }
        
        // Add new achievements
        foreach (string achievement in newAchievements)
        {
            userProgress.achievements.Add(achievement);
            OnAchievementUnlocked?.Invoke(achievement);
        }
    }
    
    private void UpdateLearningAnalytics()
    {
        // Calculate learning velocity (questions per day)
        TimeSpan timeSinceFirst = DateTime.Now - userProgress.firstSession;
        if (timeSinceFirst.TotalDays > 0)
        {
            userProgress.learningVelocity = userProgress.totalQuestionsAnswered / (float)timeSinceFirst.TotalDays;
        }
        
        // Calculate retention rate
        if (userProgress.totalQuestionsAnswered > 0)
        {
            userProgress.retentionRate = (float)userProgress.totalCorrectAnswers / userProgress.totalQuestionsAnswered;
        }
        
        // Calculate learning efficiency (questions per hour)
        TimeSpan totalLearningTime = DateTime.Now - userProgress.firstSession;
        if (totalLearningTime.TotalHours > 0)
        {
            userProgress.learningEfficiency = userProgress.totalQuestionsAnswered / (float)totalLearningTime.TotalHours;
        }
    }
    
    public void StartSession(string module)
    {
        // Create new session data
        SessionData session = new SessionData();
        session.module = module;
        userProgress.sessionHistory.Add(session);
    }
    
    public void EndSession(string module, int questionsAnswered, int correctAnswers, float averageResponseTime)
    {
        // Update last session
        if (userProgress.sessionHistory.Count > 0)
        {
            var lastSession = userProgress.sessionHistory[userProgress.sessionHistory.Count - 1];
            lastSession.questionsAnswered = questionsAnswered;
            lastSession.correctAnswers = correctAnswers;
            lastSession.accuracy = questionsAnswered > 0 ? (float)correctAnswers / questionsAnswered * 100f : 0f;
            lastSession.averageResponseTime = averageResponseTime;
        }
        
        userProgress.lastSession = DateTime.Now;
        SaveProgress();
    }
    
    // Public getters for UI
    public float GetDifficultyRating(QuestionData question)
    {
        return Mathf.Clamp(5f - question.easeFactor, 0f, 5f);
    }
    
    public int GetModuleScore(string module)
    {
        if (userProgress.moduleScores.ContainsKey(module))
            return userProgress.moduleScores[module];
        return 0;
    }
    
    public float GetOverallAccuracy()
    {
        if (userProgress.totalQuestionsAnswered == 0)
            return 0f;
        return (float)userProgress.totalCorrectAnswers / userProgress.totalQuestionsAnswered * 100f;
    }
    
    public List<QuestionData> GetAllQuestions()
    {
        return userProgress.questions;
    }
    
    public UserProgress GetUserProgress()
    {
        return userProgress;
    }
    
    public float GetModuleMastery(string module)
    {
        if (userProgress.moduleMastery.ContainsKey(module))
            return userProgress.moduleMastery[module];
        return 0f;
    }
    
    public List<QuestionData> GetWeakQuestions(string module, int count = 3)
    {
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        return moduleQuestions.OrderBy(q => q.mastery).Take(count).ToList();
    }
    
    public List<QuestionData> GetStrongQuestions(string module, int count = 3)
    {
        var moduleQuestions = userProgress.questions.Where(q => q.module == module).ToList();
        return moduleQuestions.OrderByDescending(q => q.mastery).Take(count).ToList();
    }
    
    public void ResetProgress()
    {
        userProgress = new UserProgress();
        SaveProgress();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveProgress();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveProgress();
    }
}







