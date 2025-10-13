using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    [Header("Heart System Settings")]
    public int maxHearts = 5;
    public int currentHearts = 5;
    
    [Header("Heart UI Elements")]
    public Image[] heartImages; // Array of heart UI images
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public Sprite halfHeartSprite; // Optional for half hearts
    
    [Header("Heart Animation")]
    public float heartAnimationDuration = 0.3f;
    public float heartScaleOnLoss = 1.2f;
    public float heartScaleOnGain = 1.5f;
    
    [Header("Heart Position")]
    public Vector2 heartSpacing = new Vector2(60f, 0f); // Space between hearts
    public Vector2 heartSize = new Vector2(50f, 50f);
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip heartLossSound;
    public AudioClip heartGainSound;
    public AudioClip gameOverSound;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button restartButton;
    public UnityEngine.UI.Button mainMenuButton;
    
    // Events
    public System.Action<int> OnHeartsChanged;
    public System.Action OnGameOver;
    public System.Action OnHeartsRestored;
    
    // Private variables
    private bool isGameOver = false;
    private Coroutine heartAnimationCoroutine;
    
    void Start()
    {
        InitializeHearts();
        SetupGameOverPanel();
    }
    
    void InitializeHearts()
    {
        // Ensure we don't exceed max hearts
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        
        // Update heart display
        UpdateHeartDisplay();
        
        // Setup heart images if not assigned
        if (heartImages == null || heartImages.Length == 0)
        {
            CreateHeartUI();
        }
        
        Debug.Log($"Heart System initialized with {currentHearts}/{maxHearts} hearts");
    }
    
    void CreateHeartUI()
    {
        // Create heart UI elements dynamically
        GameObject heartContainer = new GameObject("HeartContainer");
        heartContainer.transform.SetParent(transform, false);
        
        RectTransform containerRect = heartContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.anchoredPosition = new Vector2(20, -20);
        
        heartImages = new Image[maxHearts];
        
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartObj = new GameObject($"Heart_{i}");
            heartObj.transform.SetParent(heartContainer.transform, false);
            
            RectTransform heartRect = heartObj.AddComponent<RectTransform>();
            heartRect.sizeDelta = heartSize;
            heartRect.anchoredPosition = new Vector2(i * heartSpacing.x, 0);
            
            Image heartImage = heartObj.AddComponent<Image>();
            heartImage.sprite = fullHeartSprite;
            
            heartImages[i] = heartImage;
        }
    }
    
    void SetupGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
        }
    }
    
    // Public methods for heart management
    public void LoseHeart(int amount = 1)
    {
        if (isGameOver) return;
        
        int previousHearts = currentHearts;
        currentHearts = Mathf.Max(0, currentHearts - amount);
        
        if (currentHearts != previousHearts)
        {
            UpdateHeartDisplay();
            PlayHeartLossAnimation(previousHearts - currentHearts);
            PlayHeartLossSound();
            
            OnHeartsChanged?.Invoke(currentHearts);
            
            if (currentHearts <= 0)
            {
                TriggerGameOver();
            }
            
            Debug.Log($"Lost {previousHearts - currentHearts} heart(s). Current hearts: {currentHearts}/{maxHearts}");
        }
    }
    
    public void GainHeart(int amount = 1)
    {
        int previousHearts = currentHearts;
        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        
        if (currentHearts != previousHearts)
        {
            UpdateHeartDisplay();
            PlayHeartGainAnimation(currentHearts - previousHearts);
            PlayHeartGainSound();
            
            OnHeartsChanged?.Invoke(currentHearts);
            
            Debug.Log($"Gained {currentHearts - previousHearts} heart(s). Current hearts: {currentHearts}/{maxHearts}");
        }
    }
    
    public void RestoreAllHearts()
    {
        int previousHearts = currentHearts;
        currentHearts = maxHearts;
        
        if (currentHearts != previousHearts)
        {
            UpdateHeartDisplay();
            PlayHeartGainAnimation(currentHearts - previousHearts);
            PlayHeartGainSound();
            
            OnHeartsChanged?.Invoke(currentHearts);
            OnHeartsRestored?.Invoke();
            
            Debug.Log($"All hearts restored! Current hearts: {currentHearts}/{maxHearts}");
        }
    }
    
    public void SetHearts(int hearts)
    {
        hearts = Mathf.Clamp(hearts, 0, maxHearts);
        
        if (currentHearts != hearts)
        {
            int difference = hearts - currentHearts;
            currentHearts = hearts;
            
            UpdateHeartDisplay();
            
            if (difference > 0)
            {
                PlayHeartGainAnimation(difference);
                PlayHeartGainSound();
            }
            else if (difference < 0)
            {
                PlayHeartLossAnimation(-difference);
                PlayHeartLossSound();
            }
            
            OnHeartsChanged?.Invoke(currentHearts);
            
            if (currentHearts <= 0 && !isGameOver)
            {
                TriggerGameOver();
            }
            
            Debug.Log($"Hearts set to {currentHearts}/{maxHearts}");
        }
    }
    
    void UpdateHeartDisplay()
    {
        if (heartImages == null) return;
        
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                if (i < currentHearts)
                {
                    heartImages[i].sprite = fullHeartSprite;
                    heartImages[i].color = Color.white;
                }
                else
                {
                    heartImages[i].sprite = emptyHeartSprite;
                    heartImages[i].color = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Dimmed
                }
            }
        }
    }
    
    void PlayHeartLossAnimation(int heartsLost)
    {
        if (heartAnimationCoroutine != null)
        {
            StopCoroutine(heartAnimationCoroutine);
        }
        
        heartAnimationCoroutine = StartCoroutine(HeartLossAnimationCoroutine(heartsLost));
    }
    
    void PlayHeartGainAnimation(int heartsGained)
    {
        if (heartAnimationCoroutine != null)
        {
            StopCoroutine(heartAnimationCoroutine);
        }
        
        heartAnimationCoroutine = StartCoroutine(HeartGainAnimationCoroutine(heartsGained));
    }
    
    IEnumerator HeartLossAnimationCoroutine(int heartsLost)
    {
        int startIndex = currentHearts; // Start from the first heart being lost
        
        for (int i = 0; i < heartsLost; i++)
        {
            int heartIndex = startIndex + i;
            if (heartIndex < heartImages.Length && heartImages[heartIndex] != null)
            {
                // Scale up then down
                Vector3 originalScale = heartImages[heartIndex].transform.localScale;
                Vector3 targetScale = originalScale * heartScaleOnLoss;
                
                // Scale up
                float elapsed = 0f;
                while (elapsed < heartAnimationDuration / 2f)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / (heartAnimationDuration / 2f);
                    heartImages[heartIndex].transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                    yield return null;
                }
                
                // Scale down
                elapsed = 0f;
                while (elapsed < heartAnimationDuration / 2f)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / (heartAnimationDuration / 2f);
                    heartImages[heartIndex].transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
                    yield return null;
                }
                
                heartImages[heartIndex].transform.localScale = originalScale;
            }
        }
    }
    
    IEnumerator HeartGainAnimationCoroutine(int heartsGained)
    {
        int startIndex = currentHearts - heartsGained; // Start from the first heart being gained
        
        for (int i = 0; i < heartsGained; i++)
        {
            int heartIndex = startIndex + i;
            if (heartIndex < heartImages.Length && heartImages[heartIndex] != null)
            {
                // Scale up then down
                Vector3 originalScale = heartImages[heartIndex].transform.localScale;
                Vector3 targetScale = originalScale * heartScaleOnGain;
                
                // Scale up
                float elapsed = 0f;
                while (elapsed < heartAnimationDuration / 2f)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / (heartAnimationDuration / 2f);
                    heartImages[heartIndex].transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                    yield return null;
                }
                
                // Scale down
                elapsed = 0f;
                while (elapsed < heartAnimationDuration / 2f)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / (heartAnimationDuration / 2f);
                    heartImages[heartIndex].transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
                    yield return null;
                }
                
                heartImages[heartIndex].transform.localScale = originalScale;
            }
        }
    }
    
    void PlayHeartLossSound()
    {
        if (audioSource != null && heartLossSound != null)
        {
            audioSource.PlayOneShot(heartLossSound);
        }
    }
    
    void PlayHeartGainSound()
    {
        if (audioSource != null && heartGainSound != null)
        {
            audioSource.PlayOneShot(heartGainSound);
        }
    }
    
    void PlayGameOverSound()
    {
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
    }
    
    void TriggerGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        PlayGameOverSound();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        OnGameOver?.Invoke();
        
        Debug.Log("Game Over! All hearts lost.");
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        RestoreAllHearts();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        Debug.Log("Game restarted!");
    }
    
    public void GoToMainMenu()
    {
        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu"); // Adjust scene name as needed
    }
    
    // Public getters
    public int GetCurrentHearts() => currentHearts;
    public int GetMaxHearts() => maxHearts;
    public bool IsGameOver() => isGameOver;
    public bool HasHearts() => currentHearts > 0;
    
    // Debug methods
    [ContextMenu("Lose 1 Heart")]
    public void DebugLoseHeart()
    {
        LoseHeart(1);
    }
    
    [ContextMenu("Gain 1 Heart")]
    public void DebugGainHeart()
    {
        GainHeart(1);
    }
    
    [ContextMenu("Restore All Hearts")]
    public void DebugRestoreAllHearts()
    {
        RestoreAllHearts();
    }
}

