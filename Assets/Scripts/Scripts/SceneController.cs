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
    
    // Overload for string difficulty (for compatibility with new system)
    public void SetSelectedDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                selectedDifficulty = DifficultyLevel.Easy;
                break;
            case "medium":
                selectedDifficulty = DifficultyLevel.Medium;
                break;
            case "hard":
                selectedDifficulty = DifficultyLevel.Hard;
                break;
            default:
                selectedDifficulty = DifficultyLevel.Easy;
                break;
        }
    }
    
    // Get difficulty as string (for compatibility with new system)
    public string GetSelectedDifficultyAsString()
    {
        return selectedDifficulty.ToString();
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
                    LoadScene("VerbsDifficultySelection");
                    break;
                case "numbers":
                    Debug.Log("🎯 Routing Numbers to NumbersDifficultySelection");
                    LoadScene("NumbersDifficultySelection");
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

    // New navigation methods for Nouns flow
    public void GoToNounsIntroduction()
    {
        Debug.Log("🎯 Navigating to NounsIntroduction scene");
        LoadScene("NounsIntroduction");
    }
    
    public void GoToNounsGame()
    {
        // Route to correct scene based on selected difficulty
        DifficultyLevel difficulty = GetSelectedDifficulty();
        
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                Debug.Log("🎯 Navigating to Nouns scene (Easy)");
                LoadScene("Nouns");
                break;
            case DifficultyLevel.Medium:
                Debug.Log("🎯 Navigating to NounsMedium scene (Medium)");
                LoadScene("NounsMedium");
                break;
            case DifficultyLevel.Hard:
                Debug.Log("🎯 Navigating to NounsHard scene (Hard)");
                LoadScene("NounsHard");
                break;
            default:
                Debug.Log("🎯 Defaulting to Nouns scene (Easy)");
                LoadScene("Nouns");
                break;
        }
    }

    public void GoToTopicModuleGame()
    {
        SM2Algorithm sm2 = SM2Algorithm.Instance;
        DifficultyLevel difficulty = GetSelectedDifficulty();
        string topic = sm2.CurrentTopic;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                Debug.Log("🎯 Navigating to Nouns scene (Easy)");
                LoadScene(topic);
                break;
            case DifficultyLevel.Medium:
                Debug.Log("🎯 Navigating to NounsMedium scene (Medium)");
                LoadScene(topic+"Medium");
                break;
            case DifficultyLevel.Hard:
                Debug.Log("🎯 Navigating to NounsHard scene (Hard)");
                LoadScene(topic+"Hard");
                break;
            default:
                Debug.Log("🎯 Defaulting to Nouns scene (Easy)");
                LoadScene(topic);
                break;
        }
    }
    
    public void GoToNounsIntroductionMedium()
    {
        Debug.Log("🎯 Navigating to NounsIntroductionMedium scene");
        LoadScene("NounsIntroductionMedium");
    }
    
    public void GoToNounsMediumGame()
    {
        Debug.Log("🎯 Navigating to NounsMedium scene (Medium)");
        LoadScene("NounsMedium");
    }
    
    public void GoToNounsSummary()
    {
        Debug.Log("🎯 Navigating to NounsSummary scene");
        LoadScene("NounsSummary");
    }
    
    public void GoToNounsDifficultySelection()
    {
        Debug.Log("🎯 Navigating to NounsDifficultySelection scene");
        LoadScene("NounsDifficultySelection");
    }
    
    // Navigation for difficulty selection
    public void GoToDifficultySelection()
    {
        string topic = GetSelectedTopic();
        if (!string.IsNullOrEmpty(topic))
        {
            switch (topic.ToLower())
            {
                case "nouns":
                    LoadScene("NounsDifficultySelection");
                    break;
                case "verbs":
                    LoadScene("VerbsDifficultySelection");
                    break;
                case "numbers":
                    LoadScene("NumbersDifficultySelection");
                    break;
                default:
                    LoadScene("Module 1");
                    break;
            }
        }
        else
        {
            LoadScene("Module 1");
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
            case "NounsIntroduction":
                // Go back to difficulty selection
                GoToDifficultySelection();
                break;
            case "NounsSummary":
                // Go back to difficulty selection (or could go back to game)
                GoToDifficultySelection();
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
