# Adaptive Dialog System Documentation

## Overview
The Adaptive Dialog System provides auto-sizing dialog boxes that expand to accommodate large amounts of text, particularly useful for session summaries. The system includes both automatic text sizing and dialog box resizing capabilities.

## File Locations
All files are located in: `C:\Users\Daniel\Documents\School\Unity\Filipknow\Assets\Scripts\Scripts\`

### Core Files:
- `AdaptiveDialogManager.cs` - Main dialog management component
- `DialogSetupHelper.cs` - Setup utility and testing component
- `TypewriterEffect.cs` - Enhanced typewriter with session summary support (located in `Scripts/TypewriterEffect.cs`)

### Integration Files:
- `NounsGameManager.cs` - Updated to use new dialog system (located in `Scripts/NounsGameManager.cs`)

## Components

### 1. AdaptiveDialogManager
The main component that handles dialog display with auto-sizing capabilities.

**Key Features:**
- Auto-sizing dialog boxes based on content length
- Scrollable text for very long content
- Smooth animations for size changes
- Special handling for session summaries

**Location:** `Scripts/Scripts/AdaptiveDialogManager.cs`

### 2. Enhanced TypewriterEffect
Updated with session summary support and better auto-sizing.

**New Features:**
- Session summary mode with optimized font sizes
- Better text alignment for long content
- Configurable font size ranges

**Location:** `Scripts/TypewriterEffect.cs`

### 3. DialogSetupHelper
Utility component for easy setup and testing.

**Features:**
- Auto-setup of the entire dialog system
- Setup status checking
- Test functions for session summaries

**Location:** `Scripts/Scripts/DialogSetupHelper.cs`

## Setup Instructions

### Method 1: Auto Setup (Recommended)
1. Add the `DialogSetupHelper` component to any GameObject in your scene
2. Check "Auto Setup" in the inspector
3. Right-click on the component and select "Auto Setup Dialog System"
4. The system will automatically create all necessary components

### Method 2: Manual Setup
1. **Create Canvas Structure:**
   ```
   Canvas
   └── DialogPanel
       ├── ScrollRect (Component)
       ├── Content (Child GameObject)
       │   └── DialogText (TextMeshProUGUI)
       ├── ContinueButton
       └── BackButton
   ```

2. **Add Components:**
   - Add `AdaptiveDialogManager` to DialogPanel
   - Add `ScrollRect` to DialogPanel
   - Add `ContentSizeFitter` to DialogText

3. **Configure References:**
   - Assign all UI references in AdaptiveDialogManager
   - Set up button listeners
   - Configure auto-sizing settings

## Configuration

### AdaptiveDialogManager Settings
```csharp
[Header("Auto-Sizing Settings")]
public bool enableAutoSizing = true;
public Vector2 minDialogSize = new Vector2(400f, 200f);
public Vector2 maxDialogSize = new Vector2(800f, 600f);
public float textPadding = 20f;
public float autoSizeMin = 12f;
public float autoSizeMax = 24f;

