using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogSetupHelper : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string setupInstructions = 
        "SETUP INSTRUCTIONS:\n\n" +
        "1. Create a Canvas with Dialog Panel\n" +
        "2. Add AdaptiveDialogManager to the Dialog Panel\n" +
        "3. Assign UI References in AdaptiveDialogManager\n" +
        "4. Add ScrollRect component to Dialog Panel for long text\n" +
        "5. Configure Auto-Sizing settings\n" +
        "6. Test with Session Summary\n\n" +
        "For detailed setup, see the documentation.";
    
    [Header("Required Components")]
    [SerializeField] private bool hasCanvas = false;
    [SerializeField] private bool hasDialogPanel = false;
    [SerializeField] private bool hasAdaptiveDialogManager = false;
    [SerializeField] private bool hasScrollRect = false;
    [SerializeField] private bool hasTextComponent = false;
    [SerializeField] private bool hasButtons = false;
    
    [Header("Auto Setup")]
    public bool autoSetup = false;
    public GameObject dialogPanelPrefab;
    
    [ContextMenu("Check Setup Status")]
    public void CheckSetupStatus()
    {
        Debug.Log("=== DIALOG SETUP STATUS ===");
        
        // Check for Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        hasCanvas = canvas != null;
        Debug.Log($"Canvas: {(hasCanvas ? "✓ Found" : "✗ Missing")}");
        
        // Check for Dialog Panel
        GameObject dialogPanel = GameObject.Find("DialogPanel");
        if (dialogPanel == null)
        {
            dialogPanel = GameObject.Find("Dialog Panel");
        }
        hasDialogPanel = dialogPanel != null;
        Debug.Log($"Dialog Panel: {(hasDialogPanel ? "✓ Found" : "✗ Missing")}");
        
        // Check for AdaptiveDialogManager
        AdaptiveDialogManager adaptiveDialog = FindObjectOfType<AdaptiveDialogManager>();
        hasAdaptiveDialogManager = adaptiveDialog != null;
        Debug.Log($"AdaptiveDialogManager: {(hasAdaptiveDialogManager ? "✓ Found" : "✗ Missing")}");
        
        // Check for ScrollRect
        ScrollRect scrollRect = FindObjectOfType<ScrollRect>();
        hasScrollRect = scrollRect != null;
        Debug.Log($"ScrollRect: {(hasScrollRect ? "✓ Found" : "✗ Missing")}");
        
        // Check for Text Component
        TextMeshProUGUI textComponent = FindObjectOfType<TextMeshProUGUI>();
        hasTextComponent = textComponent != null;
        Debug.Log($"Text Component: {(hasTextComponent ? "✓ Found" : "✗ Missing")}");
        
        // Check for Buttons
        Button[] buttons = FindObjectsOfType<Button>();
        hasButtons = buttons.Length >= 2; // Need at least continue and back buttons
        Debug.Log($"Buttons: {(hasButtons ? "✓ Found" : "✗ Missing")}");
        
        Debug.Log("=== SETUP COMPLETE ===");
    }
    
    [ContextMenu("Auto Setup Dialog System")]
    public void AutoSetupDialogSystem()
    {
        if (!autoSetup)
        {
            Debug.LogWarning("Auto setup is disabled. Enable 'autoSetup' to use this feature.");
            return;
        }
        
        Debug.Log("Starting auto setup of dialog system...");
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("Created Canvas");
        }
        
        // Create Dialog Panel
        GameObject dialogPanel = GameObject.Find("DialogPanel");
        if (dialogPanel == null)
        {
            dialogPanel = new GameObject("DialogPanel");
            dialogPanel.transform.SetParent(canvas.transform, false);
            
            // Add RectTransform
            RectTransform rectTransform = dialogPanel.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(600, 400);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Add Image component for background
            Image background = dialogPanel.AddComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0.95f);
            
            Debug.Log("Created Dialog Panel");
        }
        
        // Add ScrollRect
        ScrollRect scrollRect = dialogPanel.GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            scrollRect = dialogPanel.AddComponent<ScrollRect>();
            
            // Create ScrollRect content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(dialogPanel.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(20, 20);
            contentRect.offsetMax = new Vector2(-20, -60); // Leave space for buttons
            
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            // Add Mask
            Mask mask = dialogPanel.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            
            Debug.Log("Added ScrollRect to Dialog Panel");
        }
        
        // Create Text Component
        TextMeshProUGUI textComponent = dialogPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent == null)
        {
            GameObject textObj = new GameObject("DialogText");
            textObj.transform.SetParent(scrollRect.content, false);
            
            textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = "Dialog Text";
            textComponent.fontSize = 16;
            textComponent.color = Color.black;
            textComponent.alignment = TextAlignmentOptions.TopLeft;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Add ContentSizeFitter
            ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            Debug.Log("Created Text Component");
        }
        
        // Create Buttons
        CreateButton("ContinueButton", "Continue", dialogPanel, new Vector2(100, -320));
        CreateButton("BackButton", "Back", dialogPanel, new Vector2(-100, -320));
        
        // Add AdaptiveDialogManager
        AdaptiveDialogManager adaptiveDialog = dialogPanel.GetComponent<AdaptiveDialogManager>();
        if (adaptiveDialog == null)
        {
            adaptiveDialog = dialogPanel.AddComponent<AdaptiveDialogManager>();
            
            // Assign references
            adaptiveDialog.dialogPanel = dialogPanel;
            adaptiveDialog.dialogText = textComponent;
            adaptiveDialog.continueButton = GameObject.Find("ContinueButton")?.GetComponent<Button>();
            adaptiveDialog.backButton = GameObject.Find("BackButton")?.GetComponent<Button>();
            adaptiveDialog.scrollRect = scrollRect;
            adaptiveDialog.scrollContent = scrollRect.content.gameObject;
            
            Debug.Log("Added AdaptiveDialogManager");
        }
        
        Debug.Log("Auto setup completed!");
        CheckSetupStatus();
    }
    
    private void CreateButton(string name, string text, GameObject parent, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Add RectTransform
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 40);
        buttonRect.anchoredPosition = position;
        
        // Add Image
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        // Add Button
        Button button = buttonObj.AddComponent<Button>();
        
        // Create Button Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Debug.Log($"Created {name}");
    }
    
    [ContextMenu("Test Session Summary")]
    public void TestSessionSummary()
    {
        AdaptiveDialogManager adaptiveDialog = FindObjectOfType<AdaptiveDialogManager>();
        if (adaptiveDialog != null)
        {
            string testSummary = "🎉 Session Complete!\n\n" +
                               "📊 Session Accuracy: 87.5%\n" +
                               "⏱️ Average Response Time: 2.8s\n" +
                               "🏆 Total Score: 425\n\n" +
                               "📈 Overall Progress:\n" +
                               "• Level: 15\n" +
                               "• XP: 2,150\n" +
                               "• Streak: 12\n" +
                               "• Mastery: 78.3%\n\n" +
                               "🎓 Learning Insights:\n" +
                               "• Learning Style: Visual\n" +
                               "• Strengths: Nouns, Basic Sentences\n" +
                               "• Areas to Improve: Complex Grammar\n\n" +
                               "🏅 Recent Achievements:\n" +
                               "• Speed Master\n" +
                               "• Accuracy Expert\n" +
                               "• Dedication Star\n\n" +
                               "💡 Next Steps:\n" +
                               "Focus on practicing complex sentence structures to improve your grammar mastery. Great job on maintaining your streak!";
            
            adaptiveDialog.ShowSessionSummary(testSummary);
        }
        else
        {
            Debug.LogError("AdaptiveDialogManager not found! Please run auto setup first.");
        }
    }
}

