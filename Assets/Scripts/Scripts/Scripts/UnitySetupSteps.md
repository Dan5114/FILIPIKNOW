# Unity Setup Steps - Quick Reference

## 🎯 Setup Overview
Follow these steps in Unity Editor to complete the implementation.

---

## Part 1: Button Sprites Setup (5 minutes)

### Step 1: Create Resources Folder
```
Assets/
  └── Resources/
      └── ChoiceButton/    <- Create this folder
```

### Step 2: Add Button Sprites
Place these 3 images in `Assets/Resources/ChoiceButton/`:

| Sprite Name | Purpose | Color |
|-------------|---------|-------|
| `Choice Button 1` | Default state | Brown |
| `Choice Button2` | Correct answer | Green |
| `Choice Button3` | Wrong answer | Red |

⚠️ **Important**: Names must match EXACTLY (case-sensitive!)

### Step 3: Configure Sprite Settings
For each sprite:
1. Select sprite in Project window
2. Inspector → Texture Type: **Sprite (2D and UI)**
3. Click **Apply**

---

## Part 2: Dialog Scrolling Setup (10 minutes)

### Step 1: Find Your Dialog Panel
In Hierarchy:
- Navigate to your **Nouns Scene**
- Find the **Dialog Panel** (the white box showing text)
- Select it

### Step 2: Add ScrollRect Component
With Dialog Panel selected:
1. Inspector → **Add Component**
2. Search: "Scroll Rect"
3. Add **Scroll Rect** component

### Step 3: Configure ScrollRect
| Property | Value | Notes |
|----------|-------|-------|
| **Content** | TextMeshProUGUI RectTransform | The text component |
| **Viewport** | Dialog Panel itself | Can use same panel |
| **Horizontal** | ❌ Unchecked | No horizontal scroll |
| **Vertical** | ✅ Checked | Enable vertical scroll |
| **Movement Type** | Elastic | Smooth scrolling |
| **Elasticity** | 0.1 | Bounce effect |
| **Inertia** | ✅ Checked | Smooth stop |
| **Deceleration Rate** | 0.135 | Default is good |
| **Scroll Sensitivity** | 1 | Default is good |

### Step 4: Setup Content Size Fitter (Optional but Recommended)
1. Select the **TextMeshProUGUI** component (the text itself)
2. Add Component → **Content Size Fitter**
3. Configure:
   - Horizontal Fit: **Unconstrained**
   - Vertical Fit: **Preferred Size**

This makes the text container automatically expand for long text.

### Step 5: Your Custom Scroll Arrow
Your custom scroll arrow image should:
- Be a **child** of Dialog Panel
- Be positioned where you want it (right side)
- Have an **Image** component

To control visibility automatically:
1. Create a new script or add to existing dialog manager:
```csharp
public GameObject customScrollArrow;

void Update()
{
    if (scrollRect != null && customScrollArrow != null)
    {
        // Show arrow only when scrolling is needed
        bool needsScroll = scrollRect.content.rect.height > scrollRect.viewport.rect.height;
        customScrollArrow.SetActive(needsScroll);
    }
}
```

---

## Part 3: NounsGameManager Setup (2 minutes)

### Step 1: Find NounsGameManager
In Hierarchy:
- Find the GameObject with **NounsGameManager** script
- Usually named "GameManager" or similar

### Step 2: Verify References
In Inspector, check that these are assigned:

| Field | Should Point To |
|-------|-----------------|
| `dialogText` | TextMeshProUGUI with dialog |
| `choiceButtons` | Array of 4 Button components |
| `continueButton` | Continue/Next button |
| `backButton` | Back button |
| `typewriterEffect` | TypewriterEffect component |
| `adaptiveDialogManager` | AdaptiveDialogManager component |

If any are missing (**null**), drag the appropriate GameObjects into the fields.

---

## Part 4: Test Everything (5 minutes)

### Test Checklist:

