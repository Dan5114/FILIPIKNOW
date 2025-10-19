using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextQualityTest : MonoBehaviour
{
    [Header("Test Configuration")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public bool autoRunTest = false;
    
    [Header("Test Messages")]
    [TextArea(3, 5)]
    public string shortMessage = "This is a short test message.";
    
    [TextArea(3, 10)]
    public string longMessage = "This is a very long test message that should test the adaptive dialog system's ability to handle extensive content without breaking the UI layout. The dialog should automatically expand to accommodate this text, and if it becomes too long, it should become scrollable. This tests the system's ability to adapt to different content lengths while maintaining readability and proper formatting. The auto-sizing feature should ensure that the text remains visible and properly formatted regardless of length.";
    
    [TextArea(3, 8)]
    public string sessionSummaryMessage = "🎮 Session Complete!\n\n" +
                                        "Score: 85/100\n" +
                                        "Accuracy: 85%\n" +
                                        "Time: 2m 30s\n" +
                                        "Questions Answered: 10/10\n\n" +
                                        "Great job! You're improving in Filipino nouns. " +
                                        "Keep practicing to master more vocabulary words. " +
                                        "Your progress has been saved and will help personalize your next learning session.";
    
    void Start()
    {
        if (autoRunTest)
        {
            Invoke(nameof(RunTextQualityTest), 1f);
        }
    }
    
    [ContextMenu("Run Text Quality Test")]
    public void RunTextQualityTest()
    {
        if (adaptiveDialogManager == null)
        {
            adaptiveDialogManager = FindObjectOfType<AdaptiveDialogManager>();
        }
        
        if (adaptiveDialogManager == null)
        {
            Debug.LogError("❌ AdaptiveDialogManager not found! Please assign it in the inspector.");
            return;
        }
        
        Debug.Log("🧪 Starting Text Quality Test...");
        StartCoroutine(RunTestSequence());
    }
    
    System.Collections.IEnumerator RunTestSequence()
    {
        // Test 1: Short message
        Debug.Log("📝 Testing short message...");
        adaptiveDialogManager.ShowDialog(shortMessage, () => {
            Debug.Log("✅ Short message test completed");
        });
        
        yield return new WaitForSeconds(3f);
        
        // Test 2: Long message
        Debug.Log("📝 Testing long message...");
        adaptiveDialogManager.ShowDialog(longMessage, () => {
            Debug.Log("✅ Long message test completed");
        });
        
        yield return new WaitForSeconds(5f);
        
        // Test 3: Session summary
        Debug.Log("📝 Testing session summary...");
        adaptiveDialogManager.ShowDialog(sessionSummaryMessage, () => {
            Debug.Log("✅ Session summary test completed");
        });
        
        yield return new WaitForSeconds(4f);
        
        Debug.Log("🎉 Text Quality Test completed! Check the dialog for readability improvements.");
    }
    
    [ContextMenu("Test Short Message")]
    public void TestShortMessage()
    {
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(shortMessage, null);
        }
    }
    
    [ContextMenu("Test Long Message")]
    public void TestLongMessage()
    {
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(longMessage, null);
        }
    }
    
    [ContextMenu("Test Session Summary")]
    public void TestSessionSummary()
    {
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(sessionSummaryMessage, null);
        }
    }
    
    [ContextMenu("Test Mobile Font Size")]
    public void TestMobileFontSize()
    {
        if (adaptiveDialogManager != null)
        {
            // Test with mobile-optimized message
            string mobileMessage = "📱 Mobile Test\n\n" +
                                 "This text should be larger and more readable on mobile devices. " +
                                 "The font size should be automatically adjusted for better mobile viewing. " +
                                 "Check if the text is clear and easy to read on your mobile screen.";
            
            adaptiveDialogManager.ShowDialog(mobileMessage, null);
        }
    }
}
