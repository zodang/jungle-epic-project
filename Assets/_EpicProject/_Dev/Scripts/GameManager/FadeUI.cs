using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    
    public IEnumerator FadeCo(float startAlpha, float endAlpha, float duration = 1)
    {
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        // 최종값 보정
        color.a = endAlpha;
        fadeImage.color = color;
    }
}
