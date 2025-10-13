using UnityEngine;
using UnityEngine.UI;

public class Module1Selection : MonoBehaviour
{
    [Header("Option Buttons")]
    public Button nounsButton;
    public Button verbsButton;
    public Button numbersButton;
    public Button backButton;

    private void Start()
    {
        // Set up button listeners
        if (nounsButton != null)
            nounsButton.onClick.AddListener(() => SelectOption("Nouns"));
        
        if (verbsButton != null)
            verbsButton.onClick.AddListener(() => SelectOption("Verbs"));
        
        if (numbersButton != null)
            numbersButton.onClick.AddListener(() => SelectOption("Numbers"));
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    public void SelectOption(string option)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModuleContent(1, option);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(option);
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
    public void SelectNouns()
    {
        SelectOption("Nouns");
    }

    public void SelectVerbs()
    {
        SelectOption("Verbs");
    }

    public void SelectNumbers()
    {
        SelectOption("Numbers");
    }
}

