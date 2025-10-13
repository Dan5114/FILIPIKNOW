using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to easily set up level icons for difficulty selection
/// </summary>
public class LevelIconSetupHelper : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(6, 10)]
    public string setupInstructions = 
        "LEVEL ICON SETUP:\n" +
        "1. Create Image components above each difficulty button:\n" +
        "   - Add Image component above EasyButton (name it EasyLevelIcon)\n" +
        "   - Add Image component above MediumButton (name it MediumLevelIcon)\n" +
        "   - Add Image component above HardButton (name it HardLevelIcon)\n" +
        "2. Assign these Image components to DifficultySelectionManager:\n" +
        "   - easyLevelIcon field\n" +
        "   - mediumLevelIcon field\n" +
        "   - hardLevelIcon field\n" +
        "3. Icons will automatically load from Resources/LevelIcons/:\n" +
        "   - 🔒 Padlock icon = locked levels\n" +
        "   - 🔓 Exclamation icon = available levels\n" +
        "   - No icon = completed levels (normal button)\n" +
        "4. Icons positioned above difficulty buttons with hover effects!";
    
    [Header("Manual Icon Assignment")]
    [Space(10)]
    [Tooltip("Drag your Locked Normal icon here")]
    public Sprite lockedNormalIcon;
    [Tooltip("Drag your Locked Highlighted icon here")]
    public Sprite lockedHighlightedIcon;
    
    [Tooltip("Drag your Unlocked Normal icon here")]
    public Sprite unlockedNormalIcon;
    [Tooltip("Drag your Unlocked Highlighted icon here")]
    public Sprite unlockedHighlightedIcon;
    
    [Header("Apply to DifficultySelectionManager")]
    [Space(10)]
    public DifficultySelectionManager difficultyManager;
    
    [ContextMenu("Apply Icons to Difficulty Manager")]
    public void ApplyIconsToManager()
    {
        if (difficultyManager == null)
        {
            Debug.LogError("❌ DifficultySelectionManager not assigned!");
            return;
        }
        
        // Apply locked icons
        if (lockedNormalIcon != null)
        {
            difficultyManager.lockedLevelNormalIcon = lockedNormalIcon;
            Debug.Log("✅ Applied Locked Normal icon");
        }
        
        if (lockedHighlightedIcon != null)
        {
            difficultyManager.lockedLevelHighlightedIcon = lockedHighlightedIcon;
            Debug.Log("✅ Applied Locked Highlighted icon");
        }
        
        // Apply unlocked icons
        if (unlockedNormalIcon != null)
        {
            difficultyManager.unlockedLevelNormalIcon = unlockedNormalIcon;
            Debug.Log("✅ Applied Unlocked Normal icon");
        }
        
        if (unlockedHighlightedIcon != null)
        {
            difficultyManager.unlockedLevelHighlightedIcon = unlockedHighlightedIcon;
            Debug.Log("✅ Applied Unlocked Highlighted icon");
        }
        
        Debug.Log("🔒 All level status icons applied to DifficultySelectionManager!");
    }
    
    [ContextMenu("Test Load Icons from Resources")]
    public void TestLoadFromResources()
    {
        Debug.Log("🔍 Testing level icon loading from Resources...");
        
        // Test locked icons
        Sprite lockedNormal = Resources.Load<Sprite>("LevelIcons/Locked_Normal");
        Sprite lockedHighlighted = Resources.Load<Sprite>("LevelIcons/Locked_Highlighted");
        
        // Test unlocked icons
        Sprite unlockedNormal = Resources.Load<Sprite>("LevelIcons/Unlocked_Normal");
        Sprite unlockedHighlighted = Resources.Load<Sprite>("LevelIcons/Unlocked_Highlighted");
        
        // Report results
        Debug.Log($"Locked Normal: {(lockedNormal != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Locked Highlighted: {(lockedHighlighted != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Unlocked Normal: {(unlockedNormal != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Unlocked Highlighted: {(unlockedHighlighted != null ? "✅ Found" : "❌ Missing")}");
    }
    
    [ContextMenu("Setup Level Icons")]
    public void SetupLevelIcons()
    {
        if (difficultyManager == null)
        {
            Debug.LogError("❌ DifficultySelectionManager not assigned!");
            return;
        }
        
        Debug.Log("🎯 To setup level status icons:");
        Debug.Log("1. Create Image components above each difficulty button");
        Debug.Log("2. Assign those Image components to the DifficultySelectionManager");
        Debug.Log("3. Icons will automatically load from Resources/LevelIcons/:");
        Debug.Log("   🔒 Padlock icon = locked levels");
        Debug.Log("   🔓 Exclamation icon = available levels");
        Debug.Log("   (No icon) = completed levels (normal button)");
        Debug.Log("4. Icons will be positioned above difficulty buttons with hover effects!");
    }
    
    void Start()
    {
        // Auto-apply icons if manager is assigned
        if (difficultyManager != null)
        {
            ApplyIconsToManager();
        }
    }
}
