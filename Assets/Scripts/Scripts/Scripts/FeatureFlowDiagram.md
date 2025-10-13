# Feature Flow Diagram

## 🔄 Complete Game Flow with New Features

```
┌─────────────────────────────────────────────────────────────────┐
│                    GAME STARTS                                   │
│                    (Nouns Scene Loaded)                          │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  INTRODUCTION DIALOG                                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  📜 Scrollable Dialog Box                                 │  │
│  │  "Welcome to Nouns Quiz! Test your knowledge..."          │  │
│  │  ▲ ▼ Scroll to read more                                  │  │
│  │  [Custom Scroll Arrow Visible]                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│  Code: adaptiveDialogManager.ShowIntroductionDialog()           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  QUESTION 1 - Normal Display (No Scrolling)                     │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  "Which is a proper noun?"                                │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 1 (BROWN) │  │ Choice 2 (BROWN) │                    │
│  └──────────────────┘  └──────────────────┘                    │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 3 (BROWN) │  │ Choice 4 (BROWN) │                    │
│  └──────────────────┘  └──────────────────┘                    │
│  All buttons: DEFAULT state (brown)                             │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
            ┌────────────────┴────────────────┐
            │   Player Clicks Choice 2        │
            └────────────────┬────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  ANSWER FEEDBACK - Button Changes Color                         │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  "Correct! Manila is a proper noun."                      │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 1 (BROWN) │  │ Choice 2 (GREEN) │ ✅ Correct         │
│  └──────────────────┘  └──────────────────┘                    │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 3 (BROWN) │  │ Choice 4 (BROWN) │                    │
│  └──────────────────┘  └──────────────────┘                    │
│                                                                  │
│  Code Flow:                                                      │
│  1. ShowButtonFeedback(1, true) → GREEN                         │
│  2. DisableAllChoiceButtons() → Can't click again              │
│  3. Play correct sound + haptic feedback                        │
│  4. Update score, analytics, SM2 data                           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  QUESTION 2 - Reset to Default                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  "Which is an abstract noun?"                             │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 1 (BROWN) │  │ Choice 2 (BROWN) │ 🔄 All Reset       │
│  └──────────────────┘  └──────────────────┘                    │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 3 (BROWN) │  │ Choice 4 (BROWN) │                    │
│  └──────────────────┘  └──────────────────┘                    │
│                                                                  │
│  Code: ResetAllButtonFeedback() called automatically            │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
            ┌────────────────┴────────────────┐
            │   Player Clicks Choice 1        │
            │   (Wrong Answer)                │
            └────────────────┬────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  WRONG ANSWER FEEDBACK - Red Button                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  "Incorrect. The correct answer is: Love (pag-ibig)"      │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 1 (RED)   │  │ Choice 2 (BROWN) │ ❌ Wrong            │
│  └──────────────────┘  └──────────────────┘                    │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ Choice 3 (BROWN) │  │ Choice 4 (BROWN) │                    │
│  └──────────────────┘  └──────────────────┘                    │
│                                                                  │
│  Code Flow:                                                      │
│  1. ShowButtonFeedback(0, false) → RED                          │
│  2. DisableAllChoiceButtons()                                   │
│  3. Play wrong sound + haptic feedback                          │
│  4. Track mistake, update analytics                             │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
                    [More Questions...]
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  SESSION SUMMARY - Scrollable                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  📊 SESSION SUMMARY                                        │  │
│  │  ════════════════════                                      │  │
│  │  Total Questions: 10                                       │  │
│  │  Correct: 8 (80%)                                          │  │
│  │  Score: 850 points                                         │  │
│  │  ▲ ▼ Scroll for more details                              │  │
│  │  [Custom Scroll Arrow Visible]                            │  │
│  │                                                             │  │
│  │  DETAILED BREAKDOWN:                                       │  │
│  │  Q1: Correct (+100) - Response: 3.2s                      │  │
│  │  Q2: Wrong (-50) - Response: 5.1s                         │  │
│  │  Q3: Correct (+100) - Response: 2.8s                      │  │
│  │  ... (scrollable content)                                  │  │
│  └──────────────────────────────────────────────────────────┘  │
│  Code: adaptiveDialogManager.ShowSessionSummary()               │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Button State Transitions

```
╔════════════════════════════════════════════════════════════╗
║  BUTTON LIFECYCLE                                           ║
╚════════════════════════════════════════════════════════════╝

