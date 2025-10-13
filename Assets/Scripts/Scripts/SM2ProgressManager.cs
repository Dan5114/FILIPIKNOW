using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SM2ProgressManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI moduleScoreText;
    public TextMeshProUGUI nextReviewText;
    public Button resetProgressButton;
    
    [Header("Progress Display")]
    public GameObject progressPanel;
    public bool showDetailedStats = true;
    
    private void Start()
    {
        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(ResetProgress);
        }
        
        UpdateProgressDisplay();
    }
    
    public void UpdateProgressDisplay()
    {
        if (SM2Algorithm.Instance == null) return;
        
        // Update overall accuracy
        float overallAccuracy = SM2Algorithm.Instance.GetOverallAccuracy();
        if (accuracyText != null)
        {
            accuracyText.text = $"Overall Accuracy: {overallAccuracy:F1}%";
        }
        
        // Update module scores
        if (moduleScoreText != null)
        {
            int nounsScore = SM2Algorithm.Instance.GetModuleScore("Nouns");
            int numbersScore = SM2Algorithm.Instance.GetModuleScore("Numbers");
            int verbsScore = SM2Algorithm.Instance.GetModuleScore("Verbs");
            
            moduleScoreText.text = $"Module Scores:\nNouns: {nounsScore}\nNumbers: {numbersScore}\nVerbs: {verbsScore}";
        }
        
        // Update detailed progress
        if (progressText != null && showDetailedStats)
        {
            List<QuestionData> allQuestions = SM2Algorithm.Instance.GetAllQuestions();
            var nounsQuestions = allQuestions.Where(q => q.module == "Nouns").ToList();
            
            string progressInfo = "Nouns Module Progress:\n\n";
            
            if (nounsQuestions.Count > 0)
            {
                int mastered = nounsQuestions.Count(q => q.repetitions >= 3);
                int learning = nounsQuestions.Count(q => q.repetitions > 0 && q.repetitions < 3);
                int newQuestions = nounsQuestions.Count(q => q.repetitions == 0);
                
                progressInfo += $"Mastered: {mastered}/{nounsQuestions.Count}\n";
                progressInfo += $"Learning: {learning}/{nounsQuestions.Count}\n";
                progressInfo += $"New: {newQuestions}/{nounsQuestions.Count}\n\n";
                
                // Show next review questions
                var reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Nouns");
                progressInfo += $"Questions due for review: {reviewQuestions.Count}\n";
                
                if (reviewQuestions.Count > 0)
                {
                    progressInfo += "Next reviews:\n";
                    for (int i = 0; i < Mathf.Min(3, reviewQuestions.Count); i++)
                    {
                        var q = reviewQuestions[i];
                        progressInfo += $"• Q{q.questionId}: {q.nextReview.ToString("MM/dd")}\n";
                    }
                }
            }
            else
            {
                progressInfo += "No questions in Nouns module yet.";
            }
            
            progressText.text = progressInfo;
        }
        
        // Update next review time
        if (nextReviewText != null)
        {
            var reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Nouns");
            if (reviewQuestions.Count > 0)
            {
                var nextReview = reviewQuestions.OrderBy(q => q.nextReview).First();
                nextReviewText.text = $"Next Review: {nextReview.nextReview.ToString("MMM dd, yyyy")}";
            }
            else
            {
                nextReviewText.text = "No reviews scheduled";
            }
        }
    }
    
    public void ShowProgressPanel()
    {
        if (progressPanel != null)
        {
            progressPanel.SetActive(true);
            UpdateProgressDisplay();
        }
    }
    
    public void HideProgressPanel()
    {
        if (progressPanel != null)
        {
            progressPanel.SetActive(false);
        }
    }
    
    public void ResetProgress()
    {
        if (SM2Algorithm.Instance != null)
        {
            SM2Algorithm.Instance.ResetProgress();
            UpdateProgressDisplay();
            Debug.Log("Progress reset successfully!");
        }
    }
    
    // Method to get learning recommendations
    public string GetLearningRecommendations()
    {
        if (SM2Algorithm.Instance == null) return "";
        
        var reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Nouns");
        var allQuestions = SM2Algorithm.Instance.GetAllQuestions();
        var nounsQuestions = allQuestions.Where(q => q.module == "Nouns").ToList();
        
        if (nounsQuestions.Count == 0)
        {
            return "Start with the Nouns module to begin your learning journey!";
        }
        
        if (reviewQuestions.Count > 5)
        {
            return "You have many questions due for review. Focus on reviewing before learning new content.";
        }
        
        if (reviewQuestions.Count == 0)
        {
            return "Great job! All questions are up to date. You can learn new content or review mastered topics.";
        }
        
        float accuracy = SM2Algorithm.Instance.GetOverallAccuracy();
        if (accuracy < 60f)
        {
            return "Your accuracy is below 60%. Consider reviewing easier questions to build confidence.";
        }
        else if (accuracy > 90f)
        {
            return "Excellent accuracy! You're ready for more challenging content.";
        }
        
        return "Continue with your current learning pace. You're doing well!";
    }
    
    // Method to get difficulty analysis
    public Dictionary<string, int> GetDifficultyAnalysis()
    {
        var analysis = new Dictionary<string, int>();
        
        if (SM2Algorithm.Instance == null) return analysis;
        
        var allQuestions = SM2Algorithm.Instance.GetAllQuestions();
        var nounsQuestions = allQuestions.Where(q => q.module == "Nouns").ToList();
        
        analysis["Easy"] = nounsQuestions.Count(q => SM2Algorithm.Instance.GetDifficultyRating(q) < 2f);
        analysis["Medium"] = nounsQuestions.Count(q => SM2Algorithm.Instance.GetDifficultyRating(q) >= 2f && SM2Algorithm.Instance.GetDifficultyRating(q) < 4f);
        analysis["Hard"] = nounsQuestions.Count(q => SM2Algorithm.Instance.GetDifficultyRating(q) >= 4f);
        
        return analysis;
    }
}






