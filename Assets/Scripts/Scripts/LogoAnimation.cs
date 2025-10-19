using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LogoAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Entrance Animation")]
    public bool playOnStart = true;
    public float fadeInDuration = 1.0f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Idle Animation - Gentle Float")]
    public bool useFloatEffect = true;
    public float floatAmplitude = 10f; // How far up/down it moves
    public float floatSpeed = 1f; // How fast it floats
    public AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Interactive Animation - Hover Scale")]
    public bool useHoverScale = true;
    public float hoverScale = 1.05f; // 5% larger on hover
    public float hoverAnimationSpeed = 8f;
    public AnimationCurve hoverCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Exit Animation")]
    public bool useFadeOut = true;
    public float fadeOutDuration = 0.8f;
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Visual Effects")]
    public bool useGlowEffect = false;
    public Color glowColor = new Color(1f, 1f, 0.8f, 0.3f);
    public float glowIntensity = 1.2f;
    
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public float soundVolume = 0.5f;
    
    // Private variables
    private Image logoImage;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isHovering = false;
    private bool isAnimating = false;
    private Coroutine currentAnimation;
    private Coroutine floatCoroutine;
    private Coroutine entranceCoroutine;
    
    void Start()
    {
        InitializeLogo();
        
        if (playOnStart)
        {
            PlayEntranceAnimation();
        }
    }
    
    void InitializeLogo()
    {
        // Get the Image component
        logoImage = GetComponent<Image>();
        
        // Store original values
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        
        if (logoImage != null)
        {
            originalColor = logoImage.color;
        }
        
        // Get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayEntranceAnimation()
    {
        if (entranceCoroutine != null)
            StopCoroutine(entranceCoroutine);
            
        entranceCoroutine = StartCoroutine(AnimateEntrance());
    }
    
    private IEnumerator AnimateEntrance()
    {
        isAnimating = true;
        
        // Start with transparent
        if (logoImage != null)
        {
            Color startColor = originalColor;
            startColor.a = 0f;
            logoImage.color = startColor;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            float curveValue = fadeInCurve.Evaluate(progress);
            
            // Fade in
            if (logoImage != null)
            {
                Color currentColor = originalColor;
                currentColor.a = curveValue;
                logoImage.color = currentColor;
            }
            
            yield return null;
        }
        
        // Ensure final color is set
        if (logoImage != null)
        {
            logoImage.color = originalColor;
        }
        
        isAnimating = false;
        
        // Start idle animation after entrance
        if (useFloatEffect)
        {
            StartFloatAnimation();
        }
    }
    
    private void StartFloatAnimation()
    {
        if (floatCoroutine != null)
            StopCoroutine(floatCoroutine);
            
        floatCoroutine = StartCoroutine(FloatAnimation());
    }
    
    private IEnumerator FloatAnimation()
    {
        float time = 0f;
        Vector3 startPosition = originalPosition;
        
        while (useFloatEffect && !isHovering)
        {
            time += Time.deltaTime * floatSpeed;
            
            // Create gentle floating motion
            float floatOffset = Mathf.Sin(time) * floatAmplitude;
            Vector3 newPosition = startPosition + Vector3.up * floatOffset;
            
            transform.localPosition = newPosition;
            
            yield return null;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating && useHoverScale)
        {
            isHovering = true;
            PlayHoverAnimation();
            PlaySound(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating && useHoverScale)
        {
            isHovering = false;
            PlayNormalAnimation();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickSound);
    }
    
    private void PlayHoverAnimation()
    {
        StopCurrentAnimation();
        
        currentAnimation = StartCoroutine(AnimateHover(true));
    }
    
    private void PlayNormalAnimation()
    {
        StopCurrentAnimation();
        
        currentAnimation = StartCoroutine(AnimateHover(false));
    }
    
    private IEnumerator AnimateHover(bool isHover)
    {
        Vector3 targetScale = isHover ? originalScale * hoverScale : originalScale;
        Vector3 startScale = transform.localScale;
        
        float elapsedTime = 0f;
        float duration = 1f / hoverAnimationSpeed;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float curveValue = hoverCurve.Evaluate(progress);
            
            // Animate scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            // Apply glow effect if enabled
            if (useGlowEffect && logoImage != null)
            {
                Color currentColor = originalColor;
                if (isHover)
                {
                    currentColor = Color.Lerp(originalColor, glowColor, glowIntensity * curveValue);
                }
                logoImage.color = currentColor;
            }
            
            yield return null;
        }
        
        // Ensure final values are set
        transform.localScale = targetScale;
        
        if (logoImage != null && useGlowEffect)
        {
            logoImage.color = isHover ? glowColor : originalColor;
        }
        
        // Resume float animation if not hovering
        if (!isHover && useFloatEffect)
        {
            StartFloatAnimation();
        }
    }
    
    public void PlayExitAnimation()
    {
        if (useFadeOut)
        {
            StartCoroutine(AnimateExit());
        }
    }
    
    private IEnumerator AnimateExit()
    {
        isAnimating = true;
        
        // Stop other animations
        StopCurrentAnimation();
        if (floatCoroutine != null)
        {
            StopCoroutine(floatCoroutine);
            floatCoroutine = null;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeOutDuration;
            float curveValue = fadeOutCurve.Evaluate(progress);
            
            // Fade out
            if (logoImage != null)
            {
                Color currentColor = originalColor;
                currentColor.a = curveValue;
                logoImage.color = currentColor;
            }
            
            yield return null;
        }
        
        // Ensure final color is set
        if (logoImage != null)
        {
            Color finalColor = originalColor;
            finalColor.a = 0f;
            logoImage.color = finalColor;
        }
        
        isAnimating = false;
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
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
        if (logoImage != null)
        {
            logoImage.color = originalColor;
        }
        isHovering = false;
        isAnimating = false;
    }
    
    public void SetHoverState()
    {
        StopCurrentAnimation();
        transform.localScale = originalScale * hoverScale;
        if (logoImage != null && useGlowEffect)
        {
            logoImage.color = glowColor;
        }
        isHovering = true;
        isAnimating = false;
    }
    
    public void EnableFloatEffect(bool enable)
    {
        useFloatEffect = enable;
        if (enable && !isHovering)
        {
            StartFloatAnimation();
        }
        else if (!enable && floatCoroutine != null)
        {
            StopCoroutine(floatCoroutine);
            floatCoroutine = null;
        }
    }
    
    public void EnableHoverScale(bool enable)
    {
        useHoverScale = enable;
    }
    
    public void EnableGlowEffect(bool enable)
    {
        useGlowEffect = enable;
    }
    
    // Reset to original state
    void OnDisable()
    {
        SetNormalState();
    }
}
