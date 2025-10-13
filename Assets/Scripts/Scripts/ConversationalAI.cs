using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConversationalAI : MonoBehaviour
{
    [Header("Conversation Settings")]
    public float responseDelay = 1.5f;
    public int maxConversationTurns = 5;
    
    [Header("AI Personality")]
    public string teacherName = "Teacher Ana";
    public PersonalityType personalityType = PersonalityType.Encouraging;
    
    private Dictionary<string, List<string>> conversationMemory = new Dictionary<string, List<string>>();
    private int currentConversationTurn = 0;
    private string currentTopic = "";
    
    public enum PersonalityType
    {
        Encouraging,    // Positive and supportive
        Challenging,   // Pushes student to think deeper
        Playful,       // Fun and engaging
        Academic       // Formal and educational
    }
    
    [System.Serializable]
    public class ConversationContext
    {
        public string topic;
        public string difficultyLevel;
        public List<string> previousAnswers;
        public float studentConfidence;
        public int correctAnswers;
        public int totalAnswers;
    }
    
    public string GenerateResponse(string studentInput, ConversationContext context)
    {
        currentConversationTurn++;
        currentTopic = context.topic;
        
        // Store conversation in memory
        if (!conversationMemory.ContainsKey(context.topic))
        {
            conversationMemory[context.topic] = new List<string>();
        }
        conversationMemory[context.topic].Add(studentInput);
        
        // Analyze student input
        ResponseAnalysis analysis = AnalyzeStudentInput(studentInput, context);
        
        // Generate appropriate response based on personality and analysis
        string response = GeneratePersonalityResponse(analysis, context);
        
        return response;
    }
    
    ResponseAnalysis AnalyzeStudentInput(string input, ConversationContext context)
    {
        ResponseAnalysis analysis = new ResponseAnalysis();
        
        // Check for correct answers
        analysis.isCorrect = CheckIfCorrect(input, context.topic);
        
        // Analyze confidence level
        analysis.confidenceLevel = AnalyzeConfidence(input);
        
        // Check for questions
        analysis.containsQuestion = input.Contains("?") || input.ToLower().Contains("ano") || input.ToLower().Contains("bakit");
        
        // Check for confusion indicators
        analysis.showsConfusion = input.ToLower().Contains("hindi ko alam") || 
                                 input.ToLower().Contains("confused") ||
                                 input.ToLower().Contains("di ko maintindihan");
        
        // Check for enthusiasm
        analysis.showsEnthusiasm = input.Contains("!") || 
                                  input.ToLower().Contains("excited") ||
                                  input.ToLower().Contains("gusto ko");
        
        return analysis;
    }
    
    bool CheckIfCorrect(string input, string topic)
    {
        // Simple keyword matching - can be enhanced with NLP
        string lowerInput = input.ToLower();
        
        switch (topic.ToLower())
        {
            case "pangngalan":
                return lowerInput.Contains("pangngalan") || 
                       lowerInput.Contains("noun") ||
                       lowerInput.Contains("tao") ||
                       lowerInput.Contains("bagay") ||
                       lowerInput.Contains("lugar");
            
            case "bilang":
                return lowerInput.Contains("numero") || 
                       lowerInput.Contains("number") ||
                       lowerInput.Contains("bilang");
            
            default:
                return true; // Default to encouraging
        }
    }
    
    float AnalyzeConfidence(string input)
    {
        string lowerInput = input.ToLower();
        
        // High confidence indicators
        if (lowerInput.Contains("sigurado") || lowerInput.Contains("alam ko") || lowerInput.Contains("tama"))
            return 0.8f;
        
        // Medium confidence indicators
        if (lowerInput.Contains("siguro") || lowerInput.Contains("maybe") || lowerInput.Contains("baka"))
            return 0.5f;
        
        // Low confidence indicators
        if (lowerInput.Contains("hindi ko alam") || lowerInput.Contains("di ko sure") || lowerInput.Contains("confused"))
            return 0.2f;
        
        return 0.6f; // Default medium confidence
    }
    
    string GeneratePersonalityResponse(ResponseAnalysis analysis, ConversationContext context)
    {
        List<string> responses = new List<string>();
        
        switch (personalityType)
        {
            case PersonalityType.Encouraging:
                responses = GenerateEncouragingResponse(analysis, context);
                break;
            case PersonalityType.Challenging:
                responses = GenerateChallengingResponse(analysis, context);
                break;
            case PersonalityType.Playful:
                responses = GeneratePlayfulResponse(analysis, context);
                break;
            case PersonalityType.Academic:
                responses = GenerateAcademicResponse(analysis, context);
                break;
        }
        
        // Select random response from appropriate category
        if (responses.Count > 0)
        {
            return responses[UnityEngine.Random.Range(0, responses.Count)];
        }
        
        return "Magaling! Patuloy lang sa pag-aaral!";
    }
    
    List<string> GenerateEncouragingResponse(ResponseAnalysis analysis, ConversationContext context)
    {
        List<string> responses = new List<string>();
        
        if (analysis.isCorrect)
        {
            responses.AddRange(new string[]
            {
                "Tama! Napakagaling mo! 🌟",
                "Excellent! Perfect answer! 🎉",
                "Wow! Ikaw ay napakatalino! 🧠",
                "Fantastic! Keep it up! 💪",
                "Amazing! You're learning so fast! ⚡",
                "Magaling! Nakaka-impress ka! 👏",
                "Perfect! You really understand! 🎯",
                "Outstanding! Continue learning! 🚀"
            });
        }
        else
        {
            responses.AddRange(new string[]
            {
                "Mali, pero okay lang! Subukan mo ulit! 💪",
                "Hindi pa tama, pero malapit na! 🤔",
                "Mali, pero natututo ka! Try again! 📚",
                "Not quite right, but you're getting there! 🌱",
                "Mali, pero importante na nag-try ka! 🎯",
                "Close! Think about it more carefully! 🤓",
                "Almost there! You can do it! 💫",
                "Good attempt! Let's try again! 🌟"
            });
        }
        
        if (analysis.showsConfusion)
        {
            responses.AddRange(new string[]
            {
                "Okay lang na mag-confused! Part yan ng pag-aaral! 😊",
                "Don't worry! Let me help you understand! 🤗",
                "Confusion is normal! Let's figure it out together! 💡",
                "It's okay to be confused! That's how we learn! 🌱"
            });
        }
        
        if (analysis.containsQuestion)
        {
            responses.AddRange(new string[]
            {
                "Great question! Let me explain... 🤔",
                "Excellent question! That shows you're thinking! 💭",
                "Good question! Let's explore that together! 🔍",
                "I love your questions! Keep asking! ❓"
            });
        }
        
        return responses;
    }
    
    List<string> GenerateChallengingResponse(ResponseAnalysis analysis, ConversationContext context)
    {
        List<string> responses = new List<string>();
        
        if (analysis.isCorrect)
        {
            responses.AddRange(new string[]
            {
                "Good! But can you explain why? 🤔",
                "Correct! Now, what else can you tell me? 💭",
                "Right! But let's go deeper... 🔍",
                "Yes! Can you give me another example? 📝",
                "Correct! Now challenge yourself more! 💪",
                "Good! But I know you can do better! 🚀",
                "Right! Let's push your limits! ⚡",
                "Yes! Now let's make it harder! 🎯"
            });
        }
        else
        {
            responses.AddRange(new string[]
            {
                "Think harder! You can figure this out! 🧠",
                "Not quite! Try thinking differently! 💡",
                "Close! Push your thinking further! 🔥",
                "Almost! Challenge yourself more! 💪",
                "Think about it from another angle! 🔄",
                "You're getting there! Think deeper! 🌊",
                "Not yet! Use what you know! 🎯",
                "Try again! I believe in you! 💫"
            });
        }
        
        return responses;
    }
    
    List<string> GeneratePlayfulResponse(ResponseAnalysis analysis, ConversationContext context)
    {
        List<string> responses = new List<string>();
        
        if (analysis.isCorrect)
        {
            responses.AddRange(new string[]
            {
                "Yay! You got it! 🎉🎊",
                "Woohoo! Amazing! 🚀✨",
                "Fantastic! You're on fire! 🔥",
                "Awesome! You're rocking it! 🎸",
                "Brilliant! You're a star! ⭐",
                "Super! You're incredible! 🦸‍♀️",
                "Wonderful! You're amazing! 🌈",
                "Excellent! You're fantastic! 🎪"
            });
        }
        else
        {
            responses.AddRange(new string[]
            {
                "Oops! Try again! 😄",
                "Not quite! But that's okay! 😊",
                "Almost! You're getting there! 🌟",
                "Close! One more try! 🎯",
                "Nice try! Let's do it again! 🎮",
                "Good attempt! Try once more! 🎲",
                "Almost there! You can do it! 🎈",
                "Not yet! But keep trying! 🎨"
            });
        }
        
        return responses;
    }
    
    List<string> GenerateAcademicResponse(ResponseAnalysis analysis, ConversationContext context)
    {
        List<string> responses = new List<string>();
        
        if (analysis.isCorrect)
        {
            responses.AddRange(new string[]
            {
                "Correct. Your understanding is accurate. 📚",
                "Accurate response. Well done. ✅",
                "Precise answer. Good work. 🎓",
                "Correct. You demonstrate good comprehension. 📖",
                "Accurate. Your knowledge is solid. 📝",
                "Correct. You show good understanding. 📊",
                "Precise. Your learning is progressing well. 📈",
                "Accurate. You're mastering the material. 🎯"
            });
        }
        else
        {
            responses.AddRange(new string[]
            {
                "Incorrect. Please review the material. 📚",
                "Not accurate. Try again. 🔄",
                "Incorrect. Consider the concepts more carefully. 🤔",
                "Not quite right. Review and try again. 📖",
                "Incorrect. Think about the fundamentals. 📝",
                "Not accurate. Study the material again. 📊",
                "Incorrect. Focus on the key concepts. 📈",
                "Not quite. Review your understanding. 🎯"
            });
        }
        
        return responses;
    }
    
    public string GenerateHint(string topic, int hintLevel)
    {
        Dictionary<string, List<string>> hints = new Dictionary<string, List<string>>
        {
            ["pangngalan"] = new List<string>
            {
                "Ang pangngalan ay tumutukoy sa tao, bagay, lugar, o ideya.",
                "Tingnan ang mga salita na maaari mong hawakan o makita.",
                "Ang pangngalan ay nagbibigay ng pangalan sa mga bagay.",
                "Isipin ang mga salita na tumutukoy sa mga bagay sa paligid mo.",
                "Ang pangngalan ay mga salita na maaari mong gamitin sa pangungusap."
            },
            ["bilang"] = new List<string>
            {
                "Ang bilang ay tumutukoy sa dami o halaga.",
                "Tingnan ang mga simbolo na ginagamit sa pagbilang.",
                "Ang bilang ay nagpapakita ng dami ng mga bagay.",
                "Isipin ang mga salita na ginagamit sa pagbilang.",
                "Ang bilang ay mga simbolo na nagpapakita ng halaga."
            }
        };
        
        if (hints.ContainsKey(topic.ToLower()) && hintLevel < hints[topic.ToLower()].Count)
        {
            return hints[topic.ToLower()][hintLevel];
        }
        
        return "Subukan mo ulit! Kaya mo yan!";
    }
    
    public string GenerateEncouragement()
    {
        string[] encouragements = {
            "Kaya mo yan! 💪",
            "Don't give up! 🌟",
            "You're doing great! 🎉",
            "Keep learning! 📚",
            "You're getting better! ⚡",
            "Believe in yourself! 🌈",
            "You can do it! 🚀",
            "Keep trying! 💫"
        };
        
        return encouragements[UnityEngine.Random.Range(0, encouragements.Length)];
    }
    
    [System.Serializable]
    public class ResponseAnalysis
    {
        public bool isCorrect;
        public float confidenceLevel;
        public bool containsQuestion;
        public bool showsConfusion;
        public bool showsEnthusiasm;
    }
}

