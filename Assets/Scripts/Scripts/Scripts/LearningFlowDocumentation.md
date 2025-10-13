# 🎮 **Complete Learning Flow Documentation**

## **📋 System Overview**

### **Current State → Target State**
- **Before:** 5 questions per topic (Easy only)
- **After:** 10 questions per topic across 3 difficulty levels

---

## **🔄 Complete Learning Flow**

### **1. Main Menu Navigation**
```
Main Menu → Modules Available → Module Selection → Topic Selection → Difficulty Selection → Game Play
```

### **2. Module Structure**
```
📚 Module 1: Nouns (Pangngalan)
├── 🟢 Easy (10 questions) - Basic identification
├── 🟡 Medium (10 questions) - Context usage  
└── 🔴 Hard (10 questions) - Conversational approach

📚 Module 2: Verbs (Pand'iwa)
├── 🟢 Easy (10 questions) - Basic identification
├── 🟡 Medium (10 questions) - Context usage
└── 🔴 Hard (10 questions) - Conversational approach

📚 Module 3: Adjectives (Pang-uri)
├── 🟢 Easy (10 questions) - Basic identification
├── 🟡 Medium (10 questions) - Context usage
└── 🔴 Hard (10 questions) - Conversational approach
```

---

## **🧠 SM2 Algorithm Integration**

### **Spaced Repetition System:**
- **Easy Questions:** 1 day interval
- **Medium Questions:** 3 day interval  
- **Hard Questions:** 7 day interval
- **Mastery Level:** 14+ day interval

### **Algorithm Parameters:**
- **Initial Ease Factor:** 2.5
- **Easy Interval:** 1 day
- **Medium Interval:** 3 days
- **Hard Interval:** 7 days
- **Mastery Threshold:** 80% accuracy

---

## **🎯 Difficulty Level Progression**

### **🟢 Easy Level (Basic Identification)**
- **Focus:** Simple word identification
- **Example:** "Pumili ng pangngalan sa pangungusap: 'Ang aso ay tumakbo sa parke.'"
- **Choices:** [aso, tumakbo, parke, ay]
- **Goal:** Build foundational vocabulary recognition

### **🟡 Medium Level (Context Usage)**
- **Focus:** Using words in context
- **Example:** "Pumili ng tamang pangngalan upang makumpleto ang pangungusap: 'Ang ___ ay naglalaro sa bakuran.'"
- **Choices:** [mga bata, naglalaro, bakuran, ay]
- **Goal:** Develop contextual understanding

### **🔴 Hard Level (Conversational Approach)**
- **Focus:** Real conversation scenarios
- **Example:** "Sa isang pag-uusap, kung tinanong ka kung 'Sino ang nagtuturo sa inyo?', ano ang tamang sagot na may pangngalan?"
- **Choices:** [Ang guro namin ay si Teacher Ana., Nagtuturo siya ng Filipino., Sa paaralan kami nag-aaral., Mahal ko ang Filipino.]
- **Goal:** Master practical application

---

## **💾 Progress Tracking System**

### **Topic Progress Structure:**
```csharp
public class TopicProgress
{
    public string topicName;
    public DifficultyLevel currentLevel;
    public bool isEasyCompleted;
    public bool isMediumCompleted;
    public bool isHardCompleted;
    public DateTime lastPlayedDate;
    public float masteryScore; // 0-1 scale
    public List<QuestionProgress> questionHistory;
}
```

### **Question Progress Tracking:**
```csharp
public class QuestionProgress
{
    public int questionId;
    public DifficultyLevel difficulty;
    public bool isCorrect;
    public float responseTime;
    public int attempts;
    public DateTime timestamp;
    public int intervalDays; // SM2 algorithm interval
    public float easeFactor; // SM2 algorithm ease factor
    public int repetitions; // Number of times reviewed
}
```

---

## **🎮 Game Session Flow**

### **1. Session Start**
- Show topic introduction
- Display difficulty level
- Explain learning objectives

### **2. Question Loop (10 questions)**
- Display question using AdaptiveDialogManager
- Show choices using AdaptiveChoiceManager
- Record response and timing
- Provide immediate feedback
- Update SM2 algorithm parameters

### **3. Session Summary**
- Calculate accuracy percentage
- Show performance feedback
- Display progress indicators
- **Hide choice buttons completely**
- **Expand dialog vertically for summary**
- Provide motivational message

### **4. Progress Update**
- Update topic completion status
- Unlock next difficulty level (if applicable)
- Save progress to persistent storage

---

## **🔧 Technical Implementation**

### **Core Components:**

1. **LearningProgressionManager.cs**
   - Manages all topic progress
   - Implements SM2 algorithm
   - Handles data persistence

2. **EnhancedGameManager.cs**
   - Replaces basic game managers
   - Supports 3 difficulty levels
   - Integrates with adaptive dialog system

3. **DifficultySelectionManager.cs**
   - Manages difficulty selection UI
   - Shows progress indicators
   - Handles access control

4. **AdaptiveDialogManager.cs** (Enhanced)
   - Auto-expanding dialog boxes
   - Mobile-optimized text rendering
   - Session summary support

5. **AdaptiveChoiceManager.cs**
   - Auto-sizing choice buttons
   - Dynamic button management
   - Hide/show functionality

### **Scene Management:**
- **Main Menu** → **Modules Available** → **Module Selection** → **Topic Selection** → **Difficulty Selection** → **Game Play**

---

## **📱 UI/UX Improvements**

### **Session Summary Display:**
- **Vertical expansion** to accommodate all text
- **Choice buttons completely hidden** during summary
- **Mobile-optimized font sizes**
- **Clear progress indicators**

### **Difficulty Selection UI:**
- **Visual progress indicators** (✅ for completed levels)
- **Locked state** for inaccessible levels
- **Color coding** (Green/Easy, Yellow/Medium, Red/Hard)
- **Status text** (TAPOS, SIMULAN, NAKAKULONG)

### **Progress Persistence:**
- **Automatic saving** after each session
- **Cross-session progress tracking**
- **SM2 algorithm state preservation**

---

## **🎯 Learning Outcomes**

### **By completing Easy level:**
- Master basic word identification
- Build foundational vocabulary
- Unlock Medium level

### **By completing Medium level:**
- Understand contextual word usage
- Develop sentence construction skills
- Unlock Hard level

### **By completing Hard level:**
- Master conversational Filipino
- Apply knowledge in real scenarios
- Complete topic mastery

---

## **🚀 Implementation Status**

### **✅ Completed:**
- LearningProgressionManager with SM2 algorithm
- EnhancedGameManager with 3-level support
- DifficultySelectionManager with UI
- AdaptiveDialogManager improvements
- Progress tracking system
- Scene navigation updates

### **🔄 Next Steps:**
1. Create difficulty selection scenes
2. Update existing game managers
3. Implement question databases
4. Test complete flow
5. Add analytics and reporting

---

## **📊 Expected Results**

### **Learning Progression:**
- **30 questions per topic** (10 per difficulty)
- **SM2-based spaced repetition**
- **Progressive difficulty unlocking**
- **Persistent progress tracking**

### **User Experience:**
- **Clear visual feedback** on progress
- **Motivational messaging** based on performance
- **Intuitive navigation** between levels
- **Mobile-optimized** interface

This comprehensive system ensures a structured, engaging, and effective learning experience that aligns with Filipino DepED curriculum standards while incorporating modern spaced repetition techniques.
