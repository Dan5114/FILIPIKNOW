using UnityEngine;
using System.Collections.Generic;

public class DifficultyUnlockManager : MonoBehaviour
{
    public static DifficultyUnlockManager Instance;

    [Header("SETTINGS")]
    [SerializeField] private List<string> allTopics;

    [Header("Unlock Thresholds")]
    [Tooltip("Score required to unlock MEDIUM difficulty.")]
    public int unlockMediumScore = 5;

    [Tooltip("Score required to unlock HARD difficulty.")]
    public int unlockHardScore = 8;

    [Tooltip("Maximum allowed average response time (seconds) to qualify for HARD.")]
    public float hardSpeedThreshold = 3f;

    [Tooltip("Maximum allowed average response time (seconds) to qualify for MEDIUM.")]
    public float mediumSpeedThreshold = 6f;

    [Header("Debug Modes")]
    [SerializeField] private ModuleUnlockMode moduleUnlockMode;
    // public bool lockAllAtStart = false;
    // public bool unlockAllAtStart = false;

    private const string KEY = "DifficultyUnlocks";
    private Dictionary<string, HashSet<DifficultyLevel>> unlocked = new();

    public Dictionary<string, HashSet<DifficultyLevel>> Unlocked => unlocked;

    public bool IsUnlocked(string topic, DifficultyLevel level)
    {
        if (!unlocked.ContainsKey(topic))
            return false;

        Debug.Log($"{unlocked[topic]} unlock status: {unlocked[topic].Contains(level)}");
        return unlocked[topic].Contains(level);
    }

    public void Unlock(string topic, DifficultyLevel level)
    {
        if (!unlocked.ContainsKey(topic))
            unlocked[topic] = new HashSet<DifficultyLevel>();

        if (!unlocked[topic].Contains(level))
        {
            unlocked[topic].Add(level);
            Save();
            Debug.Log($"Unlocked {level} for topic {topic}");
        }
    }

    public void LockAll(bool save)
    {
        unlocked.Clear();

        if(save) Save();
    }

    public void UnlockAll(bool save = false)
    {
        // foreach (var topic in LearningProgressionManager.Instance.GetAllCurrentLevels().Keys)
        foreach(string topic in allTopics)
        {
            unlocked[topic] = new HashSet<DifficultyLevel>
            {
                DifficultyLevel.Easy,
                DifficultyLevel.Medium,
                DifficultyLevel.Hard
            };
        }
        if(save) Save();
    }

    #region QUIZ UNLOCK LOGIC
    public void EvaluateUnlocks(string topic, int score, float avgTime)
    {
        // Easy always available
        Unlock(topic, DifficultyLevel.Easy);

        // Medium unlock check
        if (score >= unlockMediumScore && avgTime <= mediumSpeedThreshold)
        {
            Unlock(topic, DifficultyLevel.Medium);
        }

        // Hard unlock check
        if (score >= unlockHardScore && avgTime <= hardSpeedThreshold)
        {
            Unlock(topic, DifficultyLevel.Hard);
        }
    }
    #endregion

    #region SAVE/LOAD
    private void Save()
    {
        string json = JsonUtility.ToJson(new Wrapper(unlocked));
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        string json = PlayerPrefs.GetString(KEY, "");

        if (moduleUnlockMode == ModuleUnlockMode.UNLOCK_ALL)
        {
            UnlockAll(false);
            return;
        }
        else if (moduleUnlockMode == ModuleUnlockMode.LOCK_ALL)
        {
            // unlocked = new();
            LockAll(false);
            return;
        }
        else if (moduleUnlockMode == ModuleUnlockMode.LOCK_ALL_AND_SAVE)
        {
            // unlocked = new();
            LockAll(true);
            return;
        }
        else if (moduleUnlockMode == ModuleUnlockMode.UNLOCK_ALL_AND_SAVE)
        {
            UnlockAll(true);
            return;
        }
        else if(moduleUnlockMode == ModuleUnlockMode.NORMAL)
        {
            if (!string.IsNullOrEmpty(json))
            {
                unlocked = JsonUtility.FromJson<Wrapper>(json).ToDict();
            }
            // else unlocked = new();
            return;
        }
    }
    #endregion

    [System.Serializable]
    private class Wrapper
    {
        public List<string> topics = new();
        public List<int[]> difficulties = new();

        public Wrapper(Dictionary<string, HashSet<DifficultyLevel>> dict)
        {
            foreach (var kvp in dict)
            {
                topics.Add(kvp.Key);

                List<int> list = new();
                foreach (var d in kvp.Value) list.Add((int)d);

                difficulties.Add(list.ToArray());
            }
        }

        public Dictionary<string, HashSet<DifficultyLevel>> ToDict()
        {
            Dictionary<string, HashSet<DifficultyLevel>> dict = new();

            for (int i = 0; i < topics.Count; i++)
            {
                HashSet<DifficultyLevel> set = new();

                foreach (int v in difficulties[i])
                {
                    set.Add((DifficultyLevel)v);
                }

                dict[topics[i]] = set;
            }

            return dict;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        Load();
        Debug.Log($"Unlocked modules: {unlocked.Values} = {unlocked.Count}");
    }
}
