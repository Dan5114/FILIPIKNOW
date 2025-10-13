using UnityEngine;
using UnityEngine.UI;

public class Module2Selection : MonoBehaviour
{
    [Header("Option Buttons")]
    public Button adjectivesButton;
    public Button pronounsButton;
    public Button basicSentenceConstructionButton;
    public Button questionWordsButton;
    public Button backButton;

    private void Start()
    {
        // Set up button listeners
        if (adjectivesButton != null)
            adjectivesButton.onClick.AddListener(() => SelectOption("Adjectives"));
        
        if (pronounsButton != null)
            pronounsButton.onClick.AddListener(() => SelectOption("Pronouns"));
        
        if (basicSentenceConstructionButton != null)
            basicSentenceConstructionButton.onClick.AddListener(() => SelectOption("BasicSentenceConstruction"));
        
        if (questionWordsButton != null)
            questionWordsButton.onClick.AddListener(() => SelectOption("QuestionWords"));
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    public void SelectOption(string option)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModuleContent(2, option);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Module 2/{option}");
        }
    }

    public void GoBack()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoBack();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Modules Available");
        }
    }

    // Individual option methods for direct button assignment
    public void SelectAdjectives()
    {
        SelectOption("Adjectives");
    }

    public void SelectPronouns()
    {
        SelectOption("Pronouns");
    }

    public void SelectBasicSentenceConstruction()
    {
        SelectOption("BasicSentenceConstruction");
    }

    public void SelectQuestionWords()
    {
        SelectOption("QuestionWords");
    }
}
