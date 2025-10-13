using UnityEngine;
using UnityEngine.UI;

public class Module2 : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button backButton;
    public Button homeButton;

    [Header("Content Display")]
    public Text moduleTitle;
    public Text selectedOptionText;

    private void Start()
    {
        // Set up button listeners
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(GoToMainMenu);

        // Display current module and selected option
        if (moduleTitle != null)
            moduleTitle.text = "Module 2";

        if (selectedOptionText != null && SceneController.Instance != null)
        {
            string selectedOption = SceneController.Instance.GetSelectedOption();
            if (!string.IsNullOrEmpty(selectedOption))
            {
                selectedOptionText.text = $"Selected: {selectedOption}";
            }
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

    public void GoToMainMenu()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }
}
