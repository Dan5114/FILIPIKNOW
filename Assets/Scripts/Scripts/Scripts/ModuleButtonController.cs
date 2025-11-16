using UnityEngine;
using UnityEngine.UI;

public class ModuleButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color unlockedColor = Color.white;

    private ModuleUnlockManager unlockManager;
    [SerializeField] private string moduleName;


    public void RefreshState()
    {
        bool unlocked = unlockManager.IsModuleUnlocked(moduleName);

        button.interactable = unlocked;

        var img = button.GetComponent<Image>();
        if (img != null)
            img.color = unlocked ? unlockedColor : lockedColor;
    }

    private void Start()
    {
        unlockManager = FindObjectOfType<ModuleUnlockManager>();
        RefreshState();
    }
}
