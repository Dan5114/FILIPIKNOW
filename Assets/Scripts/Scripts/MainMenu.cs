using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadModulesAvailable()
    {
        ModuleUnlockManager moduleUnlockManager = FindAnyObjectByType<ModuleUnlockManager>();

        if(moduleUnlockManager.GetUnlockedModules().Count <= 0)
        {
            SceneManager.LoadScene("Quiz");
        }
        else
        {
            SceneManager.LoadScene("Modules Available");
        }
    }
}