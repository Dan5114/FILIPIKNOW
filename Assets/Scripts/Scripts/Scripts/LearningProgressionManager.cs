using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TopicProgress
{
    public string topicName;
    public DifficultyLevel currentLevel;
    public bool isEasyCompleted;
    public bool isMediumCompleted;
    public bool isHardCompleted;
    public DateTime lastPlayedDate;
    public float masteryScore; // 0-1 scale
    public List<QuestionProgress> questionHistory;
    
    public TopicProgress(string topic)
    {
        topicName = topic;
        currentLevel = DifficultyLevel.Easy;
        isEasyCompleted = false;
        isMediumCompleted = false;
        isHardCompleted = false;
        lastPlayedDate = DateTime.Now;
        masteryScore = 0f;
        questionHistory = new List<QuestionProgress>();
    }
}

[System.Serializable]
public class QuestionProgress
{
    public int questionId;
    public DifficultyLevel difficulty;
    public bool isCorrect;
    public float responseTime;
    public int attempts;
    public DateTime timestamp;
    public int intervalDays; // SM2 algorithm interval
    public float easeFactor; // SM2 algorithm ease factor
    public int repetitions; // Number of times reviewed
    
    public QuestionProgress(int id, DifficultyLevel diff, bool correct, float time, int att)
    {
        questionId = id;
        difficulty = diff;
        isCorrect = correct;
        responseTime = time;
        attempts = att;
        timestamp = DateTime.Now;
        intervalDays = 1;
        easeFactor = 2.5f;
        repetitions = 0;
    }
}

