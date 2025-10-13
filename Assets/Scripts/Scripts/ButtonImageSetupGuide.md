# 🎨 Difficulty Selection Button Images Setup Guide

## Overview
This guide explains how to replace the default button images in the NounsDifficultySelection scene with your custom PNG images.

## 📁 File Structure Required

Create this folder structure in your Unity project:

```
Assets/
└── Resources/
    ├── DifficultyButtons/
    │   ├── Easy_Normal.png
    │   ├── Easy_Highlighted.png
    │   ├── Medium_Normal.png
    │   ├── Medium_Highlighted.png
    │   ├── Hard_Normal.png
    │   └── Hard_Highlighted.png
    └── LevelIcons/
        ├── Locked_Normal.png
        ├── Locked_Highlighted.png
        ├── Unlocked_Normal.png
        └── Unlocked_Highlighted.png
```

## 🖼️ Image Requirements

### File Names (Must be exact):

**Difficulty Buttons:**
- `Easy_Normal.png` - Normal state for Easy button
- `Easy_Highlighted.png` - Highlighted/clicked state for Easy button
- `Medium_Normal.png` - Normal state for Medium button
- `Medium_Highlighted.png` - Highlighted/clicked state for Medium button
- `Hard_Normal.png` - Normal state for Hard button
- `Hard_Highlighted.png` - Highlighted/clicked state for Hard button

**Level Status Icons:**
- `Locked_Normal.png` - Normal state for locked level icon
- `Locked_Highlighted.png` - Highlighted state for locked level icon
- `Unlocked_Normal.png` - Normal state for unlocked level icon
- `Unlocked_Highlighted.png` - Highlighted state for unlocked level icon

### Image Specifications:
- **Format**: PNG (with transparency support)
- **Size**: Recommended 256x256 or 512x512 pixels
- **Color Space**: RGBA (supports transparency)
- **Import Settings**: 
  - Texture Type: Sprite (2D and UI)
  - Sprite Mode: Single
  - Pixels Per Unit: 100

## 🔧 Setup Methods

### Method 1: Automatic Loading (Recommended)
1. Place your PNG files in `Assets/Resources/DifficultyButtons/`
2. Name them exactly as specified above
3. The `DifficultySelectionManager` will automatically load them
4. No additional setup required!

### Method 2: Manual Assignment
1. Select the GameObject with `DifficultySelectionManager` script
2. In the Inspector, find the "Button Images" section
3. Drag your images to the appropriate slots:
   - Easy Normal Sprite
   - Easy Highlighted Sprite
   - Medium Normal Sprite
   - Medium Highlighted Sprite
   - Hard Normal Sprite
   - Hard Highlighted Sprite

### Method 3: Using ButtonImageSetupHelper
1. Add the `ButtonImageSetupHelper` script to any GameObject in your scene
2. Drag your images to the helper script's fields
3. Assign the `DifficultySelectionManager` to the helper
4. Click "Apply Images to Difficulty Manager" in the context menu

## 🎯 How It Works

### Button States:
1. **Normal State**: Shows the normal image when button is idle
2. **Highlighted State**: Shows the highlighted image when:
   - Mouse hovers over button
   - Button is pressed/clicked
   - Button receives focus

### Visual Feedback:
- **Available Buttons**: Show normal images with full opacity
- **Completed Buttons**: Show normal images with slight green tint
- **Locked Buttons**: Show normal images with gray tint and reduced opacity

### Font Integration:
- **Universal Minecraft Font**: All button text automatically uses your universal Minecraft font from `FilipknowFontManager`
- **Consistent Styling**: Button text maintains the same font, size, and color as your game's universal font system
- **Automatic Application**: Font is applied on scene start and when button states change

## 🐛 Troubleshooting

### Images Not Showing:
1. Check file names are exact (case-sensitive)
2. Verify images are in `Assets/Resources/DifficultyButtons/`
3. Ensure images are imported as Sprites
4. Check Unity Console for loading errors

### Highlight Effect Not Working:
1. Verify highlighted images are assigned
2. Check that EventTrigger components are added to buttons
3. Ensure buttons are interactable

### Performance Issues:
- Use compressed textures for better performance
- Keep image sizes reasonable (256x256 or 512x512 max)
- Use PNG-8 for simple images with few colors

## 📝 Code Integration

The system automatically:
- Loads images from Resources folder
- Sets up EventTrigger components for hover effects
- Manages button states (normal, highlighted, disabled)
- Handles locked/completed button visual states

## 🎨 Customization Options

You can modify the visual behavior by editing:
- `SetupButtonImageStates()` - How images are applied
- `UpdateButtonState()` - Visual states for different button conditions
- `LoadImagesFromResources()` - How images are loaded

## ✅ Testing

To test your setup:
1. Run the scene
2. Hover over difficulty buttons
3. Click buttons to see highlight effects
4. Check Console for loading confirmations

The system will log success messages when images are loaded:
```
✅ Loaded Easy Normal sprite from Resources
✅ Loaded Easy Highlighted sprite from Resources
🎨 Button images configured for all difficulty levels
```
