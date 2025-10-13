using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GamificationSystem : MonoBehaviour
{
    public static GamificationSystem Instance { get; private set; }
    
    [Header("Gamification Settings")]
    public int baseXP = 10;
    public int streakBonusXP = 5;
    public int speedBonusXP = 3;
    public int difficultyBonusXP = 2;
    public int levelUpXP = 100;
    
    [Header("Achievement Settings")]
    public int streakAchievementThreshold = 5;
    public int levelAchievementThreshold = 5;
    public float accuracyAchievementThreshold = 90f;
    
    // Events
    public System.Action<int> OnLevelUp;
    public System.Action<string> OnAchievementUnlocked;
    public System.Action<int> OnXPChanged;
    public System.Action<int> OnStreakChanged;
    
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public string iconName;
        public bool isUnlocked;
        public DateTime unlockedDate;
        public int xpReward;
        
        public Achievement(string id, string name, string description, string iconName, int xpReward)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.iconName = iconName;
            this.isUnlocked = false;
            this.xpReward = xpReward;
        }
    }
    
    [System.Serializable]
    public class Badge
    {
        public string id;
        public string name;
        public string description;
        public string iconName;
        public bool isEarned;
        public DateTime earnedDate;
        public int requiredValue;
        public string category; // streak, accuracy, level, etc.
        
        public Badge(string id, string name, string description, string iconName, int requiredValue, string category)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.iconName = iconName;
            this.isEarned = false;
            this.requiredValue = requiredValue;
            this.category = category;
        }
    }
    
    private List<Achievement> achievements;
    private List<Badge> badges;
    private UserProgress userProgress;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGamification();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeGamification()
    {
        InitializeAchievements();
        InitializeBadges();
        LoadProgress();
    }
    
    void InitializeAchievements()
    {
        achievements = new List<Achievement>
        {
            // Streak Achievements
            new Achievement("streak_5", "Hot Streak", "Get 5 correct answers in a row", "streak_5", 50),
            new Achievement("streak_10", "On Fire", "Get 10 correct answers in a row", "streak_10", 100),
            new Achievement("streak_20", "Unstoppable", "Get 20 correct answers in a row", "streak_20", 200),
            new Achievement("streak_50", "Legendary", "Get 50 correct answers in a row", "streak_50", 500),
            
            // Level Achievements
            new Achievement("level_5", "Rising Star", "Reach level 5", "level_5", 100),
            new Achievement("level_10", "Expert Learner", "Reach level 10", "level_10", 250),
            new Achievement("level_20", "Master Student", "Reach level 20", "level_20", 500),
            new Achievement("level_50", "Learning Legend", "Reach level 50", "level_50", 1000),
            
            // Accuracy Achievements
            new Achievement("accuracy_80", "Sharp Shooter", "Achieve 80% accuracy", "accuracy_80", 150),
            new Achievement("accuracy_90", "Precision Master", "Achieve 90% accuracy", "accuracy_90", 300),
            new Achievement("accuracy_95", "Perfectionist", "Achieve 95% accuracy", "accuracy_95", 500),
            
            // Speed Achievements
            new Achievement("speed_demon", "Speed Demon", "Answer 10 questions in under 2 seconds each", "speed_demon", 200),
            new Achievement("lightning_fast", "Lightning Fast", "Answer 20 questions in under 1.5 seconds each", "lightning_fast", 400),
            
            // Dedication Achievements
            new Achievement("daily_learner", "Daily Learner", "Study for 7 consecutive days", "daily_learner", 300),
            new Achievement("weekend_warrior", "Weekend Warrior", "Study for 4 consecutive weekends", "weekend_warrior", 400),
            new Achievement("monthly_marathon", "Monthly Marathon", "Study for 30 consecutive days", "monthly_marathon", 1000),
            
            // Module Mastery Achievements
            new Achievement("nouns_master", "Nouns Master", "Master the Nouns module", "nouns_master", 200),
            new Achievement("verbs_master", "Verbs Master", "Master the Verbs module", "verbs_master", 200),
            new Achievement("numbers_master", "Numbers Master", "Master the Numbers module", "numbers_master", 200),
            new Achievement("all_modules", "Complete Master", "Master all modules", "all_modules", 500),
            
            // Special Achievements
            new Achievement("first_correct", "First Steps", "Answer your first question correctly", "first_correct", 25),
            new Achievement("hundred_questions", "Century Club", "Answer 100 questions", "hundred_questions", 200),
            new Achievement("thousand_questions", "Millennium Club", "Answer 1000 questions", "thousand_questions", 1000),
            new Achievement("perfect_session", "Perfect Session", "Get 100% accuracy in a session", "perfect_session", 150),
            new Achievement("comeback_kid", "Comeback Kid", "Break a losing streak with 5 correct answers", "comeback_kid", 100)
        };
    }
    
    void InitializeBadges()
    {
        badges = new List<Badge>
        {
            // Streak Badges
            new Badge("streak_bronze", "Bronze Streaker", "Achieve a 5-question streak", "badge_bronze", 5, "streak"),
            new Badge("streak_silver", "Silver Streaker", "Achieve a 10-question streak", "badge_silver", 10, "streak"),
            new Badge("streak_gold", "Gold Streaker", "Achieve a 20-question streak", "badge_gold", 20, "streak"),
            new Badge("streak_platinum", "Platinum Streaker", "Achieve a 50-question streak", "badge_platinum", 50, "streak"),
            
            // Level Badges
            new Badge("level_bronze", "Bronze Leveler", "Reach level 5", "badge_bronze", 5, "level"),
            new Badge("level_silver", "Silver Leveler", "Reach level 10", "badge_silver", 10, "level"),
            new Badge("level_gold", "Gold Leveler", "Reach level 20", "badge_gold", 20, "level"),
            new Badge("level_platinum", "Platinum Leveler", "Reach level 50", "badge_platinum", 50, "level"),
            
            // Accuracy Badges
            new Badge("accuracy_bronze", "Bronze Accuracy", "Achieve 80% accuracy", "badge_bronze", 80, "accuracy"),
            new Badge("accuracy_silver", "Silver Accuracy", "Achieve 90% accuracy", "badge_silver", 90, "accuracy"),
            new Badge("accuracy_gold", "Gold Accuracy", "Achieve 95% accuracy", "badge_gold", 95, "accuracy"),
            new Badge("accuracy_platinum", "Platinum Accuracy", "Achieve 98% accuracy", "badge_platinum", 98, "accuracy"),
            
            // Dedication Badges
            new Badge("dedication_bronze", "Bronze Dedication", "Study for 7 days", "badge_bronze", 7, "dedication"),
            new Badge("dedication_silver", "Silver Dedication", "Study for 30 days", "badge_silver", 30, "dedication"),
            new Badge("dedication_gold", "Gold Dedication", "Study for 100 days", "badge_gold", 100, "dedication"),
            new Badge("dedication_platinum", "Platinum Dedication", "Study for 365 days", "badge_platinum", 365, "dedication")
        };
    }
    
    void LoadProgress()
    {
        // Load achievement progress from PlayerPrefs
        foreach (var achievement in achievements)
        {
            string key = $"achievement_{achievement.id}";
            achievement.isUnlocked = PlayerPrefs.GetInt(key, 0) == 1;
            
            if (achievement.isUnlocked)
            {
                string dateKey = $"achievement_date_{achievement.id}";
                string dateString = PlayerPrefs.GetString(dateKey, "");
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    achievement.unlockedDate = date;
                }
            }
        }
        
        // Load badge progress from PlayerPrefs
        foreach (var badge in badges)
        {
            string key = $"badge_{badge.id}";
            badge.isEarned = PlayerPrefs.GetInt(key, 0) == 1;
            
            if (badge.isEarned)
            {
                string dateKey = $"badge_date_{badge.id}";
                string dateString = PlayerPrefs.GetString(dateKey, "");
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    badge.earnedDate = date;
                }
            }
        }
    }
    
    void SaveProgress()
    {
        // Save achievement progress
        foreach (var achievement in achievements)
        {
            string key = $"achievement_{achievement.id}";
            PlayerPrefs.SetInt(key, achievement.isUnlocked ? 1 : 0);
            
            if (achievement.isUnlocked)
            {
                string dateKey = $"achievement_date_{achievement.id}";
                PlayerPrefs.SetString(dateKey, achievement.unlockedDate.ToString());
            }
        }
        
        // Save badge progress
        foreach (var badge in badges)
        {
            string key = $"badge_{badge.id}";
            PlayerPrefs.SetInt(key, badge.isEarned ? 1 : 0);
            
            if (badge.isEarned)
            {
                string dateKey = $"badge_date_{badge.id}";
                PlayerPrefs.SetString(dateKey, badge.earnedDate.ToString());
            }
        }
        
        PlayerPrefs.Save();
    }
    
    public void AwardXP(int amount, string reason = "")
    {
        if (SM2Algorithm.Instance == null) return;
        
        userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        userProgress.experience += amount;
        
        // Check for level up
        int newLevel = CalculateLevel(userProgress.experience);
        if (newLevel > userProgress.level)
        {
            userProgress.level = newLevel;
            OnLevelUp?.Invoke(newLevel);
            
            // Unlock level achievements
            CheckLevelAchievements(newLevel);
        }
        
        OnXPChanged?.Invoke(amount);
        
        Debug.Log($"Awarded {amount} XP for {reason}. Total XP: {userProgress.experience}");
    }
    
    public void UpdateStreak(int newStreak)
    {
        if (SM2Algorithm.Instance == null) return;
        
        userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        userProgress.currentStreak = newStreak;
        
        if (newStreak > userProgress.longestStreak)
        {
            userProgress.longestStreak = newStreak;
        }
        
        OnStreakChanged?.Invoke(newStreak);
        
        // Check for streak achievements
        CheckStreakAchievements(newStreak);
    }
    
    public void CheckAchievements()
    {
        if (SM2Algorithm.Instance == null) return;
        
        userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // Check all achievement types
        CheckLevelAchievements(userProgress.level);
        CheckStreakAchievements(userProgress.currentStreak);
        CheckAccuracyAchievements();
        CheckSpeedAchievements();
        CheckDedicationAchievements();
        CheckModuleMasteryAchievements();
        CheckSpecialAchievements();
        
        // Check badges
        CheckBadges();
    }
    
    private void CheckLevelAchievements(int level)
    {
        var levelAchievements = achievements.Where(a => a.id.StartsWith("level_") && !a.isUnlocked).ToList();
        
        foreach (var achievement in levelAchievements)
        {
            int requiredLevel = int.Parse(achievement.id.Split('_')[1]);
            if (level >= requiredLevel)
            {
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void CheckStreakAchievements(int streak)
    {
        var streakAchievements = achievements.Where(a => a.id.StartsWith("streak_") && !a.isUnlocked).ToList();
        
        foreach (var achievement in streakAchievements)
        {
            int requiredStreak = int.Parse(achievement.id.Split('_')[1]);
            if (streak >= requiredStreak)
            {
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void CheckAccuracyAchievements()
    {
        float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();
        
        var accuracyAchievements = achievements.Where(a => a.id.StartsWith("accuracy_") && !a.isUnlocked).ToList();
        
        foreach (var achievement in accuracyAchievements)
        {
            int requiredAccuracy = int.Parse(achievement.id.Split('_')[1]);
            if (accuracy >= requiredAccuracy)
            {
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void CheckSpeedAchievements()
    {
        // Check if user has answered questions quickly
        var recentSessions = userProgress.sessionHistory
            .OrderByDescending(s => s.sessionDate)
            .Take(5).ToList();
        
        if (recentSessions.Count >= 2)
        {
            var fastSessions = recentSessions.Where(s => s.averageResponseTime < 2f).ToList();
            if (fastSessions.Count >= 2)
            {
                var achievement = achievements.FirstOrDefault(a => a.id == "speed_demon" && !a.isUnlocked);
                if (achievement != null)
                {
                    UnlockAchievement(achievement);
                }
            }
        }
    }
    
    private void CheckDedicationAchievements()
    {
        // Check for daily learning streaks
        var sessions = userProgress.sessionHistory.OrderBy(s => s.sessionDate).ToList();
        
        if (sessions.Count >= 7)
        {
            // Check for 7 consecutive days
            bool has7Days = CheckConsecutiveDays(sessions, 7);
            if (has7Days)
            {
                var achievement = achievements.FirstOrDefault(a => a.id == "daily_learner" && !a.isUnlocked);
                if (achievement != null)
                {
                    UnlockAchievement(achievement);
                }
            }
        }
    }
    
    private void CheckModuleMasteryAchievements()
    {
        foreach (var module in userProgress.moduleMastery.Keys)
        {
            float mastery = userProgress.moduleMastery[module];
            
            if (mastery >= 90f)
            {
                string achievementId = $"{module.ToLower()}_master";
                var achievement = achievements.FirstOrDefault(a => a.id == achievementId && !a.isUnlocked);
                if (achievement != null)
                {
                    UnlockAchievement(achievement);
                }
            }
        }
        
        // Check if all modules are mastered
        if (userProgress.moduleMastery.Values.All(m => m >= 90f))
        {
            var achievement = achievements.FirstOrDefault(a => a.id == "all_modules" && !a.isUnlocked);
            if (achievement != null)
            {
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void CheckSpecialAchievements()
    {
        // First correct answer
        if (userProgress.totalCorrectAnswers >= 1)
        {
            var achievement = achievements.FirstOrDefault(a => a.id == "first_correct" && !a.isUnlocked);
            if (achievement != null)
            {
                UnlockAchievement(achievement);
            }
        }
        
        // Hundred questions
        if (userProgress.totalQuestionsAnswered >= 100)
        {
            var achievement = achievements.FirstOrDefault(a => a.id == "hundred_questions" && !a.isUnlocked);
            if (achievement != null)
            {
                UnlockAchievement(achievement);
            }
        }
        
        // Thousand questions
        if (userProgress.totalQuestionsAnswered >= 1000)
        {
            var achievement = achievements.FirstOrDefault(a => a.id == "thousand_questions" && !a.isUnlocked);
            if (achievement != null)
            {
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void CheckBadges()
    {
        // Check streak badges
        CheckBadgeCategory("streak", userProgress.currentStreak);
        
        // Check level badges
        CheckBadgeCategory("level", userProgress.level);
        
        // Check accuracy badges
        float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();
        CheckBadgeCategory("accuracy", Mathf.RoundToInt(accuracy));
        
        // Check dedication badges
        int daysStudied = (DateTime.Now - userProgress.firstSession).Days;
        CheckBadgeCategory("dedication", daysStudied);
    }
    
    private void CheckBadgeCategory(string category, int value)
    {
        var categoryBadges = badges.Where(b => b.category == category && !b.isEarned).ToList();
        
        foreach (var badge in categoryBadges)
        {
            if (value >= badge.requiredValue)
            {
                EarnBadge(badge);
            }
        }
    }
    
    private bool CheckConsecutiveDays(List<SessionData> sessions, int requiredDays)
    {
        if (sessions.Count < requiredDays) return false;
        
        DateTime currentDate = DateTime.Now.Date;
        int consecutiveDays = 0;
        
        for (int i = 0; i < requiredDays; i++)
        {
            DateTime checkDate = currentDate.AddDays(-i);
            if (sessions.Any(s => s.sessionDate.Date == checkDate))
            {
                consecutiveDays++;
            }
            else
            {
                break;
            }
        }
        
        return consecutiveDays >= requiredDays;
    }
    
    private void UnlockAchievement(Achievement achievement)
    {
        achievement.isUnlocked = true;
        achievement.unlockedDate = DateTime.Now;
        
        // Award XP
        AwardXP(achievement.xpReward, $"Achievement: {achievement.name}");
        
        // Notify UI
        OnAchievementUnlocked?.Invoke(achievement.id);
        
        Debug.Log($"Achievement Unlocked: {achievement.name} - {achievement.description}");
        
        SaveProgress();
    }
    
    private void EarnBadge(Badge badge)
    {
        badge.isEarned = true;
        badge.earnedDate = DateTime.Now;
        
        Debug.Log($"Badge Earned: {badge.name} - {badge.description}");
        
        SaveProgress();
    }
    
    private int CalculateLevel(int experience)
    {
        // Level formula: level = sqrt(experience / 100) + 1
        return Mathf.FloorToInt(Mathf.Sqrt(experience / 100f)) + 1;
    }
    
    // Public getters
    public List<Achievement> GetAchievements()
    {
        return achievements;
    }
    
    public List<Badge> GetBadges()
    {
        return badges;
    }
    
    public List<Achievement> GetUnlockedAchievements()
    {
        return achievements.Where(a => a.isUnlocked).ToList();
    }
    
    public List<Badge> GetEarnedBadges()
    {
        return badges.Where(b => b.isEarned).ToList();
    }
    
    public Achievement GetAchievement(string id)
    {
        return achievements.FirstOrDefault(a => a.id == id);
    }
    
    public Badge GetBadge(string id)
    {
        return badges.FirstOrDefault(b => b.id == id);
    }
    
    public int GetTotalXP()
    {
        if (userProgress == null) return 0;
        return userProgress.experience;
    }
    
    public int GetCurrentLevel()
    {
        if (userProgress == null) return 1;
        return userProgress.level;
    }
    
    public int GetCurrentStreak()
    {
        if (userProgress == null) return 0;
        return userProgress.currentStreak;
    }
    
    public int GetLongestStreak()
    {
        if (userProgress == null) return 0;
        return userProgress.longestStreak;
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





