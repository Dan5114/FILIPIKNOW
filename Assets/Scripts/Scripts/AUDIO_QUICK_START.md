# 🎵 Audio Setup - 5 Minute Quick Start!

Follow these simple steps to add all sounds to your game!

---

## ⚡ **Step 1: Get Sound Files** (2 minutes)

Download these 5 free sounds:

### **Option A: Quick Free Sounds**
Go to **freesound.org** and search:
1. "button click" → Download a short click (0.1 seconds)
2. "correct answer" → Download a ding/success sound
3. "wrong answer" → Download a buzzer/error sound
4. "keyboard typing" → Download a single keypress sound
5. "victory fanfare" → Download a 5-10 second celebration music

### **Option B: Use Your Existing Sounds**
You already have:
- `minecraft-click-sound-effect.mp3` - Use this for button clicks!
- `1MAINMENU.mp3` - Use this for background music!

Just need to add:
- Correct answer sound
- Wrong answer sound
- Keyboard typing sound
- Victory music

---

## ⚡ **Step 2: Import to Unity** (30 seconds)

1. In Unity Project window, go to `Assets/Audio/`
2. Drag all your sound files into this folder
3. Done! Unity imports them automatically.

---

## ⚡ **Step 3: Setup GameAudioManager** (1 minute)

### **3a. Create GameObject**
1. Open your **Main Menu** scene
2. Right-click in Hierarchy → Create Empty
3. Name it: `GameAudioManager`

### **3b. Add Script**
1. Select the `GameAudioManager` GameObject
2. Inspector → Add Component
3. Search: `GameAudioManager`
4. Click to add it

### **3c. Assign Sounds**
In the Inspector, drag your sound files:

**Sound Effects:**
```
Button Click Sound: minecraft-click-sound-effect.mp3
Correct Answer Sound: your_correct_sound.mp3
Wrong Answer Sound: your_wrong_sound.mp3
Keyboard Typing Sound: your_typing_sound.mp3
```

**Music:**
```
Victory Music: your_victory_music.mp3
Background Music: 1MAINMENU.mp3
```

**Settings:**
```
✅ Auto Setup Button Sounds (CHECK THIS!)
SFX Volume: 1
Music Volume: 0.7
```

Save the scene (Ctrl+S)!

---

## ⚡ **Step 4: Add Code** (2 minutes)

Open `Assets/Scripts/Scripts/NounsDifficultyManager.cs`

### **4a. Add Victory Music**
Find line ~1110 (`void ShowSessionSummary()`) and add at the TOP:

```csharp
void ShowSessionSummary()
{
    // ADD THIS LINE:
    if (GameAudioManager.Instance != null)
        GameAudioManager.Instance.PlayVictoryMusic();
    
    // ... existing code stays here ...
}
```

### **4b. Update Answer Sounds**
Find line ~917 and REPLACE this section:

**FROM:**
```csharp
if (optionsMenu != null)
{
    if (isCorrect)
        optionsMenu.PlayCorrectAnswerSound();
    else
        optionsMenu.PlayIncorrectAnswerSound();
}
```

**TO:**
```csharp
if (GameAudioManager.Instance != null)
{
    if (isCorrect)
        GameAudioManager.Instance.PlayCorrectAnswer();
    else
        GameAudioManager.Instance.PlayWrongAnswer();
}
```

Save the file (Ctrl+S)!

---

## ⚡ **Step 5: Test It!** (30 seconds)

1. Press Play in Unity
2. Go to Nouns → Easy Mode
3. Click a button → **Hear click sound?** ✅
4. Answer correct → **Hear success sound?** ✅
5. Answer wrong → **Hear buzzer?** ✅
6. Complete level → **Hear victory music?** ✅

---

## 🎉 **That's It!**

Your game now has:
- ✅ Button click sounds (automatic on ALL buttons!)
- ✅ Correct answer sound
- ✅ Wrong answer sound
- ✅ Victory music on summary
- ✅ Background music (if assigned)

---

## 🎨 **Optional: Add Typing Sound** (1 minute)

If you want the keyboard typing sound when text appears:

Open `Assets/Scripts/Scripts/TypewriterEffect.cs`

Find the `for` loop in `TypeText()` (around line 180)

Add this line after `textComponent.text += currentChar;`:

```csharp
// ADD THIS:
if (GameAudioManager.Instance != null && !char.IsWhiteSpace(currentChar))
    GameAudioManager.Instance.PlayTypingSound();
```

---

## 🎛️ **Volume Too Loud/Quiet?**

### **Adjust in Unity Editor:**
1. Select `GameAudioManager` in Hierarchy
2. Inspector → adjust sliders:
   - **SFX Volume**: 0-1 (try 0.7 if too loud)
   - **Music Volume**: 0-1 (try 0.5 for subtle background)

### **Typing Sound Too Loud?**
It's automatically set to 30% volume. To change:
- Open `GameAudioManager.cs`
- Find `PlayTypingSound()` method
- Change `0.3f` to `0.1f` (quieter) or `0.5f` (louder)

---

## 🐛 **Not Working?**

### **No sounds at all?**
- Check Unity Game view is not muted (speaker icon)
- Check Windows/Mac volume is up
- Check `Auto Setup Button Sounds` is checked

### **Buttons have no click sound?**
- Make sure `GameAudioManager` GameObject exists in scene
- Make sure `Auto Setup Button Sounds` is checked
- Try clicking Play, then Stop, then Play again

### **Victory music not playing?**
- Make sure you added the code to `ShowSessionSummary()`
- Make sure victory music file is assigned in Inspector
- Check you're completing all questions (summary shows)

---

## 📱 **Android Export**

Good news: **No extra setup needed!**
- Sounds work automatically on Android
- Unity includes all audio files in the build
- Volume controls work on Android too!

---

## 🎯 **Summary**

**Total time: ~5 minutes**

1. ✅ Download 5 sounds (or use existing + add 4 more)
2. ✅ Import to Assets/Audio/
3. ✅ Create GameAudioManager GameObject
4. ✅ Assign sounds in Inspector
5. ✅ Add 2 code snippets to NounsDifficultyManager
6. ✅ Test and enjoy!

**Your game now sounds professional! 🎵🎉**

For detailed explanations, see `AUDIO_SETUP_GUIDE.md`
For code snippets, see `AUDIO_CODE_SNIPPETS.md`

