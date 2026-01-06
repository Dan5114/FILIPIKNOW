using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Filipknow.UI
{
    public class NounsIntroductionManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject scrollObject;
        public TextMeshProUGUI scrollText;
        public Button backButton;
        public Button continueButton;
        
        [Header("Animation Settings")]
        public float textGenerationDelay = 2.0f;  // Start typing at exactly 2 seconds
        public float textGenerationSpeed = 0.001f;  // Very fast typing speed (was 0.002f)
        public string speakingAnimationName = "isSpeaking";  // Parameter name for character speaking animation
        
        [Header("Existing Animation")]
        public Animator scrollAnimator;
        public string scrollAnimationName = "ScrollPage";
        public string scrollCloseAnimationName = "ScrollPage";  // Same animation but reversed
        
        [Header("Text Content")]
        public string[] introductionTexts = new string[]
        {
            "Welcome to Nouns! Let's learn about people, places, and things in Filipino!",
            "In this lesson, you'll identify different types of nouns and practice using them correctly.",
            "Ready to start your noun adventure? Let's begin!"
        };
        
        private bool isAnimationPlaying = false;
        private Coroutine currentAnimationCoroutine;
        
        void Start()
        {
            try
            {
                // Check for EventSystem
                CheckEventSystem();
                Debug.Log("✅ CheckEventSystem completed");
                
                InitializeUI();
                Debug.Log("✅ InitializeUI completed");
                
                StartIntroductionSequence();
                Debug.Log("✅ StartIntroductionSequence completed");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ EXCEPTION in Start(): " + e.Message);
                Debug.LogError("Stack trace: " + e.StackTrace);
            }
        }
        
        void CheckEventSystem()
        {
            Debug.Log("🔍 Checking for EventSystem...");
            
            UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogWarning("⚠️ NO EVENTSYSTEM FOUND! UI buttons will not work!");
                Debug.LogWarning("⚠️ Creating EventSystem automatically...");
                
                try
                {
                    // Create EventSystem automatically
                    GameObject eventSystemGO = new GameObject("EventSystem");
                    eventSystem = eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    
                    Debug.Log("✅ EventSystem created successfully!");
                    Debug.Log($"   - GameObject: {eventSystemGO.name}");
                    Debug.Log($"   - EventSystem component: {eventSystem != null}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("❌ Failed to create EventSystem: " + e.Message);
                }
            }
            else
            {
                Debug.Log("✅ EventSystem already exists: " + eventSystem.name);
            }
        }
        
        void InitializeUI()
        {
            Debug.Log("📋 InitializeUI() called");
            
            try
            {
                // Clear text initially
                if (scrollText != null)
                {
                    scrollText.text = "";
                    Debug.Log("✅ Scroll text cleared");
                }
                else
                {
                    Debug.LogWarning("⚠️ Scroll text is null!");
                }
                
                // Setup button listeners with more robust error handling
                Debug.Log("📋 Calling SetupButtonListeners...");
                SetupButtonListeners();
                Debug.Log("✅ SetupButtonListeners completed");
                
                // Disable continue button initially (will be enabled after animation)
                if (continueButton != null)
                {
                    continueButton.interactable = false;
                    Debug.Log("✅ Continue button disabled initially - will enable after animation");
                }
                else
                {
                    Debug.LogWarning("⚠️ Continue button is null");
                }
                
                // Ensure back button is always enabled
                if (backButton != null)
                {
                    backButton.interactable = true;
                    Debug.Log("✅ Back button enabled and ready to click");
                }
                else
                {
                    Debug.LogWarning("⚠️ Back button is null");
                }
                
                // Setup animator if not assigned
                if (scrollAnimator == null && scrollObject != null)
                {
                    scrollAnimator = scrollObject.GetComponent<Animator>();
                    if (scrollAnimator != null)
                    {
                        Debug.Log("✅ Scroll animator found: " + scrollAnimator.name);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ No Animator component found on Scroll Object: " + scrollObject.name);
                        // Try to find animator on children
                        scrollAnimator = scrollObject.GetComponentInChildren<Animator>();
                        if (scrollAnimator != null)
                        {
                            Debug.Log("✅ Scroll animator found on child: " + scrollAnimator.name);
                        }
                    }
                }
                
                Debug.Log("✅ InitializeUI() completed successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ EXCEPTION in InitializeUI(): " + e.Message);
                Debug.LogError("Stack trace: " + e.StackTrace);
            }
        }
        
        void SetupButtonListeners()
        {
            Debug.Log("📋 SetupButtonListeners() called");
            
            // Back button - go to difficulty selection
            if (backButton != null)
            {
                Debug.Log($"🔍 Back button found: {backButton.name}");
                Debug.Log($"   - GameObject: {backButton.gameObject.name}");
                Debug.Log($"   - Interactable: {backButton.interactable}");
                Debug.Log($"   - Active: {backButton.gameObject.activeInHierarchy}");
                
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackButtonClicked);
                
                Debug.Log($"✅ Back button listener added! Listener count: {backButton.onClick.GetPersistentEventCount()}");
            }
            else
            {
                Debug.LogError("❌ Back button is NULL! Please assign it in the Inspector.");
            }
            
            // Continue button - go to nouns game
            if (continueButton != null)
            {
                Debug.Log($"🔍 Continue button found: {continueButton.name}");
                Debug.Log($"   - GameObject: {continueButton.gameObject.name}");
                Debug.Log($"   - Interactable: {continueButton.interactable}");
                Debug.Log($"   - Active: {continueButton.gameObject.activeInHierarchy}");
                
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueButtonClicked);
                
                Debug.Log($"✅ Continue button listener added! Listener count: {continueButton.onClick.GetPersistentEventCount()}");
            }
            else
            {
                Debug.LogError("❌ Continue button is NULL! Please assign it in the Inspector.");
            }
            
            Debug.Log("📋 SetupButtonListeners() completed");
        }
        
        void StartIntroductionSequence()
        {
            if (isAnimationPlaying) return;
            
            isAnimationPlaying = true;
            currentAnimationCoroutine = StartCoroutine(IntroductionSequenceCoroutine());
        }
        
        IEnumerator IntroductionSequenceCoroutine()
        {
            // Step 1: Start scroll animation and text generation simultaneously
            Coroutine scrollCoroutine = StartCoroutine(PlayScrollAnimationCoroutine());
            Coroutine textCoroutine = StartCoroutine(GenerateTextCoroutine());
            
            // Wait for both to complete
            yield return scrollCoroutine;
            yield return textCoroutine;
            
            // Step 2: Enable continue button
            EnableContinueButton();
            
            isAnimationPlaying = false;
        }
        
        IEnumerator PlayScrollAnimationCoroutine()
        {
            // Play the existing ScrollPage animation
            if (scrollAnimator != null)
            {
                Debug.Log($"Playing scroll animation: {scrollAnimationName}");
                scrollAnimator.Play(scrollAnimationName);
                
                // Wait for animation to complete
                yield return new WaitForSeconds(scrollAnimator.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                Debug.LogWarning("ScrollAnimator is null, skipping animation");
                yield return new WaitForSeconds(1f); // Fallback delay
            }
        }
        
        IEnumerator GenerateTextCoroutine()
        {
            if (scrollText == null) yield break;
            
            // Get appropriate introduction text based on difficulty
            string fullText = GetIntroductionTextForDifficulty();
            
            // Clear text first
            scrollText.text = "";
            
            // Small delay before starting text generation
            yield return new WaitForSeconds(textGenerationDelay);
            
            // Start character speaking animation
            Debug.Log($"🎭 Starting character animation - scrollAnimator: {(scrollAnimator != null ? "ASSIGNED" : "NULL")}");
            if (scrollAnimator != null)
            {
                Debug.Log($"🎭 Setting {speakingAnimationName} to true on animator: {scrollAnimator.name}");
                scrollAnimator.SetBool(speakingAnimationName, true);
                Debug.Log($"🎭 Character speaking animation started - Parameter exists: {HasParameter(scrollAnimator, speakingAnimationName)}");
            }
            else
            {
                Debug.LogWarning("❌ Scroll animator is NULL! Please assign it in the Inspector.");
            }
            
            // Generate text character by character
            for (int i = 0; i <= fullText.Length; i++)
            {
                scrollText.text = fullText.Substring(0, i);
                
                // ⌨️ Play typing sound for non-space characters
                if (i < fullText.Length && GameAudioManager.Instance != null && !char.IsWhiteSpace(fullText[i]))
                {
                    GameAudioManager.Instance.PlayTypingSound();
                }
                
                yield return new WaitForSeconds(textGenerationSpeed);
            }
            
            // Stop character speaking animation when text is complete
            if (scrollAnimator != null)
            {
                scrollAnimator.SetBool(speakingAnimationName, false);
                Debug.Log("🎭 Character speaking animation stopped");
            }
        }
        
        /// <summary>
        /// Checks if an animator has a specific parameter
        /// </summary>
        private bool HasParameter(Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
        
        string GetIntroductionTextForDifficulty()
        {
            string difficulty = SceneController.Instance?.GetSelectedDifficultyAsString() ?? "Easy";
            Debug.Log($"DEBUG: NounsIntroductionManager - Selected difficulty: {difficulty}");

            string intro = "Welcome to Nouns!\n\n" +
                           "Let's review Filipino nouns like 'silya' (chair), 'pinto' (door), and 'sapatos' (shoes).\n\n" +
                           "You'll see questions, choose the correct Filipino word. Take your time and don't worry about making mistakes!\n\n" +
                           "Ready to begin your noun adventure?";

            return intro;
            
            // switch (difficulty.ToLower())
            // {
            //     case "easy":
            //         return "Welcome to Nouns - Easy Mode!\n\n" +
            //                "Let's start with basic Filipino nouns like 'bahay' (house), 'kotse' (car), and 'aso' (dog).\n\n" +
            //                "You'll see questions, choose the correct Filipino word. Take your time and don't worry about making mistakes!\n\n" +
            //                "Ready to begin your noun adventure?";
                           
            //     case "medium":
            //         return "Welcome to Nouns - Medium Mode!\n\n" +
            //                "Now we'll practice your advanced level! You'll see Filipino sentences with missing words.\n\n" +
            //                "Examples: 'Ang ___ ay tumatakbo sa parke.' (The ___ is running in the park.)\n\n" +
            //                "Select the correct Filipino noun to complete each sentence. You've got this!\n\n" +
            //                "Let's continue your learning journey!";
                           
            //     case "hard":
            //         return "Welcome to Nouns - Hard Mode!\n\n" +
            //                "Challenge time! Now you'll identify nouns in complex Filipino sentences.\n\n" +
            //                "Examples: 'Ang pagmamahal sa bayan ay dakila.' (His love for the country is great.)\n\n" +
            //                "Select the noun you find in each sentence. This level will test your knowledge!\n\n" +
            //                "Are you ready for the ultimate noun challenge?";
                           
            //     default:
            //         return introductionTexts[0]; // Fallback to default
            // }
        }
        
        void EnableContinueButton()
        {
            if (continueButton != null)
            {
                continueButton.interactable = true;
                Debug.Log("Continue button enabled!");
            }
            else
            {
                Debug.LogError("Continue button is null when trying to enable it!");
            }
        }
        
        void OnBackButtonClicked()
        {            
            // Use SceneController if available, otherwise use direct SceneManager
            if (SceneController.Instance != null)
            {
                Debug.Log("Using SceneController.Instance to load scene");
                // SceneController.Instance.LoadScene("NounsDifficultySelection");
                SceneManager.LoadScene("Module 1");
            }
            else
            {
                Debug.LogWarning("⚠️ SceneController.Instance is null, using SceneManager directly");
                // UnityEngine.SceneManagement.SceneManager.LoadScene("NounsDifficultySelection");
                SceneManager.LoadScene("Module 1");
            }
        }
        
        void OnContinueButtonClicked()
        {
            // Use SceneController if available, otherwise use direct SceneManager
            if (SceneController.Instance != null)
            {
                Debug.Log("Using SceneController.Instance.GoToNounsGame()");
                SceneController.Instance.GoToNounsGame();
            }
            else
            {
                Debug.LogWarning("⚠️ SceneController.Instance is null, using SceneManager directly");
                SceneManager.LoadScene("Nouns");
            }
        }
        
        // Update method to test button clicks manually
        void Update()
        {
            // Press B key to test back button
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("⌨️ B key pressed - manually triggering back button");
                OnBackButtonClicked();
            }
            
            // Press C key to test continue button
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("⌨️ C key pressed - manually triggering continue button");
                OnContinueButtonClicked();
            }
        }
        
        IEnumerator CloseAndNavigateCoroutine()
        {
            // Disable buttons during closing animation
            if (backButton != null) backButton.interactable = false;
            if (continueButton != null) continueButton.interactable = false;
            
            // Play closing animation
            if (scrollAnimator != null)
            {
                Debug.Log("Playing scroll closing animation");
                
                // If you have a separate closing animation, use it
                // Otherwise, we can reverse the current animation or use a different approach
                scrollAnimator.Play(scrollCloseAnimationName, 0, 0f);
                scrollAnimator.speed = -1f; // Play animation in reverse
                
                // Wait for animation to complete
                yield return new WaitForSeconds(scrollAnimator.GetCurrentAnimatorStateInfo(0).length);
                
                // Reset animator speed
                scrollAnimator.speed = 1f;
            }
            else
            {
                // Fallback delay if no animator
                yield return new WaitForSeconds(0.5f);
            }
            
            // Navigate to Nouns game
            if (SceneController.Instance != null)
            {
                SceneController.Instance.GoToNounsGame();
            }
            else
            {
                Debug.LogError("SceneController.Instance is null!");
            }
        }
        
        void OnDestroy()
        {
            // Stop any running coroutines
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
            }
        }
        
        // Public method to restart introduction (useful for testing)
        public void RestartIntroduction()
        {
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
            }
            
            InitializeUI();
            StartIntroductionSequence();
        }
        
        // Public method to skip animation (useful for testing)
        public void SkipAnimation()
        {
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
            }
            
            // Instantly set final states
            if (scrollText != null)
            {
                scrollText.text = GetIntroductionTextForDifficulty();
            }
            
            if (continueButton != null)
            {
                continueButton.interactable = true;
            }
            
            isAnimationPlaying = false;
        }
    }
}
