using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unified Questions", menuName = "Questions/UnifiedQuestions")]
public class UnifiedQuestions : ScriptableObject
{
    [SerializeField] private List<UnifiedQuestionData> unifiedQuestions;

    public List<UnifiedQuestionData> GetUnifiedQuestions() => unifiedQuestions;
}
