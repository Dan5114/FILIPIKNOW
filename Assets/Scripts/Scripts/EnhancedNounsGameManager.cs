using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class EnhancedNounsGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons;
    public Button backButton;
    
    [Header("Adaptive Dialog System")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;
    
    [Header("Learning System")]
    public QuestionDatabase questionDatabase;
    public DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;
    public TMP_FontAsset timesBoldFont;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Options Menu")]
    public OptionsMenu optionsMenu;
    
    [Header("Universal Font")]
    public bool useUniversalFont = true;
    
    // Game State
    private int currentQuestion = 0;
    private int correctAnswers = 0;
    private float sessionStartTime;
    private List<QuestionData> currentQuestions;
    private List<QuestionResult> sessionResults;
    
    // Language Settings
    private bool isEnglish = false;
    
    void Start()
    {
        InitializeGameSession();
        LoadCurrentDifficulty();
        StartGameSession();
    }
    
    void InitializeGameSession()
    {
        // Get difficulty from SceneController
        if (SceneController.Instance != null)
        {
            currentDifficulty = SceneController.Instance.GetSelectedDifficulty();
        }
        
        // Check language preference
        if (SettingsManager.Instance != null)
        {
            isEnglish = !SettingsManager.Instance.IsFilipinoLanguage();
        }
        
        // Set font
        if (useUniversalFont && FilipknowFontManager.Instance != null)
        {
            FilipknowFontManager.Instance.ApplyUniversalFont();
        }
        
        // Initialize question database
        if (questionDatabase == null)
        {
            questionDatabase = FindObjectOfType<QuestionDatabase>();
        }
        
        if (questionDatabase == null)
        {
            Debug.LogError("❌ QuestionDatabase not found! Please assign it in the inspector.");
        }
        
        sessionResults = new List<QuestionResult>();
    }
    
    void LoadCurrentDifficulty()
    {
        if (questionDatabase != null)
        {
            currentQuestions = questionDatabase.GetQuestions(currentDifficulty);
            Debug.Log($"📚 Loaded {currentQuestions.Count} questions for {currentDifficulty} level");
        }
        else
        {
            Debug.LogError("❌ QuestionDatabase is null! Cannot load questions.");
        }
    }
    
    void StartGameSession()
    {
        sessionStartTime = Time.time;
        correctAnswers = 0;
        currentQuestion = 0;
        
        // Start with introduction dialog
        ShowIntroductionDialog();
    }
    
    void ShowIntroductionDialog()
    {
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
        
        return $"🎮 Pangngalan - {difficultyText} na Antas\n\n" +
               $"Handa ka na ba para sa 10 na tanong?\n\n" +
               $"Ang antas na ito ay {GetDifficultyDescription()}";
    }
    
    string GetDifficultyDescription()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => "nagtuturo ng pangunahing pagkilala ng mga pangngalan.",
            DifficultyLevel.Medium => "nagtuturo ng paggamit ng mga pangngalan sa konteksto.",
            DifficultyLevel.Hard => "nagtuturo ng paggamit ng mga pangngalan sa pag-uusap.",
            _ => "nagtuturo ng pangunahing pagkilala ng mga pangngalan."
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
                "Pangngalan", currentQuestion, currentDifficulty, isCorrect, responseTime, 1);
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
                "Pangngalan", currentDifficulty, accuracy >= 0.8f, accuracy);
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
               $"📚 Paksa: Pangngalan\n" +
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
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(GoBackToMenu);
    }
    
    public void GoBackToMenu()
    {
        // Navigate back to topic selection or main menu
        SceneManager.LoadScene("Module Selection");
    }
}