[Header("Session Summary Settings")]
public bool useScrollableForLongText = true;
public float longTextThreshold = 500f; // Characters
```

### TypewriterEffect Settings
```csharp
[Header("Session Summary Settings")]
public bool enableSessionSummaryMode = false;
public float sessionSummaryFontSizeMin = 10f;
public float sessionSummaryFontSizeMax = 18f;
```

## Usage

### Basic Dialog Display
```csharp
AdaptiveDialogManager dialog = FindObjectOfType<AdaptiveDialogManager>();
dialog.ShowDialog("Your message here", () => {
    Debug.Log("Dialog completed");
});
```

### Session Summary Display
```csharp
AdaptiveDialogManager dialog = FindObjectOfType<AdaptiveDialogManager>();
dialog.ShowSessionSummary(sessionSummaryText, () => {
    Debug.Log("Session summary completed");
});
```

### Integration with Game Managers
The system automatically detects and uses the AdaptiveDialogManager in your game managers. No code changes needed in existing game managers - they will automatically use the new system if available.

## Auto-Sizing Behavior

### Short Text (< 500 characters)
- Dialog box automatically resizes to fit content
- Smooth animation during size changes
- Text uses normal font size range

### Long Text (≥ 500 characters)
- Dialog box uses maximum size
- ScrollRect becomes active for scrolling
- Text uses smaller font size range for better readability
- Left-aligned text for better readability

### Session Summaries
- Always uses scrollable mode regardless of length
- Optimized font sizes for readability
- Special formatting for better visual hierarchy

## Animation Features

### Dialog Size Animation
- Smooth transitions when dialog size changes
- Configurable animation duration
- Uses SmoothStep for natural movement

### Text Animation
- Typewriter effect with punctuation-aware pausing
- Character animation integration
- Sound effect support

## Testing

### Using DialogSetupHelper
1. Add `DialogSetupHelper` to a GameObject
2. Right-click and select "Check Setup Status" to verify setup
3. Right-click and select "Test Session Summary" to test the system
4. Use "Auto Setup Dialog System" if manual setup is needed

### Manual Testing
```csharp
// Test long text
dialog.ShowDialog("Very long text here...");

// Test session summary
dialog.ShowSessionSummary("Session summary with lots of data...");
```

## Troubleshooting

### Common Issues

1. **Dialog not resizing:**
   - Check if `enableAutoSizing` is true
   - Verify ContentSizeFitter is added to text component
   - Ensure LayoutElement is properly configured

2. **ScrollRect not working:**
   - Verify ScrollRect component is added to dialog panel
   - Check that content is properly assigned
   - Ensure Mask component is present

3. **Text not displaying:**
   - Check TextMeshProUGUI component
   - Verify font assignment
   - Check text color and visibility

4. **Buttons not responding:**
   - Verify button references in AdaptiveDialogManager
   - Check button listeners are properly assigned
   - Ensure buttons are not blocked by other UI elements

### Debug Information
The system provides extensive debug logging:
- Setup status information
- Font application confirmations
- Animation progress
- Error messages for missing components

## Performance Considerations

- Dialog resizing uses coroutines for smooth animation
- ContentSizeFitter may cause layout rebuilds - use sparingly
- Large text content may impact performance on mobile devices
- Consider limiting session summary length for better performance

## Customization

### Custom Dialog Sizes
```csharp
dialog.SetMinDialogSize(new Vector2(300f, 150f));
dialog.SetMaxDialogSize(new Vector2(1000f, 700f));
dialog.SetTextPadding(30f);
```

### Custom Font Sizes
```csharp
typewriterEffect.sessionSummaryFontSizeMin = 8f;
typewriterEffect.sessionSummaryFontSizeMax = 16f;
```

### Custom Animation Duration
```csharp
dialog.resizeAnimationDuration = 0.5f; // Slower animation
```

## Integration with Existing Systems

The new system is designed to be backward compatible:
- Existing TypewriterEffect usage continues to work
- Game managers automatically detect and use the new system
- Fallback mechanisms ensure compatibility
- No breaking changes to existing code

## File Structure Reference

```
Scripts/
├── TypewriterEffect.cs (Enhanced with session summary support)
├── NounsGameManager.cs (Updated to use new dialog system)
└── Scripts/
    ├── AdaptiveDialogManager.cs (New main dialog component)
    ├── DialogSetupHelper.cs (Setup utility)
    └── DialogSystemDocumentation.md (This documentation)
```

## Best Practices

1. **Use Session Summary Mode** for long, structured content
2. **Test on different screen sizes** to ensure proper scaling
3. **Limit session summary length** for better user experience
4. **Use appropriate font sizes** for your target audience
5. **Provide clear visual hierarchy** in session summaries
6. **Test animation performance** on target devices

## Future Enhancements

Potential improvements for future versions:
- Rich text formatting support
- Custom dialog themes
- Advanced animation options
- Accessibility features
- Multi-language text handling
- Custom scrollbar styling

