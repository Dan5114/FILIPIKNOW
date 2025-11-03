using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModulesAvailable : MonoBehaviour
{
    [Header("Module Buttons")]
    public Button module1Button;
    public Button module2Button;
    public Button module3Button;
    public Button backButton;



    private void Start()
    {
        // Set up module button listeners
        if (module1Button != null)
            module1Button.onClick.AddListener(() => GoToModule(1));
        
        if (module2Button != null)
            module2Button.onClick.AddListener(() => GoToModule(2));
        
        if (module3Button != null)
            module3Button.onClick.AddListener(() => GoToModule(3));
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);


    }

    public void GoToModule(int moduleNumber)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModule(moduleNumber);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Module {moduleNumber}");
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }

    // Individual module methods for direct button assignment
    public void GoToModule1()
    {
        GoToModule(1);
    }

    public void GoToModule2()
    {
        GoToModule(2);
    }

    public void GoToModule3()
    {
        GoToModule(3);
    }

    public void LoadQuiz()
    {
        SceneManager.LoadScene("Quiz", LoadSceneMode.Single);
    }


}
