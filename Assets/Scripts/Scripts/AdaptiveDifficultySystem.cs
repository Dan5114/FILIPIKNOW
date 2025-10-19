using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdaptiveDifficultySystem : MonoBehaviour
{
    [Header("Adaptive Settings")]
    public float baseDifficulty = 0.5f;
    public float difficultyAdjustmentSpeed = 0.1f;
    public float minDifficulty = 0.1f;
    public float maxDifficulty = 1.0f;
    
    [Header("Performance Thresholds")]
    public float easyThreshold = 0.8f;      // 80% accuracy for easy
    public float moderateThreshold = 0.7f;   // 70% accuracy for moderate
    public float hardThreshold = 0.6f;      // 60% accuracy for hard
    public float expertThreshold = 0.5f;    // 50% accuracy for expert
    
    [Header("Response Time Thresholds")]
    public float fastResponseTime = 2.0f;   // Fast response time
    public float slowResponseTime = 8.0f;   // Slow response time
    
    private Dictionary<int, List<PerformanceData>> questionPerformance = new Dictionary<int, List<PerformanceData>>();
    private Dictionary<string, float> topicDifficulty = new Dictionary<string, float>();
    
    [System.Serializable]
    public class PerformanceData
    {
        public int questionId;
        public bool isCorrect;
        public float responseTime;
        public int attempts;
        public DateTime timestamp;
        public string topic;
        public DifficultyLevel difficultyLevel;
    }
    
    [System.Serializable]
    public class DifficultyRecommendation
    {
        public DifficultyLevel recommendedLevel;
        public float confidence;
        public string reasoning;
        public List<string> suggestedTopics;
        public List<string> weakAreas;
    }
    
    public void RecordPerformance(int questionId, bool isCorrect, float responseTime, int attempts, string topic, DifficultyLevel difficultyLevel)
    {
        PerformanceData data = new PerformanceData
        {
            questionId = questionId,
            isCorrect = isCorrect,
            responseTime = responseTime,
            attempts = attempts,
            timestamp = DateTime.Now,
            topic = topic,
            difficultyLevel = difficultyLevel
        };
        
        if (!questionPerformance.ContainsKey(questionId))
        {
            questionPerformance[questionId] = new List<PerformanceData>();
        }
        
        questionPerformance[questionId].Add(data);
        
        // Update topic difficulty
        UpdateTopicDifficulty(topic, isCorrect, responseTime);
        
        Debug.Log($"Recorded performance: Q{questionId} - Correct: {isCorrect}, Time: {responseTime:F1}s, Topic: {topic}");
    }
    
    void UpdateTopicDifficulty(string topic, bool isCorrect, float responseTime)
    {
        if (!topicDifficulty.ContainsKey(topic))
        {
            topicDifficulty[topic] = baseDifficulty;
        }
        
        float currentDifficulty = topicDifficulty[topic];
        float adjustment = 0f;
        
        // Adjust based on correctness
        if (isCorrect)
        {
            adjustment -= difficultyAdjustmentSpeed * 0.5f; // Make easier
        }
        else
        {
            adjustment += difficultyAdjustmentSpeed; // Make harder
        }
        
        // Adjust based on response time
        if (responseTime < fastResponseTime)
        {
            adjustment -= difficultyAdjustmentSpeed * 0.3f; // Make easier
        }
        else if (responseTime > slowResponseTime)
        {
            adjustment += difficultyAdjustmentSpeed * 0.3f; // Make harder
        }
        
        // Apply adjustment
        float newDifficulty = Mathf.Clamp(currentDifficulty + adjustment, minDifficulty, maxDifficulty);
        topicDifficulty[topic] = newDifficulty;
        
        Debug.Log($"Updated difficulty for {topic}: {currentDifficulty:F2} → {newDifficulty:F2}");
    }
    
    public DifficultyRecommendation GetDifficultyRecommendation(string topic)
    {
        DifficultyRecommendation recommendation = new DifficultyRecommendation();
        
        // Get topic-specific performance
        var topicData = GetTopicPerformance(topic);
        
        if (topicData.Count == 0)
        {
            // No data available, start with easy
            recommendation.recommendedLevel = DifficultyLevel.Easy;
            recommendation.confidence = 0.3f;
            recommendation.reasoning = "No performance data available. Starting with easy level.";
            return recommendation;
        }
        
        // Calculate metrics
        float accuracy = CalculateAccuracy(topicData);
        float averageResponseTime = CalculateAverageResponseTime(topicData);
        float consistency = CalculateConsistency(topicData);
        
        // Determine recommended level based on performance
        if (accuracy >= easyThreshold && averageResponseTime <= fastResponseTime)
        {
            recommendation.recommendedLevel = DifficultyLevel.Hard;
            recommendation.confidence = Mathf.Min(0.9f, accuracy);
            recommendation.reasoning = $"High accuracy ({accuracy:P1}) and fast response time ({averageResponseTime:F1}s) suggest readiness for hard level.";
        }
        else if (accuracy >= moderateThreshold)
        {
            recommendation.recommendedLevel = DifficultyLevel.Medium;
            recommendation.confidence = Mathf.Min(0.8f, accuracy);
            recommendation.reasoning = $"Good accuracy ({accuracy:P1}) suggests moderate level is appropriate.";
        }
        else if (accuracy >= hardThreshold)
        {
            recommendation.recommendedLevel = DifficultyLevel.Easy;
            recommendation.confidence = Mathf.Min(0.7f, accuracy);
            recommendation.reasoning = $"Moderate accuracy ({accuracy:P1}) suggests staying with easy level for now.";
        }
        else
        {
            recommendation.recommendedLevel = DifficultyLevel.Easy;
            recommendation.confidence = 0.5f;
            recommendation.reasoning = $"Low accuracy ({accuracy:P1}) suggests need for more practice at easy level.";
        }
        
        // Identify weak areas
        recommendation.weakAreas = IdentifyWeakAreas(topicData);
        
        // Suggest topics for improvement
        recommendation.suggestedTopics = GetSuggestedTopics(topic);
        
        return recommendation;
    }
    
    List<PerformanceData> GetTopicPerformance(string topic)
    {
        List<PerformanceData> topicData = new List<PerformanceData>();
        
        foreach (var questionData in questionPerformance.Values)
        {
            foreach (var data in questionData)
            {
                if (data.topic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                {
                    topicData.Add(data);
                }
            }
        }
        
        return topicData;
    }
    
    float CalculateAccuracy(List<PerformanceData> data)
    {
        if (data.Count == 0) return 0f;
        
        int correctCount = data.Count(d => d.isCorrect);
        return (float)correctCount / data.Count;
    }
    
    float CalculateAverageResponseTime(List<PerformanceData> data)
    {
        if (data.Count == 0) return 0f;
        
        return data.Average(d => d.responseTime);
    }
    
    float CalculateConsistency(List<PerformanceData> data)
    {
        if (data.Count < 2) return 1f;
        
        // Calculate variance in response times
        float mean = CalculateAverageResponseTime(data);
        float variance = data.Average(d => Mathf.Pow(d.responseTime - mean, 2));
        float standardDeviation = Mathf.Sqrt(variance);
        
        // Lower standard deviation = higher consistency
        return Mathf.Max(0f, 1f - (standardDeviation / mean));
    }
    
    List<string> IdentifyWeakAreas(List<PerformanceData> data)
    {
        List<string> weakAreas = new List<string>();
        
        // Analyze by difficulty level
        var byDifficulty = data.GroupBy(d => d.difficultyLevel);
        
        foreach (var group in byDifficulty)
        {
            float accuracy = CalculateAccuracy(group.ToList());
            if (accuracy < 0.6f)
            {
                weakAreas.Add($"Struggling with {group.Key} level questions");
            }
        }
        
        // Analyze by response time
        float avgResponseTime = CalculateAverageResponseTime(data);
        if (avgResponseTime > slowResponseTime)
        {
            weakAreas.Add("Slow response time - needs more practice");
        }
        
        // Analyze by attempts
        int highAttemptQuestions = data.Count(d => d.attempts > 2);
        if (highAttemptQuestions > data.Count * 0.3f)
        {
            weakAreas.Add("Many questions require multiple attempts");
        }
        
        return weakAreas;
    }
    
    List<string> GetSuggestedTopics(string currentTopic)
    {
        List<string> suggestions = new List<string>();
        
        // Get all topics and their difficulties
        var topicDifficulties = topicDifficulty.ToList();
        topicDifficulties.Sort((a, b) => a.Value.CompareTo(b.Value));
        
        // Suggest easier topics if struggling
        var currentDifficulty = topicDifficulty.ContainsKey(currentTopic) ? topicDifficulty[currentTopic] : baseDifficulty;
        
        foreach (var topic in topicDifficulties)
        {
            if (topic.Value < currentDifficulty && !topic.Key.Equals(currentTopic, StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add(topic.Key);
            }
        }
        
        return suggestions.Take(3).ToList(); // Return top 3 suggestions
    }
    
    public List<EnhancedQuestionData> SelectQuestionsForSession(string topic, int questionCount)
    {
        List<EnhancedQuestionData> selectedQuestions = new List<EnhancedQuestionData>();
        
        // Get difficulty recommendation
        DifficultyRecommendation recommendation = GetDifficultyRecommendation(topic);
        
        // Select questions based on recommendation
        switch (recommendation.recommendedLevel)
        {
            case DifficultyLevel.Easy:
                selectedQuestions.AddRange(GetEasyQuestions(topic, questionCount));
                break;
            case DifficultyLevel.Medium:
                selectedQuestions.AddRange(GetModerateQuestions(topic, questionCount));
                break;
            case DifficultyLevel.Hard:
                selectedQuestions.AddRange(GetHardQuestions(topic, questionCount));
                break;
        }
        
        // If not enough questions, fill with easier ones
        if (selectedQuestions.Count < questionCount)
        {
            int needed = questionCount - selectedQuestions.Count;
            selectedQuestions.AddRange(GetEasyQuestions(topic, needed));
        }
        
        return selectedQuestions;
    }
    
    List<EnhancedQuestionData> GetEasyQuestions(string topic, int count)
    {
        // This would be populated with actual easy questions
        // For now, return empty list - will be populated by the game manager
        return new List<EnhancedQuestionData>();
    }
    
    List<EnhancedQuestionData> GetModerateQuestions(string topic, int count)
    {
        return new List<EnhancedQuestionData>();
    }
    
    List<EnhancedQuestionData> GetHardQuestions(string topic, int count)
    {
        return new List<EnhancedQuestionData>();
    }
    
    List<EnhancedQuestionData> GetExpertQuestions(string topic, int count)
    {
        return new List<EnhancedQuestionData>();
    }
    
    public string GenerateProgressReport()
    {
        string report = "📊 LEARNING PROGRESS REPORT\n\n";
        
        // Overall statistics
        var allData = questionPerformance.Values.SelectMany(x => x).ToList();
        if (allData.Count > 0)
        {
            float overallAccuracy = CalculateAccuracy(allData);
            float overallResponseTime = CalculateAverageResponseTime(allData);
            
            report += $"Overall Accuracy: {overallAccuracy:P1}\n";
            report += $"Average Response Time: {overallResponseTime:F1}s\n";
            report += $"Total Questions Attempted: {allData.Count}\n\n";
        }
        
        // Topic-specific performance
        report += "📚 TOPIC PERFORMANCE:\n";
        foreach (var topic in topicDifficulty.Keys)
        {
            var topicData = GetTopicPerformance(topic);
            if (topicData.Count > 0)
            {
                float accuracy = CalculateAccuracy(topicData);
                float responseTime = CalculateAverageResponseTime(topicData);
                DifficultyRecommendation recommendation = GetDifficultyRecommendation(topic);
                
                report += $"\n{topic.ToUpper()}:\n";
                report += $"  Accuracy: {accuracy:P1}\n";
                report += $"  Response Time: {responseTime:F1}s\n";
                report += $"  Recommended Level: {recommendation.recommendedLevel}\n";
                report += $"  Difficulty Rating: {topicDifficulty[topic]:F2}\n";
            }
        }
        
        return report;
    }
    
    public void ResetProgress()
    {
        questionPerformance.Clear();
        topicDifficulty.Clear();
        Debug.Log("Progress data reset.");
    }
}

