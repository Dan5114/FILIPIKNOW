using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    // Method to load the Modules Available scene
    public void LoadModulesAvailable()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToModulesAvailable();
        }
        else
        {
            SceneManager.LoadScene("Modules Available");
        }
    }


}