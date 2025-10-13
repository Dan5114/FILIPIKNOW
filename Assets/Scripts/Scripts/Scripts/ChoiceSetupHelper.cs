using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ChoiceSetupHelper : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string setupInstructions = 
        "CHOICE BUTTON SETUP INSTRUCTIONS:\n\n" +
        "1. Create Choice Button Prefabs\n" +
        "2. Add AdaptiveChoiceManager to scene\n" +
        "3. Configure Layout Groups\n" +
        "4. Test with various text lengths\n" +
        "5. Integrate with Game Managers\n\n" +
        "Use Auto Setup for quick configuration.";
    
    [Header("Auto Setup Options")]
    public bool autoSetup = false;
    public GameObject parentCanvas;
    public Transform choiceButtonsParent;
    
    [Header("Choice Button Settings")]
    public float buttonWidth = 200f;
    public float buttonHeight = 50f;
    public float spacing = 10f;
    public bool useVerticalLayout = true;
    public bool showChoiceNumbers = true;
    
    [Header("Test Options")]
    public bool createTestButtons = false;
    public int numberOfTestButtons = 4;
    
    [ContextMenu("Auto Setup Choice System")]
    public void AutoSetupChoiceSystem()
    {
        if (!autoSetup)
        {
            Debug.LogWarning("Auto setup is disabled. Enable 'autoSetup' to use this feature.");
            return;
        }
        
        Debug.Log("Starting auto setup of choice button system...");
        
        // Find or create Canvas
        Canvas canvas = null;
        
        // Try to use assigned parentCanvas first
        if (parentCanvas != null)
        {
            canvas = parentCanvas.GetComponent<Canvas>();
        }
        
        // If no assigned canvas, find existing one
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        // If still no canvas, create one
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("Created Canvas");
        }
        
        // Auto-assign the parentCanvas for next time
        if (parentCanvas == null)
        {
            parentCanvas = canvas.gameObject;
        }
        
        // Create Choice Buttons Parent
        GameObject choiceParent = null;
        
        // Try to use assigned choiceButtonsParent first
        if (choiceButtonsParent != null)
        {
            choiceParent = choiceButtonsParent.gameObject;
        }
        
        // If no assigned parent, create one
        if (choiceParent == null)
        {
            choiceParent = new GameObject("ChoiceButtonsParent");
            choiceParent.transform.SetParent(canvas.transform, false);
            
            RectTransform rectTransform = choiceParent.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.2f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 300);
        }
        
        // Add Layout Group
        if (useVerticalLayout)
        {
            VerticalLayoutGroup layoutGroup = choiceParent.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = choiceParent.AddComponent<VerticalLayoutGroup>();
            }
            
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
        }
        else
        {
            HorizontalLayoutGroup layoutGroup = choiceParent.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = choiceParent.AddComponent<HorizontalLayoutGroup>();
            }
            
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
        }
        
        // Add ContentSizeFitter
        ContentSizeFitter sizeFitter = choiceParent.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = choiceParent.AddComponent<ContentSizeFitter>();
        }
        
        if (useVerticalLayout)
        {
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        else
        {
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        
        // Add AdaptiveChoiceManager
        AdaptiveChoiceManager choiceManager = choiceParent.GetComponent<AdaptiveChoiceManager>();
        if (choiceManager == null)
        {
            choiceManager = choiceParent.AddComponent<AdaptiveChoiceManager>();
        }
        
        // Configure the choice manager
        choiceManager.choiceButtonsParent = choiceParent.transform;
        choiceManager.useVerticalLayout = useVerticalLayout;
        choiceManager.spacing = spacing;
        choiceManager.showChoiceNumbers = showChoiceNumbers;
        
        // Auto-assign the choiceButtonsParent for next time
        if (choiceButtonsParent == null)
        {
            choiceButtonsParent = choiceParent.transform;
        }
        
        Debug.Log("Choice system auto setup completed!");
        
        // Create test buttons if requested
        if (createTestButtons)
        {
            CreateTestChoiceButtons(choiceParent.transform);
        }
    }
    
    void CreateTestChoiceButtons(Transform parent)
    {
        Debug.Log($"Creating {numberOfTestButtons} test choice buttons...");
        
        string[] testChoices = {
            "Short",
            "Medium length choice",
            "This is a very long choice option that tests the auto-sizing functionality of the adaptive choice button system",
            "Another extremely long choice that should trigger the adaptive button system and test text wrapping"
        };
        
        for (int i = 0; i < numberOfTestButtons && i < testChoices.Length; i++)
        {
            CreateChoiceButton(parent, testChoices[i], i);
        }
    }
    
    GameObject CreateChoiceButton(Transform parent, string choiceText, int index)
    {
        GameObject buttonObj = new GameObject($"ChoiceButton_{index}");
        buttonObj.transform.SetParent(parent, false);
        
        // Add RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        
        // Add Image
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 1f);
        
        // Add Button
        Button button = buttonObj.AddComponent<Button>();
        
        // Add AdaptiveChoiceButton
        AdaptiveChoiceButton adaptiveButton = buttonObj.AddComponent<AdaptiveChoiceButton>();
        
        // Create Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = choiceText;
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Assign references
        adaptiveButton.button = button;
        adaptiveButton.choiceText = text;
        adaptiveButton.buttonImage = image;
        
        // Configure the choice
        adaptiveButton.ConfigureChoice(choiceText);
        
        Debug.Log($"Created test choice button: {choiceText}");
        
        return buttonObj;
    }
    
    [ContextMenu("Create Choice Button Prefab")]
    public void CreateChoiceButtonPrefab()
    {
        GameObject prefab = CreateChoiceButtonPrefabObject();
        
        // Save as prefab (this would need to be done manually in Unity)
        Debug.Log($"Choice button prefab created: {prefab.name}");
        Debug.Log("To save as prefab: Drag the created object to your Assets folder");
    }
    
    GameObject CreateChoiceButtonPrefabObject()
    {
        GameObject buttonObj = new GameObject("AdaptiveChoiceButton_Prefab");
        
        // Add RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        
        // Add Image
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 1f);
        
        // Add Button
        Button button = buttonObj.AddComponent<Button>();
        
        // Add ContentSizeFitter
        ContentSizeFitter sizeFitter = buttonObj.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Add LayoutElement
        LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = buttonWidth;
        layoutElement.preferredHeight = buttonHeight;
        layoutElement.minWidth = 120f;
        layoutElement.minHeight = 40f;
        
        // Add AdaptiveChoiceButton
        AdaptiveChoiceButton adaptiveButton = buttonObj.AddComponent<AdaptiveChoiceButton>();
        
        // Create Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Choice Text";
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 10f;
        text.fontSizeMax = 18f;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Assign references
        adaptiveButton.button = button;
        adaptiveButton.choiceText = text;
        adaptiveButton.buttonImage = image;
        
        return buttonObj;
    }
    
    [ContextMenu("Test Choice System")]
    public void TestChoiceSystem()
    {
        AdaptiveChoiceManager choiceManager = FindObjectOfType<AdaptiveChoiceManager>();
        if (choiceManager != null)
        {
            string[] testChoices = {
                "Yes",
                "No",
                "Maybe, I'm not sure",
                "This is a very long choice option that tests the auto-sizing functionality of the adaptive choice button system"
            };
            
            choiceManager.DisplayChoices(testChoices, (choice) => {
                Debug.Log($"Test choice selected: {choice}");
            });
        }
        else
        {
            Debug.LogError("AdaptiveChoiceManager not found! Please run auto setup first.");
        }
    }
    
    [ContextMenu("Check Setup Status")]
    public void CheckSetupStatus()
    {
        Debug.Log("=== CHOICE SYSTEM SETUP STATUS ===");
        
        // Check for Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        Debug.Log($"Canvas: {(canvas != null ? "✓ Found" : "✗ Missing")}");
        
        // Check for Choice Buttons Parent
        Transform choiceParent = choiceButtonsParent ?? GameObject.Find("ChoiceButtonsParent")?.transform;
        Debug.Log($"Choice Buttons Parent: {(choiceParent != null ? "✓ Found" : "✗ Missing")}");
        
        // Check for Layout Group
        LayoutGroup layoutGroup = choiceParent?.GetComponent<LayoutGroup>();
        Debug.Log($"Layout Group: {(layoutGroup != null ? "✓ Found" : "✗ Missing")}");
        
        // Check for AdaptiveChoiceManager
        AdaptiveChoiceManager choiceManager = FindObjectOfType<AdaptiveChoiceManager>();
        Debug.Log($"AdaptiveChoiceManager: {(choiceManager != null ? "✓ Found" : "✗ Missing")}");
        
        // Check for AdaptiveChoiceButtons
        AdaptiveChoiceButton[] choiceButtons = FindObjectsOfType<AdaptiveChoiceButton>();
        Debug.Log($"AdaptiveChoiceButtons: {choiceButtons.Length} found");
        
        Debug.Log("=== SETUP COMPLETE ===");
    }
}
