using UnityEngine;

/// <summary>
/// Ensures SceneController exists from the very beginning
/// Attach this to any GameObject in the Main Menu scene
/// </summary>
public class SceneControllerBootstrap : MonoBehaviour
{
    [Header("Scene Controller Setup")]
    [Tooltip("If true, will create SceneController if it doesn't exist")]
    public bool autoCreateSceneController = true;
    
    void Awake()
    {
        if (autoCreateSceneController && SceneController.Instance == null)
        {
            Debug.Log("🔧 SceneController not found - creating new instance...");
            
            // Create a new GameObject with SceneController
            GameObject sceneControllerGO = new GameObject("SceneController");
            sceneControllerGO.AddComponent<SceneController>();
            
            Debug.Log("✅ SceneController created successfully!");
        }
        else if (SceneController.Instance != null)
        {
            Debug.Log("✅ SceneController already exists");
        }
    }
}

