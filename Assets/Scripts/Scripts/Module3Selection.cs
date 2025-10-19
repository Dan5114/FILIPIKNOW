using UnityEngine;
using UnityEngine.UI;

public class Module3Selection : MonoBehaviour
{
    [Header("Option Buttons")]
    public Button mathVocabularyButton;
    public Button shapesColorsButton;
    public Button timeTellingButton;
    public Button moneyTransactionsButton;
    public Button backButton;

    private void Start()
    {
        // Set up button listeners
        if (mathVocabularyButton != null)
            mathVocabularyButton.onClick.AddListener(() => SelectOption("MathVocabulary"));
        
        if (shapesColorsButton != null)
            shapesColorsButton.onClick.AddListener(() => SelectOption("ShapesColors"));
        
        if (timeTellingButton != null)
            timeTellingButton.onClick.AddListener(() => SelectOption("TimeTelling"));
        
        if (moneyTransactionsButton != null)
            moneyTransactionsButton.onClick.AddListener(() => SelectOption("MoneyTransactions"));
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    public void SelectOption(string option)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModuleContent(3, option);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Module 3/{option}");
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
    public void SelectMathVocabulary()
    {
        SelectOption("MathVocabulary");
    }

    public void SelectShapesColors()
    {
        SelectOption("ShapesColors");
    }

    public void SelectTimeTelling()
    {
        SelectOption("TimeTelling");
    }

    public void SelectMoneyTransactions()
    {
        SelectOption("MoneyTransactions");
    }
}
