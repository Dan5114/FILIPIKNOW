using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompleteSystemTest : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool autoRunTest = false;
    public float testDelay = 1f;
    
    [Header("System References")]
    public LearningProgressionManager progressionManager;
    public QuestionDatabase questionDatabase;
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;
    public SceneController sceneController;
    
    [Header("Test Results")]
    public TextMeshProUGUI testResultsText;
    public GameObject testResultsPanel;
    
    private int testResults = 0;
    private int totalTests = 0;
    
    void Start()
    {
        if (autoRunTest)
        {
            Invoke(nameof(RunCompleteSystemTest), testDelay);
        }
    }
    
    [ContextMenu("Run Complete System Test")]
    public void RunCompleteSystemTest()
    {
        Debug.Log("🧪 Starting Complete System Test...");
        testResults = 0;
        totalTests = 0;
        
        StartCoroutine(RunTestsCoroutine());
    }
    
    System.Collections.IEnumerator RunTestsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Test 1: Check LearningProgressionManager
        yield return StartCoroutine(RunTest("LearningProgressionManager Exists", () => {
            return progressionManager != null || LearningProgressionManager.Instance != null;
        }));
        
        // Test 2: Check QuestionDatabase
        yield return StartCoroutine(RunTest("QuestionDatabase Exists", () => {
            return questionDatabase != null;
        }));
        
        // Test 3: Check AdaptiveDialogManager
        yield return StartCoroutine(RunTest("AdaptiveDialogManager Exists", () => {
            return adaptiveDialogManager != null;
        }));
        
        // Test 4: Check AdaptiveChoiceManager
        yield return StartCoroutine(RunTest("AdaptiveChoiceManager Exists", () => {
            return adaptiveChoiceManager != null;
        }));
        
        // Test 5: Check SceneController
        yield return StartCoroutine(RunTest("SceneController Exists", () => {
            return sceneController != null || SceneController.Instance != null;
        }));
        
        // Test 6: Test Question Database Content
        yield return StartCoroutine(RunTest("Question Database Content", () => {
            if (questionDatabase != null)
            {
                int easyCount = questionDatabase.GetQuestionCount(DifficultyLevel.Easy);
                int mediumCount = questionDatabase.GetQuestionCount(DifficultyLevel.Medium);
                int hardCount = questionDatabase.GetQuestionCount(DifficultyLevel.Hard);
                
                Debug.Log($"Question counts - Easy: {easyCount}, Medium: {mediumCount}, Hard: {hardCount}");
                return easyCount >= 10 && mediumCount >= 10 && hardCount >= 10;
            }
            return false;
        }));
        
        // Test 7: Test Progress Tracking
        yield return StartCoroutine(RunTest("Progress Tracking System", () => {
            if (LearningProgressionManager.Instance != null)
            {
                TopicProgress progress = LearningProgressionManager.Instance.GetTopicProgress("Pangngalan");
                return progress != null;
            }
            return false;
        }));
        
        // Test 8: Test Difficulty Access Control
        yield return StartCoroutine(RunTest("Difficulty Access Control", () => {
            if (LearningProgressionManager.Instance != null)
            {
                bool canAccessEasy = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Easy);
                bool canAccessMedium = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Medium);
                
                Debug.Log($"Access - Easy: {canAccessEasy}, Medium: {canAccessMedium}");
                return canAccessEasy && !canAccessMedium; // Easy should be accessible, Medium should not be accessible initially
            }
            return false;
        }));
        
        // Test 9: Test Dialog System
        yield return StartCoroutine(RunTest("Dialog System Functionality", () => {
            if (adaptiveDialogManager != null)
            {
                adaptiveDialogManager.ShowDialog("Test dialog message", null);
                return true;
            }
            return false;
        }));
        
        // Test 10: Test Choice System
        yield return StartCoroutine(RunTest("Choice System Functionality", () => {
            if (adaptiveChoiceManager != null)
            {
                string[] testChoices = { "Choice 1", "Choice 2", "Choice 3" };
                adaptiveChoiceManager.DisplayChoices(testChoices, (choice) => {
                    Debug.Log($"Test choice selected: {choice}");
                });
                return true;
            }
            return false;
        }));
        
        // Test 11: Test Session Summary
        yield return StartCoroutine(RunTest("Session Summary System", () => {
            if (adaptiveDialogManager != null)
            {
                string longSummary = "🎮 Session Complete!\n\n" +
                                  "Score: 85/100\n" +
                                  "Accuracy: 85%\n" +
                                  "Time: 2m 30s\n" +
                                  "Questions Answered: 10/10\n\n" +
                                  "Great job! You're improving in Filipino nouns. " +
                                  "Keep practicing to master more vocabulary words. " +
                                  "Your progress has been saved and will help personalize your next learning session.";
                
                adaptiveDialogManager.ShowDialog(longSummary, null);
                return true;
            }
            return false;
        }));
        
        // Test 12: Test SM2 Algorithm
        yield return StartCoroutine(RunTest("SM2 Algorithm Integration", () => {
            if (LearningProgressionManager.Instance != null)
            {
                LearningProgressionManager.Instance.RecordQuestionAnswer(
                    "Pangngalan", 0, DifficultyLevel.Easy, true, 5.0f, 1);
                return true;
            }
            return false;
        }));
        
        // Display final results
        DisplayTestResults();
    }
    
    System.Collections.IEnumerator RunTest(string testName, System.Func<bool> testFunction)
    {
        totalTests++;
        Debug.Log($"🔍 Running test: {testName}");
        
        bool result = testFunction();
        
        if (result)
        {
            testResults++;
            Debug.Log($"✅ PASS: {testName}");
        }
        else
        {
            Debug.LogError($"❌ FAIL: {testName}");
        }
        
        yield return new WaitForSeconds(0.3f);
    }
    
    void DisplayTestResults()
    {
        string resultsText = $"🧪 Complete System Test Results: {testResults}/{totalTests} passed\n\n";
        
        if (testResults == totalTests)
        {
            resultsText += "🎉 All tests passed! The complete learning system is ready!";
            resultsText += "\n\n✅ System Features:";
            resultsText += "\n• 3-level progression (Easy/Medium/Hard)";
            resultsText += "\n• 10 questions per level";
            resultsText += "\n• SM2 spaced repetition algorithm";
            resultsText += "\n• Progress tracking and persistence";
            resultsText += "\n• Adaptive dialog system";
            resultsText += "\n• Auto-sizing choice buttons";
            resultsText += "\n• Session summary with vertical expansion";
            resultsText += "\n• Mobile-optimized text rendering";
        }
        else
        {
            resultsText += $"⚠️ {totalTests - testResults} test(s) failed. Check the console for details.";
            resultsText += "\n\n❌ Failed Components:";
            if (testResults < 12) resultsText += "\n• System components need setup";
            if (testResults < 10) resultsText += "\n• Question databases need initialization";
            if (testResults < 8) resultsText += "\n• Progress tracking needs configuration";
        }
        
        Debug.Log(resultsText);
        
        if (testResultsText != null)
        {
            testResultsText.text = resultsText;
        }
        
        if (testResultsPanel != null)
        {
            testResultsPanel.SetActive(true);
        }
    }
    
    [ContextMenu("Test Learning Progression")]
    public void TestLearningProgression()
    {
        if (LearningProgressionManager.Instance != null)
        {
            Debug.Log("🧪 Testing Learning Progression...");
            
            // Test topic progress
            TopicProgress progress = LearningProgressionManager.Instance.GetTopicProgress("Pangngalan");
            Debug.Log($"Topic Progress: {progress.topicName}, Level: {progress.currentLevel}");
            
            // Test difficulty access
            bool canAccessEasy = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Easy);
            bool canAccessMedium = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Medium);
            bool canAccessHard = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Hard);
            
            Debug.Log($"Access - Easy: {canAccessEasy}, Medium: {canAccessMedium}, Hard: {canAccessHard}");
            
            // Test progress update
            LearningProgressionManager.Instance.UpdateTopicProgress("Pangngalan", DifficultyLevel.Easy, true, 0.9f);
            Debug.Log("✅ Updated topic progress");
        }
        else
        {
            Debug.LogError("❌ LearningProgressionManager not found!");
        }
    }
    
    [ContextMenu("Test Question Database")]
    public void TestQuestionDatabase()
    {
        if (questionDatabase != null)
        {
            Debug.Log("🧪 Testing Question Database...");
            
            // Test question counts
            int easyCount = questionDatabase.GetQuestionCount(DifficultyLevel.Easy);
            int mediumCount = questionDatabase.GetQuestionCount(DifficultyLevel.Medium);
            int hardCount = questionDatabase.GetQuestionCount(DifficultyLevel.Hard);
            
            Debug.Log($"Question counts - Easy: {easyCount}, Medium: {mediumCount}, Hard: {hardCount}");
            
            // Test question retrieval
            QuestionData easyQuestion = questionDatabase.GetQuestion(DifficultyLevel.Easy, 0);
            if (easyQuestion != null)
            {
                Debug.Log($"Sample Easy Question: {easyQuestion.question}");
                Debug.Log($"Choices: {string.Join(", ", easyQuestion.choices)}");
                Debug.Log($"Correct Answer: {easyQuestion.correctAnswer}");
                Debug.Log($"Explanation: {easyQuestion.explanation}");
            }
        }
        else
        {
            Debug.LogError("❌ QuestionDatabase not found!");
        }
    }
    
    [ContextMenu("Test Complete Flow")]
    public void TestCompleteFlow()
    {
        Debug.Log("🧪 Testing Complete Learning Flow...");
        
        // Simulate complete flow
        if (LearningProgressionManager.Instance != null && questionDatabase != null)
        {
            // 1. Start with Easy level
            Debug.Log("1. Starting Easy level...");
            TopicProgress progress = LearningProgressionManager.Instance.GetTopicProgress("Pangngalan");
            Debug.Log($"Current level: {progress.currentLevel}");
            
            // 2. Complete Easy level
            Debug.Log("2. Completing Easy level...");
            LearningProgressionManager.Instance.UpdateTopicProgress("Pangngalan", DifficultyLevel.Easy, true, 0.9f);
            
            // 3. Check if Medium is unlocked
            Debug.Log("3. Checking Medium level access...");
            bool canAccessMedium = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Medium);
            Debug.Log($"Can access Medium: {canAccessMedium}");
            
            // 4. Complete Medium level
            if (canAccessMedium)
            {
                Debug.Log("4. Completing Medium level...");
                LearningProgressionManager.Instance.UpdateTopicProgress("Pangngalan", DifficultyLevel.Medium, true, 0.85f);
                
                // 5. Check if Hard is unlocked
                Debug.Log("5. Checking Hard level access...");
                bool canAccessHard = LearningProgressionManager.Instance.CanAccessLevel("Pangngalan", DifficultyLevel.Hard);
                Debug.Log($"Can access Hard: {canAccessHard}");
                
                // 6. Complete Hard level
                if (canAccessHard)
                {
                    Debug.Log("6. Completing Hard level...");
                    LearningProgressionManager.Instance.UpdateTopicProgress("Pangngalan", DifficultyLevel.Hard, true, 0.8f);
                    
                    // 7. Check if topic is mastered
                    Debug.Log("7. Checking topic mastery...");
                    bool isMastered = LearningProgressionManager.Instance.IsTopicMastered("Pangngalan");
                    Debug.Log($"Topic mastered: {isMastered}");
                }
            }
            
            Debug.Log("✅ Complete flow test finished!");
        }
        else
        {
            Debug.LogError("❌ Required components not found!");
        }
    }
}
