using UnityEngine;
using UnityEngine.UI;

public class NavigationHelper : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button backButton;
    public Button homeButton;

    private void Start()
    {
        // Set up button listeners
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(GoToMainMenu);
    }

    public void GoBack()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoBack();
        }
        else
        {
            // Fallback navigation logic
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            switch (currentScene)
            {
                case "Modules Available":
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
                    break;
                case "Module 1":
                case "Module 2":
                case "Module 3":
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Modules Available");
                    break;
                default:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
                    break;
            }
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

    public void GoToModulesAvailable()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModulesAvailable();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Modules Available");
        }
    }
}
