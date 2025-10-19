using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ViewModuleAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Animation Settings")]
    [Range(1.0f, 1.5f)]
    public float hoverScale = 1.08f;
    [Range(0.5f, 2.0f)]
    public float clickScale = 0.95f;
    [Range(1f, 20f)]
    public float animationSpeed = 8f;
    [Range(1f, 30f)]
    public float clickAnimationSpeed = 15f;
    
    [Header("Sprite References")]
    public Image buttonImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;
    public Sprite clickSprite;
    
    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 0.9f, 1f);
    public Color clickColor = new Color(0.9f, 0.9f, 0.8f, 1f);
    
    [Header("Glow Effect")]
    public bool useGlowEffect = true;
    [Range(0.5f, 2.0f)]
    public float glowIntensity = 1.2f;
    public Color glowColor = new Color(1f, 1f, 0.7f, 0.3f);
    
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;
    
    [Header("Advanced Effects")]
    public bool usePulseEffect = false;
    [Range(0.5f, 3f)]
    public float pulseSpeed = 2f;
    [Range(0.9f, 1.1f)]
    public float pulseScale = 1.02f;
    
    // Private variables
    private Vector3 originalScale;
    private Color originalColor;
    private bool isAnimating = false;
    private bool isHovering = false;
    private Coroutine currentAnimation;
    private Coroutine pulseCoroutine;
    
    void Start()
    {
        InitializeButton();
    }
    
    void InitializeButton()
    {
        // Store original values
        originalScale = transform.localScale;
        
        // Get components if not assigned
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Set initial state
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            
            // Set normal sprite if assigned
            if (normalSprite != null)
                buttonImage.sprite = normalSprite;
        }
        
        // Start pulse effect if enabled
        if (usePulseEffect)
        {
            StartPulseEffect();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            isHovering = true;
            PlayHoverAnimation();
            PlaySound(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            isHovering = false;
            PlayNormalAnimation();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            PlayClickAnimation();
            PlaySound(clickSound);
        }
    }
    
    private void PlayHoverAnimation()
    {
        StopCurrentAnimation();
        
        currentAnimation = StartCoroutine(AnimateToState(
            originalScale * hoverScale, 
            hoverColor, 
            hoverSprite, 
            animationSpeed,
            true
        ));
    }
    
    private void PlayNormalAnimation()
    {
        StopCurrentAnimation();
        
        currentAnimation = StartCoroutine(AnimateToState(
            originalScale, 
            originalColor, 
            normalSprite, 
            animationSpeed,
            false
        ));
    }
    
    private void PlayClickAnimation()
    {
        StopCurrentAnimation();
        
        currentAnimation = StartCoroutine(AnimateClick());
    }
    
    private IEnumerator AnimateClick()
    {
        isAnimating = true;
        
        // Quick scale down
        Vector3 clickScale = originalScale * this.clickScale;
        yield return StartCoroutine(AnimateToState(clickScale, clickColor, clickSprite, clickAnimationSpeed, false));
        
        // Quick scale back to hover (if still hovering) or normal
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
        Color targetColor = isHovering ? hoverColor : originalColor;
        Sprite targetSprite = isHovering ? hoverSprite : normalSprite;
        
        yield return StartCoroutine(AnimateToState(targetScale, targetColor, targetSprite, clickAnimationSpeed, false));
        
        isAnimating = false;
    }
    
    private IEnumerator AnimateToState(Vector3 targetScale, Color targetColor, Sprite targetSprite, float speed, bool isHover)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = buttonImage.color;
        
        float elapsedTime = 0f;
        float duration = 1f / speed;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Smooth interpolation using ease-out
            float smoothProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            // Animate scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothProgress);
            
            // Animate color
            if (buttonImage != null)
            {
                Color currentColor = Color.Lerp(startColor, targetColor, smoothProgress);
                
                // Apply glow effect if enabled and hovering
                if (useGlowEffect && isHover)
                {
                    currentColor = Color.Lerp(currentColor, glowColor, glowIntensity * smoothProgress);
                }
                
                buttonImage.color = currentColor;
                
                // Change sprite if available
                if (targetSprite != null)
                    buttonImage.sprite = targetSprite;
            }
            
            yield return null;
        }
        
        // Ensure final values are set
        transform.localScale = targetScale;
        if (buttonImage != null)
        {
            buttonImage.color = targetColor;
            if (targetSprite != null)
                buttonImage.sprite = targetSprite;
        }
    }
    
    private void StartPulseEffect()
    {
        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);
            
        pulseCoroutine = StartCoroutine(PulseEffect());
    }
    
    private IEnumerator PulseEffect()
    {
        while (usePulseEffect && !isHovering)
        {
            float time = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 pulseScale = originalScale * this.pulseScale;
            
            // Pulse up
            while (time < 1f / pulseSpeed)
            {
                time += Time.deltaTime;
                float progress = time * pulseSpeed;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                
                transform.localScale = Vector3.Lerp(startScale, pulseScale, smoothProgress);
                yield return null;
            }
            
            time = 0f;
            startScale = transform.localScale;
            
            // Pulse down
            while (time < 1f / pulseSpeed)
            {
                time += Time.deltaTime;
                float progress = time * pulseSpeed;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                
                transform.localScale = Vector3.Lerp(startScale, originalScale, smoothProgress);
                yield return null;
            }
            
            // Wait before next pulse
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.volume = soundVolume;
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods for external control
    public void SetNormalState()
    {
        StopCurrentAnimation();
        transform.localScale = originalScale;
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
            if (normalSprite != null)
                buttonImage.sprite = normalSprite;
        }
        isAnimating = false;
        isHovering = false;
    }
    
    public void SetHoverState()
    {
        StopCurrentAnimation();
        transform.localScale = originalScale * hoverScale;
        if (buttonImage != null)
        {
            buttonImage.color = hoverColor;
            if (hoverSprite != null)
                buttonImage.sprite = hoverSprite;
        }
        isAnimating = false;
        isHovering = true;
    }
    
    public void EnablePulseEffect(bool enable)
    {
        usePulseEffect = enable;
        if (enable && !isHovering)
        {
            StartPulseEffect();
        }
        else if (!enable && pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }
    
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Clamp(speed, 1f, 20f);
    }
    
    public void SetHoverScale(float scale)
    {
        hoverScale = Mathf.Clamp(scale, 1.0f, 1.5f);
    }
    
    // Reset to original state
    void OnDisable()
    {
        SetNormalState();
    }
}
