using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestNounsGameManager : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool autoTest = false;
    public float testDelay = 2f;
    
    [Header("Test Results")]
    public TextMeshProUGUI testResultsText;
    public GameObject testResultsPanel;
    
    private NounsGameManager nounsGameManager;
    private AdaptiveDialogManager adaptiveDialogManager;
    private AdaptiveChoiceManager adaptiveChoiceManager;
    private int testResults = 0;
    private int totalTests = 0;
    
    void Start()
    {
        // Find components
        nounsGameManager = FindObjectOfType<NounsGameManager>();
        adaptiveDialogManager = FindObjectOfType<AdaptiveDialogManager>();
        adaptiveChoiceManager = FindObjectOfType<AdaptiveChoiceManager>();
        
        if (autoTest)
        {
            Invoke(nameof(RunAllTests), testDelay);
        }
    }
    
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("🧪 Starting NounsGameManager Tests...");
        testResults = 0;
        totalTests = 0;
        
        StartCoroutine(RunTestsCoroutine());
    }
    
    System.Collections.IEnumerator RunTestsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Test 1: Check if NounsGameManager exists
        yield return StartCoroutine(RunTest("NounsGameManager Exists", () => {
            return nounsGameManager != null;
        }));
        
        // Test 2: Check if AdaptiveDialogManager is assigned
        yield return StartCoroutine(RunTest("AdaptiveDialogManager Assigned", () => {
            return nounsGameManager != null && nounsGameManager.adaptiveDialogManager != null;
        }));
        
        // Test 3: Check if AdaptiveChoiceManager is assigned
        yield return StartCoroutine(RunTest("AdaptiveChoiceManager Assigned", () => {
            return nounsGameManager != null && nounsGameManager.adaptiveChoiceManager != null;
        }));
        
        // Test 4: Check if AdaptiveDialogManager is functional
        yield return StartCoroutine(RunTest("AdaptiveDialogManager Functional", () => {
            return adaptiveDialogManager != null && adaptiveDialogManager.enabled;
        }));
        
        // Test 5: Check if AdaptiveChoiceManager is functional
        yield return StartCoroutine(RunTest("AdaptiveChoiceManager Functional", () => {
            return adaptiveChoiceManager != null && adaptiveChoiceManager.enabled;
        }));
        
        // Test 6: Test dialog display
        yield return StartCoroutine(RunTest("Dialog Display Test", () => {
            if (adaptiveDialogManager != null)
            {
                adaptiveDialogManager.ShowDialog("Test dialog message", null);
                return true;
            }
            return false;
        }));
        
        // Test 7: Test choice display
        yield return StartCoroutine(RunTest("Choice Display Test", () => {
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
        
        // Test 8: Test session summary
        yield return StartCoroutine(RunTest("Session Summary Test", () => {
            if (adaptiveDialogManager != null)
            {
                string longSummary = "This is a test session summary with a lot of text to test the auto-sizing functionality. " +
                                   "The dialog should expand to accommodate this longer text content. " +
                                   "If the text is very long, it should become scrollable. " +
                                   "This tests the adaptive dialog system's ability to handle different content lengths.";
                adaptiveDialogManager.ShowDialog(longSummary, null);
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
        
        yield return new WaitForSeconds(0.5f);
    }
    
    void DisplayTestResults()
    {
        string resultsText = $"🧪 Test Results: {testResults}/{totalTests} passed\n\n";
        
        if (testResults == totalTests)
        {
            resultsText += "🎉 All tests passed! The adaptive dialog system is working correctly.";
        }
        else
        {
            resultsText += $"⚠️ {totalTests - testResults} test(s) failed. Check the console for details.";
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
    
    [ContextMenu("Test Dialog System")]
    public void TestDialogSystem()
    {
        if (adaptiveDialogManager != null)
        {
            Debug.Log("🧪 Testing Dialog System...");
            adaptiveDialogManager.ShowDialog("This is a test dialog message. The dialog should appear and auto-size correctly.", () => {
                Debug.Log("✅ Dialog test completed!");
            });
        }
        else
        {
            Debug.LogError("❌ AdaptiveDialogManager not found!");
        }
    }
    
    [ContextMenu("Test Choice System")]
    public void TestChoiceSystem()
    {
        if (adaptiveChoiceManager != null)
        {
            Debug.Log("🧪 Testing Choice System...");
            string[] testChoices = {
                "Short choice",
                "This is a medium length choice option",
                "This is a very long choice option that should test the auto-sizing functionality of the adaptive choice button system"
            };
            
            adaptiveChoiceManager.DisplayChoices(testChoices, (choice) => {
                Debug.Log($"✅ Choice selected: {choice}");
            });
        }
        else
        {
            Debug.LogError("❌ AdaptiveChoiceManager not found!");
        }
    }
    
    [ContextMenu("Test Session Summary")]
    public void TestSessionSummary()
    {
        if (adaptiveDialogManager != null)
        {
            Debug.Log("🧪 Testing Session Summary...");
            string sessionSummary = "🎮 Session Complete!\n\n" +
                                  "Score: 85/100\n" +
                                  "Accuracy: 85%\n" +
                                  "Time: 2m 30s\n" +
                                  "Questions Answered: 10/10\n\n" +
                                  "Great job! You're improving in Filipino nouns. " +
                                  "Keep practicing to master more vocabulary words. " +
                                  "Your progress has been saved and will help personalize your next learning session.";
            
            adaptiveDialogManager.ShowDialog(sessionSummary, () => {
                Debug.Log("✅ Session summary test completed!");
            });
        }
        else
        {
            Debug.LogError("❌ AdaptiveDialogManager not found!");
        }
    }
    
    [ContextMenu("Test Long Content")]
    public void TestLongContent()
    {
        if (adaptiveDialogManager != null)
        {
            Debug.Log("🧪 Testing Long Content...");
            string longContent = "This is a very long test message to verify that the adaptive dialog system can handle " +
                               "extensive content without breaking the UI layout. The dialog should automatically expand " +
                               "to accommodate this text, and if it becomes too long, it should become scrollable. " +
                               "This tests the system's ability to adapt to different content lengths while maintaining " +
                               "readability and proper formatting. The auto-sizing feature should ensure that the text " +
                               "remains visible and properly formatted regardless of length.";
            
            adaptiveDialogManager.ShowDialog(longContent, () => {
                Debug.Log("✅ Long content test completed!");
            });
        }
        else
        {
            Debug.LogError("❌ AdaptiveDialogManager not found!");
        }
    }
}
