using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FilipknowFontManager : MonoBehaviour
{
    [Header("Filipknow Universal Fonts")]
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset filipinoFont;
    [SerializeField] private TMP_FontAsset englishFont;
    
    [Header("Font Settings")]
    [SerializeField] private float defaultFontSize = 18f;
    [SerializeField] private Color defaultFontColor = Color.white;
    
    [Header("Language Support")]
    [SerializeField] private bool useLanguageSpecificFonts = true;
    
    // Singleton pattern
    public static FilipknowFontManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Delay font application to ensure all UI elements are ready
        Invoke(nameof(ApplyUniversalFont), 0.1f);
    }
    
    /// <summary>
    /// Applies the universal font to all text components in the scene
    /// </summary>
    public void ApplyUniversalFont()
    {
        Debug.Log($"FilipknowFontManager: ApplyUniversalFont called. Default font: {(defaultFont != null ? defaultFont.name : "NULL")}");
        
        // Apply to all TextMeshPro components
        TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"FilipknowFontManager: Found {tmpTexts.Length} TextMeshPro components");
        
        foreach (TextMeshProUGUI text in tmpTexts)
        {
            if (defaultFont != null)
            {
                Debug.Log($"FilipknowFontManager: Applying font '{defaultFont.name}' to '{text.name}'");
                text.font = defaultFont;
                text.fontSize = defaultFontSize;
                text.color = defaultFontColor;
            }
            else
            {
                Debug.LogWarning($"FilipknowFontManager: Default font is null, cannot apply to '{text.name}'");
            }
        }
        
        // Apply to all Legacy Text components
        Text[] legacyTexts = FindObjectsOfType<Text>();
        foreach (Text text in legacyTexts)
        {
            // For legacy text, we can't directly set TMP fonts
            // But we can ensure consistent styling
            text.fontSize = (int)defaultFontSize;
            text.color = defaultFontColor;
        }
    }
    
    /// <summary>
    /// Applies language-specific font based on current language setting
    /// </summary>
    public void ApplyLanguageFont()
    {
        if (!useLanguageSpecificFonts) return;
        
        TMP_FontAsset targetFont = defaultFont;
        
        // Check current language setting
        if (SettingsManager.Instance != null)
        {
            bool isFilipino = SettingsManager.Instance.IsFilipinoLanguage();
            targetFont = isFilipino ? filipinoFont : englishFont;
        }
        
        // Apply language-specific font
        if (targetFont != null)
        {
            TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in tmpTexts)
            {
                text.font = targetFont;
            }
        }
    }
    
    /// <summary>
    /// Changes the universal font for the entire project
    /// </summary>
    public void ChangeUniversalFont(TMP_FontAsset newFont)
    {
        defaultFont = newFont;
        ApplyUniversalFont();
    }
    
    /// <summary>
    /// Updates font size globally
    /// </summary>
    public void UpdateFontSize(float newSize)
    {
        defaultFontSize = newSize;
        ApplyUniversalFont();
    }
    
    /// <summary>
    /// Updates font color globally
    /// </summary>
    public void UpdateFontColor(Color newColor)
    {
        defaultFontColor = newColor;
        ApplyUniversalFont();
    }
    
    /// <summary>
    /// Gets the current default font
    /// </summary>
    public TMP_FontAsset GetCurrentFont()
    {
        return defaultFont;
    }
    
    /// <summary>
    /// Gets the current font size
    /// </summary>
    public float GetCurrentFontSize()
    {
        return defaultFontSize;
    }
    
    /// <summary>
    /// Gets the current font color
    /// </summary>
    public Color GetCurrentFontColor()
    {
        return defaultFontColor;
    }
    
    /// <summary>
    /// Force refresh fonts on all UI elements (call this if fonts don't apply automatically)
    /// </summary>
    public void ForceRefreshAllFonts()
    {
        Debug.Log("FilipknowFontManager: Force refreshing all fonts...");
        ApplyUniversalFont();
    }
}
