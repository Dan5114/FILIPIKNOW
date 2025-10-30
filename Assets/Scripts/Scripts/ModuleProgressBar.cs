using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModuleProgressBar : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Slider progressSlider;

    private int totalQuestions;
    private int answeredQuestions = 0;

    [SerializeField] private UnityEvent<float> onProgress;
    [SerializeField] private UnityEvent onCompleted;

    private Coroutine animationCoroutine;

    public Slider ProgressSlider => progressSlider;
    public void SetTotalQuestions(int totalQuestions) => this.totalQuestions = totalQuestions;
    public void AddToAnsweredQuestions()
    {
        if (answeredQuestions < totalQuestions)
        {
            answeredQuestions++;
        }
        float progress = (float)answeredQuestions / (float)totalQuestions;
        SetProgress(progress);
        Debug.Log($"Progress: {answeredQuestions}/{totalQuestions} = {progress}%");
    } 

    public void SetProgress(float targetValue, float animateSpeed = 3f)
    {
        if (targetValue < 0 || targetValue > 1)
        {
            targetValue = Mathf.Clamp01(targetValue);
        }

        if (Mathf.Approximately(targetValue, progressSlider.value)) return;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateSetProgress(targetValue, animateSpeed));
    }

    private IEnumerator AnimateSetProgress(float targetValue, float speed)
    {
        float startValue = progressSlider.value;
        float time = 0f;
        float duration = Mathf.Abs(targetValue - startValue) / speed;

        while (time < duration)
        {
            float t = time / duration;
            progressSlider.value = Mathf.Lerp(startValue, targetValue, t);
            onProgress?.Invoke(progressSlider.value);
            time += Time.deltaTime;
            yield return null;
        }

        progressSlider.value = targetValue;
        onProgress?.Invoke(targetValue);
        onCompleted?.Invoke();
    }
}
