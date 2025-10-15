# 🔧 Audio Code Snippets - Copy & Paste Ready!

Use these snippets to quickly add audio to your existing scripts.

---

## 📝 **1. NounsDifficultyManager.cs**

### **Add Victory Music to Summary**

Find the `ShowSessionSummary()` method (around line 1110) and add at the **BEGINNING**:

```csharp
void ShowSessionSummary()
{
    // 🎉 PLAY VICTORY MUSIC!
    if (GameAudioManager.Instance != null)
    {
        GameAudioManager.Instance.PlayVictoryMusic();
    }
    
    float accuracy = (float)sessionCorrectAnswers / sessionTotalAnswers;
    string summary = $"📊 Session Complete!\n\n";
    // ... rest of your existing code ...
}
```

### **Update Answer Sounds**

Find the `ProcessAnswer()` method (around line 892) and **REPLACE** the sound effect section:

**OLD CODE (around line 917-923):**
```csharp
// Play sound effects
if (optionsMenu != null)
{
    if (isCorrect)
        optionsMenu.PlayCorrectAnswerSound();
    else
        optionsMenu.PlayIncorrectAnswerSound();
}
```

**NEW CODE:**
```csharp
// Play sound effects
if (GameAudioManager.Instance != null)
{
    if (isCorrect)
        GameAudioManager.Instance.PlayCorrectAnswer();
    else
        GameAudioManager.Instance.PlayWrongAnswer();
}
else if (optionsMenu != null)
{
    // Fallback to old system
    if (isCorrect)
        optionsMenu.PlayCorrectAnswerSound();
    else
        optionsMenu.PlayIncorrectAnswerSound();
}
```

---

## 📝 **2. TypewriterEffect.cs**

### **Add Typing Sound**

Find the `TypeText()` coroutine (look for `IEnumerator TypeText()`).

Find the section where it shows each character (should have a for loop with `i < fullText.Length`).

**Around line 170-190, find this:**
```csharp
for (int i = 0; i < fullText.Length; i++)
{
    currentChar = fullText[i];
    textComponent.text += currentChar;
    
    // EXISTING CODE: Play audio if available
    if (typingSound != null && audioSource != null)
    {
        float timeSinceLastSound = Time.time - lastSoundTime;
        if (timeSinceLastSound >= soundInterval)
        {
            audioSource.PlayOneShot(typingSound);
            lastSoundTime = Time.time;
        }
    }
    
    // ... rest of code
}
```

**ADD THIS LINE after `textComponent.text += currentChar;`:**
```csharp
for (int i = 0; i < fullText.Length; i++)
{
    currentChar = fullText[i];
    textComponent.text += currentChar;
    
    // ⌨️ NEW: Play typing sound using GameAudioManager
    if (GameAudioManager.Instance != null && !char.IsWhiteSpace(currentChar))
    {
        GameAudioManager.Instance.PlayTypingSound();
    }
    
    // OLD: Keep this for backward compatibility
    if (typingSound != null && audioSource != null)
    {
        float timeSinceLastSound = Time.time - lastSoundTime;
        if (timeSinceLastSound >= soundInterval)
        {
            audioSource.PlayOneShot(typingSound);
            lastSoundTime = Time.time;
        }
    }
    
    // ... rest of code
}
```

---

## 📝 **3. SettingsManager.cs**

### **Integrate with GameAudioManager Volume Controls**

Find the `SetSoundEffectsVolume()` method (around line 104) and add:

```csharp
public void SetSoundEffectsVolume(float volume)
{
    soundEffectsVolume = Mathf.Clamp01(volume);
    
    // 🔊 UPDATE: Sync with GameAudioManager
    if (GameAudioManager.Instance != null)
    {
        GameAudioManager.Instance.SetSFXVolume(volume);
    }
    
    UpdateAudioSettings();
    SaveSettings();
    
    Debug.Log($"Sound Effects volume set to {soundEffectsVolume * 100}%");
}
```

Find the `SetMusicVolume()` method (around line 95) and add:

```csharp
public void SetMusicVolume(float volume)
{
    musicVolume = Mathf.Clamp01(volume);
    
    // 🎵 UPDATE: Sync with GameAudioManager  
    if (GameAudioManager.Instance != null)
    {
        GameAudioManager.Instance.SetMusicVolume(volume);
    }
    
    UpdateAudioSettings();
    SaveSettings();
    
    Debug.Log($"Music volume set to {musicVolume * 100}%");
}
```

---

## 📝 **4. NounsGameManager.cs** (if you use it)

If you're using the main `NounsGameManager.cs` script, add the same audio calls as NounsDifficultyManager.

### **When answer is correct:**
```csharp
if (GameAudioManager.Instance != null)
{
    GameAudioManager.Instance.PlayCorrectAnswer();
}
```

### **When answer is wrong:**
```csharp
if (GameAudioManager.Instance != null)
{
    GameAudioManager.Instance.PlayWrongAnswer();
}
```

### **On level complete/summary:**
```csharp
if (GameAudioManager.Instance != null)
{
    GameAudioManager.Instance.PlayVictoryMusic();
}
```

---

## 📝 **5. MainMenu.cs** (Optional - Background Music)

If you want background music on the main menu:

Add at the end of `Start()` method:

```csharp
void Start()
{
    // ... your existing code ...
    
    // 🎵 Play background music
    if (GameAudioManager.Instance != null)
    {
        GameAudioManager.Instance.PlayBackgroundMusic();
    }
}
```

---

## 📝 **6. Module Selection Scenes** (Optional)

If you want button sounds on module selection screens, add to each button's `onClick` event:

```csharp
public void OnModuleButtonClicked()
{
    // 🔊 Play click sound
    if (GameAudioManager.Instance != null)
    {
        GameAudioManager.Instance.PlayButtonClick();
    }
    
    // Your existing code
    LoadModule();
}
```

---

## 🎯 **Quick Integration Checklist**

Copy-paste these snippets in order:

1. ✅ **NounsDifficultyManager.cs**
   - [ ] Add victory music to `ShowSessionSummary()`
   - [ ] Update answer sounds in `ProcessAnswer()`

2. ✅ **TypewriterEffect.cs**
   - [ ] Add typing sound in `TypeText()` coroutine

3. ✅ **SettingsManager.cs**
   - [ ] Update `SetSoundEffectsVolume()`
   - [ ] Update `SetMusicVolume()`

4. ✅ **Test in Unity**
   - [ ] Play game
   - [ ] Test each sound type

---

## 🔍 **Where to Find Methods**

If you can't find the methods, use Ctrl+F (or Cmd+F on Mac) to search:

- **ShowSessionSummary**: Search for `void ShowSessionSummary()`
- **ProcessAnswer**: Search for `void ProcessAnswer(string answer)`
- **TypeText coroutine**: Search for `IEnumerator TypeText()`
- **SetSoundEffectsVolume**: Search for `void SetSoundEffectsVolume(`
- **SetMusicVolume**: Search for `void SetMusicVolume(`

---

## 💡 **Pro Tips**

1. **Test one sound at a time** - Add victory music first, test, then add typing sound, etc.

2. **Keep old code** - The snippets keep your existing `optionsMenu` calls as fallback

3. **Check for null** - Always check `if (GameAudioManager.Instance != null)` before using

4. **Volume control** - Use SettingsManager volume sliders to adjust all sounds

5. **Mobile testing** - Sounds work automatically on Android, no extra setup!

---

**Questions?** These snippets should work copy-paste! Just follow the line numbers and method names. 🎵

