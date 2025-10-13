using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuickSetupHelper : MonoBehaviour
{
    [Header("Quick Setup - Drag References Here")]
    [Space(10)]
    [Header("Introduction Dialog Box")]
    public GameObject introductionSummaryDialogBox;
    public ScrollRect introScrollRect;
    public RectTransform introContent;
    public TypewriterEffect introTypewriter;
    
    [Space(10)]
    [Header("Question Dialog Box (No Scrolling)")]
    public GameObject dialogBox;
    public TypewriterEffect questionTypewriter;
    
    [Space(10)]
    [Header("Font")]
    public TMP_FontAsset timesBoldFont;
    
    [Space(10)]
    [Header("Game Manager")]
    public NounsGameManager nounsGameManager;
    
    [ContextMenu("Auto-Assign All References")]
    public void AutoAssignAllReferences()
    {
        Debug.Log("🔧 QuickSetupHelper: Starting auto-assignment...");
        
        // Find GameObjects
        if (introductionSummaryDialogBox == null)
            introductionSummaryDialogBox = GameObject.Find("IntroductionSummaryDialogBox");
            
        if (dialogBox == null)
            dialogBox = GameObject.Find("DialogBox");
            
        if (nounsGameManager == null)
            nounsGameManager = FindObjectOfType<NounsGameManager>();
        
        // Assign IntroductionSummaryDialogBox references
        if (introductionSummaryDialogBox != null)
        {
            if (introScrollRect == null)
                introScrollRect = introductionSummaryDialogBox.GetComponent<ScrollRect>();
                
            if (introContent == null)
            {
                Transform contentTransform = introductionSummaryDialogBox.transform.Find("IntroContent");
                if (contentTransform != null)
                    introContent = contentTransform.GetComponent<RectTransform>();
            }
            
            if (introTypewriter == null)
                introTypewriter = introductionSummaryDialogBox.GetComponentInChildren<TypewriterEffect>();
            
            Debug.Log($"✅ IntroductionSummaryDialogBox: ScrollRect={introScrollRect != null}, Content={introContent != null}, Typewriter={introTypewriter != null}");
        }
        
        // Assign DialogBox references (no scrolling needed)
        if (dialogBox != null)
        {
            if (questionTypewriter == null)
                questionTypewriter = dialogBox.GetComponentInChildren<TypewriterEffect>();
            
            Debug.Log($"✅ DialogBox: Typewriter={questionTypewriter != null} (No scrolling needed for questions)");
        }
        
        // Assign font to NounsGameManager
        if (nounsGameManager != null && timesBoldFont != null)
        {
            nounsGameManager.timesBoldFont = timesBoldFont;
            Debug.Log($"✅ Assigned font '{timesBoldFont.name}' to NounsGameManager");
        }
        
        Debug.Log("🔧 QuickSetupHelper: Auto-assignment complete!");
    }
    
    [ContextMenu("Fix ScrollRect Content Assignments")]
    public void FixScrollRectContentAssignments()
    {
        Debug.Log("🔧 QuickSetupHelper: Fixing ScrollRect Content assignments...");
        
        // Fix IntroductionSummaryDialogBox ScrollRect (only one that needs scrolling)
        if (introScrollRect != null && introContent != null)
        {
            introScrollRect.content = introContent;
            introScrollRect.vertical = true;
            introScrollRect.horizontal = false;
            Debug.Log("✅ Fixed IntroductionSummaryDialogBox ScrollRect Content assignment");
        }
        else
        {
            Debug.LogWarning("⚠️ IntroductionSummaryDialogBox ScrollRect or Content not found!");
        }
        
        Debug.Log("ℹ️ DialogBox doesn't need ScrollRect (questions don't scroll)");
    }
    
    [ContextMenu("Verify Setup")]
    public void VerifySetup()
    {
        Debug.Log("🔍 QuickSetupHelper: Verifying setup...");
        
        bool allGood = true;
        
        // Check IntroductionSummaryDialogBox
        if (introScrollRect != null)
        {
            if (introScrollRect.content != null)
            {
                Debug.Log($"✅ IntroductionSummaryDialogBox ScrollRect has Content: {introScrollRect.content.name}");
            }
            else
            {
                Debug.LogError("❌ IntroductionSummaryDialogBox ScrollRect has NO Content assigned!");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("❌ IntroductionSummaryDialogBox ScrollRect not found!");
            allGood = false;
        }
        
        // Check DialogBox (no scrolling needed)
        if (dialogBox != null)
        {
            Debug.Log("✅ DialogBox found (no scrolling needed for questions)");
        }
        else
        {
            Debug.LogWarning("⚠️ DialogBox not found!");
        }
        
        // Check Font
        if (nounsGameManager != null && nounsGameManager.timesBoldFont != null)
        {
            Debug.Log($"✅ NounsGameManager has font: {nounsGameManager.timesBoldFont.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ NounsGameManager font not assigned!");
        }
        
        if (allGood)
        {
            Debug.Log("🎉 Setup verification PASSED! Everything should work now!");
        }
        else
        {
            Debug.LogError("💥 Setup verification FAILED! Fix the issues above.");
        }
    }
}
