using UnityEngine;
using UnityEngine.UI;

public class TopicButtonDifficultyHandler : MonoBehaviour
{
    [SerializeField] private string topicName;
    [SerializeField] private DifficultyLevel difficulty;
    [SerializeField] private Button button;

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color unlockedColor = Color.white;

    void Start()
    {
        bool unlocked = DifficultyUnlockManager.Instance.IsUnlocked(topicName, difficulty);
        Debug.Log($"{topicName} {difficulty} unlocked: {unlocked}");

        var img = button.GetComponent<Image>();
        if (img != null)
            img.color = unlocked ? unlockedColor : lockedColor;

        if (!unlocked)
        {
            // canvasGroup.alpha = 0.4f;
            button.interactable = false;
        }
        else
        {
            // canvasGroup.alpha = 1f;
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
