using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class ModuleUnlockManager : MonoBehaviour
{
    [Header("Module List")]
    [Tooltip("List of all modules in the order you want them unlocked.")]
    [SerializeField] private List<string> modules = new List<string>();

    [Header("Unlock Thresholds")]
    [Tooltip("Minimum mastery (0-100) required to unlock the next module.")]
    [SerializeField][Range(0, 100f)] private float masteryThreshold = 60f;

    [Tooltip("Minimum total accuracy (0-100) required to consider unlocking.")]
    [SerializeField][Range(0, 100f)] private float accuracyThreshold = 50f;

    [Tooltip("Minimum user level required to unlock modules.")]
    [SerializeField] private int levelThreshold = 0;

    [Header("Debug")]
    [SerializeField] private ModuleUnlockMode moduleUnlockMode;

    private const string UNLOCK_KEY = "UnlockedModules";
    
    private HashSet<string> unlockedModules = new HashSet<string>();

    public List<string> GetUnlockedModules()
    {
        return unlockedModules.ToList();
    }

    public bool IsModuleUnlocked(string module)
    {
        return unlockedModules.Contains(module);
    }

    public void UnlockModule(string module)
    {
        if (!unlockedModules.Contains(module))
        {
            unlockedModules.Add(module);
            SaveUnlockedModules();
            Debug.Log($"Unlocked module: {module}");
        }
    }

    public void LockAllModules()
    {
        unlockedModules.Clear();
        SaveUnlockedModules();
    }

    public void EvaluateUnlocks()
    {
        if (SM2Algorithm.Instance == null)
        {
            Debug.LogError("SM2Algorithm not found!");
            return;
        }

        var sm2 = SM2Algorithm.Instance;

        float accuracy = sm2.GetOverallAccuracy();
        int level = sm2.GetUserProgress().level;

        Debug.Log($"Evaluating unlocks… Accuracy={accuracy}, Level={level}");

        // Must pass global requirements first
        if (accuracy < accuracyThreshold || level < levelThreshold)
        {
            Debug.Log("Not enough stats for module unlock evaluation.");
            return;
        }

        foreach (string module in modules)
        {
            float moduleMastery = sm2.GetModuleMastery(module);

            if (moduleMastery >= masteryThreshold)
            {
                UnlockModule(module);
            }
        }
    }

    #region SAVE/LOAD
    private void SaveUnlockedModules()
    {
        string saved = string.Join(",", unlockedModules);
        PlayerPrefs.SetString(UNLOCK_KEY, saved);
        PlayerPrefs.Save();
    }

    private void LoadUnlockedModules()
    {
        string saved = PlayerPrefs.GetString(UNLOCK_KEY, "");

        if (!string.IsNullOrEmpty(saved))
        {
            unlockedModules = new HashSet<string>(saved.Split(','));
        }
        else
        {
            unlockedModules = new HashSet<string>();
        }
    }
    #endregion

    private void Start()
    {
        if(moduleUnlockMode == ModuleUnlockMode.NORMAL)
        {
            LoadUnlockedModules();
            EvaluateUnlocks();
        }
        else if(moduleUnlockMode == ModuleUnlockMode.UNLOCK_ALL)
        {
            foreach(string module in modules)
            {
                if (!unlockedModules.Contains(module))
                {
                    unlockedModules.Add(module);
                }
            } 
        }
        else if(moduleUnlockMode == ModuleUnlockMode.LOCK_ALL)
        {
            unlockedModules.Clear();
        }
        else if(moduleUnlockMode == ModuleUnlockMode.UNLOCK_ALL_AND_SAVE)
        {
            foreach(string module in modules)
            {
                UnlockModule(module);
            }
        }
        else if(moduleUnlockMode == ModuleUnlockMode.LOCK_ALL_AND_SAVE)
        {
            LockAllModules();
        }
    }
}
