using UnityEngine;

[System.Serializable]
public class UnifiedQuestionData
{
    public int questionId;
    public DifficultyLevel difficultyLevel;
    public QuestionType questionType;
    public string questionText;
    public string instruction;
    public int xpReward;
    
    // Multiple Choice (Easy)
    public string[] choices;
    public int correctChoiceIndex;
    
    // Fill-in-the-Blank (Medium)
    public string sentenceTemplate;
    public string blankWord;
    public string[] acceptableAnswers;
    
    // Type Answer + Conversational (Hard)
    public string correctAnswer;
    public bool isConversational = false;
    public string[] conversationPrompts;
    public string[] characterResponses;
    public string[] hints;
}