public enum DifficultyLevel
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class LearningProgressionManager : MonoBehaviour
{
    public static LearningProgressionManager Instance { get; private set; }
    
    [Header("Learning Configuration")]
    public int questionsPerLevel = 10;
    public float masteryThreshold = 0.8f; // 80% accuracy to advance
    
    [Header("SM2 Algorithm Settings")]
    public float initialEaseFactor = 2.5f;
    public int easyInterval = 1; // days
    public int mediumInterval = 3; // days
    public int hardInterval = 7; // days
    
    // Progress tracking
    private Dictionary<string, TopicProgress> topicProgress;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeProgressTracking();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeProgressTracking()
    {
        topicProgress = new Dictionary<string, TopicProgress>();
        
        // Initialize all topics
        string[] topics = { "Pangngalan", "Pand'iwa", "Pang-uri", "Panghalip", 
                           "Pang-abay", "Pang-ukol", "Pangatnig", "Pang-angkop" };
        
        foreach (string topic in topics)
        {
            topicProgress[topic] = new TopicProgress(topic);
        }
        
        LoadProgress();
    }
    
    // Topic Management
    public TopicProgress GetTopicProgress(string topicName)
    {
        if (topicProgress.ContainsKey(topicName))
        {
            return topicProgress[topicName];
        }
        
        TopicProgress newProgress = new TopicProgress(topicName);
        topicProgress[topicName] = newProgress;
        return newProgress;
    }
    
    public DifficultyLevel GetCurrentLevel(string topicName)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        return progress.currentLevel;
    }
    
    public bool CanAccessLevel(string topicName, DifficultyLevel level)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        
        switch (level)
        {
            case DifficultyLevel.Easy:
                return true; // Always accessible
            case DifficultyLevel.Medium:
                return progress.isEasyCompleted;
            case DifficultyLevel.Hard:
                return progress.isMediumCompleted;
            default:
                return false;
        }
    }
    
    // Progress Updates
    public void UpdateTopicProgress(string topicName, DifficultyLevel level, bool completed, float accuracy)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        progress.lastPlayedDate = DateTime.Now;
        progress.masteryScore = Mathf.Lerp(progress.masteryScore, accuracy, 0.3f); // Smooth averaging
        
        switch (level)
        {
            case DifficultyLevel.Easy:
                if (completed && accuracy >= masteryThreshold)
                {
                    progress.isEasyCompleted = true;
                    progress.currentLevel = DifficultyLevel.Medium;
                    Debug.Log($"✅ {topicName} Easy level completed! Unlocked Medium level.");
                }
                break;
                
            case DifficultyLevel.Medium:
                if (completed && accuracy >= masteryThreshold)
                {
                    progress.isMediumCompleted = true;
                    progress.currentLevel = DifficultyLevel.Hard;
                    Debug.Log($"✅ {topicName} Medium level completed! Unlocked Hard level.");
                }
                break;
                
            case DifficultyLevel.Hard:
                if (completed && accuracy >= masteryThreshold)
                {
                    progress.isHardCompleted = true;
                    Debug.Log($"🎉 {topicName} Hard level completed! Topic mastered!");
                }
                break;
        }
        
        SaveProgress();
    }
    
    public void RecordQuestionAnswer(string topicName, int questionId, DifficultyLevel difficulty, 
                                   bool isCorrect, float responseTime, int attempts)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        QuestionProgress questionProgress = new QuestionProgress(questionId, difficulty, isCorrect, responseTime, attempts);
        
        // Apply SM2 algorithm
        ApplySM2Algorithm(questionProgress);
        
        progress.questionHistory.Add(questionProgress);
        SaveProgress();
    }
    
    // SM2 Algorithm Implementation
    void ApplySM2Algorithm(QuestionProgress qp)
    {
        if (qp.isCorrect)
        {
            if (qp.repetitions == 0)
            {
                qp.intervalDays = 1;
            }
            else if (qp.repetitions == 1)
            {
                qp.intervalDays = 6;
            }
            else
            {
                qp.intervalDays = Mathf.RoundToInt(qp.intervalDays * qp.easeFactor);
            }
            
            qp.repetitions++;
            qp.easeFactor = Mathf.Max(1.3f, qp.easeFactor + (0.1f - (5f - 3f) * (0.08f + (5f - 3f) * 0.02f)));
        }
        else
        {
            qp.repetitions = 0;
            qp.intervalDays = 1;
            qp.easeFactor = Mathf.Max(1.3f, qp.easeFactor - 0.2f);
        }
    }
    
    public List<int> GetQuestionsForReview(string topicName, DifficultyLevel level)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        List<int> reviewQuestions = new List<int>();
        
        DateTime now = DateTime.Now;
        
        foreach (QuestionProgress qp in progress.questionHistory)
        {
            if (qp.difficulty == level)
            {
                DateTime reviewDate = qp.timestamp.AddDays(qp.intervalDays);
                if (now >= reviewDate)
                {
                    reviewQuestions.Add(qp.questionId);
                }
            }
        }
        
        return reviewQuestions;
    }
    
    // Data Persistence
    void SaveProgress()
    {
        string json = JsonUtility.ToJson(new SerializableDictionary<string, TopicProgress>(topicProgress));
        PlayerPrefs.SetString("LearningProgress", json);
        PlayerPrefs.Save();
        Debug.Log("💾 Learning progress saved.");
    }
    
    void LoadProgress()
    {
        if (PlayerPrefs.HasKey("LearningProgress"))
        {
            string json = PlayerPrefs.GetString("LearningProgress");
            SerializableDictionary<string, TopicProgress> data = JsonUtility.FromJson<SerializableDictionary<string, TopicProgress>>(json);
            topicProgress = data.ToDictionary();
            Debug.Log("📂 Learning progress loaded.");
        }
    }
    
    // Utility Methods
    public bool IsTopicMastered(string topicName)
    {
        TopicProgress progress = GetTopicProgress(topicName);
        return progress.isEasyCompleted && progress.isMediumCompleted && progress.isHardCompleted;
    }
    
    public float GetOverallProgress()
    {
        int totalTopics = topicProgress.Count;
        int masteredTopics = 0;
        
        foreach (var progress in topicProgress.Values)
        {
            if (IsTopicMastered(progress.topicName))
            {
                masteredTopics++;
            }
        }
        
        return (float)masteredTopics / totalTopics;
    }
    
    public Dictionary<string, DifficultyLevel> GetAllCurrentLevels()
    {
        Dictionary<string, DifficultyLevel> levels = new Dictionary<string, DifficultyLevel>();
        
        foreach (var kvp in topicProgress)
        {
            levels[kvp.Key] = kvp.Value.currentLevel;
        }
        
        return levels;
    }
}

// Helper class for JSON serialization
[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [System.Serializable]
    public class KeyValuePair
    {
        public TKey key;
        public TValue value;
    }
    
    public List<KeyValuePair> items = new List<KeyValuePair>();
    
    public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            items.Add(new KeyValuePair { key = kvp.Key, value = kvp.Value });
        }
    }
    
    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        
        foreach (var item in items)
        {
            dictionary[item.key] = item.value;
        }
        
        return dictionary;
    }
}
