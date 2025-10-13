using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to easily set up button images for difficulty selection
/// </summary>
public class ButtonImageSetupHelper : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(8, 15)]
    public string setupInstructions = 
        "SETUP INSTRUCTIONS:\n" +
        "1. Create a folder: Assets/Resources/DifficultyButtons/\n" +
        "2. Add your PNG files with these exact names:\n" +
        "   - Easy_Normal.png (normal Easy button)\n" +
        "   - Easy_Highlighted.png (highlighted Easy button)\n" +
        "   - Medium_Normal.png (normal Medium button)\n" +
        "   - Medium_Highlighted.png (highlighted Medium button)\n" +
        "   - Hard_Normal.png (normal Hard button)\n" +
        "   - Hard_Highlighted.png (highlighted Hard button)\n" +
        "3. The script will automatically load these images\n" +
        "4. Or manually assign them in the DifficultySelectionManager inspector\n\n" +
        "LEVEL ICON FILES:\n" +
        "5. Create folder: Assets/Resources/LevelIcons/\n" +
        "6. Add these PNG files:\n" +
        "   - Locked_Normal.png (locked level normal state)\n" +
        "   - Locked_Highlighted.png (locked level highlighted state)\n" +
        "   - Unlocked_Normal.png (unlocked level normal state)\n" +
        "   - Unlocked_Highlighted.png (unlocked level highlighted state)\n\n" +
        "FONT INTEGRATION:\n" +
        "✅ The DifficultySelectionManager automatically uses your\n" +
        "   universal Minecraft font from FilipknowFontManager\n" +
        "✅ Button text will display in Minecraft font style\n" +
        "✅ Consistent with your game's universal font system";
    
    [Header("Manual Image Assignment")]
    [Space(10)]
    [Tooltip("Drag your Easy Normal image here")]
    public Sprite easyNormalImage;
    [Tooltip("Drag your Easy Highlighted image here")]
    public Sprite easyHighlightedImage;
    
    [Tooltip("Drag your Medium Normal image here")]
    public Sprite mediumNormalImage;
    [Tooltip("Drag your Medium Highlighted image here")]
    public Sprite mediumHighlightedImage;
    
    [Tooltip("Drag your Hard Normal image here")]
    public Sprite hardNormalImage;
    [Tooltip("Drag your Hard Highlighted image here")]
    public Sprite hardHighlightedImage;
    
    [Header("Apply to DifficultySelectionManager")]
    [Space(10)]
    public DifficultySelectionManager difficultyManager;
    
    [ContextMenu("Apply Images to Difficulty Manager")]
    public void ApplyImagesToManager()
    {
        if (difficultyManager == null)
        {
            Debug.LogError("❌ DifficultySelectionManager not assigned!");
            return;
        }
        
        // Apply Easy images
        if (easyNormalImage != null)
        {
            difficultyManager.easyNormalSprite = easyNormalImage;
            Debug.Log("✅ Applied Easy Normal image");
        }
        
        if (easyHighlightedImage != null)
        {
            difficultyManager.easyHighlightedSprite = easyHighlightedImage;
            Debug.Log("✅ Applied Easy Highlighted image");
        }
        
        // Apply Medium images
        if (mediumNormalImage != null)
        {
            difficultyManager.mediumNormalSprite = mediumNormalImage;
            Debug.Log("✅ Applied Medium Normal image");
        }
        
        if (mediumHighlightedImage != null)
        {
            difficultyManager.mediumHighlightedSprite = mediumHighlightedImage;
            Debug.Log("✅ Applied Medium Highlighted image");
        }
        
        // Apply Hard images
        if (hardNormalImage != null)
        {
            difficultyManager.hardNormalSprite = hardNormalImage;
            Debug.Log("✅ Applied Hard Normal image");
        }
        
        if (hardHighlightedImage != null)
        {
            difficultyManager.hardHighlightedSprite = hardHighlightedImage;
            Debug.Log("✅ Applied Hard Highlighted image");
        }
        
        Debug.Log("🎨 All button images applied to DifficultySelectionManager!");
    }
    
    [ContextMenu("Test Load Images from Resources")]
    public void TestLoadFromResources()
    {
        Debug.Log("🔍 Testing image loading from Resources...");
        
        // Test Easy images
        Sprite easyNormal = Resources.Load<Sprite>("DifficultyButtons/Easy_Normal");
        Sprite easyHighlighted = Resources.Load<Sprite>("DifficultyButtons/Easy_Highlighted");
        
        // Test Medium images
        Sprite mediumNormal = Resources.Load<Sprite>("DifficultyButtons/Medium_Normal");
        Sprite mediumHighlighted = Resources.Load<Sprite>("DifficultyButtons/Medium_Highlighted");
        
        // Test Hard images
        Sprite hardNormal = Resources.Load<Sprite>("DifficultyButtons/Hard_Normal");
        Sprite hardHighlighted = Resources.Load<Sprite>("DifficultyButtons/Hard_Highlighted");
        
        // Report results
        Debug.Log($"Easy Normal: {(easyNormal != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Easy Highlighted: {(easyHighlighted != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Medium Normal: {(mediumNormal != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Medium Highlighted: {(mediumHighlighted != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Hard Normal: {(hardNormal != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Hard Highlighted: {(hardHighlighted != null ? "✅ Found" : "❌ Missing")}");
    }
    
    void Start()
    {
        // Auto-apply images if manager is assigned
        if (difficultyManager != null)
        {
            ApplyImagesToManager();
        }
    }
}
