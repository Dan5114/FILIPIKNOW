using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class NumbersGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons; // Legacy - will be replaced by adaptive system
    public Button backButton;
    
    [Header("Adaptive Dialog System")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;
    
    [Header("Options Menu")]
    public OptionsMenu optionsMenu;
    
    [Header("Universal Font")]
    public bool useUniversalFont = true;
    
    [Header("Font Settings")]
    public TMP_FontAsset timesBoldFont;

    private int currentQuestion = 0;
    private int score = 0;
    private List<QuestionData> reviewQuestions = new List<QuestionData>();
    
    // Dialog content - Filipino
    private string[] dialogMessagesFilipino = {
        "Kumusta! Ako si Teacher Ana.",
        "Ngayon ay matututunan natin ang tungkol sa MGA NUMERO.",
        "Ang mga numero ay mga simbolo o salita na ginagamit sa pagbilang at pagsukat.",
        "Halimbawa: isa, dalawa, tatlo, apat, lima",
        "Handa ka na ba para sa ating laro?"
    };
    
    // Dialog content - English
    private string[] dialogMessagesEnglish = {
        "Hello! I am Teacher Ana.",
        "Today we will learn about NUMBERS.",
        "Numbers are symbols or words used for counting and measuring.",
        "Examples: one, two, three, four, five",
        "Are you ready for our game?"
    };
    
    private int currentDialogIndex = 0;
    
    // Game content - Filipino
    private string[] questionsFilipino = {
        "Ano ang Filipino word para sa number 7?",
        "I-translate ang 'Labing-apat' sa English:",
        "Ano ang 5 plus 3 sa Filipino?",
        "Anong number ang 'Labinsiyam'?",
        "I-translate ang 'Dalawampu' sa English:"
    };
    
    private string[][] choicesFilipino = {
        new string[] { "Anim", "Pito", "Walo", "Siyam" },
        new string[] { "Eleven", "Twelve", "Thirteen", "Fourteen" },
        new string[] { "Pito", "Walo", "Siyam", "Sampu" },
        new string[] { "17", "18", "19", "20" },
        new string[] { "Ten", "Twenty", "Thirty", "Forty" }
    };
    
    private int[] correctAnswersFilipino = { 1, 3, 1, 2, 1 }; // Index of correct answer for each question
    
    // Game content - English
    private string[] questionsEnglish = {
        "What is the Filipino word for number 7?",
        "Translate 'Labing-apat' to English:",
        "What is 5 plus 3 in Filipino?",
        "What number is 'Labinsiyam'?",
        "Translate 'Dalawampu' to English:"
    };
    
    private string[][] choicesEnglish = {
        new string[] { "Anim", "Pito", "Walo", "Siyam" },
        new string[] { "Eleven", "Twelve", "Thirteen", "Fourteen" },
        new string[] { "Pito", "Walo", "Siyam", "Sampu" },
        new string[] { "17", "18", "19", "20" },
        new string[] { "Ten", "Twenty", "Thirty", "Forty" }
    };
    
    private int[] correctAnswersEnglish = { 1, 3, 1, 2, 1 }; // Index of correct answer for each question
    
    private string[] feedbackMessages = {
        "Tama! Ang 'Pito' ay Filipino word para sa 7.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'Labing-apat' ay Fourteen sa English.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 5 plus 3 ay 'Walo' sa Filipino.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'Labinsiyam' ay 19.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'Dalawampu' ay Twenty sa English.",
        "Mali. Subukan mo ulit."
    };

    void Start()
    {
        SetupUniversalFont();
        
        // Hide choice buttons initially
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        // Set up button listeners
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        // Initialize SM2 system
        InitializeQuestions();
        
        // Show first dialog message
        ShowDialogMessage();
    }
    
    void SetupUniversalFont()
    {
        if (useUniversalFont && FilipknowFontManager.Instance != null)
        {
            // Apply universal font to dialog text
            if (dialogText != null)
            {
                dialogText.font = FilipknowFontManager.Instance.GetCurrentFont();
                dialogText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                dialogText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
            }
            
            // Apply universal font to choice buttons
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.font = FilipknowFontManager.Instance.GetCurrentFont();
                        buttonText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                        buttonText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                    }
                }
            }
        }
        else if (timesBoldFont != null)
        {
            // Apply FilipknowMainFont directly to dialog text
            if (dialogText != null)
            {
                dialogText.font = timesBoldFont;
                Debug.Log($"NumbersGameManager: Applied FilipknowMainFont to dialog text");
            }
            
            // Apply FilipknowMainFont to choice buttons
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.font = timesBoldFont;
                        Debug.Log($"NumbersGameManager: Applied FilipknowMainFont to choice button");
                    }
                }
            }
        }
    }
    
    // Get appropriate dialog messages based on language setting
    private string[] GetDialogMessages()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return dialogMessagesFilipino;
        else
            return dialogMessagesEnglish;
    }
    
    // Get appropriate questions based on language setting
    private string[] GetQuestions()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return questionsFilipino;
        else
            return questionsEnglish;
    }
    
    // Get appropriate choices based on language setting
    private string[][] GetChoices()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return choicesFilipino;
        else
            return choicesEnglish;
    }
    
    // Get appropriate correct answers based on language setting
    private int[] GetCorrectAnswers()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return correctAnswersFilipino;
        else
            return correctAnswersEnglish;
    }
    
    void InitializeQuestions()
    {
        // Add questions to SM2 system if not already added
        string[] questions = GetQuestions();
        string[][] choices = GetChoices();
        int[] correctAnswers = GetCorrectAnswers();
        for (int i = 0; i < questions.Length; i++)
        {
            if (SM2Algorithm.Instance != null)
            {
                SM2Algorithm.Instance.AddQuestion(i, "Numbers", questions[i], choices[i], correctAnswers[i]);
            }
        }
        
        // Get review questions
        if (SM2Algorithm.Instance != null)
        {
            reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Numbers");
        }
    }

    void ShowDialogMessage()
    {
        string[] dialogMessages = GetDialogMessages();
        if (currentDialogIndex < dialogMessages.Length)
        {
            dialogText.text = dialogMessages[currentDialogIndex];
            currentDialogIndex++;
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            // Start the game
            StartGame();
        }
    }

    void StartGame()
    {
        ShowQuestion();
    }

    void ShowQuestion()
    {
        string[] questions = GetQuestions();
        string[][] choices = GetChoices();
        if (currentQuestion < questions.Length)
        {
            string questionText = questions[currentQuestion];
            
            // Use adaptive dialog system if available
            if (adaptiveDialogManager != null)
            {
                adaptiveDialogManager.ShowDialog(questionText, () => {
                    // Display choices after dialog is shown
                    DisplayChoices(choices[currentQuestion]);
                });
            }
            else
            {
                dialogText.text = questionText;
                DisplayChoices(choices[currentQuestion]);
            }
        }
        else
        {
            // Game completed
            ShowGameComplete();
        }
    }
    
    void DisplayChoices(string[] choices)
    {
        // Use adaptive choice manager if available
        if (adaptiveChoiceManager != null)
        {
            adaptiveChoiceManager.DisplayChoices(choices, OnChoiceSelected);
            return;
        }
        
        // Fallback to legacy choice buttons
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
                
                // Remove existing listeners and add new one
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
    
    // New method for adaptive choice system
    public void OnChoiceSelected(string selectedChoice)
    {
        Debug.Log($"Adaptive choice selected: {selectedChoice}");
        
        // Find the choice index for compatibility
        int choiceIndex = -1;
        string[][] choices = GetChoices();
        for (int i = 0; i < choices[currentQuestion].Length; i++)
        {
            if (choices[currentQuestion][i] == selectedChoice)
            {
                choiceIndex = i;
                break;
            }
        }
        
        if (choiceIndex >= 0)
        {
            OnAnswerSelected(choiceIndex);
        }
    }

    void OnAnswerSelected(int choiceIndex)
    {
        // Get arrays once at the beginning
        string[] questions = GetQuestions();
        string[][] choices = GetChoices();
        int[] correctAnswers = GetCorrectAnswers();
        
        bool isCorrect = choiceIndex == correctAnswers[currentQuestion];
        
        // Play sound effects and haptic feedback
        if (optionsMenu != null)
        {
            if (isCorrect)
            {
                optionsMenu.PlayCorrectAnswerSound();
            }
            else
            {
                optionsMenu.PlayIncorrectAnswerSound();
            }
        }
        
        // Trigger haptic feedback
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        // Hide choice buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        if (isCorrect)
        {
            score++;
            dialogText.text = feedbackMessages[currentQuestion * 2]; // Correct feedback
        }
        else
        {
            dialogText.text = feedbackMessages[currentQuestion * 2 + 1]; // Incorrect feedback
        }
        
        // Process answer with SM2 algorithm
        if (SM2Algorithm.Instance != null)
        {
            QuestionData currentQuestionData = new QuestionData(currentQuestion, "Numbers", 
                questions[currentQuestion], choices[currentQuestion], correctAnswers[currentQuestion]);
            SM2Algorithm.Instance.ProcessAnswer(currentQuestionData, isCorrect);
        }
        
        currentQuestion++;
        
        // Show continue button for feedback
        continueButton.gameObject.SetActive(true);
    }

    void OnContinue()
    {
        continueButton.gameObject.SetActive(false);
        
        string[] dialogMessages = GetDialogMessages();
        if (currentDialogIndex < dialogMessages.Length)
        {
            ShowDialogMessage();
        }
        else
        {
            ShowQuestion();
        }
    }

    void ShowGameComplete()
    {
        string[] questions = GetQuestions();
        dialogText.text = $"Tapos na ang laro!\n\nAng iyong puntos: {score}/{questions.Length}\n\nSalamat sa paglalaro!";
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(GoBack);
    }

    void GoBack()
    {
        SceneManager.LoadScene("Module 1");
    }
}
