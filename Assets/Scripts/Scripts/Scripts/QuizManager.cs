using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class QuizManager : MonoBehaviour
{
    public static string SelectedTopic = "";
    public static DifficultyLevel SelectedDifficulty = DifficultyLevel.Medium;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button continueButton;

    [Header("Quiz Settings")]
    [SerializeField] private List<UnifiedQuestions> topicQuestions;
    [SerializeField] private int questionCount = 10;

    private List<UnifiedQuestionData> quizQuestions = new();
    private UnifiedQuestionData currentQuestion;

    private int currentIndex = 0;
    private int correctCount = 0;
    private float totalResponseTime = 0f;
    private float questionStartTime = 0f;

    private void Start()
    {
        SelectedTopic = SM2Algorithm.Instance.CurrentTopic;

        if (string.IsNullOrEmpty(SelectedTopic))
        {
            Debug.LogError("QuizManager: No topic selected!");
            questionText.text = "No topic selected!";
            return;
        }

        LoadQuestions();
        ShowNextQuestion();

        if (backButton != null)
            backButton.onClick.AddListener(() =>
                UnityEngine.SceneManagement.SceneManager.LoadScene("Module 1"));
    }

    private void LoadQuestions()
    {
        // Pull medium questions only
        var allQuestions = topicQuestions
            .SelectMany(q => q.GetUnifiedQuestions())
            .Where(q => q.difficultyLevel == DifficultyLevel.Medium &&
                        q.questionText.Length > 0 &&
                        q.choices != null &&
                        q.choices.Length > 0 &&
                        q.moduleName.ToLower() == SelectedTopic.ToLower())
            // .Where(q => q.questionText.Contains(SelectedTopic) || true) // fallback if topic not tagged
            .ToList();

        if (allQuestions.Count == 0)
        {
            Debug.LogError("No medium questions found for topic: " + SelectedTopic);
            questionText.text = "No available questions!";
            return;
        }

        // Shuffle
        for (int i = allQuestions.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = allQuestions[i];
            allQuestions[i] = allQuestions[j];
            allQuestions[j] = temp;
        }

        quizQuestions = allQuestions.Take(questionCount).ToList();
    }

    private void ShowNextQuestion()
    {
        if (currentIndex >= quizQuestions.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = quizQuestions[currentIndex];
        questionText.text = currentQuestion.questionText;

        SetupButtons();
        questionStartTime = Time.time; // timer starts
    }

    private void SetupButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentQuestion.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.choices[i];

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
        float responseTime = Time.time - questionStartTime;
        totalResponseTime += responseTime;

        bool isCorrect = index == currentQuestion.correctChoiceIndex;

        if (isCorrect)
        {
            correctCount++;
            questionText.text = $"Correct!\n\n{currentQuestion.instruction}";
        }
        else
        {
            questionText.text =
                $"Wrong!\nCorrect answer: {currentQuestion.choices[currentQuestion.correctChoiceIndex]}";
        }

        // Feedback
        ShowButtonFeedback(index, isCorrect);

        // SM2 + Learning Progression store the result
        LearningProgressionManager.Instance.RecordQuestionAnswer(
            SelectedTopic,
            currentQuestion.questionId,
            DifficultyLevel.Medium,
            isCorrect,
            responseTime,
            1
        );

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            ResetButtonColors();
            currentIndex++;
            ShowNextQuestion();
        });
    }

    private void ShowButtonFeedback(int index, bool isCorrect)
    {
        var btn = choiceButtons[index];
        var feedback = btn.GetComponent<ChoiceButtonFeedback>();

        if (feedback != null)
        {
            if (isCorrect) feedback.ShowCorrect();
            else feedback.ShowWrong();
        }
        else
        {
            btn.image.color = isCorrect ? Color.green : Color.red;
        }
    }

    private void ResetButtonColors()
    {
        foreach (var btn in choiceButtons)
        {
            var feedback = btn.GetComponent<ChoiceButtonFeedback>();
            if (feedback != null) feedback.ResetToDefault();
            else btn.image.color = Color.white;
        }
    }

    private void EndQuiz()
    {
        float accuracy = (float)correctCount / quizQuestions.Count;
        float avgResponseTime = totalResponseTime / quizQuestions.Count;

        questionText.text =
            $"Quiz Finished!\nScore: {correctCount}/{quizQuestions.Count}\nAvg Speed: {avgResponseTime:F2}s";

        // Unlock difficulty based on score + speed
        DifficultyUnlockManager.Instance.EvaluateUnlocks(SelectedTopic, correctCount, avgResponseTime);

        // Update actual topic progress
        LearningProgressionManager.Instance.UpdateTopicProgress(
            SelectedTopic,
            DifficultyLevel.Medium,
            true,
            accuracy
        );

        continueButton.gameObject.SetActive(false);
        foreach (var b in choiceButtons) b.gameObject.SetActive(false);
    }
}
