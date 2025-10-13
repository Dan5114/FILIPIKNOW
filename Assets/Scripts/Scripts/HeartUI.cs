using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public Sprite halfHeartSprite;
    
    [Header("Heart Settings")]
    public Color fullHeartColor = Color.white;
    public Color emptyHeartColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    public Color halfHeartColor = new Color(1f, 0.5f, 0.5f, 0.8f);
    
    [Header("Animation")]
    public float pulseScale = 1.1f;
    public float pulseSpeed = 2f;
    
    private Image heartImage;
    private bool isAnimating = false;
    private Vector3 originalScale;
    
    void Awake()
    {
        heartImage = GetComponent<Image>();
        originalScale = transform.localScale;
    }
    
    public void SetHeartState(HeartState state)
    {
        if (heartImage == null) return;
        
        switch (state)
        {
            case HeartState.Full:
                heartImage.sprite = fullHeartSprite;
                heartImage.color = fullHeartColor;
                break;
            case HeartState.Half:
                heartImage.sprite = halfHeartSprite ?? fullHeartSprite;
                heartImage.color = halfHeartColor;
                break;
            case HeartState.Empty:
                heartImage.sprite = emptyHeartSprite;
                heartImage.color = emptyHeartColor;
                break;
        }
    }
    
    public void PlayLossAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(LossAnimationCoroutine());
    }
    
    public void PlayGainAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(GainAnimationCoroutine());
    }
    
    public void PlayPulseAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(PulseAnimationCoroutine());
    }
    
    private System.Collections.IEnumerator LossAnimationCoroutine()
    {
        isAnimating = true;
        
        // Scale up
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 targetScale = originalScale * 1.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }
        
        // Scale down
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }
        
        transform.localScale = originalScale;
        isAnimating = false;
    }
    
    private System.Collections.IEnumerator GainAnimationCoroutine()
    {
        isAnimating = true;
        
        // Scale up
        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 targetScale = originalScale * 1.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }
        
        // Scale down
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }
        
        transform.localScale = originalScale;
        isAnimating = false;
    }
    
    private System.Collections.IEnumerator PulseAnimationCoroutine()
    {
        isAnimating = true;
        
        float elapsed = 0f;
        float duration = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.Sin(elapsed * pulseSpeed) * 0.1f + 1f;
            transform.localScale = originalScale * pulse;
            yield return null;
        }
        
        transform.localScale = originalScale;
        isAnimating = false;
    }
}

public enum HeartState
{
    Full,
    Half,
    Empty
}