#### 1. Test Button Feedback
1. **Play** the Nouns Scene
2. Answer a question **correctly** → Button should turn **GREEN** ✅
3. Next question, answer **wrong** → Button should turn **RED** ❌
4. Next question → Buttons back to **BROWN**
5. Check Console for logs: "✅ Button X: Showing GREEN"

#### 2. Test Scrolling
1. **Play** the Nouns Scene
2. Watch **introduction text**:
   - Should display in dialog box
   - Should be **scrollable** if long text
   - Your custom arrow should appear
3. Play through questions
4. At end, watch **session summary**:
   - Should be scrollable
   - Text should stay readable (not tiny)
5. During questions:
   - Regular questions should **NOT** scroll
   - Text should be normal size

#### 3. Check Console
Look for these success messages:
- ✅ "Added ChoiceButtonFeedback component to Button_Choice1"
- ✅ "ChoiceButtonFeedback: Sprites loaded - Default: True, Correct: True, Wrong: True"
- ✅ "Button 0: Showing GREEN (correct)"
- ✅ "All buttons reset to default appearance"
- ✅ "Introduction dialog displayed with XXX characters (scrollable)"

---

## 🚨 Troubleshooting

### Problem: "Failed to load sprite from Resources/ChoiceButton/"
**Solution**:
- Check folder: `Assets/Resources/ChoiceButton/`
- Check names: "Choice Button 1", "Choice Button2", "Choice Button3"
- Check Texture Type: Sprite (2D and UI)

### Problem: Buttons don't change color
**Solution**:
- Check that buttons have **Image** component
- Play the scene and check Inspector on buttons - should see **ChoiceButtonFeedback** component added
- Check Console for warnings

### Problem: Can't scroll
**Solution**:
- Verify ScrollRect **Content** is assigned
- Check **Vertical** is checked
- Ensure Content has **Content Size Fitter** with Vertical: Preferred Size
- Try manually: Click and drag on dialog text area

### Problem: Scroll arrow always shows/never shows
**Solution**:
- Add visibility control script (see Step 5 above)
- Or manually set: Active when intro/summary, Inactive during questions

---

## 📐 Recommended Dialog Panel Layout

```
Canvas
└── DialogPanel (RectTransform + ScrollRect)
    ├── Viewport (Optional, can use DialogPanel itself)
    │   └── Content (TextMeshProUGUI + Content Size Fitter)
    │       └── Dialog text displays here
    └── ScrollArrow (Your custom image)
        └── Image component
```

### RectTransform Settings:

**DialogPanel**:
- Anchor: Center
- Width: 600-800px
- Height: 200-400px
- Pivot: Center (0.5, 0.5)

**Content (TextMeshProUGUI)**:
- Anchor: Top Stretch
- Width: Match parent
- Height: Auto (Content Size Fitter handles this)
- Pivot: Top (0.5, 1)

**ScrollArrow**:
- Anchor: Middle Right
- Position: Right edge of panel
- Size: 40x40px (or your custom size)

---

## ✅ Final Verification

Before closing Unity:
- [ ] Resources/ChoiceButton folder exists with 3 sprites
- [ ] DialogPanel has ScrollRect component
- [ ] ScrollRect Content is assigned
- [ ] Content Size Fitter on text (optional)
- [ ] NounsGameManager references are assigned
- [ ] Tested in Play mode - buttons change color
- [ ] Tested in Play mode - intro/summary scrolls
- [ ] Tested in Play mode - regular questions don't scroll
- [ ] Console shows success messages, no errors

---

## 🎉 You're Done!

The implementation is complete. Players will now see:
1. **Green buttons** for correct answers
2. **Red buttons** for wrong answers  
3. **Scrollable text** for long introductions and summaries
4. **Readable text** (no more tiny unreadable font)

**Need help?** Check `ImplementationGuide.md` for detailed explanations.

---

**Estimated Total Time**: 20-25 minutes  
**Difficulty**: ⭐⭐☆☆☆ (Beginner-Intermediate)  
**Unity Version**: 2020.3+ (should work on all recent versions)

