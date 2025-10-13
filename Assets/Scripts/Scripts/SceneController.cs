using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    
    private string selectedModule;
    private string selectedOption;
    private string selectedTopic;
    private DifficultyLevel selectedDifficulty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetSelectedModule(string module)
    {
        selectedModule = module;
    }

    public string GetSelectedModule()
    {
        return selectedModule;
    }

    public void SetSelectedOption(string option)
    {
        selectedOption = option;
    }

    public string GetSelectedOption()
    {
        return selectedOption;
    }

    public void SetSelectedTopic(string topic)
    {
        selectedTopic = topic;
    }

    public string GetSelectedTopic()
    {
        return selectedTopic;
    }

    public void SetSelectedDifficulty(DifficultyLevel difficulty)
    {
        selectedDifficulty = difficulty;
    }

    public DifficultyLevel GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }

    // Navigation methods for common scene transitions
    public void GoToMainMenu()
    {
        LoadScene("Main Menu");
    }

    public void GoToModulesAvailable()
    {
        LoadScene("Modules Available");
    }

    public void GoToModule(int moduleNumber)
    {
        SetSelectedModule($"Module {moduleNumber}");
        LoadScene($"Module {moduleNumber}");
    }

    public void GoToModuleContent(int moduleNumber, string contentType)
    {
        SetSelectedModule($"Module {moduleNumber}");
        SetSelectedOption(contentType.ToLower());
        
        Debug.Log($"🎯 GoToModuleContent called: Module {moduleNumber}, Content: {contentType}");
        
        // For Module 1, route through difficulty selection for Nouns, Verbs, and Numbers
        if (moduleNumber == 1)
        {
            switch (contentType.ToLower())
            {
                case "nouns":
                    Debug.Log("🎯 Routing Nouns to NounsDifficultySelection");
                    LoadScene("NounsDifficultySelection");
                    break;
                case "verbs":
                    Debug.Log("🎯 Routing Verbs to VerbDifficultySelection");
                    LoadScene("VerbDifficultySelection");
                    break;
                case "numbers":
                    Debug.Log("🎯 Routing Numbers to NumberDifficultySelection");
                    LoadScene("NumberDifficultySelection");
                    break;
                default:
                    Debug.Log($"🎯 Routing {contentType} directly to {contentType} scene");
                    LoadScene(contentType);
                    break;
            }
        }
        else
        {
            Debug.Log($"🎯 Routing Module {moduleNumber} content {contentType} directly");
            LoadScene(contentType);
        }
    }

    public void GoBack()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        switch (currentScene)
        {
            case "Modules Available":
                GoToMainMenu();
                break;
            case "Module 1":
            case "Module 2":
            case "Module 3":
                GoToModulesAvailable();
                break;
            case "NounsDifficultySelection":
            case "VerbsDifficultySelection":
            case "NumbersDifficultySelection":
                // Go back to Module 1 selection
                LoadScene("Module 1");
                break;
            case "Nouns":
            case "Verbs":
            case "Numbers":
            case "Adjectives":
            case "Pronouns":
            case "BasicSentenceConstruction":
            case "QuestionWords":
            case "MathVocabulary":
            case "ShapesColors":
            case "TimeTelling":
            case "MoneyTransactions":
                // Go back to the appropriate difficulty selection scene
                if (currentScene.StartsWith("Nouns"))
                    LoadScene("NounsDifficultySelection");
                else if (currentScene.StartsWith("Verbs"))
                    LoadScene("VerbsDifficultySelection");
                else if (currentScene.StartsWith("Numbers"))
                    LoadScene("NumbersDifficultySelection");
                else
                {
                    // Go back to the module selection scene for other topics
                    string selectedModule = GetSelectedModule();
                    if (!string.IsNullOrEmpty(selectedModule))
                    {
                        LoadScene(selectedModule);
                    }
                    else
                    {
                        GoToModulesAvailable();
                    }
                }
                break;
            default:
                GoToMainMenu();
                break;
        }
    }
}
