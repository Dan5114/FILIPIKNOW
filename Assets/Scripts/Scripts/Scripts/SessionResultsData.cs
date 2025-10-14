using UnityEngine;
using System;
using System.Collections.Generic;

namespace Filipknow.Data
{
    [System.Serializable]
    public class SessionResultsData
    {
        [Header("Session Info")]
        public string topicName;
        public string difficulty;
        public DateTime sessionStartTime;
        public DateTime sessionEndTime;
        public float sessionDuration;
        
        [Header("Performance Metrics")]
        public int totalQuestions;
        public int correctAnswers;
        public int incorrectAnswers;
        public float accuracy;
        public int score;
        public int maxPossibleScore;
        
        [Header("Learning Analytics")]
        public float averageResponseTime;
        public List<string> masteredQuestions;
        public List<string> strugglingQuestions;
        public List<string> newQuestionsLearned;
        
        [Header("Gamification")]
        public int xpGained;
        public int levelUpOccurred;
        public List<string> achievementsUnlocked;
        public List<string> badgesEarned;
        public int streakBonus;
        
        [Header("SM2 Algorithm Results")]
        public int questionsReviewed;
        public int questionsMastered;
        public float averageEF;
        public int nextReviewDays;
        
        [Header("Learning Style Insights")]
        public string dominantLearningStyle;
        public List<string> studyTips;
        public string performanceTrend;
        
        [Header("Progression")]
        public bool difficultyCompleted;
        public bool readyForNextDifficulty;
        public string nextRecommendedDifficulty;
        public bool topicMastered;
        
        // Constructor
        public SessionResultsData()
        {
            sessionStartTime = DateTime.Now;
            masteredQuestions = new List<string>();
            strugglingQuestions = new List<string>();
            newQuestionsLearned = new List<string>();
            achievementsUnlocked = new List<string>();
            badgesEarned = new List<string>();
            studyTips = new List<string>();
        }
        
        // Calculate session duration
        public void EndSession()
        {
            sessionEndTime = DateTime.Now;
            sessionDuration = (float)(sessionEndTime - sessionStartTime).TotalSeconds;
        }
        
        // Calculate accuracy percentage
        public void CalculateAccuracy()
        {
            if (totalQuestions > 0)
            {
                accuracy = (float)correctAnswers / totalQuestions * 100f;
            }
            else
            {
                accuracy = 0f;
            }
        }
        
        // Determine if ready for next difficulty
        public void EvaluateProgression()
        {
            // Mark difficulty as completed if accuracy >= 80%
            difficultyCompleted = accuracy >= 80f;
            
            // Ready for next difficulty if completed current difficulty
            readyForNextDifficulty = difficultyCompleted;
            
            // Determine next recommended difficulty
            if (readyForNextDifficulty)
            {
                switch (difficulty.ToLower())
                {
                    case "easy":
                        nextRecommendedDifficulty = "Medium";
                        break;
                    case "medium":
                        nextRecommendedDifficulty = "Hard";
                        break;
                    case "hard":
                        nextRecommendedDifficulty = "Master";
                        topicMastered = true;
                        break;
                    default:
                        nextRecommendedDifficulty = "Easy";
                        break;
                }
            }
            else
            {
                nextRecommendedDifficulty = difficulty; // Stay on current difficulty
            }
        }
        
        // Generate session summary text
        public string GenerateSessionSummary()
        {
            string summary = $"🎯 <b>Session Complete!</b>\n\n";
            
            // Performance section
            summary += $"📊 <b>Performance Summary:</b>\n";
            summary += $"• Questions Answered: {totalQuestions}\n";
            summary += $"• Correct Answers: {correctAnswers}/{totalQuestions}\n";
            summary += $"• Accuracy: {accuracy:F1}%\n";
            summary += $"• Score: {score}/{maxPossibleScore}\n";
            summary += $"• Time: {FormatTime(sessionDuration)}\n\n";
            
            // Learning progress
            summary += $"📚 <b>Learning Progress:</b>\n";
            summary += $"• Questions Mastered: {questionsMastered}\n";
            summary += $"• New Questions Learned: {newQuestionsLearned.Count}\n";
            summary += $"• Average Response Time: {averageResponseTime:F1}s\n\n";
            
            // Gamification
            if (xpGained > 0 || achievementsUnlocked.Count > 0 || badgesEarned.Count > 0)
            {
                summary += $"🎮 <b>Rewards Earned:</b>\n";
                if (xpGained > 0)
                {
                    summary += $"• XP Gained: +{xpGained}\n";
                }
                if (levelUpOccurred > 0)
                {
                    summary += $"• Level Up! (+{levelUpOccurred} levels)\n";
                }
                if (achievementsUnlocked.Count > 0)
                {
                    summary += $"• Achievements: {string.Join(", ", achievementsUnlocked)}\n";
                }
                if (badgesEarned.Count > 0)
                {
                    summary += $"• Badges: {string.Join(", ", badgesEarned)}\n";
                }
                if (streakBonus > 0)
                {
                    summary += $"• Streak Bonus: +{streakBonus} XP\n";
                }
                summary += "\n";
            }
            
            // Progression
            summary += $"🚀 <b>Progression:</b>\n";
            if (difficultyCompleted)
            {
                summary += $"• ✅ {difficulty} Difficulty Completed!\n";
                if (readyForNextDifficulty && nextRecommendedDifficulty != difficulty)
                {
                    summary += $"• 🎉 Ready for {nextRecommendedDifficulty} difficulty!\n";
                }
            }
            else
            {
                summary += $"• 📈 Keep practicing {difficulty} difficulty\n";
                summary += $"• Target: 80% accuracy to advance\n";
            }
            
            // Study tips
            if (studyTips.Count > 0)
            {
                summary += $"\n💡 <b>Study Tips:</b>\n";
                foreach (string tip in studyTips)
                {
                    summary += $"• {tip}\n";
                }
            }
            
            return summary;
        }
        
        // Helper method to format time
        private string FormatTime(float seconds)
        {
            if (seconds < 60)
            {
                return $"{seconds:F1}s";
            }
            else
            {
                int minutes = Mathf.FloorToInt(seconds / 60);
                float remainingSeconds = seconds % 60;
                return $"{minutes}m {remainingSeconds:F0}s";
            }
        }
    }
    
    // Static class for data transfer between scenes
    public static class SessionResultsTransfer
    {
        private static SessionResultsData _currentSessionResults;
        
        public static SessionResultsData CurrentSessionResults
        {
            get { return _currentSessionResults; }
            set { _currentSessionResults = value; }
        }
        
        public static void ClearResults()
        {
            _currentSessionResults = null;
        }
        
        public static bool HasResults()
        {
            return _currentSessionResults != null;
        }
    }
}
