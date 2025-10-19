using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class EnhancedGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons;
    public Button backButton;
    
    [Header("Adaptive Dialog System")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;
    
    [Header("Game Configuration")]
    public string topicName;
    public DifficultyLevel currentDifficulty;
    public int questionsPerLevel = 10;
    
    [Header("Session Summary")]
    public GameObject sessionSummaryPanel;
    public TextMeshProUGUI summaryText;
    public Button summaryContinueButton;
    
    // Game State
    private int currentQuestion = 0;
    private int correctAnswers = 0;
    private float sessionStartTime;
    private List<QuestionData> currentQuestions;
    private List<QuestionResult> sessionResults;
    
    // Difficulty-specific question sets
    private Dictionary<DifficultyLevel, List<QuestionData>> questionDatabase;
    
    void Start()
    {
        InitializeGame();
        LoadQuestionsForDifficulty();
        StartGameSession();
    }
    
    void InitializeGame()
    {
        sessionStartTime = Time.time;
        sessionResults = new List<QuestionResult>();
        questionDatabase = new Dictionary<DifficultyLevel, List<QuestionData>>();
        
        // Initialize question database
        InitializeQuestionDatabase();
        
        Debug.Log($"🎮 Starting {topicName} - {currentDifficulty} level");
    }
    
    void InitializeQuestionDatabase()
    {
        // Easy Level Questions (Basic Identification)
        questionDatabase[DifficultyLevel.Easy] = CreateEasyQuestions();
        
        // Medium Level Questions (Context Usage)
        questionDatabase[DifficultyLevel.Medium] = CreateMediumQuestions();
        
        // Hard Level Questions (Conversational Approach)
        questionDatabase[DifficultyLevel.Hard] = CreateHardQuestions();
    }
    
    List<QuestionData> CreateEasyQuestions()
    {
        // Basic identification questions
        List<QuestionData> questions = new List<QuestionData>();
        
        if (topicName == "Pangngalan")
        {
            questions.Add(new QuestionData
            {
                question = "Pumili ng pangngalan sa pangungusap:\n'Ang aso ay tumakbo sa parke.'",
                choices = new string[] { "aso", "tumakbo", "parke", "ay" },
                correctAnswer = 0,
                explanation = "Ang 'aso' ay isang pangngalan dahil ito ay tumutukoy sa isang hayop."
            });
            
            questions.Add(new QuestionData
            {
                question = "Ano ang pangngalan sa pangungusap:\n'Si Maria ay nagluluto ng pagkain.'",
                choices = new string[] { "Maria", "nagluluto", "pagkain", "ay" },
                correctAnswer = 0,
                explanation = "Ang 'Maria' ay isang pangngalan dahil ito ay tumutukoy sa isang tao."
            });
            
            // Add 8 more easy questions...
        }
        
        return questions;
    }
    
    List<QuestionData> CreateMediumQuestions()
    {
        // Context usage questions
        List<QuestionData> questions = new List<QuestionData>();
        
        if (topicName == "Pangngalan")
        {
            questions.Add(new QuestionData
            {
                question = "Pumili ng tamang pangngalan upang makumpleto ang pangungusap:\n'Ang ___ ay naglalaro sa bakuran.'",
                choices = new string[] { "mga bata", "naglalaro", "bakuran", "ay" },
                correctAnswer = 0,
                explanation = "Ang 'mga bata' ay angkop na pangngalan para sa pangungusap na ito."
            });
            
            // Add 9 more medium questions...
        }
        
        return questions;
    }
    
    List<QuestionData> CreateHardQuestions()
    {
        // Conversational approach questions
        List<QuestionData> questions = new List<QuestionData>();
        
        if (topicName == "Pangngalan")
        {
            questions.Add(new QuestionData
            {
                question = "Sa isang pag-uusap, kung tinanong ka kung 'Sino ang nagtuturo sa inyo?', ano ang tamang sagot na may pangngalan?",
                choices = new string[] { 
                    "Ang guro namin ay si Teacher Ana.", 
                    "Nagtuturo siya ng Filipino.", 
                    "Sa paaralan kami nag-aaral.", 
                    "Mahal ko ang Filipino." 
                },
                correctAnswer = 0,
                explanation = "Ang sagot na 'Ang guro namin ay si Teacher Ana' ay naglalaman ng dalawang pangngalan: 'guro' at 'Teacher Ana'."
            });
            
            // Add 9 more hard questions...
        }
        
        return questions;
    }
    
    void LoadQuestionsForDifficulty()
    {
        if (questionDatabase.ContainsKey(currentDifficulty))
        {
            currentQuestions = questionDatabase[currentDifficulty];
            Debug.Log($"📚 Loaded {currentQuestions.Count} questions for {currentDifficulty} level");
        }
        else
        {
            Debug.LogError($"❌ No questions found for difficulty: {currentDifficulty}");
        }
    }
    
    void StartGameSession()
    {
        // Show introduction dialog
        string introMessage = GetIntroductionMessage();
        
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(introMessage, () => {
                DisplayCurrentQuestion();
            });
        }
        else
        {
            dialogText.text = introMessage;
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(DisplayCurrentQuestion);
        }
    }
    
    string GetIntroductionMessage()
    {
        string difficultyText = currentDifficulty switch
        {
            DifficultyLevel.Easy => "Madali",
            DifficultyLevel.Medium => "Katamtaman",
            DifficultyLevel.Hard => "Mahirap",
            _ => "Madali"
        };
        
        return $"🎮 {topicName} - {difficultyText} na Antas\n\n" +
               $"Handa ka na ba para sa {questionsPerLevel} na tanong?\n\n" +
               $"Ang antas na ito ay {GetDifficultyDescription()}";
    }
    
    string GetDifficultyDescription()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => "nagtuturo ng pangunahing pagkilala ng mga salita.",
            DifficultyLevel.Medium => "nagtuturo ng paggamit ng mga salita sa konteksto.",
            DifficultyLevel.Hard => "nagtuturo ng paggamit ng mga salita sa pag-uusap.",
            _ => "nagtuturo ng pangunahing pagkilala ng mga salita."
        };
    }
    
    void DisplayCurrentQuestion()
    {
        continueButton.gameObject.SetActive(false);
        
        if (currentQuestion < currentQuestions.Count)
        {
            QuestionData question = currentQuestions[currentQuestion];
            
            if (adaptiveDialogManager != null)
            {
                adaptiveDialogManager.ShowDialog(question.question, () => {
                    DisplayChoices(question.choices);
                });
            }
            else
            {
                dialogText.text = question.question;
                DisplayChoices(question.choices);
            }
        }
        else
        {
            EndGameSession();
        }
    }
    
    void DisplayChoices(string[] choices)
    {
        if (adaptiveChoiceManager != null)
        {
            adaptiveChoiceManager.DisplayChoices(choices, OnChoiceSelected);
        }
        else
        {
            // Fallback to legacy buttons
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
                    choiceButtons[i].onClick.RemoveAllListeners();
                    
                    int choiceIndex = i;
                    choiceButtons[i].onClick.AddListener(() => OnAnswerSelected(choiceIndex));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void OnChoiceSelected(string selectedChoice)
    {
        QuestionData question = currentQuestions[currentQuestion];
        int choiceIndex = System.Array.IndexOf(question.choices, selectedChoice);
        OnAnswerSelected(choiceIndex);
    }
    
    public void OnAnswerSelected(int choiceIndex)
    {
        QuestionData question = currentQuestions[currentQuestion];
        bool isCorrect = choiceIndex == question.correctAnswer;
        float responseTime = Time.time - sessionStartTime;
        
        // Record result
        QuestionResult result = new QuestionResult
        {
            questionIndex = currentQuestion,
            selectedAnswer = choiceIndex,
            isCorrect = isCorrect,
            responseTime = responseTime,
            question = question
        };
        sessionResults.Add(result);
        
        if (isCorrect)
        {
            correctAnswers++;
        }
        
        // Hide choices
        HideChoices();
        
        // Show feedback
        ShowFeedback(question, isCorrect, choiceIndex);
        
        // Update SM2 algorithm
        if (LearningProgressionManager.Instance != null)
        {
            LearningProgressionManager.Instance.RecordQuestionAnswer(
                topicName, currentQuestion, currentDifficulty, isCorrect, responseTime, 1);
        }
    }
    
    void HideChoices()
    {
        if (adaptiveChoiceManager != null)
        {
            adaptiveChoiceManager.HideChoices();
        }
        
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
    
    void ShowFeedback(QuestionData question, bool isCorrect, int selectedIndex)
    {
        string feedbackText = GetFeedbackText(question, isCorrect, selectedIndex);
        
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(feedbackText, () => {
                ShowContinueButton();
            });
        }
        else
        {
            dialogText.text = feedbackText;
            ShowContinueButton();
        }
    }
    
    string GetFeedbackText(QuestionData question, bool isCorrect, int selectedIndex)
    {
        string feedback = isCorrect ? "✅ Tama! Magaling!" : "❌ Hindi tama.";
        
        if (!isCorrect)
        {
            feedback += $"\n\nAng tamang sagot ay: {question.choices[question.correctAnswer]}";
        }
        
        feedback += $"\n\n{question.explanation}";
        
        return feedback;
    }
    
    void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(NextQuestion);
    }
    
    public void NextQuestion()
    {
        currentQuestion++;
        continueButton.gameObject.SetActive(false);
        DisplayCurrentQuestion();
    }
    
    void EndGameSession()
    {
        HideChoices();
        continueButton.gameObject.SetActive(false);
        
        // Calculate session results
        float sessionTime = Time.time - sessionStartTime;
        float accuracy = (float)correctAnswers / currentQuestions.Count;
        
        // Update learning progression
        if (LearningProgressionManager.Instance != null)
        {
            LearningProgressionManager.Instance.UpdateTopicProgress(
                topicName, currentDifficulty, accuracy >= 0.8f, accuracy);
        }
        
        // Show session summary
        ShowSessionSummary(accuracy, sessionTime);
    }
    
    void ShowSessionSummary(float accuracy, float sessionTime)
    {
        string summary = GenerateSessionSummary(accuracy, sessionTime);
        
        // Hide choice buttons completely for summary
        if (adaptiveChoiceManager != null)
        {
            adaptiveChoiceManager.HideChoices();
        }
        
        // Show summary in adaptive dialog (this will expand vertically)
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(summary, () => {
                ShowSummaryContinueButton();
            });
        }
        else
        {
            dialogText.text = summary;
            ShowSummaryContinueButton();
        }
    }
    
    string GenerateSessionSummary(float accuracy, float sessionTime)
    {
        string levelText = currentDifficulty switch
        {
            DifficultyLevel.Easy => "Madali",
            DifficultyLevel.Medium => "Katamtaman", 
            DifficultyLevel.Hard => "Mahirap",
            _ => "Madali"
        };
        
        string performance = accuracy switch
        {
            >= 0.9f => "Napakahusay! 🌟",
            >= 0.8f => "Mahusay! 👍",
            >= 0.7f => "Mabuti! 👌",
            >= 0.6f => "Kailangan ng higit na practice. 💪",
            _ => "Kailangan ang practice. Don't give up! 🔥"
        };
        
        return $"🎮 Tapos na ang Laro!\n\n" +
               $"📚 Paksa: {topicName}\n" +
               $"📊 Antas: {levelText}\n" +
               $"📈 Katumpakan: {accuracy:P1}\n" +
               $"⏱️ Oras: {sessionTime:F1} segundo\n" +
               $"✅ Tamang sagot: {correctAnswers}/{currentQuestions.Count}\n\n" +
               $"{performance}\n\n" +
               $"📝 Pag-unlad: {GetProgressMessage(accuracy)}";
    }
    
    string GetProgressMessage(float accuracy)
    {
        if (accuracy >= 0.8f)
        {
            if (currentDifficulty != DifficultyLevel.Hard)
            {
                return "Handa ka na para sa susunod na antas!";
            }
            else
            {
                return "Naisakatuparan mo na ang paksang ito!";
            }
        }
        else
        {
            return "Patuloy na magsanay para sa mas mahusay na resulta.";
        }
    }
    
    void ShowSummaryContinueButton()
    {
        summaryContinueButton.gameObject.SetActive(true);
        summaryContinueButton.onClick.RemoveAllListeners();
        summaryContinueButton.onClick.AddListener(GoBackToMenu);
    }
    
    public void GoBackToMenu()
    {
        // Navigate back to topic selection or main menu
        SceneManager.LoadScene("Module Selection");
    }
}

[System.Serializable]
public class QuestionResult
{
    public int questionIndex;
    public int selectedAnswer;
    public bool isCorrect;
    public float responseTime;
    public QuestionData question;
}
