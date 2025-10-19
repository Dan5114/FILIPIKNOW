# Quick Setup Guide for Filipknow Dialog System

## 🚀 Quick Start (5 Minutes)

### Step 1: Add Setup Helper
1. In Unity, create an empty GameObject in your scene
2. Name it "DialogSetupHelper"
3. Add the `DialogSetupHelper` component to it

### Step 2: Auto Setup
1. In the DialogSetupHelper component, check "Auto Setup"
2. Right-click on the component → "Auto Setup Dialog System"
3. Wait for "Auto setup completed!" message in console

### Step 3: Test
1. Right-click on DialogSetupHelper component → "Test Session Summary"
2. You should see a large dialog box with scrolling text

### Step 4: Integration
Your existing game managers (like NounsGameManager) will automatically use the new system!

## 📁 File Locations
All files are in: `C:\Users\Daniel\Documents\School\Unity\Filipknow\Assets\Scripts\Scripts\`

## 🎯 What This Solves
- **Auto-expanding dialog boxes** for session summaries
- **Scrollable text** for long content
- **Smooth animations** when dialog resizes
- **Better readability** with optimized font sizes

## 🔧 Manual Configuration (Optional)

### Adjust Dialog Size Limits
```csharp
AdaptiveDialogManager dialog = FindObjectOfType<AdaptiveDialogManager>();
dialog.SetMinDialogSize(new Vector2(400f, 200f));  // Minimum size
dialog.SetMaxDialogSize(new Vector2(800f, 600f));  // Maximum size
```

### Adjust Font Sizes
```csharp
TypewriterEffect typewriter = FindObjectOfType<TypewriterEffect>();
typewriter.sessionSummaryFontSizeMin = 10f;  // Smallest font
typewriter.sessionSummaryFontSizeMax = 18f;  // Largest font
```

## 🧪 Testing Commands

### Check Setup Status
- Right-click DialogSetupHelper → "Check Setup Status"

### Test Session Summary
- Right-click DialogSetupHelper → "Test Session Summary"

### Test Long Text
- Right-click AdaptiveDialogManager → "Test Long Text"

## 🐛 Troubleshooting

### Dialog Not Appearing
- Check console for error messages
- Verify Canvas exists in scene
- Ensure DialogPanel is active

### Text Not Scrolling
- Check if ScrollRect component exists on DialogPanel
- Verify Content GameObject is properly assigned

### Dialog Not Resizing
- Check if `enableAutoSizing` is true in AdaptiveDialogManager
- Verify ContentSizeFitter is on DialogText

## 📱 Mobile Considerations
- Test on actual mobile devices
- Consider reducing max dialog size for smaller screens
- Adjust font sizes for better mobile readability

## 🔄 Integration Notes
- Existing code continues to work unchanged
- New system automatically activates when available
- Fallback to old system if new components missing
- No breaking changes to current functionality

## 📞 Support
If you encounter issues:
1. Check the console for debug messages
2. Use "Check Setup Status" to verify components
3. Try "Auto Setup Dialog System" to recreate components
4. Refer to full documentation in `DialogSystemDocumentation.md`

