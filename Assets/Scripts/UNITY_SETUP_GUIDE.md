# Unity Dialog Box Setup Guide

## Current Hierarchy (CORRECT)
```
IntroductionSummaryDialogBox (SCROLLABLE)
├── ScrollRect (Component) ✅
├── Mask (Component) ✅
├── Image (Component) ✅
├── IntroductionScrollManager (Component) ✅
└── IntroContent (Child GameObject)
    ├── ContentSizeFitter (Component) - Vertical: Preferred Size ✅
    ├── ScrollBackground (Child GameObject)
    │   └── Image (Component) - Scrollbar background ✅
    ├── ScrollIndicator (Child GameObject)
    │   └── Image (Component) - Scrollbar handle ✅
    └── IntroDialogText (Child GameObject)
        ├── TextMeshProUGUI (Component) ✅
        └── TypewriterEffect (Component) ✅

DialogBox (NON-SCROLLABLE)
├── Image (Component) ✅
└── DialogText (Child GameObject)
    ├── TextMeshProUGUI (Component) ✅
    └── TypewriterEffect (Component) ✅
```

## Unity Editor Setup Steps

### 1. IntroductionSummaryDialogBox Setup
1. **Select IntroductionSummaryDialogBox**
2. **Add Components:**
   - `ScrollRect`
   - `Mask` (set to "Mask" mode)
   - `Image` (for background)
3. **ScrollRect Settings:**
   - Content: Drag `IntroContent` here
   - Horizontal: **UNCHECKED**
   - Vertical: **CHECKED**
   - Movement Type: Elastic
   - Elasticity: 0.1
   - Scroll Sensitivity: 20

### 2. IntroContent Setup
1. **Select IntroContent**
2. **Add Components:**
   - `ContentSizeFitter`
   - `Vertical Layout Group` (optional)
3. **ContentSizeFitter Settings:**
   - Horizontal Fit: Unenforced
   - Vertical Fit: **Preferred Size**

### 3. ScrollBackground Setup
1. **Select ScrollBackground**
2. **Add Components:**
   - `Image`
3. **Image Settings:**
   - Color: Light gray or your scrollbar color
   - Set RectTransform to desired scrollbar width/height

### 4. ScrollIndicator Setup
1. **Select ScrollIndicator**
2. **Add Components:**
   - `Image`
3. **Image Settings:**
   - Color: Darker than ScrollBackground
   - Set RectTransform smaller than ScrollBackground

### 5. IntroDialogText Setup
1. **Select IntroDialogText**
2. **Add Components:**
   - `TextMeshProUGUI`
   - `TypewriterEffect`
3. **TextMeshProUGUI Settings:**
   - Font: Assign your font (timesBoldFont)
   - Font Size: 80 (will be set by TypewriterEffect)
   - Alignment: Top Left
   - Overflow: Overflow
   - Word Wrapping: Enabled

### 6. IntroductionScrollManager Setup
1. **Select IntroductionSummaryDialogBox**
2. **Add Component:** `IntroductionScrollManager`
3. **Drag References:**
   - Scroll Rect: Drag ScrollRect component
   - Scroll Background: Drag ScrollBackground GameObject
   - Scroll Indicator: Drag ScrollIndicator GameObject

## Critical Unity Settings

### ScrollRect Configuration
- **Content MUST be assigned to IntroContent**
- **Vertical scrolling MUST be enabled**
- **Horizontal scrolling MUST be disabled**

### ContentSizeFitter Configuration
- **Vertical Fit MUST be "Preferred Size"**
- This allows the content to expand based on text length

### Mask Configuration
- **Mask mode MUST be set to "Mask"**
- This clips the text inside the dialog box

## Testing Checklist

1. **Start the game**
2. **Check console logs for:**
   ```
   IntroductionScrollManager: Found ScrollRect on IntroductionSummaryDialogBox
   IntroductionScrollManager: Found Content: IntroContent with 3 children
   IntroductionScrollManager: Child 0: ScrollBackground
   IntroductionScrollManager: Child 1: ScrollIndicator
   IntroductionScrollManager: Child 2: IntroDialogText
   IntroductionScrollManager: Found ScrollBackground at ScrollBackground
   IntroductionScrollManager: Found ScrollIndicator at ScrollIndicator
   IntroductionScrollManager: Final status - ScrollRect=True, ScrollBackground=True, ScrollIndicator=True
   ```

3. **Verify scrolling works:**
   - Text should appear with 80pt font
   - If text is long, you should be able to scroll
   - ScrollIndicator should move when scrolling

## Troubleshooting

### If ScrollBackground/ScrollIndicator not found:
1. Check GameObject names are exactly "ScrollBackground" and "ScrollIndicator"
2. Ensure they are direct children of IntroContent
3. Check that IntroductionScrollManager is on IntroductionSummaryDialogBox

### If text doesn't appear:
1. Check that IntroDialogText has TextMeshProUGUI component
2. Check that TypewriterEffect is on IntroDialogText
3. Check that font is assigned in NounsGameManager

### If scrolling doesn't work:
1. Check ScrollRect has IntroContent assigned as Content
2. Check ContentSizeFitter is set to Preferred Size
3. Check Mask is set to "Mask" mode
4. Check IntroductionScrollManager has all references assigned
