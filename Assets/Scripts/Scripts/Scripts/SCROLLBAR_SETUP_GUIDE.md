# 🎨 Custom Scrollbar Setup Guide

## ✅ Your Sprites (Already in Resources/DialogBox)

- ✅ **ScrollBar** - Background/track
- ✅ **scrollpage-Sheet** - Handle/thumb (draggable part)
- ✅ **scroll** - Arrow buttons (up/down)

---

## 🚀 **Easy Setup (3 Steps!)**

### **Step 1: Attach Script**

1. Select your **DialogBox** GameObject
2. Click **Add Component**
3. Search for `DialogScrollbarSetup`
4. Add it

### **Step 2: Auto-Setup**

In the Inspector, you'll see:

```
DialogScrollbarSetup
├── [✓] Auto Setup Scrollbar  ← CHECK THIS BOX!
├── Scrollbar Width: 40
├── Scrollbar Offset From Right: 10
└── Handle Height: 60
```

**Just check the box** and it will:
- ✅ Load your sprites from `Resources/DialogBox/`
- ✅ Create the scrollbar structure
- ✅ Apply your custom sprites
- ✅ Link everything to ScrollRect

### **Step 3: Play & Test!**

- Play the scene
- Introduction text displays at **80pt**
- Your custom scrollbar appears when text is long
- **Scroll with mouse wheel** OR **drag the handle**

---

## 🎨 **Optional: Add Scroll Arrow Buttons**

Want clickable up/down arrows?

1. Select **DialogBox**
2. In Inspector, find `DialogScrollbarSetup`
3. **Right-click** the component → `Add Scroll Arrows`

This creates:
- **UpArrow** button (top) - Click to scroll up
- **DownArrow** button (bottom) - Click to scroll down

Both use your **scroll** sprite (auto-rotated for down arrow)!

---

## 📐 **Adjust Settings (Optional)**

In `DialogScrollbarSetup` component:

| Setting | Default | What it does |
|---------|---------|--------------|
| **Scrollbar Width** | 40 | Width of scrollbar track |
| **Scrollbar Offset From Right** | 10 | Distance from right edge |
| **Handle Height** | 60 | Height of draggable handle |

Adjust these to match your sprite sizes!

---

## 🔧 **Manual Setup (Right-Click Menu)**

If you want more control:

1. Select **DialogBox**
2. **Right-click** `DialogScrollbarSetup` component
3. Choose:
   - `Setup Scrollbar` - Creates scrollbar only
   - `Add Scroll Arrows` - Adds arrow buttons

---

## 📊 **Final Structure**

After setup, your hierarchy will be:

```
DialogBox (ScrollRect + DialogScrollbarSetup)
├── Content (ContentSizeFitter)
│   └── DialogText (80pt fixed)
├── VerticalScrollbar (Scrollbar)
│   ├── Image (ScrollBar sprite)
│   └── Sliding Area
│       └── Handle
│           └── Image (scrollpage-Sheet sprite)
├── UpArrow (Button - optional)
│   └── Image (scroll sprite)
└── DownArrow (Button - optional)
    └── Image (scroll sprite, rotated 180°)
```

---

## ✅ **What the Script Does:**

1. **Loads Sprites**:
   ```
   Resources/DialogBox/ScrollBar
   Resources/DialogBox/scrollpage-Sheet
   Resources/DialogBox/scroll
   ```

2. **Creates Structure**:
   - Content with ContentSizeFitter
   - Vertical Scrollbar with your sprites
   - (Optional) Arrow buttons

3. **Configures ScrollRect**:
   - Vertical scrolling only
   - Auto-hide scrollbar when not needed
   - Elastic movement
   - Mouse wheel sensitivity

4. **Applies Your Sprites**:
   - ScrollBar → background
   - scrollpage-Sheet → handle
   - scroll → arrows (if added)

---

## 🎮 **Usage:**

### **In Code (Already Working!):**

Your existing code already uses fixed 80pt:
```csharp
// Introduction
typewriterEffect.StartTypewriter(introMessage, false, true);

// Summary
typewriterEffect.StartTypewriter(summary, false, true);
```

The scrollbar will **automatically appear** when text is longer than the dialog box!

---

## 🧪 **Test Checklist:**

- [ ] Check **Auto Setup Scrollbar** in Inspector
- [ ] Play scene
- [ ] Introduction shows at **80pt**
- [ ] Scrollbar appears with **your sprites**
- [ ] Mouse wheel scrolls text
- [ ] Dragging handle scrolls text
- [ ] (If arrows added) Clicking arrows scrolls

---

## 💡 **Troubleshooting:**

### **Sprites Not Showing?**

Check console for:
```
✅ Loaded ScrollBar background sprite
✅ Loaded scrollpage-Sheet handle sprite
✅ Loaded scroll arrow sprite
```

If you see warnings:
1. Ensure sprites are in `Resources/DialogBox/`
2. Sprite names are **exactly**: `ScrollBar`, `scrollpage-Sheet`, `scroll`
3. Texture Type is `Sprite (2D and UI)`

### **Scrolling Not Working?**

1. Check **Content** has `ContentSizeFitter`
2. Check **DialogText** is inside Content
3. Check **Vertical** is checked in ScrollRect

---

## 🚀 **You're Done!**

1. ✅ Attach `DialogScrollbarSetup` to DialogBox
2. ✅ Check **Auto Setup Scrollbar**
3. ✅ Play and enjoy your custom scrollbar!

**Everything will work automatically with your sprites!** 🎉