START OF QUESTION
    │
    ├─→ [BROWN BUTTON] ◄─── Default State
    │   └─ Sprite: "Choice Button 1"
    │   └─ Interactable: YES
    │
    ▼
PLAYER CLICKS
    │
    ├─→ IF CORRECT:
    │   └─→ [GREEN BUTTON] ✅
    │       └─ Sprite: "Choice Button2"
    │       └─ Interactable: NO (all buttons disabled)
    │       └─ Sound: Correct beep
    │       └─ Haptic: Success vibration
    │
    └─→ IF WRONG:
        └─→ [RED BUTTON] ❌
            └─ Sprite: "Choice Button3"
            └─ Interactable: NO (all buttons disabled)
            └─ Sound: Wrong buzz
            └─ Haptic: Error vibration
            │
            ▼
NEXT QUESTION LOADS
    │
    └─→ [BROWN BUTTON] ◄─── Reset to Default
        └─ All buttons back to brown
        └─ Interactable: YES
        └─ Ready for next answer
```

---

## 📜 Scroll Behavior Flow

```
╔════════════════════════════════════════════════════════════╗
║  DIALOG SCROLL BEHAVIOR                                     ║
╚════════════════════════════════════════════════════════════╝

┌──────────────────┬────────────────┬──────────────────────┐
│   Dialog Type    │   Scrollable?  │  Custom Arrow Shows? │
├──────────────────┼────────────────┼──────────────────────┤
│ Introduction     │      ✅ YES    │      ✅ YES          │
│ Session Summary  │      ✅ YES    │      ✅ YES          │
│ Regular Question │      ❌ NO     │      ❌ NO           │
│ Answer Feedback  │      ❌ NO     │      ❌ NO           │
└──────────────────┴────────────────┴──────────────────────┘

HOW IT DETERMINES:

Introduction:
    ShowIntroductionDialog()
    └─→ Sets: longTextThreshold = 0
    └─→ Sets: useScrollableForLongText = true
    └─→ Result: ALWAYS scrollable

Session Summary:
    ShowSessionSummary()
    └─→ Sets: longTextThreshold = 0
    └─→ Sets: useScrollableForLongText = true
    └─→ Result: ALWAYS scrollable

Regular Questions:
    ShowDialog()
    └─→ Uses: default settings
    └─→ Result: NOT scrollable (text auto-sizes instead)
```

---

## 🔧 Code Execution Flow

```
╔════════════════════════════════════════════════════════════╗
║  CODE EXECUTION SEQUENCE                                    ║
╚════════════════════════════════════════════════════════════╝

GAME START
    │
    ├─→ NounsGameManager.Start()
    │   └─→ SetupButtons()
    │       └─→ For each choiceButton:
    │           └─→ AddComponent<ChoiceButtonFeedback>()
    │               └─→ Awake() loads sprites from Resources
    │                   └─→ "Choice Button 1" (brown)
    │                   └─→ "Choice Button2" (green)
    │                   └─→ "Choice Button3" (red)
    │
    └─→ StartDialog()
        └─→ ShowIntroductionDialog()
            └─→ adaptiveDialogManager.ShowIntroductionDialog(text)
                └─→ Forces scrollable mode
                └─→ Displays with ScrollRect enabled

DISPLAY QUESTION
    │
    └─→ DisplayQuestion()
        └─→ ShowDialog(questionText)
            └─→ Normal display (no scroll)
        └─→ DisplayChoices(choices[])
            └─→ ResetAllButtonFeedback()
                └─→ All buttons → BROWN
            └─→ EnableAllChoiceButtons()
                └─→ All buttons → interactable = true

PLAYER CLICKS BUTTON
    │
    └─→ OnChoiceButtonClick(buttonIndex)
        └─→ ProcessAnswer(answer, correct?, buttonIndex)
            ├─→ ShowButtonFeedback(buttonIndex, isCorrect)
            │   └─→ If correct: button.ShowCorrect() → GREEN
            │   └─→ If wrong: button.ShowWrong() → RED
            │
            ├─→ DisableAllChoiceButtons()
            │   └─→ All buttons → interactable = false
            │
            ├─→ Play sounds
            ├─→ Haptic feedback
            ├─→ Update score
            └─→ Track analytics

