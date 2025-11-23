using UnityEngine;
using UnityEngine.UI;

public class TopicButtonDifficultyHandler : MonoBehaviour
{
    [SerializeField] private string topicName;
    [SerializeField] private DifficultyLevel difficulty;
    [SerializeField] private Button button;
    [SerializeField] private Image unlockStatusIcon;

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color unlockedColor = Color.white;

    void Start()
    {
        DifficultyUnlockManager difficultyUnlockManager = DifficultyUnlockManager.Instance;
        bool unlocked = difficultyUnlockManager.IsUnlocked(topicName, difficulty);
        Debug.Log($"{topicName} {difficulty} unlocked: {unlocked}");

        var img = button.GetComponent<Image>();
        if (img != null)
            img.color = unlocked ? unlockedColor : lockedColor;

        if (!unlocked)
        {
            // canvasGroup.alpha = 0.4f;
            unlockStatusIcon.sprite = difficultyUnlockManager.LockedIcon;
            button.interactable = false;
        }
        else
        {
            // canvasGroup.alpha = 1f;
            unlockStatusIcon.sprite = difficultyUnlockManager.UnlockedIcon;
            button.interactable = true;

            // button.onClick.AddListener(() =>
            // {
            //     QuizManager.SelectedTopic = topicName;
            //     QuizManager.SelectedDifficulty = difficulty;
            //     UnityEngine.SceneManagement.SceneManager.LoadScene("QuizScene");
            // });
        }
    }
}
