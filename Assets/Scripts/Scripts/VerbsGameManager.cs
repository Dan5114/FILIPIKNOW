using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class VerbsGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons; // Legacy - will be replaced by adaptive system
    public Button backButton;
    
    [Header("Adaptive Dialog System")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;

    private int currentQuestion = 0;
    private int score = 0;
    private List<QuestionData> reviewQuestions = new List<QuestionData>();
    
    // Dialog content
    private string[] dialogMessages = {
        "Kumusta! Ako si Teacher Ana.",
        "Ngayon ay matututunan natin ang tungkol sa PANDIWA.",
        "Ang pandiwa ay mga salitang nagsasaad ng kilos o galaw.",
        "Halimbawa: tumakbo, kumain, naglaro, umiyak",
        "Handa ka na ba para sa ating laro?"
    };
    
    private int currentDialogIndex = 0;
    
    // Game content - 5 questions focused on Filipino verbs
    private string[] questions = {
        "Pumili ng pandiwa sa pangungusap:\n'Si Maria ay tumakbo sa parke.'",
        "Ano ang pandiwa sa pangungusap?\n'Ang mga bata ay naglalaro sa bakuran.'",
        "Pumili ng tamang pandiwa:\n'Si Ben ay ___ ng tinapay.'",
        "Ano ang pandiwa sa pangungusap?\n'Ang guro ay nagtuturo sa mga estudyante.'",
        "Pumili ng tamang pandiwa:\n'Ang mga bulaklak ay ___ sa hardin.'"
    };
    
    private string[][] choices = {
        new string[] { "Maria", "tumakbo", "parke", "ay" },
        new string[] { "mga bata", "naglalaro", "bakuran", "ay" },
        new string[] { "kumakain", "tinapay", "Ben", "ay" },
        new string[] { "guro", "nagtuturo", "mga estudyante", "ay" },
        new string[] { "mga bulaklak", "tumutubo", "hardin", "ay" }
    };
    
    private int[] correctAnswers = { 1, 1, 0, 1, 1 }; // Index of correct answer for each question
    
    private string[] feedbackMessages = {
        "Tama! Ang 'tumakbo' ay isang pandiwa.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'naglalaro' ay isang pandiwa.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'kumakain' ay isang pandiwa.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'nagtuturo' ay isang pandiwa.",
        "Mali. Subukan mo ulit.",
        "Tama! Ang 'tumutubo' ay isang pandiwa.",
        "Mali. Subukan mo ulit."
    };

    void Start()
    {
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
    
    void InitializeQuestions()
    {
        // Add questions to SM2 system if not already added
        for (int i = 0; i < questions.Length; i++)
        {
            if (SM2Algorithm.Instance != null)
            {
                SM2Algorithm.Instance.AddQuestion(i, "Verbs", questions[i], choices[i], correctAnswers[i]);
            }
        }
        
        // Get review questions
        if (SM2Algorithm.Instance != null)
        {
            reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Verbs");
        }
    }

    void ShowDialogMessage()
    {
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
        // Hide choice buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        bool isCorrect = choiceIndex == correctAnswers[currentQuestion];
        
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
            QuestionData currentQuestionData = new QuestionData(currentQuestion, "Verbs", 
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
