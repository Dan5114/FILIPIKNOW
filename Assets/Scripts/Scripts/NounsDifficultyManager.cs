using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NounsDifficultyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button[] choiceButtons;
    public Button backButton;
    public TMP_InputField typingInputField;
    public GameObject fillInBlankPanel;
    public GameObject typingPanel;
    public GameObject conversationalPanel;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;
    public TMP_FontAsset timesBoldFont;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Options Menu")]
    public OptionsMenu optionsMenu;
    
    [Header("Universal Font")]
    public bool useUniversalFont = true;
    
    [Header("Difficulty Settings")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    public int questionsPerLevel = 5;
    public float accuracyThreshold = 0.7f;
    
    [Header("Curriculum Alignment")]
    public bool useDepEdCurriculum = true;
    public GradeLevel currentGradeLevel = GradeLevel.Grade1;
    
    [Header("Question Database")]
    public bool loadQuestionsFromInspector = false;
    public List<EnhancedQuestionData> inspectorEasyQuestions = new List<EnhancedQuestionData>();
    public List<EnhancedQuestionData> inspectorModerateQuestions = new List<EnhancedQuestionData>();
    public List<EnhancedQuestionData> inspectorHardQuestions = new List<EnhancedQuestionData>();
    public List<EnhancedQuestionData> inspectorExpertQuestions = new List<EnhancedQuestionData>();
    
    // Game State
    private int currentQuestionIndex = 0;
    private int score = 0;
    private List<EnhancedQuestionData> currentQuestions = new List<EnhancedQuestionData>();
    private EnhancedQuestionData currentQuestion;
    private bool isTypingMode = false;
    private bool isFillInBlankMode = false;
    private bool isConversationalMode = false;
    
    // Performance Tracking
    private float sessionStartTime;
    private int sessionCorrectAnswers = 0;
    private int sessionTotalAnswers = 0;
    private int currentStreak = 0;
    private int longestStreak = 0;
    
    // Enhanced Questions Database
    private List<EnhancedQuestionData> easyQuestions = new List<EnhancedQuestionData>();
    private List<EnhancedQuestionData> moderateQuestions = new List<EnhancedQuestionData>();
    private List<EnhancedQuestionData> hardQuestions = new List<EnhancedQuestionData>();
    private List<EnhancedQuestionData> expertQuestions = new List<EnhancedQuestionData>();
    
    void Start()
    {
        SetupUniversalFont();
        SetupTypewriter();
        SetupButtons();
        InitializeEnhancedQuestions();
        StartDialog();
    }
    
    void InitializeEnhancedQuestions()
    {
        if (loadQuestionsFromInspector)
        {
            // Load questions from Inspector (for easy editing)
            LoadQuestionsFromInspector();
        }
        else
        {
            // Load default questions programmatically
            LoadDefaultQuestions();
        }
        
        LoadCurrentQuestions();
    }
    
    void LoadQuestionsFromInspector()
    {
        easyQuestions.Clear();
        moderateQuestions.Clear();
        hardQuestions.Clear();
        expertQuestions.Clear();
        
        easyQuestions.AddRange(inspectorEasyQuestions);
        moderateQuestions.AddRange(inspectorModerateQuestions);
        hardQuestions.AddRange(inspectorHardQuestions);
        expertQuestions.AddRange(inspectorExpertQuestions);
        
        Debug.Log($"Loaded questions from Inspector - Easy: {easyQuestions.Count}, Moderate: {moderateQuestions.Count}, Hard: {hardQuestions.Count}, Expert: {expertQuestions.Count}");
    }
    
    void LoadDefaultQuestions()
    {
        // Clear existing questions
        easyQuestions.Clear();
        moderateQuestions.Clear();
        hardQuestions.Clear();
        expertQuestions.Clear();
        
        if (useDepEdCurriculum)
        {
            LoadDepEdCurriculumQuestions();
        }
        else
        {
            LoadGenericQuestions();
        }
        
        Debug.Log($"Loaded questions - Easy: {easyQuestions.Count}, Moderate: {moderateQuestions.Count}, Hard: {hardQuestions.Count}, Expert: {expertQuestions.Count}");
    }
    
    void LoadDepEdCurriculumQuestions()
    {
        // EASY QUESTIONS - Grade 1 Level (Multiple Choice)
        // Based on DepEd MELC: F1WG-Ia-e-1 (Nakikilala ang mga pangngalan sa pangungusap)
        AddDepEdEasyQuestion(1, GradeLevel.Grade1, "F1WG-Ia-e-1", 
            "Nakikilala ang mga pangngalan sa pangungusap",
            "Ano ang pangngalan sa pangungusap: 'Ang aso ay tumatakbo sa parke.'", 
            new string[] { "aso", "tumatakbo", "parke", "Ang" }, "aso", 
            new string[] { "grade1", "animals", "identification", "melc" });
        
        AddDepEdEasyQuestion(2, GradeLevel.Grade1, "F1WG-Ia-e-1", 
            "Nakikilala ang mga pangngalan sa pangungusap",
            "Piliin ang pangngalan:", 
            new string[] { "maganda", "bahay", "mabilis", "malakas" }, "bahay", 
            new string[] { "grade1", "objects", "identification", "melc" });
        
        AddDepEdEasyQuestion(3, GradeLevel.Grade1, "F1WG-Ia-e-1", 
            "Nakikilala ang mga pangngalan sa pangungusap",
            "Ano ang pangngalan sa pangungusap: 'Ang mga bata ay naglalaro sa bakuran.'", 
            new string[] { "bata", "naglalaro", "bakuran", "mga" }, "bata", 
            new string[] { "grade1", "people", "identification", "melc" });
        
        AddDepEdEasyQuestion(4, GradeLevel.Grade1, "F1WG-Ia-e-1", 
            "Nakikilala ang mga pangngalan sa pangungusap",
            "Piliin ang pangngalan sa pangungusap: 'Ang guro ay nagtuturo sa paaralan.'", 
            new string[] { "guro", "nagtuturo", "paaralan", "sa" }, "guro", 
            new string[] { "grade1", "profession", "identification", "melc" });
        
        AddDepEdEasyQuestion(5, GradeLevel.Grade1, "F1WG-Ia-e-1", 
            "Nakikilala ang mga pangngalan sa pangungusap",
            "Ano ang pangngalan sa pangungusap: 'Ang mga bulaklak ay namumulaklak sa hardin.'", 
            new string[] { "bulaklak", "namumulaklak", "hardin", "mga" }, "bulaklak", 
            new string[] { "grade1", "nature", "identification", "melc" });
        
        // MODERATE QUESTIONS - Grade 2 Level (Fill in the Blank)
        // Based on DepEd MELC: F2WG-Ia-e-1 (Nakikilala ang mga uri ng pangngalan)
        AddDepEdModerateQuestion(6, GradeLevel.Grade2, "F2WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan",
            "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay naglalaro sa bakuran.", "bata", 1, 
            new string[] { "bata", "anak", "child" }, 
            new string[] { "grade2", "people", "completion", "melc" });
        
        AddDepEdModerateQuestion(7, GradeLevel.Grade2, "F2WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan",
            "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay namumulaklak sa hardin.", "bulaklak", 1, 
            new string[] { "bulaklak", "flower", "rosas", "lily" }, 
            new string[] { "grade2", "nature", "completion", "melc" });
        
        AddDepEdModerateQuestion(8, GradeLevel.Grade2, "F2WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan",
            "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay nagtuturo sa mga estudyante.", "guro", 1, 
            new string[] { "guro", "teacher", "titser", "maestro" }, 
            new string[] { "grade2", "profession", "completion", "melc" });
        
        AddDepEdModerateQuestion(9, GradeLevel.Grade2, "F2WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan",
            "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay tumatakbo sa parke.", "aso", 1, 
            new string[] { "aso", "dog", "hayop" }, 
            new string[] { "grade2", "animals", "completion", "melc" });
        
        AddDepEdModerateQuestion(10, GradeLevel.Grade2, "F2WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan",
            "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay matatagpuan sa lungsod.", "bahay", 1, 
            new string[] { "bahay", "house", "tahanan", "building" }, 
            new string[] { "grade2", "objects", "completion", "melc" });
        
        // HARD QUESTIONS - Grade 3 Level (Type Answer)
        // Based on DepEd MELC: F3WG-Ia-e-1 (Nakikilala ang mga uri ng pangngalan: pambalana at pantangi)
        AddDepEdHardQuestion(11, GradeLevel.Grade3, "F3WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Ano ang pangngalan sa pangungusap na ito? 'Ang guro ay nagtuturo sa mga estudyante.'", 
            "guro", new string[] { "guro", "teacher", "titser", "maestro" }, 
            new string[] { "grade3", "profession", "typing", "melc" });
        
        AddDepEdHardQuestion(12, GradeLevel.Grade3, "F3WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Ibigay ang pangngalan na tumutukoy sa lugar kung saan nag-aaral ang mga bata:", 
            "paaralan", new string[] { "paaralan", "school", "eskwelahan", "unibersidad" }, 
            new string[] { "grade3", "places", "typing", "melc" });
        
        AddDepEdHardQuestion(13, GradeLevel.Grade3, "F3WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Ano ang pangngalan sa pangungusap: 'Ang mga bulaklak ay namumulaklak sa hardin.'", 
            "bulaklak", new string[] { "bulaklak", "flower", "rosas", "lily" }, 
            new string[] { "grade3", "nature", "typing", "melc" });
        
        AddDepEdHardQuestion(14, GradeLevel.Grade3, "F3WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Ibigay ang pangngalan na tumutukoy sa hayop na tumatakbo sa parke:", 
            "aso", new string[] { "aso", "dog", "hayop" }, 
            new string[] { "grade3", "animals", "typing", "melc" });
        
        AddDepEdHardQuestion(15, GradeLevel.Grade3, "F3WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Ano ang pangngalan sa pangungusap: 'Ang mga bata ay naglalaro sa bakuran.'", 
            "bata", new string[] { "bata", "child", "anak", "kids" }, 
            new string[] { "grade3", "people", "typing", "melc" });
        
        // EXPERT QUESTIONS - Grade 4+ Level (Conversational)
        // Based on DepEd MELC: F4WG-Ia-e-1 (Nakikilala ang mga uri ng pangngalan: kongkreto at di-kongkreto)
        AddDepEdExpertQuestion(16, GradeLevel.Grade4, "F4WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: kongkreto at di-kongkreto",
            "Mag-usap tayo tungkol sa pangngalan. Ano ang pangngalan sa pangungusap na ito at bakit ito mahalaga?", 
            new string[] { "Ano ang pangngalan sa pangungusap?", "Bakit mahalaga ang pangngalan?", "Magbigay ng halimbawa ng pangngalan." }, 
            new string[] { "Tama! Ang pangngalan ay tumutukoy sa tao, bagay, lugar, o ideya.", "Mahusay! Ang pangngalan ay nagbibigay ng pangalan sa mga bagay.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Tingnan ang mga salita na tumutukoy sa mga bagay...", "Ang pangngalan ay nagbibigay ng pangalan...", "Isipin ang mga salita na maaari mong hawakan o makita..." }, 
            "pangngalan", new string[] { "grade4", "conversation", "analysis", "melc" });
        
        AddDepEdExpertQuestion(17, GradeLevel.Grade4, "F4WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: kongkreto at di-kongkreto",
            "Talakayin natin ang mga uri ng pangngalan. Ano ang pagkakaiba ng pangngalang kongkreto at di-kongkreto?", 
            new string[] { "Ano ang pangngalang kongkreto?", "Ano ang pangngalang di-kongkreto?", "Magbigay ng halimbawa ng bawat uri." }, 
            new string[] { "Tama! Ang pangngalang kongkreto ay maaari mong hawakan o makita.", "Mahusay! Ang pangngalang di-kongkreto ay mga ideya o damdamin.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga bagay na maaari mong hawakan...", "Tingnan ang mga ideya o damdamin...", "Halimbawa: libro (kongkreto) vs pagmamahal (di-kongkreto)..." }, 
            "pangngalan", new string[] { "grade4", "conversation", "classification", "melc" });
        
        AddDepEdExpertQuestion(18, GradeLevel.Grade5, "F5WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: payak at maylapi",
            "Pag-usapan natin ang tungkol sa pangngalang payak at maylapi. Paano mo ito makikilala?", 
            new string[] { "Ano ang pangngalang payak?", "Ano ang pangngalang maylapi?", "Paano mo ito makikilala?" }, 
            new string[] { "Tama! Ang pangngalang payak ay binubuo ng isang salita lamang.", "Mahusay! Ang pangngalang maylapi ay binubuo ng salitang-ugat at panlapi.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga salita na walang panlapi...", "Tingnan ang mga salita na may panlapi...", "Halimbawa: bahay (payak) vs kabahayan (maylapi)..." }, 
            "pangngalan", new string[] { "grade5", "conversation", "structure", "melc" });
        
        AddDepEdExpertQuestion(19, GradeLevel.Grade5, "F5WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: payak at maylapi",
            "Talakayin natin ang tungkol sa pangngalang tahas at basal. Ano ang pagkakaiba nila?", 
            new string[] { "Ano ang pangngalang tahas?", "Ano ang pangngalang basal?", "Magbigay ng halimbawa ng bawat uri." }, 
            new string[] { "Tama! Ang pangngalang tahas ay maaari mong bilangin.", "Mahusay! Ang pangngalang basal ay hindi maaaring bilangin.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga bagay na maaari mong bilangin...", "Tingnan ang mga bagay na hindi maaaring bilangin...", "Halimbawa: libro (tahas) vs tubig (basal)..." }, 
            "pangngalan", new string[] { "grade5", "conversation", "classification", "melc" });
        
        AddDepEdExpertQuestion(20, GradeLevel.Grade6, "F6WG-Ia-e-1", 
            "Nakikilala ang mga uri ng pangngalan: pambalana at pantangi",
            "Pag-usapan natin ang tungkol sa pangngalang pambalana at pantangi. Paano mo ito makikilala?", 
            new string[] { "Ano ang pangngalang pambalana?", "Ano ang pangngalang pantangi?", "Paano mo ito makikilala?" }, 
            new string[] { "Tama! Ang pangngalang pambalana ay tumutukoy sa pangkalahatang ngalan.", "Mahusay! Ang pangngalang pantangi ay tumutukoy sa tiyak na ngalan.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga pangkalahatang ngalan...", "Tingnan ang mga tiyak na ngalan...", "Halimbawa: aso (pambalana) vs Bantay (pantangi)..." }, 
            "pangngalan", new string[] { "grade6", "conversation", "classification", "melc" });
    }
    
    void LoadGenericQuestions()
    {
        // EASY QUESTIONS - Multiple Choice (5 questions)
        AddEasyQuestion(1, "Ano ang pangngalan sa pangungusap: 'Ang aso ay tumatakbo sa parke.'", 
            new string[] { "aso", "tumatakbo", "parke", "Ang" }, "aso", 
            new string[] { "basic", "animals", "identification" });
        
        AddEasyQuestion(2, "Piliin ang pangngalan:", 
            new string[] { "maganda", "bahay", "mabilis", "malakas" }, "bahay", 
            new string[] { "basic", "objects", "identification" });
        
        AddEasyQuestion(3, "Ano ang pangngalan sa pangungusap: 'Ang mga bata ay naglalaro sa bakuran.'", 
            new string[] { "bata", "naglalaro", "bakuran", "mga" }, "bata", 
            new string[] { "basic", "people", "identification" });
        
        AddEasyQuestion(4, "Piliin ang pangngalan sa pangungusap: 'Ang guro ay nagtuturo sa paaralan.'", 
            new string[] { "guro", "nagtuturo", "paaralan", "sa" }, "guro", 
            new string[] { "basic", "profession", "identification" });
        
        AddEasyQuestion(5, "Ano ang pangngalan sa pangungusap: 'Ang mga bulaklak ay namumulaklak sa hardin.'", 
            new string[] { "bulaklak", "namumulaklak", "hardin", "mga" }, "bulaklak", 
            new string[] { "basic", "nature", "identification" });
        
        // MODERATE QUESTIONS - Fill in the Blank (5 questions)
        AddModerateQuestion(6, "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay naglalaro sa bakuran.", "bata", 1, 
            new string[] { "bata", "anak", "child" }, 
            new string[] { "intermediate", "people", "completion" });
        
        AddModerateQuestion(7, "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay namumulaklak sa hardin.", "bulaklak", 1, 
            new string[] { "bulaklak", "flower", "rosas", "lily" }, 
            new string[] { "intermediate", "nature", "completion" });
        
        AddModerateQuestion(8, "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay nagtuturo sa mga estudyante.", "guro", 1, 
            new string[] { "guro", "teacher", "titser", "maestro" }, 
            new string[] { "intermediate", "profession", "completion" });
        
        AddModerateQuestion(9, "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay tumatakbo sa parke.", "aso", 1, 
            new string[] { "aso", "dog", "hayop" }, 
            new string[] { "intermediate", "animals", "completion" });
        
        AddModerateQuestion(10, "Kumpletuhin ang pangungusap:", 
            "Ang ___ ay matatagpuan sa lungsod.", "bahay", 1, 
            new string[] { "bahay", "house", "tahanan", "building" }, 
            new string[] { "intermediate", "objects", "completion" });
        
        // HARD QUESTIONS - Type Answer (5 questions)
        AddHardQuestion(11, "Ano ang pangngalan sa pangungusap na ito? 'Ang guro ay nagtuturo sa mga estudyante.'", 
            "guro", new string[] { "guro", "teacher", "titser", "maestro" }, 
            new string[] { "advanced", "profession", "typing" });
        
        AddHardQuestion(12, "Ibigay ang pangngalan na tumutukoy sa lugar kung saan nag-aaral ang mga bata:", 
            "paaralan", new string[] { "paaralan", "school", "eskwelahan", "unibersidad" }, 
            new string[] { "advanced", "places", "typing" });
        
        AddHardQuestion(13, "Ano ang pangngalan sa pangungusap: 'Ang mga bulaklak ay namumulaklak sa hardin.'", 
            "bulaklak", new string[] { "bulaklak", "flower", "rosas", "lily" }, 
            new string[] { "advanced", "nature", "typing" });
        
        AddHardQuestion(14, "Ibigay ang pangngalan na tumutukoy sa hayop na tumatakbo sa parke:", 
            "aso", new string[] { "aso", "dog", "hayop" }, 
            new string[] { "advanced", "animals", "typing" });
        
        AddHardQuestion(15, "Ano ang pangngalan sa pangungusap: 'Ang mga bata ay naglalaro sa bakuran.'", 
            "bata", new string[] { "bata", "child", "anak", "kids" }, 
            new string[] { "advanced", "people", "typing" });
        
        // EXPERT QUESTIONS - Conversational (5 questions)
        AddExpertQuestion(16, "Mag-usap tayo tungkol sa pangngalan. Ano ang pangngalan sa pangungusap na ito at bakit ito mahalaga?", 
            new string[] { "Ano ang pangngalan sa pangungusap?", "Bakit mahalaga ang pangngalan?", "Magbigay ng halimbawa ng pangngalan." }, 
            new string[] { "Tama! Ang pangngalan ay tumutukoy sa tao, bagay, lugar, o ideya.", "Mahusay! Ang pangngalan ay nagbibigay ng pangalan sa mga bagay.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Tingnan ang mga salita na tumutukoy sa mga bagay...", "Ang pangngalan ay nagbibigay ng pangalan...", "Isipin ang mga salita na maaari mong hawakan o makita..." }, 
            "pangngalan", new string[] { "expert", "conversation", "analysis" });
        
        AddExpertQuestion(17, "Talakayin natin ang mga uri ng pangngalan. Ano ang pagkakaiba ng pangngalang pambalana at pantangi?", 
            new string[] { "Ano ang pangngalang pambalana?", "Ano ang pangngalang pantangi?", "Magbigay ng halimbawa ng bawat uri." }, 
            new string[] { "Tama! Ang pangngalang pambalana ay tumutukoy sa pangkalahatang ngalan.", "Excellent! Ang pangngalang pantangi ay tumutukoy sa tiyak na ngalan.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga pangkalahatang ngalan...", "Tingnan ang mga tiyak na ngalan...", "Halimbawa: aso (pambalana) vs Bantay (pantangi)..." }, 
            "pangngalan", new string[] { "expert", "conversation", "classification" });
        
        AddExpertQuestion(18, "Pag-usapan natin ang tungkol sa pangngalang kongkreto at di-kongkreto. Paano mo ito makikilala?", 
            new string[] { "Ano ang pangngalang kongkreto?", "Ano ang pangngalang di-kongkreto?", "Paano mo ito makikilala?" }, 
            new string[] { "Tama! Ang pangngalang kongkreto ay maaari mong hawakan o makita.", "Mahusay! Ang pangngalang di-kongkreto ay mga ideya o damdamin.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga bagay na maaari mong hawakan...", "Tingnan ang mga ideya o damdamin...", "Halimbawa: libro (kongkreto) vs pagmamahal (di-kongkreto)..." }, 
            "pangngalan", new string[] { "expert", "conversation", "classification" });
        
        AddExpertQuestion(19, "Talakayin natin ang tungkol sa pangngalang payak at maylapi. Ano ang pagkakaiba nila?", 
            new string[] { "Ano ang pangngalang payak?", "Ano ang pangngalang maylapi?", "Magbigay ng halimbawa ng bawat uri." }, 
            new string[] { "Tama! Ang pangngalang payak ay binubuo ng isang salita lamang.", "Excellent! Ang pangngalang maylapi ay binubuo ng salitang-ugat at panlapi.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga salita na walang panlapi...", "Tingnan ang mga salita na may panlapi...", "Halimbawa: bahay (payak) vs kabahayan (maylapi)..." }, 
            "pangngalan", new string[] { "expert", "conversation", "structure" });
        
        AddExpertQuestion(20, "Pag-usapan natin ang tungkol sa pangngalang tahas at basal. Paano mo ito makikilala?", 
            new string[] { "Ano ang pangngalang tahas?", "Ano ang pangngalang basal?", "Paano mo ito makikilala?" }, 
            new string[] { "Tama! Ang pangngalang tahas ay maaari mong bilangin.", "Mahusay! Ang pangngalang basal ay hindi maaaring bilangin.", "Napakagaling! Ikaw ay tunay na nakakaunawa na!" }, 
            new string[] { "Isipin ang mga bagay na maaari mong bilangin...", "Tingnan ang mga bagay na hindi maaaring bilangin...", "Halimbawa: libro (tahas) vs tubig (basal)..." }, 
            "pangngalan", new string[] { "expert", "conversation", "classification" });
        
        Debug.Log($"Loaded default questions - Easy: {easyQuestions.Count}, Moderate: {moderateQuestions.Count}, Hard: {hardQuestions.Count}, Expert: {expertQuestions.Count}");
    }
    
    // Helper methods for adding questions (scalable approach)
    void AddEasyQuestion(int id, string questionText, string[] choices, string correctAnswer, string[] tags)
    {
        easyQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.MultipleChoice,
            difficultyLevel = DifficultyLevel.Easy,
            questionText = questionText,
            choices = choices,
            correctAnswer = correctAnswer,
            baseXP = 10,
            difficultyRating = 0.2f,
            learningTags = tags
        });
    }
    
    // DepEd Curriculum-specific helper methods
    void AddDepEdEasyQuestion(int id, GradeLevel gradeLevel, string curriculumCode, string learningObjective, string questionText, string[] choices, string correctAnswer, string[] tags)
    {
        easyQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.MultipleChoice,
            difficultyLevel = DifficultyLevel.Easy,
            gradeLevel = gradeLevel,
            depEdMELC = learningObjective,
            curriculumCode = curriculumCode,
            learningObjective = learningObjective,
            questionText = questionText,
            choices = choices,
            correctAnswer = correctAnswer,
            baseXP = 10,
            difficultyRating = 0.2f,
            learningTags = tags
        });
    }
    
    void AddDepEdModerateQuestion(int id, GradeLevel gradeLevel, string curriculumCode, string learningObjective, string questionText, string sentenceTemplate, string blankWord, int blankPosition, string[] acceptableAnswers, string[] tags)
    {
        moderateQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.FillInTheBlank,
            difficultyLevel = DifficultyLevel.Medium,
            gradeLevel = gradeLevel,
            depEdMELC = learningObjective,
            curriculumCode = curriculumCode,
            learningObjective = learningObjective,
            questionText = questionText,
            sentenceTemplate = sentenceTemplate,
            blankWord = blankWord,
            blankPosition = blankPosition,
            correctAnswer = blankWord,
            acceptableAnswers = acceptableAnswers,
            baseXP = 20,
            difficultyRating = 0.5f,
            learningTags = tags
        });
    }
    
    void AddDepEdHardQuestion(int id, GradeLevel gradeLevel, string curriculumCode, string learningObjective, string questionText, string correctAnswer, string[] acceptableAnswers, string[] tags)
    {
        hardQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.TypeAnswer,
            difficultyLevel = DifficultyLevel.Hard,
            gradeLevel = gradeLevel,
            depEdMELC = learningObjective,
            curriculumCode = curriculumCode,
            learningObjective = learningObjective,
            questionText = questionText,
            correctAnswer = correctAnswer,
            acceptableAnswers = acceptableAnswers,
            baseXP = 30,
            difficultyRating = 0.8f,
            learningTags = tags
        });
    }
    
    void AddDepEdExpertQuestion(int id, GradeLevel gradeLevel, string curriculumCode, string learningObjective, string questionText, string[] conversationPrompts, string[] characterResponses, string[] hints, string correctAnswer, string[] tags)
    {
        expertQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.Conversational,
            difficultyLevel = DifficultyLevel.Hard,
            gradeLevel = gradeLevel,
            depEdMELC = learningObjective,
            curriculumCode = curriculumCode,
            learningObjective = learningObjective,
            questionText = questionText,
            conversationPrompts = conversationPrompts,
            characterResponses = characterResponses,
            hints = hints,
            correctAnswer = correctAnswer,
            baseXP = 50,
            difficultyRating = 1.0f,
            learningTags = tags
        });
    }
    
    void AddModerateQuestion(int id, string questionText, string sentenceTemplate, string blankWord, int blankPosition, string[] acceptableAnswers, string[] tags)
    {
        moderateQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.FillInTheBlank,
            difficultyLevel = DifficultyLevel.Medium,
            questionText = questionText,
            sentenceTemplate = sentenceTemplate,
            blankWord = blankWord,
            blankPosition = blankPosition,
            correctAnswer = blankWord,
            acceptableAnswers = acceptableAnswers,
            baseXP = 20,
            difficultyRating = 0.5f,
            learningTags = tags
        });
    }
    
    void AddHardQuestion(int id, string questionText, string correctAnswer, string[] acceptableAnswers, string[] tags)
    {
        hardQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.TypeAnswer,
            difficultyLevel = DifficultyLevel.Hard,
            questionText = questionText,
            correctAnswer = correctAnswer,
            acceptableAnswers = acceptableAnswers,
            baseXP = 30,
            difficultyRating = 0.8f,
            learningTags = tags
        });
    }
    
    void AddExpertQuestion(int id, string questionText, string[] conversationPrompts, string[] characterResponses, string[] hints, string correctAnswer, string[] tags)
    {
        expertQuestions.Add(new EnhancedQuestionData
        {
            questionId = id,
            module = "Nouns",
            questionType = QuestionType.Conversational,
            difficultyLevel = DifficultyLevel.Hard,
            questionText = questionText,
            conversationPrompts = conversationPrompts,
            characterResponses = characterResponses,
            hints = hints,
            correctAnswer = correctAnswer,
            baseXP = 50,
            difficultyRating = 1.0f,
            learningTags = tags
        });
    }
    
    void LoadCurrentQuestions()
    {
        currentQuestions.Clear();
        
        switch (currentDifficulty)
        {
            case DifficultyLevel.Easy:
                currentQuestions.AddRange(easyQuestions.Take(questionsPerLevel));
                break;
            case DifficultyLevel.Medium:
                currentQuestions.AddRange(moderateQuestions.Take(questionsPerLevel));
                break;
            case DifficultyLevel.Hard:
                currentQuestions.AddRange(hardQuestions.Take(questionsPerLevel));
                break;
        }
        
        Debug.Log($"Loaded {currentQuestions.Count} {currentDifficulty} questions");
    }
    
    // Public methods for adding questions dynamically (for future expansion)
    public void AddNewEasyQuestion(string questionText, string[] choices, string correctAnswer, string[] tags)
    {
        int newId = easyQuestions.Count + 1;
        AddEasyQuestion(newId, questionText, choices, correctAnswer, tags);
        Debug.Log($"Added new Easy question: {questionText}");
    }
    
    public void AddNewModerateQuestion(string questionText, string sentenceTemplate, string blankWord, int blankPosition, string[] acceptableAnswers, string[] tags)
    {
        int newId = moderateQuestions.Count + 6; // Start from 6 since easy questions go 1-5
        AddModerateQuestion(newId, questionText, sentenceTemplate, blankWord, blankPosition, acceptableAnswers, tags);
        Debug.Log($"Added new Moderate question: {questionText}");
    }
    
    public void AddNewHardQuestion(string questionText, string correctAnswer, string[] acceptableAnswers, string[] tags)
    {
        int newId = hardQuestions.Count + 11; // Start from 11 since moderate questions go 6-10
        AddHardQuestion(newId, questionText, correctAnswer, acceptableAnswers, tags);
        Debug.Log($"Added new Hard question: {questionText}");
    }
    
    public void AddNewExpertQuestion(string questionText, string[] conversationPrompts, string[] characterResponses, string[] hints, string correctAnswer, string[] tags)
    {
        int newId = expertQuestions.Count + 16; // Start from 16 since hard questions go 11-15
        AddExpertQuestion(newId, questionText, conversationPrompts, characterResponses, hints, correctAnswer, tags);
        Debug.Log($"Added new Expert question: {questionText}");
    }
    
    // Method to get question statistics
    public string GetQuestionStatistics()
    {
        string stats = "📊 QUESTION DATABASE STATISTICS\n\n";
        
        if (useDepEdCurriculum)
        {
            stats += "🏛️ DEPED CURRICULUM ALIGNED\n";
            stats += $"📚 Current Grade Level: {currentGradeLevel}\n";
            stats += $"🎯 Curriculum Mode: DepEd MELCs\n\n";
        }
        else
        {
            stats += "🎮 GENERIC DIFFICULTY MODE\n";
            stats += $"🎯 Curriculum Mode: Generic Difficulty\n\n";
        }
        
        stats += $"🟢 Easy Questions: {easyQuestions.Count}\n";
        stats += $"🟡 Moderate Questions: {moderateQuestions.Count}\n";
        stats += $"🔴 Hard Questions: {hardQuestions.Count}\n";
        stats += $"🟣 Expert Questions: {expertQuestions.Count}\n";
        stats += $"📝 Total Questions: {easyQuestions.Count + moderateQuestions.Count + hardQuestions.Count + expertQuestions.Count}\n\n";
        
        stats += $"🎯 Questions per Level: {questionsPerLevel}\n";
        stats += $"📈 Accuracy Threshold: {accuracyThreshold:P1}\n";
        stats += $"🎮 Current Difficulty: {currentDifficulty}\n";
        
        if (useDepEdCurriculum)
        {
            stats += "\n📋 CURRICULUM COVERAGE:\n";
            stats += $"• Grade 1: {easyQuestions.Count(q => q.gradeLevel == GradeLevel.Grade1)} questions\n";
            stats += $"• Grade 2: {moderateQuestions.Count(q => q.gradeLevel == GradeLevel.Grade2)} questions\n";
            stats += $"• Grade 3: {hardQuestions.Count(q => q.gradeLevel == GradeLevel.Grade3)} questions\n";
            stats += $"• Grade 4: {expertQuestions.Count(q => q.gradeLevel == GradeLevel.Grade4)} questions\n";
            stats += $"• Grade 5: {expertQuestions.Count(q => q.gradeLevel == GradeLevel.Grade5)} questions\n";
            stats += $"• Grade 6: {expertQuestions.Count(q => q.gradeLevel == GradeLevel.Grade6)} questions\n";
        }
        
        return stats;
    }
    
    // Method to reload questions (useful for runtime updates)
    public void ReloadQuestions()
    {
        InitializeEnhancedQuestions();
        Debug.Log("Questions reloaded successfully!");
    }
    
    void StartDialog()
    {
        string welcomeMessage = GetWelcomeMessage();
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(welcomeMessage);
        }
        else if (dialogText != null)
        {
            dialogText.text = welcomeMessage;
        }
    }
    
    string GetWelcomeMessage()
    {
        switch (currentDifficulty)
        {
            case DifficultyLevel.Easy:
                return "Kumusta! Ako si Teacher Ana. Ngayon ay magsisimula tayo sa EASY level. Piliin lang ang tamang sagot!";
            case DifficultyLevel.Medium:
                return "Magaling! Nakarating ka na sa MODERATE level. Ngayon ay kailangan mong kumpletuhin ang mga pangungusap!";
            case DifficultyLevel.Hard:
                return "Wow! Nasa HARD level ka na! Ngayon ay kailangan mong mag-type ng sagot mo mismo!";
            default:
                return "Kumusta! Ako si Teacher Ana. Mag-aral tayo ng pangngalan!";
        }
    }
    
    public void StartGame()
    {
        currentQuestionIndex = 0;
        sessionCorrectAnswers = 0;
        sessionTotalAnswers = 0;
        ShowQuestion();
    }
    
    void ShowQuestion()
    {
        if (currentQuestionIndex >= currentQuestions.Count)
        {
            CheckLevelProgression();
            return;
        }
        
        currentQuestion = currentQuestions[currentQuestionIndex];
        sessionStartTime = Time.time;
        
        // Hide all panels first
        HideAllPanels();
        
        // Show appropriate UI based on question type
        switch (currentQuestion.questionType)
        {
            case QuestionType.MultipleChoice:
                ShowMultipleChoiceQuestion();
                break;
            case QuestionType.FillInTheBlank:
                ShowFillInBlankQuestion();
                break;
            case QuestionType.TypeAnswer:
                ShowTypeAnswerQuestion();
                break;
            case QuestionType.Conversational:
                ShowConversationalQuestion();
                break;
        }
    }
    
    void ShowMultipleChoiceQuestion()
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(currentQuestion.questionText);
        }
        else if (dialogText != null)
        {
            dialogText.text = currentQuestion.questionText;
        }
        
        // Show choice buttons
        for (int i = 0; i < choiceButtons.Length && i < currentQuestion.choices.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.choices[i];
            
            // Remove existing listeners and add new one
            choiceButtons[i].onClick.RemoveAllListeners();
            int choiceIndex = i;
            choiceButtons[i].onClick.AddListener(() => OnMultipleChoiceAnswer(choiceIndex));
        }
    }
    
    void ShowFillInBlankQuestion()
    {
        isFillInBlankMode = true;
        if (fillInBlankPanel != null)
            fillInBlankPanel.SetActive(true);
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(currentQuestion.questionText);
        }
        else if (dialogText != null)
        {
            dialogText.text = currentQuestion.questionText;
        }
        
        // Show sentence template with blank
        string[] words = currentQuestion.sentenceTemplate.Split(' ');
        string displayText = "";
        for (int i = 0; i < words.Length; i++)
        {
            if (i == currentQuestion.blankPosition)
            {
                displayText += "____ ";
            }
            else
            {
                displayText += words[i] + " ";
            }
        }
        
        // Update fill-in-blank display
        if (fillInBlankPanel != null)
        {
            TextMeshProUGUI fillText = fillInBlankPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (fillText != null)
            {
                fillText.text = displayText.Trim();
            }
            
            // Setup input field
            TMP_InputField inputField = fillInBlankPanel.GetComponentInChildren<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = "";
                inputField.onEndEdit.RemoveAllListeners();
                inputField.onEndEdit.AddListener(OnFillInBlankAnswer);
            }
        }
    }
    
    void ShowTypeAnswerQuestion()
    {
        isTypingMode = true;
        if (typingPanel != null)
            typingPanel.SetActive(true);
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(currentQuestion.questionText);
        }
        else if (dialogText != null)
        {
            dialogText.text = currentQuestion.questionText;
        }
        
        // Setup typing input
        if (typingInputField != null)
        {
            typingInputField.text = "";
            typingInputField.onEndEdit.RemoveAllListeners();
            typingInputField.onEndEdit.AddListener(OnTypeAnswer);
        }
    }
    
    void ShowConversationalQuestion()
    {
        isConversationalMode = true;
        if (conversationalPanel != null)
            conversationalPanel.SetActive(true);
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(currentQuestion.questionText);
        }
        else if (dialogText != null)
        {
            dialogText.text = currentQuestion.questionText;
        }
        
        // Show conversation prompts as buttons
        if (conversationalPanel != null)
        {
            Transform buttonContainer = conversationalPanel.transform.Find("ButtonContainer");
            if (buttonContainer != null)
            {
                Button[] convButtons = buttonContainer.GetComponentsInChildren<Button>();
                for (int i = 0; i < convButtons.Length && i < currentQuestion.conversationPrompts.Length; i++)
                {
                    convButtons[i].gameObject.SetActive(true);
                    convButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.conversationPrompts[i];
                    
                    convButtons[i].onClick.RemoveAllListeners();
                    int promptIndex = i;
                    convButtons[i].onClick.AddListener(() => OnConversationalResponse(promptIndex));
                }
            }
        }
    }
    
    void HideAllPanels()
    {
        if (fillInBlankPanel != null)
            fillInBlankPanel.SetActive(false);
        if (typingPanel != null)
            typingPanel.SetActive(false);
        if (conversationalPanel != null)
            conversationalPanel.SetActive(false);
        
        foreach (Button button in choiceButtons)
        {
            if (button != null)
                button.gameObject.SetActive(false);
        }
    }
    
    // Answer handling methods
    void OnMultipleChoiceAnswer(int choiceIndex)
    {
        string selectedAnswer = currentQuestion.choices[choiceIndex];
        ProcessAnswer(selectedAnswer);
    }
    
    void OnFillInBlankAnswer(string answer)
    {
        ProcessAnswer(answer.Trim());
    }
    
    void OnTypeAnswer(string answer)
    {
        ProcessAnswer(answer.Trim());
    }
    
    void OnConversationalResponse(int promptIndex)
    {
        // For conversational questions, we'll provide hints and feedback
        string response = currentQuestion.characterResponses[promptIndex];
        string hint = currentQuestion.hints[promptIndex];
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(response + "\n\n" + hint);
        }
        
        // Mark as correct for conversational (they're learning through dialogue)
        ProcessAnswer("conversational");
    }
    
    void ProcessAnswer(string answer)
    {
        bool isCorrect = CheckAnswer(answer);
        float responseTime = Time.time - sessionStartTime;
        
        sessionTotalAnswers++;
        if (isCorrect) 
        {
            sessionCorrectAnswers++;
            currentStreak++;
            longestStreak = Mathf.Max(longestStreak, currentStreak);

            // Play correct sound 🎵
            if (GameAudioManager.Instance != null)
            {
                GameAudioManager.Instance.PlayCorrectAnswer();
                Debug.Log("✅ Playing correct answer sound via GameAudioManager");
            }
            else
            {
                Debug.LogWarning("⚠️ GameAudioManager.Instance is null! Create GameAudioManager in Main Menu scene.");
            }
        }
        else
        {
            currentStreak = 0;

            // Play wrong sound 🔊
            if (GameAudioManager.Instance != null)
            {
                GameAudioManager.Instance.PlayWrongAnswer();
                Debug.Log("❌ Playing wrong answer sound via GameAudioManager");
            }
            else
            {
                Debug.LogWarning("⚠️ GameAudioManager.Instance is null! Create GameAudioManager in Main Menu scene.");
            }
        }
        
        // Award XP and update score
        int xpGained = CalculateXPGained(isCorrect, responseTime);
        score += xpGained;
        
        // Show feedback
        ShowFeedback(isCorrect, answer, xpGained);
        
        // Play sound effects (fallback to old system if GameAudioManager not available)
        if (optionsMenu != null)
        {
            if (isCorrect)
                optionsMenu.PlayCorrectAnswerSound();
            else
                optionsMenu.PlayIncorrectAnswerSound();
        }
        
        // Trigger haptic feedback
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        // Move to next question after delay
        Invoke(nameof(NextQuestion), 2f);
    }
    
    bool CheckAnswer(string answer)
    {
        switch (currentQuestion.questionType)
        {
            case QuestionType.MultipleChoice:
                return string.Equals(answer, currentQuestion.correctAnswer, StringComparison.OrdinalIgnoreCase);
            
            case QuestionType.FillInTheBlank:
            case QuestionType.TypeAnswer:
                return currentQuestion.acceptableAnswers.Any(a => 
                    string.Equals(answer, a, StringComparison.OrdinalIgnoreCase));
            
            case QuestionType.Conversational:
                return true; // Always correct for conversational learning
            
            default:
                return false;
        }
    }
    
    int CalculateXPGained(bool isCorrect, float responseTime)
    {
        int baseXP = currentQuestion.baseXP;
        int difficultyMultiplier = (int)currentQuestion.difficultyLevel + 1;
        int speedBonus = responseTime < 3f ? 5 : 0;
        int streakBonus = currentStreak * 2;
        
        if (isCorrect)
        {
            return (baseXP * difficultyMultiplier) + speedBonus + streakBonus;
        }
        else
        {
            return Mathf.Max(1, baseXP / 4); // Small XP for attempts
        }
    }
    
    void ShowFeedback(bool isCorrect, string answer, int xpGained)
    {
        string feedback = GenerateFeedback(isCorrect, answer, xpGained);
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(feedback);
        }
        else if (dialogText != null)
        {
            dialogText.text = feedback;
        }
    }
    
    string GenerateFeedback(bool isCorrect, string answer, int xpGained)
    {
        string feedback = "";
        
        if (isCorrect)
        {
            feedback = GetCorrectFeedback();
            feedback += $"\n\n+{xpGained} XP";
            feedback += $"\nScore: {score}";
            
            // Add streak info
            if (currentStreak > 1)
            {
                feedback += $"\nStreak: {currentStreak} 🔥";
            }
        }
        else
        {
            feedback = GetIncorrectFeedback();
            feedback += $"\nTamang sagot: {currentQuestion.correctAnswer}";
            feedback += $"\n+{xpGained} XP (para sa pagsubok)";
        }
        
        return feedback;
    }
    
    string GetCorrectFeedback()
    {
        string[] correctMessages = {
            "Tama! Napakagaling! 🌟",
            "Excellent! Perfect! 🎉",
            "Wow! Ikaw ay napakatalino! 🧠",
            "Fantastic! Keep it up! 💪",
            "Amazing! You're learning fast! ⚡"
        };
        
        return correctMessages[UnityEngine.Random.Range(0, correctMessages.Length)];
    }
    
    string GetIncorrectFeedback()
    {
        string[] incorrectMessages = {
            "Mali, pero okay lang! Subukan mo ulit! 💪",
            "Hindi pa tama, pero malapit na! 🤔",
            "Mali, pero natututo ka! Try again! 📚",
            "Not quite right, but you're getting there! 🌱",
            "Mali, pero importante na nag-try ka! 🎯"
        };
        
        return incorrectMessages[UnityEngine.Random.Range(0, incorrectMessages.Length)];
    }
    
    void NextQuestion()
    {
        currentQuestionIndex++;
        HideAllPanels();
        
        if (currentQuestionIndex < currentQuestions.Count)
        {
            ShowQuestion();
        }
        else
        {
            CheckLevelProgression();
        }
    }
    
    void CheckLevelProgression()
    {
        float accuracy = (float)sessionCorrectAnswers / sessionTotalAnswers;
        
        // Check if ready to advance
        if (accuracy >= accuracyThreshold && currentStreak >= 2)
        {
            AdvanceToNextLevel();
        }
        else
        {
            ShowSessionSummary();
                // Play victory music! 🎉 gin add ni Ryan ni
                if (GameAudioManager.Instance != null)
                {
                    GameAudioManager.Instance.PlayVictoryMusic();
                }

                // ... rest of existing code
        }
    }
    
    void AdvanceToNextLevel()
    {
        string levelUpMessage = GetLevelUpMessage();
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(levelUpMessage);
        }
        else if (dialogText != null)
        {
            dialogText.text = levelUpMessage;
        }
        
        // Advance to next level
        int currentLevelIndex = (int)currentDifficulty;
        if (currentLevelIndex < 3)
        {
            currentDifficulty = (DifficultyLevel)(currentLevelIndex + 1);
            LoadCurrentQuestions();
        }
        
        // Reset for next level
        currentQuestionIndex = 0;
        sessionCorrectAnswers = 0;
        sessionTotalAnswers = 0;
        
        Invoke(nameof(ShowQuestion), 3f);
    }
    
    string GetLevelUpMessage()
    {
        switch (currentDifficulty)
        {
            case DifficultyLevel.Medium:
                return "🎉 LEVEL UP! 🎉\n\nNakarating ka na sa MODERATE level!\n\nNgayon ay magkakaroon ng fill-in-the-blank questions!\n\nHanda ka na ba?";
            case DifficultyLevel.Hard:
                return "🚀 LEVEL UP! 🚀\n\nNasa HARD level ka na!\n\nNgayon ay kailangan mong mag-type ng sagot mo mismo!\n\nChallenge accepted?";
            default:
                return "🎊 CONGRATULATIONS! 🎊\n\nNakumpleto mo na ang lahat ng levels!\n\nIkaw ay tunay na expert na sa pangngalan!";
        }
    }
    
    void ShowSessionSummary()
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
        
        float accuracy = (float)sessionCorrectAnswers / sessionTotalAnswers;
        string summary = $"📊 Session Complete!\n\n";
        summary += $"Accuracy: {accuracy:P1}\n";
        summary += $"Score: {score}\n";
        summary += $"Current Level: {currentDifficulty}\n";
        summary += $"Streak: {currentStreak}\n\n";
        
        if (accuracy >= accuracyThreshold)
        {
            summary += "Great job! Keep practicing to advance to the next level! 💪";
        }
        else
        {
            summary += "Good effort! Practice more to improve your accuracy! 📚";
        }
        
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(summary);
        }
        else if (dialogText != null)
        {
            dialogText.text = summary;
        }
    }
    
    // Setup methods (same as before)
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
                Debug.Log($"NounsDifficultyManager: Applied FilipknowMainFont to dialog text");
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
                        Debug.Log($"NounsDifficultyManager: Applied FilipknowMainFont to choice button");
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
            
            // Set the font first
            if (timesBoldFont != null)
            {
                typewriterEffect.SetFont(timesBoldFont);
                Debug.Log($"NounsDifficultyManager: Set font {timesBoldFont.name} to typewriter");
            }
            else if (useUniversalFont && FilipknowFontManager.Instance != null)
            {
                typewriterEffect.RefreshUniversalFont();
                Debug.Log("NounsDifficultyManager: Applied universal font to typewriter");
            }
            
            // Force horizontal text orientation (this will re-apply the font)
            typewriterEffect.ForceHorizontalText();
            
            // Verify the font was applied correctly
            typewriterEffect.VerifyCurrentFont();
        }
    }
    
    void SetupButtons()
    {
        // Hide choice buttons initially
        foreach (Button button in choiceButtons)
        {
            if (button != null)
                button.gameObject.SetActive(false);
        }
        
        // Set up button listeners
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }
    
    void OnContinue()
    {
        if (isTypingMode || isFillInBlankMode || isConversationalMode)
        {
            // Handle continue for special modes
            NextQuestion();
        }
        else
        {
            StartGame();
        }
    }
    
    void GoBack()
    {
        SceneManager.LoadScene("ModuleSelection");
    }
}
