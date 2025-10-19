using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum QuestionType
{
    MultipleChoice,      // Easy: Choose from options
    FillInTheBlank,     // Moderate: Complete the sentence
    TypeAnswer,         // Hard: Type the answer yourself
    Conversational      // Advanced: Interactive dialogue
}

// DifficultyLevel enum is defined in LearningProgressionManager.cs

[System.Serializable]
public enum GradeLevel
{
    Kindergarten,   // Pre-school level
    Grade1,        // DepEd Grade 1 MELCs
    Grade2,        // DepEd Grade 2 MELCs
    Grade3,        // DepEd Grade 3 MELCs
    Grade4,        // DepEd Grade 4 MELCs
    Grade5,        // DepEd Grade 5 MELCs
    Grade6         // DepEd Grade 6 MELCs
}

[System.Serializable]
public class EnhancedQuestionData
{
    [Header("Basic Info")]
    public int questionId;
    public string module;
    public QuestionType questionType;
    public DifficultyLevel difficultyLevel;
    public GradeLevel gradeLevel;
    
    [Header("Curriculum Alignment")]
    public string depEdMELC;           // DepEd Most Essential Learning Competency
    public string curriculumCode;      // Curriculum code (e.g., F1WG-Ia-e-1)
    public string learningObjective;   // Specific learning objective
    
    [Header("Question Content")]
    public string questionText;
    public string[] choices;           // For multiple choice
    public string correctAnswer;       // For all types
    public string[] acceptableAnswers; // For type answer (multiple correct variations)
    
    [Header("Fill-in-the-Blank")]
    public string sentenceTemplate;    // "The ___ is red" -> "The apple is red"
    public string blankWord;           // Word to fill in
    public int blankPosition;         // Position in sentence
    
    [Header("Conversational")]
    public string[] conversationPrompts;
    public string[] characterResponses;
    public string[] hints;
    
    [Header("SM2 Algorithm Data")]
    public int interval;
    public int repetitions;
    public float easeFactor;
    public float mastery;
    public float quality;
    public DateTime nextReview;
    public DateTime firstSeen;
    public List<float> responseTimes;
    public List<float> qualityHistory;
    public int consecutiveCorrectAnswers;
    public int totalAttempts;
    public float averageResponseTime;
    
    [Header("Gamification")]
    public int baseXP;
    public int streakBonus;
    public string[] achievements;
    public string feedbackMessage;
    
    [Header("Learning Analytics")]
    public float difficultyRating;     // 0.0 to 1.0
    public string[] learningTags;      // Categories for analytics
    public string[] prerequisiteTags; // Required knowledge
}

[System.Serializable]
public class DifficultyProgression
{
    [Header("Progression Settings")]
    public DifficultyLevel currentLevel;
    public int questionsPerLevel;
    public float masteryThreshold;    // Required mastery to advance
    public float accuracyThreshold;   // Required accuracy to advance
    
    [Header("Level Requirements")]
    public int easyQuestionsCompleted;
    public int moderateQuestionsCompleted;
    public int hardQuestionsCompleted;
    public int expertQuestionsCompleted;
    
    [Header("Performance Tracking")]
    public float overallAccuracy;
    public float averageResponseTime;
    public int currentStreak;
    public int longestStreak;
    
    [Header("Unlocked Content")]
    public bool[] unlockedLevels;
    public string[] unlockedAchievements;
    public string[] unlockedConversations;
}
