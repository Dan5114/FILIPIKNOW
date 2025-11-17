using UnityEngine;
using UnityEngine.SceneManagement;

public class Module1Selection : MonoBehaviour
{
    public void SelectOption(string option)
    {
        DifficultyUnlockManager difficultyUnlockManager = DifficultyUnlockManager.Instance;
        SM2Algorithm sm2 = SM2Algorithm.Instance;
        sm2.SetCurrentTopic(option);

        if(difficultyUnlockManager.Unlocked.Count <= 0)
        {
            SceneManager.LoadScene("Quiz");
        }
        else
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.GoToModuleContent(1, option);
            }
        }

        // else
        // {
        //     SceneManager.LoadScene(option);
        // }
    }

    public void GoBack()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoBack();
        }
        else
        {
            SceneManager.LoadScene("Modules Available");
        }
    }
}

