using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ProgressDashboard : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dashboardPanel;
    public Button openDashboardButton;
    public Button closeDashboardButton;
    
    [Header("Progress Display")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI totalQuestionsText;
    
    [Header("Progress Bars")]
    public Slider xpProgressBar;
    public Slider accuracyProgressBar;
    public Slider streakProgressBar;
    
    [Header("Module Progress")]
    public Transform moduleProgressParent;
    public GameObject moduleProgressPrefab;
    
    [Header("Achievements")]
    public Transform achievementsParent;
    public GameObject achievementPrefab;
    public ScrollRect achievementsScrollRect;
    
    [Header("Analytics")]
    public TextMeshProUGUI learningStyleText;
    public TextMeshProUGUI studyTipText;
    public TextMeshProUGUI weakAreasText;
    public TextMeshProUGUI strongAreasText;
    public TextMeshProUGUI learningVelocityText;
    public TextMeshProUGUI retentionRateText;
    
    [Header("Charts")]
    public Transform chartParent;
    public GameObject chartPrefab;
    
    private bool isDashboardOpen = false;
    private List<GameObject> moduleProgressItems = new List<GameObject>();
    private List<GameObject> achievementItems = new List<GameObject>();
    
    void Start()
    {
        InitializeDashboard();
        SetupButtons();
        UpdateDashboard();
    }
    
    void InitializeDashboard()
    {
        // Hide dashboard initially
        if (dashboardPanel != null)
            dashboardPanel.SetActive(false);
        
        // Subscribe to events
        if (GamificationSystem.Instance != null)
        {
            GamificationSystem.Instance.OnLevelUp += OnLevelUp;
            GamificationSystem.Instance.OnAchievementUnlocked += OnAchievementUnlocked;
            GamificationSystem.Instance.OnXPChanged += OnXPChanged;
            GamificationSystem.Instance.OnStreakChanged += OnStreakChanged;
        }
    }
    
    void SetupButtons()
    {
        if (openDashboardButton != null)
            openDashboardButton.onClick.AddListener(OpenDashboard);
        
        if (closeDashboardButton != null)
            closeDashboardButton.onClick.AddListener(CloseDashboard);
    }
    
    public void OpenDashboard()
    {
        isDashboardOpen = true;
        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);
        
        UpdateDashboard();
    }
    
    public void CloseDashboard()
    {
        isDashboardOpen = false;
        if (dashboardPanel != null)
            dashboardPanel.SetActive(false);
    }
    
    public void ToggleDashboard()
    {
        if (isDashboardOpen)
            CloseDashboard();
        else
            OpenDashboard();
    }
    
    public void UpdateDashboard()
    {
        if (!isDashboardOpen) return;
        
        UpdateBasicStats();
        UpdateProgressBars();
        UpdateModuleProgress();
        UpdateAchievements();
        UpdateAnalytics();
        UpdateCharts();
    }
    
    void UpdateBasicStats()
    {
        if (SM2Algorithm.Instance == null) return;
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // Level and XP
        if (levelText != null)
            levelText.text = $"Level {userProgress.level}";
        
        if (xpText != null)
            xpText.text = $"{userProgress.experience} XP";
        
        // Streak
        if (streakText != null)
            streakText.text = $"Streak: {userProgress.currentStreak}";
        
        // Accuracy
        float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();
        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {accuracy:F1}%";
        
        // Total Questions
        if (totalQuestionsText != null)
            totalQuestionsText.text = $"Questions: {userProgress.totalQuestionsAnswered}";
    }
    
    void UpdateProgressBars()
    {
        if (SM2Algorithm.Instance == null) return;
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // XP Progress Bar
        if (xpProgressBar != null)
        {
            int currentLevelXP = (userProgress.level - 1) * 100;
            int nextLevelXP = userProgress.level * 100;
            int xpInCurrentLevel = userProgress.experience - currentLevelXP;
            int xpNeededForNextLevel = nextLevelXP - currentLevelXP;
            
            xpProgressBar.value = (float)xpInCurrentLevel / xpNeededForNextLevel;
        }
        
        // Accuracy Progress Bar
        if (accuracyProgressBar != null)
        {
            float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();
            accuracyProgressBar.value = accuracy / 100f;
        }
        
        // Streak Progress Bar
        if (streakProgressBar != null)
        {
            float streakProgress = Mathf.Clamp((float)userProgress.currentStreak / 10f, 0f, 1f);
            streakProgressBar.value = streakProgress;
        }
    }
    
    void UpdateModuleProgress()
    {
        if (SM2Algorithm.Instance == null || moduleProgressParent == null) return;
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // Clear existing items
        foreach (var item in moduleProgressItems)
        {
            if (item != null)
                Destroy(item);
        }
        moduleProgressItems.Clear();
        
        // Create module progress items
        foreach (var module in userProgress.moduleMastery.Keys)
        {
            if (moduleProgressPrefab != null)
            {
                GameObject moduleItem = Instantiate(moduleProgressPrefab, moduleProgressParent);
                moduleProgressItems.Add(moduleItem);
                
                // Set module data
                var moduleNameText = moduleItem.GetComponentInChildren<TextMeshProUGUI>();
                if (moduleNameText != null)
                    moduleNameText.text = module;
                
                var progressBar = moduleItem.GetComponentInChildren<Slider>();
                if (progressBar != null)
                {
                    float mastery = userProgress.moduleMastery[module];
                    progressBar.value = mastery / 100f;
                }
                
                var masteryText = moduleItem.GetComponentsInChildren<TextMeshProUGUI>()[1];
                if (masteryText != null)
                {
                    float mastery = userProgress.moduleMastery[module];
                    masteryText.text = $"{mastery:F1}%";
                }
            }
        }
    }
    
    void UpdateAchievements()
    {
        if (GamificationSystem.Instance == null || achievementsParent == null) return;
        
        // Clear existing items
        foreach (var item in achievementItems)
        {
            if (item != null)
                Destroy(item);
        }
        achievementItems.Clear();
        
        // Get unlocked achievements
        var unlockedAchievements = GamificationSystem.Instance.GetUnlockedAchievements();
        
        // Create achievement items
        foreach (var achievement in unlockedAchievements)
        {
            if (achievementPrefab != null)
            {
                GameObject achievementItem = Instantiate(achievementPrefab, achievementsParent);
                achievementItems.Add(achievementItem);
                
                // Set achievement data
                var achievementNameText = achievementItem.GetComponentInChildren<TextMeshProUGUI>();
                if (achievementNameText != null)
                    achievementNameText.text = achievement.name;
                
                var achievementDescText = achievementItem.GetComponentsInChildren<TextMeshProUGUI>()[1];
                if (achievementDescText != null)
                    achievementDescText.text = achievement.description;
                
                // Set achievement icon (if available)
                var achievementIcon = achievementItem.GetComponentInChildren<Image>();
                if (achievementIcon != null)
                {
                    // You can load achievement icons here
                    // achievementIcon.sprite = Resources.Load<Sprite>($"Achievements/{achievement.iconName}");
                }
            }
        }
    }
    
    void UpdateAnalytics()
    {
        if (LearningAnalytics.Instance == null) return;
        
        var insights = LearningAnalytics.Instance.GetLearningInsights();
        if (insights == null) return;
        
        // Learning Style
        if (learningStyleText != null)
            learningStyleText.text = $"Learning Style: {insights.learningStyle}";
        
        // Study Tip
        if (studyTipText != null)
            studyTipText.text = $"Tip: {LearningAnalytics.Instance.GetPersonalizedStudyTip()}";
        
        // Weak Areas
        if (weakAreasText != null)
        {
            if (insights.weakAreas.Count > 0)
                weakAreasText.text = $"Weak Areas: {string.Join(", ", insights.weakAreas)}";
            else
                weakAreasText.text = "No weak areas identified!";
        }
        
        // Strong Areas
        if (strongAreasText != null)
        {
            if (insights.strongAreas.Count > 0)
                strongAreasText.text = $"Strong Areas: {string.Join(", ", insights.strongAreas)}";
            else
                strongAreasText.text = "Keep practicing to identify strengths!";
        }
        
        // Learning Velocity
        if (learningVelocityText != null)
            learningVelocityText.text = $"Learning Velocity: {insights.learningVelocity:F1} questions/day";
        
        // Retention Rate
        if (retentionRateText != null)
            retentionRateText.text = $"Retention Rate: {insights.retentionRate:P1}";
    }
    
    void UpdateCharts()
    {
        if (SM2Algorithm.Instance == null || chartParent == null) return;
        
        var userProgress = SM2Algorithm.Instance.GetUserProgress();
        if (userProgress == null) return;
        
        // Create simple text-based charts for now
        // You can enhance this with actual chart libraries later
        
        // Accuracy over time chart
        var accuracyChart = CreateSimpleChart("Accuracy Over Time", userProgress.sessionHistory.Select(s => s.accuracy).ToList());
        
        // Questions per session chart
        var questionsChart = CreateSimpleChart("Questions Per Session", userProgress.sessionHistory.Select(s => (float)s.questionsAnswered).ToList());
    }
    
    GameObject CreateSimpleChart(string title, List<float> data)
    {
        if (chartPrefab == null || data.Count == 0) return null;
        
        GameObject chart = Instantiate(chartPrefab, chartParent);
        
        var titleText = chart.GetComponentInChildren<TextMeshProUGUI>();
        if (titleText != null)
            titleText.text = title;
        
        // Create simple bar chart representation
        var chartData = chart.GetComponentsInChildren<TextMeshProUGUI>()[1];
        if (chartData != null)
        {
            string chartString = "";
            float maxValue = data.Max();
            
            for (int i = 0; i < Mathf.Min(data.Count, 10); i++) // Show last 10 data points
            {
                int barLength = Mathf.RoundToInt((data[i] / maxValue) * 20);
                chartString += $"{i + 1}: {new string('█', barLength)} {data[i]:F1}\n";
            }
            
            chartData.text = chartString;
        }
        
        return chart;
    }
    
    // Event handlers
    void OnLevelUp(int newLevel)
    {
        Debug.Log($"Level Up! New Level: {newLevel}");
        UpdateDashboard();
    }
    
    void OnAchievementUnlocked(string achievementId)
    {
        Debug.Log($"Achievement Unlocked: {achievementId}");
        UpdateDashboard();
    }
    
    void OnXPChanged(int xpGained)
    {
        Debug.Log($"XP Gained: {xpGained}");
        UpdateDashboard();
    }
    
    void OnStreakChanged(int newStreak)
    {
        Debug.Log($"Streak Updated: {newStreak}");
        UpdateDashboard();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GamificationSystem.Instance != null)
        {
            GamificationSystem.Instance.OnLevelUp -= OnLevelUp;
            GamificationSystem.Instance.OnAchievementUnlocked -= OnAchievementUnlocked;
            GamificationSystem.Instance.OnXPChanged -= OnXPChanged;
            GamificationSystem.Instance.OnStreakChanged -= OnStreakChanged;
        }
    }
    
    // Public methods for external access
    public void RefreshDashboard()
    {
        UpdateDashboard();
    }
    
    public bool IsDashboardOpen()
    {
        return isDashboardOpen;
    }
    
    public void SetDashboardVisibility(bool visible)
    {
        if (visible)
            OpenDashboard();
        else
            CloseDashboard();
    }
}





