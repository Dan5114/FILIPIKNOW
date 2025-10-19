using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages visual feedback for choice buttons (default, correct, wrong states)
/// Loads sprites from Assets/Resources/ChoiceButton/ folder
/// </summary>
public class ChoiceButtonFeedback : MonoBehaviour
{
    [Header("Button Image Component")]
    private Image buttonImage;
    
    [Header("Sprites - Loaded from Resources")]
    private Sprite defaultSprite;   // Choice Button 1 (brown)
    private Sprite correctSprite;   // Choice Button2 (green)
    private Sprite wrongSprite;     // Choice Button3 (red)
    
    [Header("State")]
    private ButtonState currentState = ButtonState.Default;
    
    public enum ButtonState
    {
        Default,
        Correct,
        Wrong
    }
    
    void Awake()
    {
        // Get the Image component from this button
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError($"ChoiceButtonFeedback: No Image component found on {gameObject.name}");
            return;
        }
        
        // Save the current sprite as default fallback
        if (buttonImage.sprite != null && defaultSprite == null)
        {
            defaultSprite = buttonImage.sprite;
            Debug.Log($"ChoiceButtonFeedback: Using current sprite as default fallback for {gameObject.name}");
        }
        
        // Load sprites from Resources folder
        LoadSprites();
        
        // Set to default state initially
        ResetToDefault();
    }
    
    void LoadSprites()
    {
        // Try to load sprites from Assets/Resources/ChoiceButton/ folder
        Sprite loadedDefault = Resources.Load<Sprite>("ChoiceButton/Choices Button1");
        Sprite loadedCorrect = Resources.Load<Sprite>("ChoiceButton/Choices Button2");
        Sprite loadedWrong = Resources.Load<Sprite>("ChoiceButton/Choices Button3");
        
        // Only override if successfully loaded
        if (loadedDefault != null) defaultSprite = loadedDefault;
        if (loadedCorrect != null) correctSprite = loadedCorrect;
        if (loadedWrong != null) wrongSprite = loadedWrong;
        
        // Verify sprites loaded successfully
        if (loadedDefault == null)
        {
            Debug.LogWarning($"ChoiceButtonFeedback: 'Choices Button1' not found in Resources/ChoiceButton/ - using fallback for {gameObject.name}");
        }
        if (loadedCorrect == null)
        {
            Debug.LogWarning($"ChoiceButtonFeedback: 'Choices Button2' not found in Resources/ChoiceButton/ - COLOR FEEDBACK DISABLED for {gameObject.name}");
        }
        if (loadedWrong == null)
        {
            Debug.LogWarning($"ChoiceButtonFeedback: 'Choices Button3' not found in Resources/ChoiceButton/ - COLOR FEEDBACK DISABLED for {gameObject.name}");
        }
        
        Debug.Log($"ChoiceButtonFeedback on {gameObject.name}: Sprites - Default: {defaultSprite != null}, Correct: {correctSprite != null}, Wrong: {wrongSprite != null}");
    }
    
    /// <summary>
    /// Shows the correct answer feedback (green button)
    /// </summary>
    public void ShowCorrect()
    {
        if (buttonImage != null && correctSprite != null)
        {
            buttonImage.sprite = correctSprite;
            currentState = ButtonState.Correct;
            Debug.Log($"{gameObject.name}: Showing CORRECT feedback (green)");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Cannot show correct feedback - buttonImage or correctSprite is null");
        }
    }
    
    /// <summary>
    /// Shows the wrong answer feedback (red button)
    /// </summary>
    public void ShowWrong()
    {
        if (buttonImage != null && wrongSprite != null)
        {
            buttonImage.sprite = wrongSprite;
            currentState = ButtonState.Wrong;
            Debug.Log($"{gameObject.name}: Showing WRONG feedback (red)");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Cannot show wrong feedback - buttonImage or wrongSprite is null");
        }
    }
    
    /// <summary>
    /// Resets button to default appearance (brown)
    /// </summary>
    public void ResetToDefault()
    {
        if (buttonImage != null && defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
            currentState = ButtonState.Default;
            Debug.Log($"{gameObject.name}: Reset to DEFAULT (brown)");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Cannot reset to default - buttonImage or defaultSprite is null");
        }
    }
    
    /// <summary>
    /// Gets the current state of the button
    /// </summary>
    public ButtonState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Manually set sprites (useful if not using Resources folder)
    /// </summary>
    public void SetSprites(Sprite defaultSpr, Sprite correctSpr, Sprite wrongSpr)
    {
        defaultSprite = defaultSpr;
        correctSprite = correctSpr;
        wrongSprite = wrongSpr;
        ResetToDefault();
    }
}

