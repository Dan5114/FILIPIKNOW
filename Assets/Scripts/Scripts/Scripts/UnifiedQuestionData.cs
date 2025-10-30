using UnityEngine;

[System.Serializable]
public class UnifiedQuestionData
{
    public int questionId;
    public DifficultyLevel difficultyLevel;
    public QuestionType questionType;
    [TextArea(3, 20)] public string questionText;
    [TextArea(3, 20)]public string instruction;
    public int xpReward;
    
    [Header("GENERAL")]
    // Multiple Choice (Easy)
    public string[] choices;
    [Tooltip("Only add 1, kinda buggy rn")] public string[] acceptableAnswers;
    public int correctChoiceIndex;
    
    [Header("MEDIUM MODE")]
    // Fill-in-the-Blank (Medium)
    [TextArea(3, 20)] public string sentenceTemplate;
    public string blankWord;
    
    [Header("HARD MODE")]
    // Type Answer + Conversational (Hard)
    public string correctAnswer;
    public bool isConversational = false;
    public string[] conversationPrompts;
    public string[] characterResponses;
    public string[] hints;
}
