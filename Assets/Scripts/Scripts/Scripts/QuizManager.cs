using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class QuizManager : MonoBehaviour
{
    [Header("UI REFERENCES")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button continueButton;

    [Header("SETTINGS")]
    [SerializeField] private string moduleName = "Nouns";
    [SerializeField] private int questionCount = 5;

    private List<QuestionData> quizQuestions = new List<QuestionData>();
    private int currentIndex = 0;
    private QuestionData currentQuestion;

    private void GenerateMiniQuiz(string moduleName, int questionCount)
    {
        if (SM2Algorithm.Instance == null)
        {
            Debug.LogError("SM2Algorithm.Instance not found!");
            return;
        }
        List<QuestionData> allQuestions = new List<QuestionData>();

        allQuestions.AddRange(SM2Algorithm.Instance.GetQuestionsForReview(moduleName, questionCount));
        allQuestions.AddRange(SM2Algorithm.Instance.GetWeakQuestions(moduleName, questionCount));

        var questions = allQuestions.ToList();
        for (int i = questions.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var tmp = questions[i];
            questions[i] = questions[j];
            questions[j] = tmp;
        }
        int takeCount = Math.Min(questionCount, questions.Count);
        quizQuestions.AddRange(questions.Take(questionCount));

        if (quizQuestions.Count < questionCount)
        {
            var all = SM2Algorithm.Instance.GetAllQuestions()
                .Where(q => q.module == moduleName)
                .ToList();

            quizQuestions.AddRange(all.Take(questionCount).ToList());
            Debug.Log("No weak questions. Using random ones from module instead.");
        }

        Debug.Log($"Quiz generated with {quizQuestions.Count} questions for module '{moduleName}'.");
    }

    private void ShowNextQuestion()
    {
        if (quizQuestions == null || quizQuestions.Count == 0)
        {
            questionText.text = "No available quiz questions right now!";
            return;
        }

        if (currentIndex >= quizQuestions.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = quizQuestions[currentIndex];
        questionText.text = currentQuestion.question;

        SetupChoiceButtons();
    }

    private void SetupChoiceButtons()
    {
        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            Debug.LogError("No choice buttons assigned!");
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentQuestion.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                var btnText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = currentQuestion.choices[i];

                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        continueButton.gameObject.SetActive(false);
    }

    private void OnChoiceSelected(int index)
    {
        bool isCorrect = index == currentQuestion.correctAnswer;
        float responseTime = 0f;

        SM2Algorithm.Instance.ProcessAnswer(currentQuestion, isCorrect, responseTime);

        if (isCorrect)
            questionText.text = $"Correct!\n\n{currentQuestion.explanation}";
        else
            questionText.text = $"Wrong!\n\nCorrect answer: {currentQuestion.choices[currentQuestion.correctAnswer]}";

        ShowButtonFeedback(index, isCorrect);
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            currentIndex++;
            ShowNextQuestion();
            ResetButtons(choiceButtons);
        });
    }

    private void ShowButtonFeedback(int buttonIndex, bool isCorrect)
    {
        if (choiceButtons == null || buttonIndex >= choiceButtons.Length) return;
        
        Button clickedButton = choiceButtons[buttonIndex];
        if (clickedButton == null) return;
        
        ChoiceButtonFeedback feedback = clickedButton.GetComponent<ChoiceButtonFeedback>();
        if (feedback != null)
        {
            if (isCorrect)
            {
                feedback.ShowCorrect();
            }
            else
            {
                feedback.ShowWrong();
            }
        }
        else
        {
            Debug.LogWarning($"ChoiceButtonFeedback component not found on button {buttonIndex}");
        }
    }

    private void EndQuiz()
    {
        questionText.text = $"You’ve finished your quiz!\n{ShowStats()}";
        continueButton.gameObject.SetActive(false);
        foreach(Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private string ShowStats()
    {
        if (SM2Algorithm.Instance == null) return "";

        var progress = SM2Algorithm.Instance.GetUserProgress();
        float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();

        string stats = $"Level: {progress.level}\nXP: {progress.experience}\nAccuracy: {accuracy:F1}%";

        Debug.Log(stats);

        return stats;
    }

    private void ResetButtons(Button[] choiceButtons)
    {
        foreach(Button button in choiceButtons)
        {
            button.GetComponent<ChoiceButtonFeedback>().ResetToDefault();
        }
    }

    private void Start()
    {
        GenerateMiniQuiz(moduleName, questionCount);
        ShowNextQuestion();

        if (backButton != null)
            backButton.onClick.AddListener(() =>
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu"));
    }
}
