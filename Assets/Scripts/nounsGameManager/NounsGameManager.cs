using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class NounsGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons; // For Easy (Multiple Choice)
    public Button backButton;
    
    [Header("Difficulty-Specific UI Panels")]
    public GameObject easyPanel;           // Multiple choice UI
    public GameObject mediumPanel;         // Fill-in-the-blank UI
    public GameObject hardPanel;          // Type answer + conversational UI
    
    [Header("Medium UI Elements")]
    public TMP_InputField fillInBlankInput;
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI mediumInstructionText;
    
    [Header("Hard UI Elements")]
    public TMP_InputField typingInput;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI hardInstructionText;
    public GameObject conversationalPanel;
    public TextMeshProUGUI conversationText;
    public Button[] conversationButtons;
    public TextMeshProUGUI[] conversationButtonTexts;
    
    [Header("Adaptive Dialog System")]
    public AdaptiveDialogManager adaptiveDialogManager;
    public AdaptiveChoiceManager adaptiveChoiceManager;
    
    [Header("Learning System")]
    public QuestionDatabase questionDatabase;
    public DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;
    public TMP_FontAsset timesBoldFont;
    
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Options Menu")]
    public OptionsMenu optionsMenu;
    
    [Header("Universal Font")]
    public bool useUniversalFont = true;

    // Game State
    private int currentQuestion = 0;
    private int score = 0;
    private List<UnifiedQuestionData> currentQuestions = new List<UnifiedQuestionData>();
    private UnifiedQuestionData currentQuestionData;
    private int sessionCorrectAnswers = 0;
    private int sessionTotalAnswers = 0;
    private float sessionStartTime;
    private float questionStartTime;
    private int conversationStep = 0;
    
    // Legacy support
    private List<QuestionData> reviewQuestions = new List<QuestionData>();
    private List<QuestionData> currentSessionQuestions = new List<QuestionData>();
    private Dictionary<int, float> questionResponseTimes = new Dictionary<int, float>();
    private Dictionary<int, int> questionAttempts = new Dictionary<int, int>();
    
    // Dialog content - Filipino
    private string[] dialogMessagesFilipino = {
        "Kumusta! Ako si Teacher Ana.",
        "Ngayon ay matututunan natin ang tungkol sa PANGNGALAN.",
        "Ang pangngalan ay mga salitang tumutukoy sa tao, lugar, bagay, o hayop.",
        "Halimbawa: aso, paaralan, Maria, tubig",
        "Handa ka na ba para sa ating laro?"
    };
    
    // Dialog content - English
    private string[] dialogMessagesEnglish = {
        "Hello! I am Teacher Ana.",
        "Today we will learn about NOUNS.",
        "Nouns are words that name people, places, things, or animals.",
        "Examples: dog, school, Maria, water",
        "Are you ready for our game?"
    };
    
    private int currentDialogIndex = 0;
    
    // UnifiedQuestionData class is now in its own file: Scripts/Scripts/UnifiedQuestionData.cs
    
    // Game content - Filipino
    private string[] questionsFilipino = {
        "Which of the following is a noun (pangngalan) in Filipino?",
        "Which is a proper noun (pangngalang pantangi) in Filipino?",
        "Which is a common noun (pangngalang pambalana) in Filipino?",
        "Which is a concrete noun (pangngalang tahas) in Filipino?",
        "Which is an abstract noun (pangngalang basal) in Filipino?",
        "Which is a collective noun (pangngalang lansakan) in Filipino?",
        "Which is a derived noun (pangngalang hango) in Filipino?",
        "How many nouns are in this sentence: 'Si Ana ay bumili ng tinapay sa panaderya.' (Ana bought bread at the bakery.)",
        "Fill in the blank: 'Ang ___ ay lumilipad sa langit.' (The ___ flies in the sky.)",
        "Which is a proper noun (pangngalang pantangi) in Filipino?"
    };
    
    private string[][] choicesFilipino = {
        new string[] { "tumakbo (ran)", "masaya (happy)", "aso (dog)", "maganda (beautiful)" },
        new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" },
        new string[] { "Maria (name)", "doktor (doctor)", "Pilipinas (Philippines)", "Juan (name)" },
        new string[] { "pag-ibig (love)", "lamesa (table)", "kaligayahan (happiness)", "takot (fear)" },
        new string[] { "aklat (book)", "pagkakaibigan (friendship)", "sapatos (shoes)", "telepono (phone)" },
        new string[] { "bundok (mountain)", "kawan (flock)", "bahay (house)", "kotse (car)" },
        new string[] { "guro (teacher)", "kaguruan (teaching staff)", "paaralan (school)", "mag-aaral (student)" },
        new string[] { "one", "two", "three", "four" },
        new string[] { "isda (fish)", "aso (dog)", "ibon (bird)", "pusa (cat)" },
        new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" }
    };
    
    private string[][] correctAnswersFilipino = {
        new string[] { "aso (dog)" },
        new string[] { "Maynila (Manila)" },
        new string[] { "doktor (doctor)" },
        new string[] { "lamesa (table)" },
        new string[] { "pagkakaibigan (friendship)" },
        new string[] { "kawan (flock)" },
        new string[] { "kaguruan (teaching staff)" },
        new string[] { "three" },
        new string[] { "ibon (bird)" },
        new string[] { "Maynila (Manila)" }
    };
    
    // Game content - English (same as Filipino for unified system)
    private string[] questionsEnglish = {
        "Which of the following is a noun (pangngalan) in Filipino?",
        "Which is a proper noun (pangngalang pantangi) in Filipino?",
        "Which is a common noun (pangngalang pambalana) in Filipino?",
        "Which is a concrete noun (pangngalang tahas) in Filipino?",
        "Which is an abstract noun (pangngalang basal) in Filipino?",
        "Which is a collective noun (pangngalang lansakan) in Filipino?",
        "Which is a derived noun (pangngalang hango) in Filipino?",
        "How many nouns are in this sentence: 'Si Ana ay bumili ng tinapay sa panaderya.' (Ana bought bread at the bakery.)",
        "Fill in the blank: 'Ang ___ ay lumilipad sa langit.' (The ___ flies in the sky.)",
        "Which is a proper noun (pangngalang pantangi) in Filipino?"
    };
    
    private string[][] choicesEnglish = {
        new string[] { "tumakbo (ran)", "masaya (happy)", "aso (dog)", "maganda (beautiful)" },
        new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" },
        new string[] { "Maria (name)", "doktor (doctor)", "Pilipinas (Philippines)", "Juan (name)" },
        new string[] { "pag-ibig (love)", "lamesa (table)", "kaligayahan (happiness)", "takot (fear)" },
        new string[] { "aklat (book)", "pagkakaibigan (friendship)", "sapatos (shoes)", "telepono (phone)" },
        new string[] { "bundok (mountain)", "kawan (flock)", "bahay (house)", "kotse (car)" },
        new string[] { "guro (teacher)", "kaguruan (teaching staff)", "paaralan (school)", "mag-aaral (student)" },
        new string[] { "one", "two", "three", "four" },
        new string[] { "isda (fish)", "aso (dog)", "ibon (bird)", "pusa (cat)" },
        new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" }
    };
    
    private string[][] correctAnswersEnglish = {
        new string[] { "aso (dog)" },
        new string[] { "Maynila (Manila)" },
        new string[] { "doktor (doctor)" },
        new string[] { "lamesa (table)" },
        new string[] { "pagkakaibigan (friendship)" },
        new string[] { "kawan (flock)" },
        new string[] { "kaguruan (teaching staff)" },
        new string[] { "three" },
        new string[] { "ibon (bird)" },
        new string[] { "Maynila (Manila)" }
    };

    // Unified Question Database - DepEd Filipino Curriculum Aligned (English Interface)
    private UnifiedQuestionData[] allQuestions = {
        // EASY QUESTIONS (Multiple Choice) - Grade 1 Level
        new UnifiedQuestionData {
            questionId = 1,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which of the following is a noun (pangngalan) in Filipino?",
            instruction = "Choose the noun from the options:",
            choices = new string[] { "tumakbo (ran)", "masaya (happy)", "aso (dog)", "maganda (beautiful)" },
            correctChoiceIndex = 2,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 2,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a proper noun (pangngalang pantangi) in Filipino?",
            instruction = "Choose the proper noun:",
            choices = new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 3,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a common noun (pangngalang pambalana) in Filipino?",
            instruction = "Choose the common noun:",
            choices = new string[] { "Maria (name)", "doktor (doctor)", "Pilipinas (Philippines)", "Juan (name)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 4,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a concrete noun (pangngalang tahas) in Filipino?",
            instruction = "Choose the concrete noun:",
            choices = new string[] { "pag-ibig (love)", "lamesa (table)", "kaligayahan (happiness)", "takot (fear)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 5,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is an abstract noun (pangngalang basal) in Filipino?",
            instruction = "Choose the abstract noun:",
            choices = new string[] { "aklat (book)", "pagkakaibigan (friendship)", "sapatos (shoes)", "telepono (phone)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 6,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a collective noun (pangngalang lansakan) in Filipino?",
            instruction = "Choose the collective noun:",
            choices = new string[] { "bundok (mountain)", "kawan (flock)", "bahay (house)", "kotse (car)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 7,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a derived noun (pangngalang hango) in Filipino?",
            instruction = "Choose the derived noun:",
            choices = new string[] { "guro (teacher)", "kaguruan (teaching staff)", "paaralan (school)", "mag-aaral (student)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 8,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "How many nouns are in this sentence: 'Si Ana ay bumili ng tinapay sa panaderya.' (Ana bought bread at the bakery.)",
            instruction = "Count the nouns in the sentence:",
            choices = new string[] { "one", "two", "three", "four" },
            correctChoiceIndex = 2,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 9,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Fill in the blank: 'Ang ___ ay lumilipad sa langit.' (The ___ flies in the sky.)",
            instruction = "Choose the appropriate noun:",
            choices = new string[] { "isda (fish)", "aso (dog)", "ibon (bird)", "pusa (cat)" },
            correctChoiceIndex = 2,
            xpReward = 10
        },
        new UnifiedQuestionData {
            questionId = 10,
            difficultyLevel = DifficultyLevel.Easy,
            questionType = QuestionType.MultipleChoice,
            questionText = "Which is a proper noun (pangngalang pantangi) in Filipino?",
            instruction = "Choose the proper noun:",
            choices = new string[] { "lungsod (city)", "Maynila (Manila)", "paaralan (school)", "guro (teacher)" },
            correctChoiceIndex = 1,
            xpReward = 10
        },
        
        // MEDIUM QUESTIONS (Fill-in-the-Blank) - Grade 2 Level
        new UnifiedQuestionData {
            questionId = 11,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with the correct Filipino noun:",
            instruction = "Fill in the blank with the correct noun:",
            sentenceTemplate = "Ang ___ ay tumatakbo sa parke. (The ___ is running in the park.)",
            blankWord = "aso",
            acceptableAnswers = new string[] {"aso", "mga aso", "ang aso", "pusa"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 12,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a proper noun (pangngalang pantangi):",
            instruction = "Fill in the blank with a proper noun:",
            sentenceTemplate = "Si ___ ay nagluluto ng adobo. (___ is cooking adobo.)",
            blankWord = "Maria",
            acceptableAnswers = new string[] {"Maria", "Si Maria", "Ana", "Juan"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 13,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a common noun (pangngalang pambalana):",
            instruction = "Fill in the blank with a common noun:",
            sentenceTemplate = "Ang ___ ay nagtuturo sa paaralan. (The ___ teaches at school.)",
            blankWord = "guro",
            acceptableAnswers = new string[] {"guro", "ang guro", "doktor", "nars"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 14,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a concrete noun (pangngalang tahas):",
            instruction = "Fill in the blank with a concrete noun:",
            sentenceTemplate = "Ang ___ ay nasa ibabaw ng mesa. (The ___ is on top of the table.)",
            blankWord = "aklat",
            acceptableAnswers = new string[] {"aklat", "ang aklat", "lapis", "papel"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 15,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with an abstract noun (pangngalang basal):",
            instruction = "Fill in the blank with an abstract noun:",
            sentenceTemplate = "Ang ___ ay napakahalaga sa buhay ng tao. (___ is very important in human life.)",
            blankWord = "pagmamahal",
            acceptableAnswers = new string[] {"pagmamahal", "pag-ibig", "kaligayahan", "kapayapaan"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 16,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a collective noun (pangngalang lansakan):",
            instruction = "Fill in the blank with a collective noun:",
            sentenceTemplate = "Ang ___ ay naglalakad sa damuhan. (The ___ is walking in the grass.)",
            blankWord = "kawan",
            acceptableAnswers = new string[] {"kawan", "ang kawan", "grupo", "pulutong"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 17,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a derived noun (pangngalang hango):",
            instruction = "Fill in the blank with a derived noun:",
            sentenceTemplate = "Ang ___ ay nagtuturo sa mga mag-aaral. (The ___ teaches students.)",
            blankWord = "kaguruan",
            acceptableAnswers = new string[] {"kaguruan", "ang kaguruan", "pamunuan", "pamahalaan"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 18,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a proper noun (pangngalang pantangi):",
            instruction = "Fill in the blank with a proper noun:",
            sentenceTemplate = "Si ___ ay nakatira sa Maynila. (___ lives in Manila.)",
            blankWord = "Juan",
            acceptableAnswers = new string[] {"Juan", "Si Juan", "Maria", "Pedro"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 19,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a common noun (pangngalang pambalana):",
            instruction = "Fill in the blank with a common noun:",
            sentenceTemplate = "Ang ___ ay nagbibigay ng gamot sa mga pasyente. (The ___ gives medicine to patients.)",
            blankWord = "doktor",
            acceptableAnswers = new string[] {"doktor", "ang doktor", "nars", "guro"},
            xpReward = 20
        },
        new UnifiedQuestionData {
            questionId = 20,
            difficultyLevel = DifficultyLevel.Medium,
            questionType = QuestionType.FillInTheBlank,
            questionText = "Complete the sentence with a concrete noun (pangngalang tahas):",
            instruction = "Fill in the blank with a concrete noun:",
            sentenceTemplate = "Ang ___ ay nasa ilalim ng puno. (The ___ is under the tree.)",
            blankWord = "bato",
            acceptableAnswers = new string[] {"bato", "ang bato", "dahon", "bulaklak"},
            xpReward = 20
        },
        
        // HARD QUESTIONS (Type Answer + Conversational) - Grade 3 Level
        new UnifiedQuestionData {
            questionId = 21,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "What noun (pangngalan) is in this sentence: 'Ang mga mag-aaral ay nag-aaral sa silid-aralan.' (The students are studying in the classroom.)",
            instruction = "Type the noun you see in the sentence:",
            correctAnswer = "mga mag-aaral",
            acceptableAnswers = new string[] {"mga mag-aaral", "mag-aaral", "silid-aralan", "mga mag-aaral at silid-aralan"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 22,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "Identify the noun in this sentence: 'Ang kanyang pagmamahal sa bayan ay dakila.' (His love for the country is great.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "pagmamahal",
            acceptableAnswers = new string[] {"pagmamahal", "bayan", "pagmamahal at bayan", "kanyang"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 23,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "What noun is in this sentence: 'Ang kagandahan ng kalikasan ay dapat pangalagaan.' (The beauty of nature should be protected.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "kagandahan",
            acceptableAnswers = new string[] {"kagandahan", "kalikasan", "kagandahan at kalikasan"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 24,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "Identify the noun in this sentence: 'Ang kabutihan ng isang tao ay nasa kanyang kalooban.' (The goodness of a person is in their heart.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "kabutihan",
            acceptableAnswers = new string[] {"kabutihan", "tao", "kalooban", "kabutihan, tao, at kalooban"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 25,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "What noun is in this sentence: 'Ang pagtulong sa kapwa ay tanda ng mabuting pag-uugali.' (Helping others is a sign of good character.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "pagtulong",
            acceptableAnswers = new string[] {"pagtulong", "kapwa", "pag-uugali", "pagtulong, kapwa, at pag-uugali"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 26,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "Identify the noun in this sentence: 'Ang kaalaman ay susi sa tagumpay.' (Knowledge is the key to success.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "kaalaman",
            acceptableAnswers = new string[] {"kaalaman", "susi", "tagumpay", "kaalaman at tagumpay"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 27,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "What noun is in this sentence: 'Ang pag-asa ay nagbibigay lakas sa mga taong may problema.' (Hope gives strength to people with problems.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "pag-asa",
            acceptableAnswers = new string[] {"pag-asa", "lakas", "tao", "problema"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 28,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "Identify the noun in this sentence: 'Ang pagkakaisa ng mga mamamayan ay nagdadala ng kapayapaan.' (The unity of citizens brings peace.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "pagkakaisa",
            acceptableAnswers = new string[] {"pagkakaisa", "mamamayan", "kapayapaan", "pagkakaisa at kapayapaan"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 29,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.TypeAnswer,
            questionText = "What noun is in this sentence: 'Ang paggalang sa nakatatanda ay tanda ng mabuting pagpapalaki.' (Respect for elders is a sign of good upbringing.)",
            instruction = "Type the noun in the sentence:",
            correctAnswer = "paggalang",
            acceptableAnswers = new string[] {"paggalang", "nakatatanda", "pagpapalaki", "paggalang at pagpapalaki"},
            xpReward = 30
        },
        new UnifiedQuestionData {
            questionId = 30,
            difficultyLevel = DifficultyLevel.Hard,
            questionType = QuestionType.Conversational,
            questionText = "Let's have a conversation about Filipino nouns!",
            instruction = "Answer Teacher Ana's questions about nouns:",
            correctAnswer = "pangngalan",
            acceptableAnswers = new string[] {"pangngalan", "noun", "pangalan"},
            isConversational = true,
            conversationPrompts = new string[] {
                "What do we call words that refer to names of people, things, animals, places, or events?",
                "Give an example of a proper noun that refers to a place.",
                "Give an example of an abstract noun that refers to feelings."
            },
            characterResponses = new string[] {
                "Correct! We call those words 'pangngalan' (nouns).",
                "Great! Examples of proper nouns for places are Maynila, Cebu, or Baguio.",
                "Excellent! Examples of abstract nouns for feelings are pagmamahal (love), kaligayahan (happiness), or pag-asa (hope)."
            },
            hints = new string[] {
                "Hint: This is the Filipino word for 'noun'.",
                "Hint: Places with specific names like cities.",
                "Hint: Feelings that you cannot touch like love."
            },
            xpReward = 30
        }
    };

    void Start()
    {
        // Get difficulty from SceneController first
        LoadDifficultyFromSceneController();
        Debug.Log($"🎯 NounsGameManager Start: Current Difficulty after loading: {currentDifficulty}");
        
        // Hide Summary Panel initially - it should only show after game ends
        HideSummaryPanel();
        
        // Initialize dialog boxes - hide question dialog, show intro dialog
        
        SetupUniversalFont();
        SetupTypewriter();
        SetupButtons();
        InitializeQuestions();
        InitializeAdvancedSystems();
        StartDialog();
    }
    
    
    void LoadDifficultyFromSceneController()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentSceneName == "NounsMedium")
        {
            currentDifficulty = DifficultyLevel.Medium;
            Debug.Log($"🎯 Auto-detected NounsMedium scene - setting difficulty to Medium");
            return;
        }
        if (currentSceneName == "NounsHard")
        {
            currentDifficulty = DifficultyLevel.Hard;
            Debug.Log($"🎯 Auto-detected NounsHard scene - setting difficulty to Hard");
            return;
        }
        // Default behavior for Nouns scene - get from SceneController
        if (SceneController.Instance != null)
        {
            DifficultyLevel selectedDifficulty = SceneController.Instance.GetSelectedDifficulty();
            if (selectedDifficulty != DifficultyLevel.Easy)
            {
                currentDifficulty = selectedDifficulty;
                Debug.Log($"🎯 Loaded difficulty from SceneController: {currentDifficulty}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ SceneController not found, using default difficulty: Easy");
        }
    }
    
    // Get appropriate dialog messages based on language setting
    private string[] GetDialogMessages()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return dialogMessagesFilipino;
        else
            return dialogMessagesEnglish;
    }
    
    // Get appropriate questions based on language setting
    private string[] GetQuestions()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return questionsFilipino;
        else
            return questionsEnglish;
    }
    
    // Get appropriate choices based on language setting
    private string[][] GetChoices()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return choicesFilipino;
        else
            return choicesEnglish;
    }
    
    // Get appropriate correct answers based on language setting
    private string[][] GetCorrectAnswers()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.IsFilipinoLanguage())
            return correctAnswersFilipino;
        else
            return correctAnswersEnglish;
    }
    
    void SetupUniversalFont()
    {
        if (useUniversalFont && FilipknowFontManager.Instance != null)
        {
            // Apply universal font to dialog text
            if (dialogText != null)
            {
                dialogText.font = FilipknowFontManager.Instance.GetCurrentFont();
                dialogText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                dialogText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
            }
            
            // Apply universal font to choice buttons
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.font = FilipknowFontManager.Instance.GetCurrentFont();
                        buttonText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                        buttonText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                    }
                }
            }
        }
        else if (timesBoldFont != null)
        {
            // Apply FilipknowMainFont directly to dialog text
            if (dialogText != null)
            {
                dialogText.font = timesBoldFont;
                Debug.Log($"NounsGameManager: Applied FilipknowMainFont to dialog text");
            }
            
            // Apply FilipknowMainFont to choice buttons
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.font = timesBoldFont;
                        Debug.Log($"NounsGameManager: Applied FilipknowMainFont to choice button");
                    }
                }
            }
        }
    }
    
    void SetupTypewriter()
    {
        // Set up typewriter effect
        if (typewriterEffect == null)
            typewriterEffect = GetComponent<TypewriterEffect>();
            
        if (typewriterEffect != null)
        {
            typewriterEffect.textComponent = dialogText;
            typewriterEffect.characterAnimator = characterAnimator;
            
<<<<<<< HEAD
=======
            // Set typing speed to be faster but still readable
            typewriterEffect.SetTypingSpeed(0.05f);  // Faster than default but still smooth
            Debug.Log("NounsGameManager: Set typing speed to 0.05f for faster typewriter effect");
            
>>>>>>> master
            // Set the font first
            if (timesBoldFont != null)
            {
                typewriterEffect.SetFont(timesBoldFont);
                Debug.Log($"NounsGameManager: Set font {timesBoldFont.name} to typewriter");
            }
            else if (useUniversalFont && FilipknowFontManager.Instance != null)
            {
                typewriterEffect.RefreshUniversalFont();
                Debug.Log("NounsGameManager: Applied universal font to typewriter");
            }
            
            // Force horizontal text orientation (this will re-apply the font)
            typewriterEffect.ForceHorizontalText();
            
            // Verify the font was applied correctly
            typewriterEffect.VerifyCurrentFont();
        }
    }
    
    void ConfigureDialogTextForAutoSizing()
    {
        if (dialogText != null)
        {
            // Enable auto-sizing for dialog text to prevent truncation
            dialogText.enableAutoSizing = true;
            dialogText.fontSizeMin = 30f;  // Increased for readability
            dialogText.fontSizeMax = 80f;  // Increased to 80pt as requested
            dialogText.enableWordWrapping = true;
            dialogText.overflowMode = TMPro.TextOverflowModes.Overflow;
            
            // Ensure proper alignment for question text
            dialogText.alignment = TMPro.TextAlignmentOptions.Center;
            dialogText.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            dialogText.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
            
            Debug.Log("Configured dialog text for auto-sizing (up to 80pt font)");
        }
    }
    
    void InitializeQuestions()
    {
        // Initialize unified questions based on current difficulty
        InitializeUnifiedQuestions();
        
        // Legacy SM2 integration (keep for backward compatibility)
        InitializeLegacyQuestions();
    }
    
    void InitializeUnifiedQuestions()
    {
        // Filter questions based on current difficulty
        currentQuestions.Clear();
        currentQuestions.AddRange(allQuestions.Where(q => q.difficultyLevel == currentDifficulty));
        
        sessionStartTime = Time.time;
        sessionCorrectAnswers = 0;
        sessionTotalAnswers = 0;
        currentQuestion = 0;
        
        Debug.Log($"🎯 Initialized {currentQuestions.Count} {currentDifficulty} questions");
    }
    
    void InitializeLegacyQuestions()
    {
        // Ensure SM2Algorithm exists
        if (SM2Algorithm.Instance == null)
        {
            Debug.LogWarning("SM2Algorithm.Instance is null, creating new instance...");
            GameObject sm2Object = new GameObject("SM2Algorithm");
            sm2Object.AddComponent<SM2Algorithm>();
        }
        
        // Add questions to SM2 system if not already added
        if (SM2Algorithm.Instance != null)
        {
            // Check if questions are already added
            List<QuestionData> existingQuestions = SM2Algorithm.Instance.GetAllQuestions();
            bool hasNounsQuestions = existingQuestions.Any(q => q.module == "Nouns");
            
            if (!hasNounsQuestions)
            {
                Debug.Log("Adding Nouns questions to SM2Algorithm...");
                string[] questions = GetQuestions();
                string[][] choices = GetChoices();
                for (int i = 0; i < questions.Length; i++)
                {
                    SM2Algorithm.Instance.AddQuestion(i, "Nouns", questions[i], choices[i], GetCorrectAnswerIndex(i));
                    Debug.Log($"Added question {i}: {questions[i]}");
                }
            }
        }
        
        // Get review questions and new questions
        if (SM2Algorithm.Instance != null)
        {
            reviewQuestions = SM2Algorithm.Instance.GetQuestionsForReview("Nouns");
            Debug.Log($"Found {reviewQuestions.Count} review questions");
            
            // If no review questions, get all new questions
            if (reviewQuestions.Count == 0)
            {
                List<QuestionData> allQuestions = SM2Algorithm.Instance.GetAllQuestions();
                foreach (QuestionData q in allQuestions)
                {
                    if (q.module == "Nouns" && q.repetitions == 0)
                    {
                        reviewQuestions.Add(q);
                    }
                }
                Debug.Log($"Added {reviewQuestions.Count} new questions");
            }
            
            // Ensure we have at least 10 questions for Easy mode
            if (currentDifficulty == DifficultyLevel.Easy && reviewQuestions.Count < 10)
            {
                // Add more questions from static arrays to reach 10
                string[] staticQuestions = GetQuestions();
                string[][] staticChoices = GetChoices();
                
                for (int i = reviewQuestions.Count; i < 10 && i < staticQuestions.Length; i++)
                {
                    QuestionData newQuestion = new QuestionData
                    {
                        questionId = i,
                        module = "Nouns",
                        question = staticQuestions[i],
                        choices = staticChoices[i],
                        correctAnswer = GetCorrectAnswerIndex(i),
                        repetitions = 0,
                        interval = 1,
                        easeFactor = 2.5f,
                        nextReview = DateTime.Now
                    };
                    reviewQuestions.Add(newQuestion);
                }
            }
            
            // Limit to 10 questions per session for complete learning
            if (reviewQuestions.Count > 10)
            {
                reviewQuestions = reviewQuestions.GetRange(0, 10);
            }
            
            Debug.Log($"Final review questions count: {reviewQuestions.Count}");
        }
        
        // Initialize session tracking
        sessionStartTime = Time.time;
        sessionCorrectAnswers = 0;
        sessionTotalAnswers = 0;
        currentSessionQuestions = new List<QuestionData>(reviewQuestions);
    }
    
    void InitializeAdvancedSystems()
    {
        // Initialize Learning Analytics
        if (LearningAnalytics.Instance == null)
        {
            GameObject analyticsObject = new GameObject("LearningAnalytics");
            analyticsObject.AddComponent<LearningAnalytics>();
        }
        
        // Initialize Gamification System
        if (GamificationSystem.Instance == null)
        {
            GameObject gamificationObject = new GameObject("GamificationSystem");
            gamificationObject.AddComponent<GamificationSystem>();
        }
        
        // Start session tracking
        if (SM2Algorithm.Instance != null)
        {
            SM2Algorithm.Instance.StartSession("Nouns");
        }
        
        // Subscribe to gamification events
        if (GamificationSystem.Instance != null)
        {
            GamificationSystem.Instance.OnLevelUp += OnLevelUp;
            GamificationSystem.Instance.OnAchievementUnlocked += OnAchievementUnlocked;
        }
    }
    
    
    int GetCorrectAnswerIndex(int questionIndex)
    {
        // Convert correct answers array to single index
        string[][] correctAnswers = GetCorrectAnswers();
        string[][] choices = GetChoices();
        
        if (questionIndex < correctAnswers.Length && questionIndex < choices.Length)
        {
            string[] correctAnswersForQuestion = correctAnswers[questionIndex];
            for (int i = 0; i < choices[questionIndex].Length; i++)
            {
                if (correctAnswersForQuestion.Contains(choices[questionIndex][i]))
                    return i;
            }
        }
        return 0;
    }

    void SetupButtons()
    {
        continueButton.onClick.AddListener(OnContinue);
        backButton.onClick.AddListener(GoBack);
        
        // Setup choice buttons for Easy (Multiple Choice)
        if (choiceButtons != null)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int index = i;
                choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClick(index));
                
                // Add ChoiceButtonFeedback component to each button
                ChoiceButtonFeedback feedback = choiceButtons[i].GetComponent<ChoiceButtonFeedback>();
                if (feedback == null)
                {
                    feedback = choiceButtons[i].gameObject.AddComponent<ChoiceButtonFeedback>();
                    Debug.Log($"Added ChoiceButtonFeedback component to {choiceButtons[i].name}");
                }
            }
            
            // Hide choice buttons initially
            foreach (Button button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
        
        // Setup conversation buttons for Hard (Conversational)
        if (conversationButtons != null)
        {
            for (int i = 0; i < conversationButtons.Length; i++)
            {
                int index = i;
                conversationButtons[i].onClick.AddListener(() => OnConversationButtonClick(index));
            }
            
            // Hide conversation buttons initially
            foreach (Button button in conversationButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    void StartDialog()
    {
        // Skip introduction dialog since we now have a dedicated NounsIntroduction scene
        // Go straight to the game questions
        Debug.Log("Skipping intro dialog - going straight to questions");
        StartGame();
    }
    
    void ShowIntroductionDialog()
    {
        string introMessage = GetIntroductionMessage();
        
        if (typewriterEffect != null)
        {
            // Use normal typewriter effect for introduction (no scrolling needed for short intro)
            typewriterEffect.StartTypewriter(introMessage, true);
            Debug.Log("Introduction dialog started");
        }
        else
        {
            Debug.LogError("TypewriterEffect is null! Cannot show introduction dialog.");
        }
    }
    
    void StartLegacyGame()
    {
        currentQuestion = 0;
        score = 0;
        
        // Hide all choice buttons initially - DisplayChoices will show them as needed
        if (choiceButtons != null)
        {
            foreach (Button button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
        
        continueButton.gameObject.SetActive(false);
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }
        
        Debug.Log("Starting legacy game for Easy difficulty");
        DisplayQuestion();
    }
    
    string GetIntroductionMessage()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => "Hello! I am Teacher Ana.\n\n" +
                                  "Today we will learn about FILIPINO NOUNS (Pangngalan).\n\n" +
                                  "📚 Aligned with DepEd Filipino Curriculum (Grade 1)\n\n" +
                                  "In Easy level, you just choose from four options.\n\n" +
                                  "🎯 You will learn:\n" +
                                  "• Identifying nouns (pangngalan)\n" +
                                  "• Types of nouns (Pantangi, Pambalana)\n" +
                                  "• Concrete and Abstract nouns (Tahas at Basal)\n" +
                                  "• Collective and Derived nouns (Lansakan at Hango)\n\n" +
                                  "Example: 'Which of the following is a noun?'\n" +
                                  "Options: tumakbo (ran), masaya (happy), aso (dog), maganda (beautiful)\n" +
                                  "Correct answer: aso (dog)\n\n" +
                                  "Are you ready?",
                                  
            DifficultyLevel.Medium => "Hello! I am Teacher Ana.\n\n" +
                                    "Now we're in Medium level!\n\n" +
                                    "📚 Aligned with DepEd Filipino Curriculum (Grade 2)\n\n" +
                                    "In Medium level, you need to fill in the blank in sentences.\n\n" +
                                    "🎯 You will learn:\n" +
                                    "• Using nouns in sentences\n" +
                                    "• Building sentences with nouns\n" +
                                    "• Identifying noun types in context\n" +
                                    "• Creating sentences\n\n" +
                                    "Example: 'Ang ___ ay tumatakbo sa parke.' (The ___ is running in the park.)\n" +
                                    "Answer: 'aso' (dog)\n\n" +
                                    "Are you ready?",
                                    
            DifficultyLevel.Hard => "Hello! I am Teacher Ana.\n\n" +
                                  "Now we're in Hard level!\n\n" +
                                  "📚 Aligned with DepEd Filipino Curriculum (Grade 3)\n\n" +
                                  "In Hard level, you need to:\n\n" +
                                  "🎯 You will learn:\n" +
                                  "• Identifying nouns in complex sentences\n" +
                                  "• Recognizing abstract and concrete nouns\n" +
                                  "• Building sentences with different types of nouns\n" +
                                  "• Having conversations about nouns\n\n" +
                                  "Example: 'Identify the noun in: Ang pagmamahal sa bayan ay dakila.' (His love for the country is great.)\n" +
                                  "Answer: 'pagmamahal' (love)\n\n" +
                                  "Are you ready for the challenge?",
                                  
            _ => "Welcome to Filipino Nouns learning!"
        };
    }

    void ShowDialog()
    {
        string[] dialogMessages = GetDialogMessages();
        if (currentDialogIndex < dialogMessages.Length)
        {
            if (typewriterEffect != null)
            {
                typewriterEffect.StartTypewriter(dialogMessages[currentDialogIndex]);
            }
            else
            {
                dialogText.text = dialogMessages[currentDialogIndex];
            }
        }
    }

    public void NextDialog()
    {
        // Stop typewriter effect if it's running (user pressed continue)
        if (typewriterEffect != null && typewriterEffect.IsTyping())
        {
            typewriterEffect.OnContinuePressed();
            return; // Don't advance dialog yet, let the user press continue again
        }
        
        currentDialogIndex++;
        
        string[] dialogMessages = GetDialogMessages();
        if (currentDialogIndex >= dialogMessages.Length)
        {
            StartGame();
        }
        else
        {
            ShowDialog();
        }
    }

    void StartGame()
    {
        currentQuestion = 0;
        score = 0;
        
        // Hide choice buttons initially - they'll be shown after question text completes
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        continueButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        
        // Use legacy system for ALL difficulties to maintain same flow
        Debug.Log($"🎯 Using legacy system for {currentDifficulty} difficulty");
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        // Start timing this question
        questionStartTime = Time.time;
        Debug.Log($"⏱️ Question {currentQuestion} started at {questionStartTime:F2}");
        
        // Handle Medium difficulty - use EXACT same flow as Easy, just different content
        if (currentDifficulty == DifficultyLevel.Medium && currentQuestions.Count > 0 && currentQuestion < currentQuestions.Count)
        {
            Debug.Log($"Using Medium question {currentQuestion} - SAME FLOW AS EASY");
            currentQuestionData = currentQuestions[currentQuestion];
            
            // Use EXACT same logic as Easy mode fallback
            string questionText = currentQuestionData.sentenceTemplate;
            string[] choices = currentQuestionData.acceptableAnswers;
            
            Debug.Log($"Question text: {questionText}");
            Debug.Log($"Choices count: {choices.Length}");
            
            // Use adaptive dialog system if available
            if (adaptiveDialogManager != null)
            {
                Debug.Log("Using adaptive dialog system");
                adaptiveDialogManager.ShowDialog(questionText, () => {
                    // Display choices after dialog is shown
                    DisplayChoices(choices);
                });
            }
            else if (typewriterEffect != null)
            {
                Debug.Log("Using typewriter effect");
                // Configure dialog text for auto-sizing first
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                // Clear any existing callbacks to prevent multiple subscriptions
                typewriterEffect.OnTypingCompleted = null;
                // Start typewriter with completion callback
                typewriterEffect.StartTypewriter(questionText);
                typewriterEffect.OnTypingCompleted += () => {
                    // Display choices after typewriter completes
                    DisplayChoices(choices);
                };
            }
            else if (dialogText != null)
            {
                Debug.Log("Using direct dialog text");
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                dialogText.text = questionText;
                DisplayChoices(choices);
            }
            else
            {
                Debug.LogError("❌ No dialog system available!");
            }
            return;
        }
        
        // Fallback to static questions if no review questions available
        string[] questions = GetQuestions();
        if (reviewQuestions.Count == 0 && currentQuestion < questions.Length)
        {
            Debug.Log($"Using fallback static question {currentQuestion}");
            
            string questionText = questions[currentQuestion];
            string[][] choices = GetChoices();
            
            Debug.Log($"Question text: {questionText}");
            Debug.Log($"Choices count: {choices[currentQuestion].Length}");
            
            // Use adaptive dialog system if available
            if (adaptiveDialogManager != null)
            {
                Debug.Log("Using adaptive dialog system");
                adaptiveDialogManager.ShowDialog(questionText, () => {
                    // Display choices after dialog is shown
                    DisplayChoices(choices[currentQuestion]);
                });
            }
            else if (typewriterEffect != null)
            {
                Debug.Log("Using typewriter effect");
                // Configure dialog text for auto-sizing first
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                // Clear any existing callbacks to prevent multiple subscriptions
                typewriterEffect.OnTypingCompleted = null;
                // Start typewriter with completion callback
                typewriterEffect.StartTypewriter(questionText);
                typewriterEffect.OnTypingCompleted += () => {
                    // Display choices after typewriter completes
                    DisplayChoices(choices[currentQuestion]);
                };
            }
            else if (dialogText != null)
            {
                Debug.Log("Using direct dialog text");
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                dialogText.text = questionText;
                DisplayChoices(choices[currentQuestion]);
            }
            else
            {
                Debug.LogError("No dialog system available!");
            }
            return;
        }
        
        if (currentQuestion >= reviewQuestions.Count)
        {
            EndGame();
            return;
        }

        // Get the current question from the review list
        QuestionData currentQ = reviewQuestions[currentQuestion];
        Debug.Log($"Displaying question {currentQuestion}: {currentQ.question}");
        
        // Use adaptive dialog system if available
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(currentQ.question, () => {
                // Display choices after dialog is shown
                DisplayChoices(currentQ.choices);
            });
        }
        else if (typewriterEffect != null)
        {
            // Configure dialog text for auto-sizing first
            // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
            // Clear any existing callbacks to prevent multiple subscriptions
            typewriterEffect.OnTypingCompleted = null;
            // Start typewriter with completion callback
            typewriterEffect.StartTypewriter(currentQ.question);
            typewriterEffect.OnTypingCompleted += () => {
                // Display choices after typewriter completes
                DisplayChoices(currentQ.choices);
            };
        }
        else
        {
            // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
            dialogText.text = currentQ.question;
            DisplayChoices(currentQ.choices);
        }
    }
    
    void DisplayChoices(string[] choices)
    {
        Debug.Log($"DisplayChoices called with {choices.Length} choices");
        
        // Use adaptive choice manager if available
        if (adaptiveChoiceManager != null)
        {
            Debug.Log("Using adaptive choice manager");
            adaptiveChoiceManager.DisplayChoices(choices, OnChoiceSelected);
            return;
        }
        
        // Fallback to legacy choice buttons
        Debug.Log($"Using legacy choice buttons. Available buttons: {choiceButtons?.Length ?? 0}");
        
        if (choiceButtons == null)
        {
            Debug.LogError("Choice buttons array is null!");
            return;
        }
        
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                Debug.Log($"Setting up button {i}: {choices[i]}");
                choiceButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                
                if (buttonText != null)
                {
                    buttonText.text = choices[i];
                    
                    // Enable auto-sizing for choice buttons with larger font sizes
                    buttonText.enableAutoSizing = true;
                    buttonText.fontSizeMin = 18f;  // Increased from 16f
                    buttonText.fontSizeMax = 40f;  // Increased from 36f
                    buttonText.enableWordWrapping = true;
                    buttonText.overflowMode = TMPro.TextOverflowModes.Overflow;
                    
                    // Set text color to WHITE for visibility against brown background
                    buttonText.color = Color.white;
                    
                    // Set alignment for better readability
                    buttonText.alignment = TMPro.TextAlignmentOptions.Center;
                    buttonText.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                    buttonText.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
                    
                    Debug.Log($"Button {i} text set to: {buttonText.text} (WHITE color)");
                }
                else
                {
                    Debug.LogError($"Button {i} has no TextMeshProUGUI component!");
                }
                
                choiceButtons[i].interactable = true;
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
                Debug.Log($"Button {i} hidden (no choice for this index)");
            }
        }
        
        // DON'T reset buttons here - they should stay green/red until Continue is clicked
        // ResetAllButtonFeedback(); // REMOVED - moved to NextQuestion()
    }
    
    /// <summary>
    /// Shows visual feedback on the clicked button (green for correct, red for wrong)
    /// </summary>
    void ShowButtonFeedback(int buttonIndex, bool isCorrect)
    {
        if (choiceButtons == null || buttonIndex >= choiceButtons.Length) return;
        
        Button clickedButton = choiceButtons[buttonIndex];
        if (clickedButton == null) return;
        
        ChoiceButtonFeedback feedback = clickedButton.GetComponent<ChoiceButtonFeedback>();
        if (feedback != null)
        {
            if (isCorrect)
            {
                feedback.ShowCorrect();
                Debug.Log($"✅ Button {buttonIndex}: Showing GREEN (correct)");
            }
            else
            {
                feedback.ShowWrong();
                Debug.Log($"❌ Button {buttonIndex}: Showing RED (wrong)");
            }
        }
        else
        {
            Debug.LogWarning($"ChoiceButtonFeedback component not found on button {buttonIndex}");
        }
    }
    
    /// <summary>
    /// Resets all choice buttons to default appearance
    /// </summary>
    void ResetAllButtonFeedback()
    {
        if (choiceButtons == null) return;
        
        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                ChoiceButtonFeedback feedback = button.GetComponent<ChoiceButtonFeedback>();
                if (feedback != null)
                {
                    feedback.ResetToDefault();
                }
            }
        }
        
        Debug.Log("🔄 All buttons reset to default appearance");
    }
    
    private bool canClickChoices = true; // Flag to prevent clicks without transparency
    
    /// <summary>
    /// Disables all choice buttons to prevent multiple clicks
    /// </summary>
    void DisableAllChoiceButtons()
    {
        // Just set flag - buttons stay fully opaque!
        canClickChoices = false;
        Debug.Log("🔒 All choice buttons disabled (flag set, stays opaque)");
    }
    
    /// <summary>
    /// Enables all choice buttons for the next question
    /// </summary>
    void EnableAllChoiceButtons()
    {
        // Just set flag - buttons stay fully opaque!
        canClickChoices = true;
        Debug.Log("🔓 All choice buttons enabled (flag set)");
    }

    // New method for adaptive choice system
    public void OnChoiceSelected(string selectedChoice)
    {
        Debug.Log($"Adaptive choice selected: {selectedChoice}");
        OnAnswerSelected(selectedChoice);
    }
    
    // Overloaded method for adaptive choice system
    public void OnAnswerSelected(string selectedAnswer)
    {
        string correctAnswer = "";
        bool isCorrect = false;
        
        // Find the button index for legacy compatibility
        int buttonIndex = -1;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i].gameObject.activeInHierarchy)
            {
                string buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text;
                if (buttonText == selectedAnswer)
                {
                    buttonIndex = i;
                    break;
                }
            }
        }
        
        ProcessAnswer(selectedAnswer, correctAnswer, isCorrect, buttonIndex);
    }
    
    public void OnAnswerSelected(int buttonIndex)
    {
        
        if (choiceButtons == null || buttonIndex >= choiceButtons.Length)
        {
            Debug.LogError($"Invalid button index: {buttonIndex}");
            return;
        }
        
        // Check if current question is valid
        string[] questions = GetQuestions();
        if (reviewQuestions.Count == 0 && currentQuestion >= questions.Length)
        {
            Debug.Log("Game has ended, ignoring choice button click");
            return;
        }
        
        if (reviewQuestions.Count > 0 && currentQuestion >= reviewQuestions.Count)
        {
            Debug.Log("Review questions ended, ignoring choice button click");
            return;
        }
        
        string selectedAnswer = choiceButtons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        string correctAnswer = "";
        bool isCorrect = false;
        
        Debug.Log($"Answer selected: {selectedAnswer}");
        ProcessAnswer(selectedAnswer, correctAnswer, isCorrect, buttonIndex);
    }
    
    void ProcessAnswer(string selectedAnswer, string correctAnswer, bool isCorrect, int buttonIndex)
    {
        
        // Handle fallback case (static questions)
        string[] questions = GetQuestions();
        string[][] correctAnswers = GetCorrectAnswers();
        if (reviewQuestions.Count == 0 && currentQuestion < questions.Length)
        {
            Debug.Log("Using fallback answer checking");
            
            // Handle Medium difficulty
            if (currentDifficulty == DifficultyLevel.Medium && currentQuestionData != null)
            {
                Debug.Log("Medium difficulty: Using unified question data");
                correctAnswer = currentQuestionData.blankWord;
                
                // Check if selected answer matches the blank word
                isCorrect = selectedAnswer.ToLower().Contains(currentQuestionData.blankWord.ToLower());
                
                Debug.Log($"Medium answer check: '{selectedAnswer}' contains '{correctAnswer}' = {isCorrect}");
            }
            else
            {
                // Handle Easy difficulty (original logic)
                string[] correctAnswersForQuestion = correctAnswers[currentQuestion];
                correctAnswer = correctAnswersForQuestion[0]; // Use first correct answer for display
                
                // Check if selected answer is in the correct answers array
                isCorrect = System.Array.Exists(correctAnswersForQuestion, answer => 
                    string.Equals(selectedAnswer.Trim(), answer.Trim(), System.StringComparison.OrdinalIgnoreCase));
                
                Debug.Log($"Easy answer check: '{selectedAnswer}' vs '{correctAnswer}' = {isCorrect}");
            }
        }
        else if (currentQuestion >= reviewQuestions.Count)
        {
            Debug.LogError("Current question index is out of range!");
            return;
        }
        else
        {
            // Use SM2 question data
            QuestionData currentQ = reviewQuestions[currentQuestion];
            correctAnswer = currentQ.choices[currentQ.correctAnswer];
            
            // More robust answer comparison
            isCorrect = string.Equals(selectedAnswer.Trim(), correctAnswer.Trim(), System.StringComparison.OrdinalIgnoreCase);
        }
        
        // Debug logging
        Debug.Log($"Selected Answer: '{selectedAnswer}'");
        Debug.Log($"Correct Answer: '{correctAnswer}'");
        Debug.Log($"Is Correct: {isCorrect}");
        
        // Track response time
        float responseTime = Time.time - sessionStartTime;
        
        // Determine question ID for tracking
        int questionId = 0;
        if (reviewQuestions.Count > 0 && currentQuestion < reviewQuestions.Count)
        {
            questionId = reviewQuestions[currentQuestion].questionId;
        }
        else if (reviewQuestions.Count == 0 && currentQuestion < questions.Length)
        {
            questionId = currentQuestion; // Use currentQuestion as ID for fallback
        }
        
        // Track response time
        questionResponseTimes[questionId] = responseTime;
        
        // Track attempts
        if (!questionAttempts.ContainsKey(questionId))
            questionAttempts[questionId] = 0;
        questionAttempts[questionId]++;
        
        // Play sound effects via GameAudioManager
        if (GameAudioManager.Instance != null)
        {
            if (isCorrect)
            {
                GameAudioManager.Instance.PlayCorrectAnswer();
                Debug.Log("✅ Playing correct answer sound via GameAudioManager");
            }
            else
            {
                GameAudioManager.Instance.PlayWrongAnswer();
                Debug.Log("❌ Playing wrong answer sound via GameAudioManager");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ GameAudioManager.Instance is null! Create GameAudioManager in Main Menu scene.");
        }
        
        // Fallback: Play sound effects via OptionsMenu
        if (optionsMenu != null)
        {
            if (isCorrect)
            {
                optionsMenu.PlayCorrectAnswerSound();
            }
            else
            {
                optionsMenu.PlayIncorrectAnswerSound();
            }
        }
        
        // Trigger haptic feedback
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        // Show visual button feedback (green for correct, red for wrong)
        ShowButtonFeedback(buttonIndex, isCorrect);
        
        // Disable all buttons to prevent multiple clicks during feedback
        DisableAllChoiceButtons();
        
        // Update session statistics
        sessionTotalAnswers++;
        if (isCorrect)
        {
            sessionCorrectAnswers++;
            score += CalculateQuestionScore(currentQuestion, responseTime, questionAttempts[questionId]);
        }
        
        // Process answer with enhanced SM2 algorithm
        if (SM2Algorithm.Instance != null)
        {
            if (reviewQuestions.Count > 0 && currentQuestion < reviewQuestions.Count)
            {
                QuestionData questionData = reviewQuestions[currentQuestion];
                SM2Algorithm.Instance.ProcessAnswer(questionData, isCorrect, responseTime);
            }
            else if (reviewQuestions.Count == 0 && currentQuestion < questions.Length)
            {
                // Create a temporary QuestionData for fallback questions
                string[][] choices = GetChoices();
                QuestionData tempQuestion = new QuestionData(currentQuestion, "Nouns", questions[currentQuestion], choices[currentQuestion], GetCorrectAnswerIndex(currentQuestion));
                SM2Algorithm.Instance.ProcessAnswer(tempQuestion, isCorrect, responseTime);
            }
        }
        else
        {
            Debug.LogWarning("SM2Algorithm.Instance is null, skipping answer processing");
        }
        
        // Update gamification system
        if (GamificationSystem.Instance != null)
        {
            // Award XP based on performance
            int xpGained = CalculateXPGained(isCorrect, responseTime, questionAttempts[questionId]);
            GamificationSystem.Instance.AwardXP(xpGained, isCorrect ? "Correct Answer" : "Incorrect Answer");
            
            // Update streak
            if (isCorrect)
            {
                var userProgress = SM2Algorithm.Instance.GetUserProgress();
                if (userProgress != null)
                {
                    GamificationSystem.Instance.UpdateStreak(userProgress.currentStreak);
                }
            }
            
            // Check for achievements
            GamificationSystem.Instance.CheckAchievements();
        }
        
        // Generate detailed feedback
        string feedbackText = GenerateDetailedFeedback(selectedAnswer, correctAnswer, isCorrect, responseTime, questionId);
        
        // Use adaptive dialog system for feedback if available
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(feedbackText, () => {
                // DON'T hide choices - keep buttons visible with color until Continue is clicked
                // HideChoices(); // REMOVED - buttons stay visible
                ShowContinueButton();
            });
        }
        else if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(feedbackText);
            // DON'T hide choices - keep buttons visible with color until Continue is clicked
            // HideChoices(); // REMOVED - buttons stay visible
            ShowContinueButton();
        }
        else
        {
            // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
            dialogText.text = feedbackText;
            // DON'T hide choices - keep buttons visible with color until Continue is clicked
            // HideChoices(); // REMOVED - buttons stay visible
            ShowContinueButton();
        }
    }
    
    void HideChoices()
    {
        Debug.Log("🔒 HideChoices called - hiding choice buttons only");
        
        // Hide adaptive choices if available
        if (adaptiveChoiceManager != null)
        {
            adaptiveChoiceManager.HideChoices();
        }
        
        // Hide only the choice buttons (ChoiceButton1, ChoiceButton2, ChoiceButton3, ChoiceButton4)
        if (choiceButtons != null)
        {
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    button.interactable = false;
                    Debug.Log($"🔒 Hidden choice button: {button.name}");
                }
            }
        }
        
        // Hide conversation buttons if they exist
        if (conversationButtons != null)
        {
            foreach (Button button in conversationButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    button.interactable = false;
                    Debug.Log($"🔒 Hidden conversation button: {button.name}");
                }
            }
        }
        
        // DO NOT hide panels - keep the game UI intact
        
        // Also try to hide choice buttons by name as a backup
        HideChoiceButtonsByName();
    }
    
    void HideChoiceButtonsByName()
    {
        // Find and hide choice buttons by name as a backup method
        string[] buttonNames = { "ChoiceButton1", "ChoiceButton2", "ChoiceButton3", "ChoiceButton4" };
        
        foreach (string buttonName in buttonNames)
        {
            GameObject buttonObj = GameObject.Find(buttonName);
            if (buttonObj != null)
            {
                buttonObj.SetActive(false);
                Debug.Log($"🔒 Hidden choice button by name: {buttonName}");
            }
        }
    }
    
    void ForceHideChoices()
    {
        Debug.Log("🔒 ForceHideChoices called - hiding choice buttons only");
        
        // Force hide only the choice buttons (no parent hiding)
        if (choiceButtons != null)
        {
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    button.interactable = false;
                    Debug.Log($"🔒 Force hidden choice button: {button.name}");
                }
            }
        }
        
        // Force hide conversation buttons
        if (conversationButtons != null)
        {
            foreach (Button button in conversationButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    button.interactable = false;
                    Debug.Log($"🔒 Force hidden conversation button: {button.name}");
                }
            }
        }
        
        // DO NOT hide panels - keep the game UI intact
        
        // Also try to hide choice buttons by name as a backup
        HideChoiceButtonsByName();
    }
    
    
    void ShowContinueButton()
    {
        // Show continue button for next question
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(NextQuestion);
    }
    

    public void NextQuestion()
    {
        currentQuestion++;
        continueButton.gameObject.SetActive(false);
        
        // HIDE choice buttons immediately (prevent flashing of old buttons)
        HideChoices();
        
        // Reset button feedback when moving to next question
        ResetAllButtonFeedback();
        EnableAllChoiceButtons();  // Re-enable for when DisplayChoices is called
        Debug.Log("🔄 NextQuestion: Hidden buttons, reset feedback, ready for new question");
        
        // Check if we have more questions available
        string[] questions = GetQuestions();
        if (reviewQuestions.Count == 0 && currentQuestion >= questions.Length)
        {
            Debug.Log("No more questions available - ending game");
            EndGame();
            return;
        }
        else if (reviewQuestions.Count > 0 && currentQuestion >= reviewQuestions.Count)
        {
            Debug.Log("No more review questions available - ending game");
            EndGame();
            return;
        }
        
        DisplayQuestion();
    }

    void EndGame()
    {
        Debug.Log("🎯 EndGame called - hiding dialog box and showing summary panel");
        
        // Hide the dialog box so summary panel can display properly
        HideDialogBox();
        
        // Hide all choice systems
        HideChoices();
        
        // Hide continue button initially
        continueButton.gameObject.SetActive(false);
        
        // Show summary panel with session results
        ShowSummaryPanel();
    }
    
    void HideDialogBox()
    {
        // Hide the main dialog box so summary panel can display properly
        GameObject dialogBox = GameObject.Find("DialogBox");
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
            Debug.Log("✅ DialogBox hidden for summary display");
        }
        
        // Also hide dialog text component if it exists separately
        if (dialogText != null)
        {
            dialogText.gameObject.SetActive(false);
            Debug.Log("✅ DialogText hidden for summary display");
        }
        
        // Hide Back button
        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
            Debug.Log("✅ Back button hidden for summary display");
        }
        
        // Hide Character
        GameObject character = GameObject.Find("Character");
        if (character != null)
        {
            character.SetActive(false);
            Debug.Log("✅ Character hidden for summary display");
        }
    }

    void HideSummaryPanel()
    {
        // Hide Summary Panel at the start of the game - try multiple ways to find it
        GameObject summaryPanel = FindSummaryPanel();
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
            Debug.Log("✅ Summary Panel hidden at game start");
        }
        else
        {
            Debug.LogWarning("⚠️ Summary Panel not found - it may not exist in this scene");
        }
    }
    
    void ShowSummaryPanel()
    {
        // 🎉 Play victory music!
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PlayVictoryMusic();
            Debug.Log("🎉 Playing victory music via GameAudioManager");
        }
        else
        {
            Debug.LogWarning("⚠️ GameAudioManager.Instance is null! Create GameAudioManager in Main Menu scene.");
        }
        
        // Calculate session statistics
        float sessionAccuracy = sessionTotalAnswers > 0 ? (float)sessionCorrectAnswers / sessionTotalAnswers * 100f : 0f;
        float averageResponseTime = questionResponseTimes.Values.Count > 0 ? questionResponseTimes.Values.Average() : 0f;
        int currentStreak = 0;
        
        // Get streak from SM2 if available
        if (SM2Algorithm.Instance != null)
        {
            var userProgress = SM2Algorithm.Instance.GetUserProgress();
            if (userProgress != null)
            {
                currentStreak = userProgress.currentStreak;
            }
        }
        
        // Show summary panel
        GameObject summaryPanel = FindSummaryPanel();
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(true);
            
            // Update summary text elements with clean data only, Minecraft font, and black color
            TextMeshProUGUI[] summaryTexts = summaryPanel.GetComponentsInChildren<TextMeshProUGUI>();
            if (summaryTexts.Length >= 4)
            {
                // Apply Minecraft font and change text color to black
                foreach (TextMeshProUGUI text in summaryTexts)
                {
                    if (timesBoldFont != null) // Assuming timesBoldFont is Minecraft font
                    {
                        text.font = timesBoldFont;
                    }
                    text.color = Color.black; // Change text color to black
                }
                
                // Display only clean data (numbers) without labels
                summaryTexts[0].text = score.ToString(); // Total XP
                summaryTexts[1].text = averageResponseTime.ToString("F1"); // Speed
                summaryTexts[2].text = sessionAccuracy.ToString("F1"); // Accuracy
                summaryTexts[3].text = currentStreak.ToString(); // Streak
            }
            
            Debug.Log("✅ Summary panel displayed with clean data and Minecraft font");
        }
        else
        {
            Debug.LogError("❌ Summary Panel not found! Please check if 'Summary Panel' GameObject exists in the scene.");
            // Fallback: Show summary in dialog text instead
            ShowSummaryInDialog(sessionAccuracy, averageResponseTime, currentStreak);
        }
        
        // Set up tap-to-continue functionality
        SetupTapToContinue();
    }
    
    GameObject FindSummaryPanel()
    {
        // Try multiple ways to find the summary panel
        GameObject summaryPanel = GameObject.Find("Summary Panel");
        
        if (summaryPanel == null)
        {
            // Try alternative names
            summaryPanel = GameObject.Find("SummaryPanel");
        }
        
        if (summaryPanel == null)
        {
            // Try finding in children of MainCanvas
            GameObject mainCanvas = GameObject.Find("MainCanvas");
            if (mainCanvas != null)
            {
                Transform summaryTransform = mainCanvas.transform.Find("Summary Panel");
                if (summaryTransform != null)
                {
                    summaryPanel = summaryTransform.gameObject;
                }
            }
        }
        
        if (summaryPanel == null)
        {
            // Try finding by searching all GameObjects
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "Summary Panel")
                {
                    summaryPanel = obj;
                    break;
                }
            }
        }
        
        return summaryPanel;
    }
    
    void ShowSummaryInDialog(float sessionAccuracy, float averageResponseTime, int currentStreak)
    {
        // Fallback: Show summary in dialog text if Summary Panel not found
        string summaryText = $"🎉 Session Complete!\n\n" +
                           $"Total XP: {score}\n" +
                           $"Speed: {averageResponseTime:F1}s avg\n" +
                           $"Accuracy: {sessionAccuracy:F1}%\n" +
                           $"Streak: {currentStreak}\n\n" +
                           $"Tap anywhere to continue";
        
        if (dialogText != null)
        {
            dialogText.text = summaryText;
            Debug.Log("✅ Summary displayed in dialog text as fallback");
        }
        
        // Set up tap-to-continue on dialog
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnSummaryContinue);
        }
    }
    
    void SetupTapToContinue()
    {
        // Make the entire screen tappable to go back to difficulty selection
        GameObject summaryPanel = FindSummaryPanel();
        if (summaryPanel != null)
        {
            // Add a button component if it doesn't have one
            Button tapButton = summaryPanel.GetComponent<Button>();
            if (tapButton == null)
            {
                tapButton = summaryPanel.AddComponent<Button>();
            }
            
            // Set up tap to continue - tap anywhere on screen to go back
            tapButton.onClick.RemoveAllListeners();
            tapButton.onClick.AddListener(OnSummaryContinue);
            
            Debug.Log("✅ Screen tap-to-continue setup complete - tap anywhere to go back to difficulty selection");
        }
        else
        {
            Debug.LogWarning("⚠️ Summary Panel not found for tap-to-continue setup");
        }
    }

    public void GoBack()
    {
        // Navigate back to difficulty selection instead of Module 1
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene("NounsDifficultySelection");
        }
        else
        {
            SceneManager.LoadScene("NounsDifficultySelection");
        }
    }
    
    public void OnSummaryContinue()
    {
        // Navigate back to difficulty selection after summary
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene("NounsDifficultySelection");
        }
        else
        {
            SceneManager.LoadScene("NounsDifficultySelection");
        }
    }
    
    // SM2 Algorithm Support Methods
    private int CalculateQuestionScore(int questionIndex, float responseTime, int attempts)
    {
        int baseScore = 10;
        
        // Speed bonus (faster = higher score)
        float speedMultiplier = Mathf.Clamp(1.5f - (responseTime / 10f), 0.5f, 1.5f);
        
        // Attempt penalty (more attempts = lower score)
        float attemptMultiplier = Mathf.Clamp(1.0f - (attempts - 1) * 0.2f, 0.3f, 1.0f);
        
        // Difficulty bonus (harder questions = higher score)
        float difficultyMultiplier = 1.0f;
        if (questionIndex < reviewQuestions.Count)
        {
            QuestionData q = reviewQuestions[questionIndex];
            difficultyMultiplier = 1.0f + (SM2Algorithm.Instance.GetDifficultyRating(q) * 0.1f);
        }
        
        return Mathf.RoundToInt(baseScore * speedMultiplier * attemptMultiplier * difficultyMultiplier);
    }
    
    private int CalculateXPGained(bool isCorrect, float responseTime, int attempts)
    {
        int baseXP = 10;
        
        if (!isCorrect) return 0;
        
        // Speed bonus
        if (responseTime < 2f) baseXP += 5;
        else if (responseTime < 4f) baseXP += 3;
        else if (responseTime < 6f) baseXP += 1;
        
        // Attempt penalty
        if (attempts > 1) baseXP = Mathf.Max(1, baseXP - (attempts - 1) * 2);
        
        // Streak bonus
        if (SM2Algorithm.Instance != null)
        {
            var userProgress = SM2Algorithm.Instance.GetUserProgress();
            if (userProgress != null && userProgress.currentStreak > 1)
            {
                baseXP += userProgress.currentStreak;
            }
        }
        
        return baseXP;
    }
    
    private string GenerateDetailedFeedback(string selectedAnswer, string correctAnswer, bool isCorrect, float responseTime, int questionId)
    {
        string feedback = "";
        
        if (isCorrect)
        {
            feedback = "Tama! Ang '" + selectedAnswer + "' ay isang pangngalan.";
            
            // Add performance feedback
            if (responseTime < 2f)
                feedback += "\nNapakabilis mo! ⚡ Perfect!";
            else if (responseTime < 4f)
                feedback += "\nMabuti! 👍 Great job!";
            else if (responseTime < 6f)
                feedback += "\nTama! Good work!";
            else
                feedback += "\nTama, pero subukan mong mag-isip nang mas mabilis.";
                
            // Add score and XP information
            feedback += $"\nPuntos: {score}";
            
            // Add XP gained
            int xpGained = CalculateXPGained(isCorrect, responseTime, questionAttempts.ContainsKey(questionId) ? questionAttempts[questionId] : 1);
            if (xpGained > 0)
            {
                feedback += $"\nXP Gained: +{xpGained}";
            }
            
            // Add streak information
            if (SM2Algorithm.Instance != null)
            {
                var userProgress = SM2Algorithm.Instance.GetUserProgress();
                if (userProgress != null && userProgress.currentStreak > 1)
                {
                    feedback += $"\nStreak: {userProgress.currentStreak} 🔥";
                }
            }
            
            // Add SM2 progress info
            if (currentQuestion < reviewQuestions.Count && SM2Algorithm.Instance != null)
            {
                QuestionData q = reviewQuestions[currentQuestion];
                feedback += $"\nProgreso: {q.repetitions + 1} beses na naitama";
                feedback += $"\nMastery: {q.mastery:F1}%";
            }
        }
        else
        {
            feedback = "Mali. Subukan ulit!";
            
            // Show correct answer
            feedback += $"\nTamang sagot: {correctAnswer}";
            
            feedback += $"\nPuntos: {score}";
        }
        
        return feedback;
    }
    
    
    // Gamification event handlers
    void OnLevelUp(int newLevel)
    {
        Debug.Log($"🎉 Level Up! New Level: {newLevel}");
        
        // Show level up notification
        if (typewriterEffect != null)
        {
            string levelUpMessage = $"🎉 LEVEL UP! 🎉\n\nYou've reached Level {newLevel}!\n\nKeep up the great work!";
            typewriterEffect.StartTypewriter(levelUpMessage);
        }
    }
    
    void OnAchievementUnlocked(string achievementId)
    {
        Debug.Log($"🏅 Achievement Unlocked: {achievementId}");
        
        if (GamificationSystem.Instance != null)
        {
            var achievement = GamificationSystem.Instance.GetAchievement(achievementId);
            if (achievement != null)
            {
                // Show achievement notification
                if (typewriterEffect != null)
                {
                    string achievementMessage = $"🏅 ACHIEVEMENT UNLOCKED! 🏅\n\n{achievement.name}\n\n{achievement.description}\n\n+{achievement.xpReward} XP";
                    typewriterEffect.StartTypewriter(achievementMessage);
                }
            }
        }
    }
    
    // ==================== UNIFIED SYSTEM METHODS ====================
    
    void ShowNextQuestion()
    {
        if (currentQuestion >= currentQuestions.Count)
        {
            EndUnifiedGame();
            return;
        }
        
        currentQuestionData = currentQuestions[currentQuestion];
        
        // Show appropriate UI panel based on question type
        ShowQuestionUI();
        
        Debug.Log($"📝 Showing {currentDifficulty} question {currentQuestion + 1}: {currentQuestionData.questionText}");
    }
    
    void ShowQuestionUI()
    {
        // Hide all panels first
        HideAllPanels();
        
        // Show appropriate panel based on question type
        switch (currentQuestionData.questionType)
        {
            case QuestionType.MultipleChoice:
                ShowEasyUI();
                break;
            case QuestionType.FillInTheBlank:
                ShowMediumUI();
                break;
            case QuestionType.TypeAnswer:
                ShowHardUI();
                break;
            case QuestionType.Conversational:
                ShowConversationalUI();
                break;
        }
    }
    
    void HideAllPanels()
    {
        if (easyPanel != null) easyPanel.SetActive(false);
        if (mediumPanel != null) mediumPanel.SetActive(false);
        if (hardPanel != null) hardPanel.SetActive(false);
        if (conversationalPanel != null) conversationalPanel.SetActive(false);
    }
    
    void ShowEasyUI()
    {
        if (easyPanel != null)
        {
            easyPanel.SetActive(true);
        }
        
        // Hide choice buttons initially - they will be shown after typewriter completes
        if (choiceButtons != null)
        {
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
        
        // Display question text with typewriter effect, then show choices
        if (dialogText != null && currentQuestionData.questionText != null)
        {
            if (typewriterEffect != null)
            {
                // Configure dialog text for auto-sizing
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                
                // Clear any existing callbacks to prevent multiple subscriptions
                typewriterEffect.OnTypingCompleted = null;
                
                // Start typewriter with completion callback
                typewriterEffect.StartTypewriter(currentQuestionData.questionText);
                typewriterEffect.OnTypingCompleted += () => {
                    // Display choices after typewriter completes
                    DisplayChoices(currentQuestionData.choices);
                };
            }
            else
            {
                // Fallback: Set text directly and show choices immediately
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                dialogText.text = currentQuestionData.questionText;
                DisplayChoices(currentQuestionData.choices);
            }
        }
    }
    
    void ShowMediumUI()
    {
        // Use Easy UI structure (multiple choice buttons) for Medium difficulty
        if (easyPanel != null)
        {
            easyPanel.SetActive(true);
        }
        
        // Hide choice buttons initially - they will be shown after typewriter completes
        if (choiceButtons != null)
        {
            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
        
        // Display fill-in-the-blank sentence with typewriter effect, then show choices
        if (dialogText != null && currentQuestionData.sentenceTemplate != null)
        {
            if (typewriterEffect != null)
            {
                // Configure dialog text for auto-sizing
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                
                // Clear any existing callbacks to prevent multiple subscriptions
                typewriterEffect.OnTypingCompleted = null;
                
                // Start typewriter with completion callback
                typewriterEffect.StartTypewriter(currentQuestionData.sentenceTemplate);
                typewriterEffect.OnTypingCompleted += () => {
                    // Display choices after typewriter completes
                    if (currentQuestionData.acceptableAnswers != null && currentQuestionData.acceptableAnswers.Length > 0)
                    {
                        DisplayChoices(currentQuestionData.acceptableAnswers);
                    }
                };
            }
            else
            {
                // Fallback: Set text directly and show choices immediately
                // ConfigureDialogTextForAutoSizing(); // Disabled to allow manual ScrollRect setup
                dialogText.text = currentQuestionData.sentenceTemplate;
                
                if (currentQuestionData.acceptableAnswers != null && currentQuestionData.acceptableAnswers.Length > 0)
                {
                    DisplayChoices(currentQuestionData.acceptableAnswers);
                }
            }
        }
    }
    
    void ShowHardUI()
    {
        if (hardPanel != null)
        {
            hardPanel.SetActive(true);
        }
        
        if (questionText != null)
        {
            questionText.text = currentQuestionData.questionText;
        }
        
        if (hardInstructionText != null)
        {
            hardInstructionText.text = currentQuestionData.instruction;
        }
        
        if (typingInput != null)
        {
            typingInput.text = "";
            typingInput.ActivateInputField();
        }
    }
    
    void ShowConversationalUI()
    {
        conversationStep = 0;
        
        if (conversationalPanel != null)
        {
            conversationalPanel.SetActive(true);
        }
        
        ShowConversationPrompt();
    }
    
    void ShowConversationPrompt()
    {
        if (conversationStep >= currentQuestionData.conversationPrompts.Length)
        {
            ShowConversationTypingPrompt();
            return;
        }
        
        string prompt = currentQuestionData.conversationPrompts[conversationStep];
        
        if (conversationText != null)
        {
            conversationText.text = $"Teacher Ana: {prompt}";
        }
        
        SetupConversationButtons();
    }
    
    void SetupConversationButtons()
    {
        if (conversationButtons == null) return;
        
        // Hide all buttons first
        foreach (var button in conversationButtons)
        {
            if (button != null)
                button.gameObject.SetActive(false);
        }
        
        // Show hint button
        if (conversationButtons.Length > 0 && conversationButtons[0] != null)
        {
            conversationButtons[0].gameObject.SetActive(true);
            if (conversationButtonTexts != null && conversationButtonTexts[0] != null)
            {
                conversationButtonTexts[0].text = "💡 Hint";
            }
        }
        
        // Show "I know the answer" button
        if (conversationButtons.Length > 1 && conversationButtons[1] != null)
        {
            conversationButtons[1].gameObject.SetActive(true);
            if (conversationButtonTexts != null && conversationButtonTexts[1] != null)
            {
                conversationButtonTexts[1].text = "✅ I Know the Answer";
            }
        }
    }
    
    void OnChoiceButtonClick(int choiceIndex)
    {
        // Prevent clicks if disabled (buttons stay opaque!)
        if (!canClickChoices)
        {
            Debug.Log("🔒 Choice button click blocked - buttons disabled");
            return;
        }
        
        // Prevent clicks if game has ended or we're in summary mode
        
        // Check if we're using the unified system or legacy system
        if (currentQuestionData != null)
        {
            // Unified system (Medium/Hard)
            bool isCorrect = choiceIndex == currentQuestionData.correctChoiceIndex;
            string userAnswer = currentQuestionData.choices[choiceIndex];
            ProcessUnifiedAnswer(isCorrect, userAnswer);
        }
        else
        {
            // Legacy system (Easy)
            OnAnswerSelected(choiceIndex);
        }
    }
    
    void OnConversationButtonClick(int buttonIndex)
    {
        if (buttonIndex == 0) // Hint button
        {
            ShowHint();
        }
        else if (buttonIndex == 1) // I know the answer button
        {
            ShowConversationTypingPrompt();
        }
    }
    
    void ShowHint()
    {
        if (conversationStep < currentQuestionData.hints.Length)
        {
            string hint = currentQuestionData.hints[conversationStep];
            
            if (conversationText != null)
            {
                conversationText.text = $"💡 Hint: {hint}";
            }
        }
        
        Invoke(nameof(ShowCharacterResponse), 2f);
    }
    
    void ShowCharacterResponse()
    {
        if (conversationStep < currentQuestionData.characterResponses.Length)
        {
            string response = currentQuestionData.characterResponses[conversationStep];
            
            if (conversationText != null)
            {
                conversationText.text = $"Teacher Ana: {response}";
            }
        }
        
        conversationStep++;
        Invoke(nameof(ShowConversationPrompt), 2f);
    }
    
    void ShowConversationTypingPrompt()
    {
        if (conversationButtons != null)
        {
            foreach (var button in conversationButtons)
            {
                if (button != null)
                    button.gameObject.SetActive(false);
            }
        }
        
        if (conversationText != null)
        {
            conversationText.text = "Now, type your answer:";
        }
        
        if (typingInput != null)
        {
            typingInput.text = "";
            typingInput.ActivateInputField();
        }
        
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }
    
    void OnContinue()
    {
        
        // Skip intro transition since we're going straight to questions
        
        // Check if we're using the unified system or legacy system
        if (currentQuestionData != null)
        {
            // Unified system (Medium/Hard)
            string userAnswer = "";
            bool isCorrect = false;
            
            switch (currentQuestionData.questionType)
            {
                case QuestionType.FillInTheBlank:
                    if (fillInBlankInput != null)
                    {
                        userAnswer = fillInBlankInput.text.Trim();
                        isCorrect = CheckFillInBlankAnswer(userAnswer);
                    }
                    break;
                    
                case QuestionType.TypeAnswer:
                case QuestionType.Conversational:
                    if (typingInput != null)
                    {
                        userAnswer = typingInput.text.Trim();
                        isCorrect = CheckTypeAnswer(userAnswer);
                    }
                    break;
            }
            
            ProcessUnifiedAnswer(isCorrect, userAnswer);
        }
        else
        {
            // Legacy system (Easy) - this shouldn't be called in Easy mode
            // but if it is, redirect to legacy NextQuestion
            NextQuestion();
        }
    }
    
    bool CheckFillInBlankAnswer(string userAnswer)
    {
        if (string.Equals(userAnswer, currentQuestionData.blankWord, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        foreach (string acceptable in currentQuestionData.acceptableAnswers)
        {
            if (string.Equals(userAnswer, acceptable, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    bool CheckTypeAnswer(string userAnswer)
    {
        if (string.Equals(userAnswer, currentQuestionData.correctAnswer, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        foreach (string acceptable in currentQuestionData.acceptableAnswers)
        {
            if (string.Equals(userAnswer, acceptable, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    void ProcessUnifiedAnswer(bool isCorrect, string userAnswer)
    {
        sessionTotalAnswers++;
        
        // Play sound effects via GameAudioManager
        if (GameAudioManager.Instance != null)
        {
            if (isCorrect)
            {
                GameAudioManager.Instance.PlayCorrectAnswer();
                Debug.Log("✅ Playing correct answer sound via GameAudioManager");
            }
            else
            {
                GameAudioManager.Instance.PlayWrongAnswer();
                Debug.Log("❌ Playing wrong answer sound via GameAudioManager");
            }
        }
        
        if (isCorrect)
        {
            sessionCorrectAnswers++;
            score += currentQuestionData.xpReward;
            
            ShowUnifiedCorrectFeedback();
        }
        else
        {
            ShowUnifiedIncorrectFeedback(userAnswer);
        }
        
        currentQuestion++;
        Invoke(nameof(ShowNextQuestion), 2f);
    }
    
    void ShowUnifiedCorrectFeedback()
    {
        string correctAnswer = currentQuestionData.questionType == QuestionType.MultipleChoice 
            ? currentQuestionData.choices[currentQuestionData.correctChoiceIndex]
            : currentQuestionData.questionType == QuestionType.FillInTheBlank
            ? currentQuestionData.blankWord
            : currentQuestionData.correctAnswer;
            
        string feedback = $"🎉 Correct! '{correctAnswer}' is the right answer!\n\n" +
                         $"Points: +{currentQuestionData.xpReward}\n" +
                         $"Total Points: {score}";
        
        ShowUnifiedFeedback(feedback);
        Debug.Log($"✅ Correct! Answer: {correctAnswer}");
    }
    
    void ShowUnifiedIncorrectFeedback(string userAnswer)
    {
        string correctAnswer = currentQuestionData.questionType == QuestionType.MultipleChoice 
            ? currentQuestionData.choices[currentQuestionData.correctChoiceIndex]
            : currentQuestionData.questionType == QuestionType.FillInTheBlank
            ? currentQuestionData.blankWord
            : currentQuestionData.correctAnswer;
            
        string feedback = $"❌ '{userAnswer}' is not correct.\n\n" +
                         $"The correct answer is: '{correctAnswer}'\n\n" +
                         $"Try again on the next question!";
        
        ShowUnifiedFeedback(feedback);
        Debug.Log($"❌ Incorrect. User: '{userAnswer}', Correct: '{correctAnswer}'");
    }
    
    void ShowUnifiedFeedback(string feedback)
    {
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowDialog(feedback, null);
        }
        else if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(feedback);
        }
        else if (dialogText != null)
        {
            dialogText.text = feedback;
        }
    }
    
    void EndUnifiedGame()
    {
        float sessionDuration = Time.time - sessionStartTime;
        float accuracy = sessionTotalAnswers > 0 ? (float)sessionCorrectAnswers / sessionTotalAnswers : 0f;
        
        string summary = $"🎉 {currentDifficulty} level completed!\n\n" +
                        $"📊 Accuracy: {(accuracy * 100):F1}%\n" +
                        $"⏱️ Time: {sessionDuration:F1}s\n" +
                        $"🏆 Total Points: {score}\n" +
                        $"✅ Correct: {sessionCorrectAnswers}/{sessionTotalAnswers}\n\n" +
                        GetCompletionMessage();
        
        if (adaptiveDialogManager != null)
        {
            adaptiveDialogManager.ShowSessionSummary(summary, () => {
                Debug.Log($"{currentDifficulty} level session completed");
            });
        }
        else if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(summary, true);
        }
        else if (dialogText != null)
        {
            dialogText.text = summary;
        }
        
        Debug.Log($"🏁 {currentDifficulty} level completed. Accuracy: {accuracy:F2}, Score: {score}");
    }
    
    string GetCompletionMessage()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => "🌟 Excellent! You completed the Grade 1 level!\n\n" +
                                  "📚 You now know:\n" +
                                  "• Identifying nouns (pangngalan)\n" +
                                  "• Types of nouns (Pantangi, Pambalana)\n" +
                                  "• Concrete and Abstract nouns (Tahas at Basal)\n" +
                                  "• Collective and Derived nouns (Lansakan at Hango)\n\n" +
                                  "🎯 Are you ready for Grade 2 level?",
                                  
            DifficultyLevel.Medium => "🌟 Excellent! You completed the Grade 2 level!\n\n" +
                                    "📚 You now know:\n" +
                                    "• Using nouns in sentences\n" +
                                    "• Building sentences with nouns\n" +
                                    "• Identifying noun types in context\n" +
                                    "• Creating sentences\n\n" +
                                    "🎯 Are you ready for Grade 3 level?",
                                    
            DifficultyLevel.Hard => "🌟 Excellent! You completed the Grade 3 level!\n\n" +
                                  "📚 You now know:\n" +
                                  "• Identifying nouns in complex sentences\n" +
                                  "• Recognizing abstract and concrete nouns\n" +
                                  "• Building sentences with different types of nouns\n" +
                                  "• Having conversations about nouns\n\n" +
                                  "🎓 You are now an expert in Filipino nouns!\n" +
                                  "🏆 Ready for more challenging lessons!",
                                  
            _ => "🌟 Congratulations!"
        };
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (GamificationSystem.Instance != null)
        {
            GamificationSystem.Instance.OnLevelUp -= OnLevelUp;
            GamificationSystem.Instance.OnAchievementUnlocked -= OnAchievementUnlocked;
        }
    }
}