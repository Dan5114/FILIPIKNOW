using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class AdaptiveChoiceManager : MonoBehaviour
{
    [Header("Choice Button Setup")]
    public Transform choiceButtonsParent;
    public GameObject choiceButtonPrefab;
    public AdaptiveChoiceButton[] choiceButtons;
    
    [Header("Layout Settings")]
    public bool useVerticalLayout = true;
    public bool useHorizontalLayout = false;
    public float spacing = 10f;
    public RectOffset padding;
    
    [Header("Choice Display Settings")]
    public bool showChoiceNumbers = true;
    public bool enableChoiceAnimations = true;
    public float choiceAnimationDelay = 0.1f;
    
    [Header("Visual Settings")]
    public Color defaultButtonColor = new Color(1f, 1f, 1f, 1f);
    public Color selectedButtonColor = new Color(0.2f, 0.6f, 1f, 1f);
    public Color disabledButtonColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private List<AdaptiveChoiceButton> activeChoiceButtons = new List<AdaptiveChoiceButton>();
    private System.Action<string> onChoiceSelected;
    private Coroutine displayChoicesCoroutine;
    
    // Events
    public System.Action<string> OnChoiceSelected;
    public System.Action OnChoicesDisplayed;
    public System.Action OnChoicesHidden;
    
    void Start()
    {
        // Initialize padding in Start to avoid constructor issues
        if (padding == null)
        {
            padding = new RectOffset(10, 10, 10, 10);
        }
        
        SetupChoiceButtonsParent();
        InitializeChoiceButtons();
    }
    
    void SetupChoiceButtonsParent()
    {
        if (choiceButtonsParent == null)
        {
            choiceButtonsParent = transform;
        }
        
        // Add LayoutGroup if not present
        if (useVerticalLayout)
        {
            VerticalLayoutGroup verticalLayout = choiceButtonsParent.GetComponent<VerticalLayoutGroup>();
            if (verticalLayout == null)
            {
                verticalLayout = choiceButtonsParent.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            
            verticalLayout.spacing = spacing;
            verticalLayout.padding = padding;
            verticalLayout.childAlignment = TextAnchor.MiddleCenter;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
        }
        else if (useHorizontalLayout)
        {
            HorizontalLayoutGroup horizontalLayout = choiceButtonsParent.GetComponent<HorizontalLayoutGroup>();
            if (horizontalLayout == null)
            {
                horizontalLayout = choiceButtonsParent.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            
            horizontalLayout.spacing = spacing;
            horizontalLayout.padding = padding;
            horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayout.childControlWidth = true;
            horizontalLayout.childControlHeight = false;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
        }
        
        // Add ContentSizeFitter for the parent
        ContentSizeFitter contentSizeFitter = choiceButtonsParent.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter == null)
        {
            contentSizeFitter = choiceButtonsParent.gameObject.AddComponent<ContentSizeFitter>();
        }
        
        if (useVerticalLayout)
        {
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        else
        {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
    
    void InitializeChoiceButtons()
    {
        // Clear existing buttons
        ClearAllChoices();
        
        // Initialize from assigned buttons array
        if (choiceButtons != null && choiceButtons.Length > 0)
        {
            foreach (AdaptiveChoiceButton button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    activeChoiceButtons.Add(button);
                }
            }
        }
    }
    
    public void DisplayChoices(string[] choices, System.Action<string> onSelected = null)
    {
        if (choices == null || choices.Length == 0)
        {
            Debug.LogWarning("AdaptiveChoiceManager: No choices provided");
            return;
        }
        
        if (displayChoicesCoroutine != null)
        {
            StopCoroutine(displayChoicesCoroutine);
        }
        
        onChoiceSelected = onSelected;
        displayChoicesCoroutine = StartCoroutine(DisplayChoicesCoroutine(choices));
    }
    
    IEnumerator DisplayChoicesCoroutine(string[] choices)
    {
        
        // Clear existing choices
        ClearAllChoices();
        
        // Create or configure buttons for each choice
        for (int i = 0; i < choices.Length; i++)
        {
            AdaptiveChoiceButton choiceButton = GetOrCreateChoiceButton(i);
            if (choiceButton != null)
            {
                // Format choice text
                string choiceText = FormatChoiceText(choices[i], i);
                
                // Configure the button
                choiceButton.ConfigureChoiceWithCallback(choiceText, OnChoiceButtonSelected);
                choiceButton.gameObject.SetActive(true);
                
                // Add animation delay if enabled
                if (enableChoiceAnimations && i > 0)
                {
                    yield return new WaitForSeconds(choiceAnimationDelay);
                }
            }
        }
        
        // Wait for all buttons to finish sizing
        yield return new WaitForSeconds(0.1f);
        
        OnChoicesDisplayed?.Invoke();
        
        Debug.Log($"AdaptiveChoiceManager: Displayed {choices.Length} choices");
    }
    
    AdaptiveChoiceButton GetOrCreateChoiceButton(int index)
    {
        // Try to use existing button
        if (index < activeChoiceButtons.Count && activeChoiceButtons[index] != null)
        {
            return activeChoiceButtons[index];
        }
        
        // Create new button if needed
        GameObject buttonObj = null;
        
        if (choiceButtonPrefab != null)
        {
            buttonObj = Instantiate(choiceButtonPrefab, choiceButtonsParent);
        }
        else
        {
            // Create default button
            buttonObj = CreateDefaultChoiceButton();
        }
        
        AdaptiveChoiceButton choiceButton = buttonObj.GetComponent<AdaptiveChoiceButton>();
        if (choiceButton == null)
        {
            choiceButton = buttonObj.AddComponent<AdaptiveChoiceButton>();
        }
        
        // Ensure it's in the active list
        while (activeChoiceButtons.Count <= index)
        {
            activeChoiceButtons.Add(null);
        }
        activeChoiceButtons[index] = choiceButton;
        
        return choiceButton;
    }
    
    GameObject CreateDefaultChoiceButton()
    {
        GameObject buttonObj = new GameObject($"ChoiceButton_{activeChoiceButtons.Count}");
        buttonObj.transform.SetParent(choiceButtonsParent, false);
        
        // Add RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        // Add Image
        Image image = buttonObj.AddComponent<Image>();
        image.color = defaultButtonColor;
        
        // Add Button
        Button button = buttonObj.AddComponent<Button>();
        
        // Add AdaptiveChoiceButton
        AdaptiveChoiceButton adaptiveButton = buttonObj.AddComponent<AdaptiveChoiceButton>();
        
        // Create Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Choice";
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
        
        return buttonObj;
    }
    
    string FormatChoiceText(string choiceText, int index)
    {
        if (showChoiceNumbers)
        {
            return $"{index + 1}. {choiceText}";
        }
        return choiceText;
    }
    
    void OnChoiceButtonSelected(string selectedChoice)
    {
        // Remove choice numbers if they were added
        string cleanChoice = selectedChoice;
        if (showChoiceNumbers && selectedChoice.Length > 3)
        {
            // Remove "1. " prefix if present
            if (selectedChoice.Substring(2, 2) == ". ")
            {
                cleanChoice = selectedChoice.Substring(3);
            }
        }
        
        OnChoiceSelected?.Invoke(cleanChoice);
        onChoiceSelected?.Invoke(cleanChoice);
        
        Debug.Log($"AdaptiveChoiceManager: Choice selected - '{cleanChoice}'");
        
        // Hide choices after selection
        HideChoices();
    }
    
    public void HideChoices()
    {
        ClearAllChoices();
        OnChoicesHidden?.Invoke();
        Debug.Log("AdaptiveChoiceManager: Choices hidden");
    }
    
    void ClearAllChoices()
    {
        if (activeChoiceButtons != null)
        {
            foreach (AdaptiveChoiceButton button in activeChoiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void SetEnabled(bool enabled)
    {
        if (activeChoiceButtons != null)
        {
            foreach (AdaptiveChoiceButton button in activeChoiceButtons)
            {
                if (button != null)
                {
                    button.SetEnabled(enabled);
                }
            }
        }
    }
    
    public void HighlightChoice(int choiceIndex)
    {
        if (activeChoiceButtons != null && choiceIndex >= 0 && choiceIndex < activeChoiceButtons.Count)
        {
            AdaptiveChoiceButton button = activeChoiceButtons[choiceIndex];
            if (button != null)
            {
                button.HighlightButton(true);
            }
        }
    }
    
    public void ClearHighlight()
    {
        if (activeChoiceButtons != null)
        {
            foreach (AdaptiveChoiceButton button in activeChoiceButtons)
            {
                if (button != null)
                {
                    button.HighlightButton(false);
                }
            }
        }
    }
    
    // Public configuration methods
    public void SetLayoutOrientation(bool vertical)
    {
        useVerticalLayout = vertical;
        useHorizontalLayout = !vertical;
        SetupChoiceButtonsParent();
    }
    
    public void SetSpacing(float newSpacing)
    {
        spacing = newSpacing;
        SetupChoiceButtonsParent();
    }
    
    public void SetShowChoiceNumbers(bool show)
    {
        showChoiceNumbers = show;
    }
    
    public void SetChoiceAnimation(bool enabled, float delay = 0.1f)
    {
        enableChoiceAnimations = enabled;
        choiceAnimationDelay = delay;
    }
    
    // Debug methods
    [ContextMenu("Test Short Choices")]
    public void TestShortChoices()
    {
        string[] choices = { "Yes", "No", "Maybe" };
        DisplayChoices(choices);
    }
    
    [ContextMenu("Test Long Choices")]
    public void TestLongChoices()
    {
        string[] choices = {
            "This is a very long choice option that tests the auto-sizing functionality",
            "Another extremely long choice that should trigger the adaptive button system",
            "Short",
            "This is another medium length choice to test various text lengths"
        };
        DisplayChoices(choices);
    }
    
    [ContextMenu("Test Mixed Choices")]
    public void TestMixedChoices()
    {
        string[] choices = {
            "Yes",
            "No, I don't think so",
            "Maybe, but I'm not entirely sure about this decision",
            "Absolutely not"
        };
        DisplayChoices(choices);
    }
}
