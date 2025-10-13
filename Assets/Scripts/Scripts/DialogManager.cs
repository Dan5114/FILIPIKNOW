using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button backButton;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Font Settings")]
    public TMP_FontAsset timesBoldFont;
    
    private System.Action onDialogComplete;
    private bool isDialogActive = false;
    
    void Start()
    {
        // Set up typewriter effect
        if (typewriterEffect == null)
            typewriterEffect = GetComponent<TypewriterEffect>();
            
        if (typewriterEffect != null)
        {
            typewriterEffect.textComponent = dialogText;
            typewriterEffect.characterAnimator = characterAnimator;
            typewriterEffect.SetFont(timesBoldFont);
            
            // Subscribe to typewriter events
            typewriterEffect.OnTypingStarted += OnTypingStarted;
            typewriterEffect.OnTypingCompleted += OnTypingCompleted;
        }
        
        // Set up buttons
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
        
        // Hide dialog initially
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }
    
    public void ShowDialog(string message, System.Action onComplete = null)
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
        
        onDialogComplete = onComplete;
        isDialogActive = true;
        
        // Hide continue button while typing
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
        
        // Start typewriter effect
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(message);
        }
        else
        {
            // Fallback to instant text display
            if (dialogText != null)
            {
                dialogText.text = message;
            }
            OnTypingCompleted();
        }
    }
    
    public void HideDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
        isDialogActive = false;
        onDialogComplete = null;
        
        // Stop any ongoing typewriter effect
        if (typewriterEffect != null)
        {
            typewriterEffect.StopTypewriter();
        }
    }
    
    private void OnTypingStarted()
    {
        // Hide continue button while typing
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
    }
    
    private void OnTypingCompleted()
    {
        // Show continue button when typing is complete
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }
    
    private void OnContinueClicked()
    {
        if (isDialogActive)
        {
            HideDialog();
            onDialogComplete?.Invoke();
        }
    }
    
    private void OnBackClicked()
    {
        // Handle back button logic
        // This can be customized based on your game flow
        HideDialog();
    }
    
    // Public methods for external control
    public bool IsDialogActive()
    {
        return isDialogActive;
    }
    
    public void SetCharacterAnimator(Animator animator)
    {
        characterAnimator = animator;
        if (typewriterEffect != null)
        {
            typewriterEffect.SetCharacterAnimator(animator);
        }
    }
    
    public void SetTypingSpeed(float speed)
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.SetTypingSpeed(speed);
        }
    }
}