NEXT QUESTION
    │
    └─→ DisplayQuestion() ← Loop back to "DISPLAY QUESTION"

END OF GAME
    │
    └─→ DisplaySessionSummary()
        └─→ adaptiveDialogManager.ShowSessionSummary(summary)
            └─→ Forces scrollable mode
            └─→ Long text displays with scroll
```

---

## 🗂️ File Dependency Map

```
╔════════════════════════════════════════════════════════════╗
║  FILE DEPENDENCIES                                          ║
╚════════════════════════════════════════════════════════════╝

NounsGameManager.cs
    │
    ├─→ Uses: ChoiceButtonFeedback.cs (NEW)
    │   └─→ Controls: Button sprite changes
    │   └─→ Loads: Resources/ChoiceButton/*
    │
    └─→ Uses: AdaptiveDialogManager.cs
        │
        ├─→ ShowIntroductionDialog() (NEW)
        │   └─→ Enables scrolling for intro
        │
        ├─→ ShowSessionSummary()
        │   └─→ Enables scrolling for summary
        │
        └─→ ShowDialog()
            └─→ Normal display for questions

TypewriterEffect.cs
    │
    └─→ Used by: NounsGameManager (fallback)
        └─→ Provides animated text display

Resources/ChoiceButton/
    │
    ├─→ Choice Button 1 (Sprite)
    ├─→ Choice Button2 (Sprite)
    └─→ Choice Button3 (Sprite)
        │
        └─→ Loaded by: ChoiceButtonFeedback.Awake()
```

---

## 🎮 User Experience Flow

```
╔════════════════════════════════════════════════════════════╗
║  PLAYER EXPERIENCE                                          ║
╚════════════════════════════════════════════════════════════╝

STEP 1: Game Loads
    ↓
    Player sees: Scrollable introduction
    Player action: Read & scroll through intro
    Visual: Custom scroll arrow visible

STEP 2: First Question
    ↓
    Player sees: Question with 4 brown buttons
    Player action: Read question, tap a choice
    Visual: All buttons are brown (default)

STEP 3: Answer Feedback (Correct)
    ↓
    Player sees: Clicked button turns GREEN ✅
    Player hears: Success sound "ding!"
    Player feels: Haptic success vibration
    Visual: One green button, others brown

STEP 4: Next Question
    ↓
    Player sees: New question, all buttons brown again
    Player action: Read and answer
    Visual: All buttons reset to default

STEP 5: Answer Feedback (Wrong)
    ↓
    Player sees: Clicked button turns RED ❌
    Player hears: Wrong sound "buzz"
    Player feels: Haptic error vibration
    Visual: One red button, others brown
    Message: Shows correct answer

STEP 6: Continue...
    ↓
    [Repeat Steps 4-5 for remaining questions]

STEP 7: Session Summary
    ↓
    Player sees: Scrollable summary with stats
    Player action: Scroll to see detailed breakdown
    Visual: Custom scroll arrow visible
    Content: Score, accuracy, question-by-question results
```

---

## 📊 Data Flow

```
╔════════════════════════════════════════════════════════════╗
║  BUTTON STATE DATA                                          ║
╚════════════════════════════════════════════════════════════╝

ChoiceButtonFeedback Component (per button)
    │
    ├─ buttonImage: Image
    ├─ defaultSprite: Sprite (brown)
    ├─ correctSprite: Sprite (green)
    ├─ wrongSprite: Sprite (red)
    └─ currentState: ButtonState enum
        ├─ Default
        ├─ Correct
        └─ Wrong

NounsGameManager Tracking
    │
    ├─ sessionCorrectAnswers: int
    ├─ sessionTotalAnswers: int
    ├─ questionResponseTimes: Dictionary<int, float>
    ├─ questionAttempts: Dictionary<int, int>
    └─ Current button states managed by ChoiceButtonFeedback
```

---

**Visual Guide Version**: 1.0  
**Last Updated**: October 13, 2025  
**Purpose**: Understanding feature flow and behavior

