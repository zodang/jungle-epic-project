using UnityEngine;
using System.Collections;

public class ArrowWaveSmooth : MonoBehaviour
{
    [Header("Renderers (bottom→top)")]
    public SpriteRenderer[] arrows;

    [Header("Alpha Settings")]
    [Range(0, 1)] public float baseAlpha = 0.5f;
    [Range(0, 1)] public float highlightAlpha = 1f;

    [Header("Timing")]
    public float interval = 0.3f;      // 한 단계가 유지되는 시간
    public float fadeDuration = 0.2f;  // 페이드에 걸리는 시간
    public bool loop = true;

    private void Start()
    {
        // 모두 baseAlpha 로 초기화
        foreach (var sr in arrows)
        {
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, baseAlpha);
        }
        StartCoroutine(WaveSmooth());
    }

    private IEnumerator WaveSmooth()
    {
        int idx = 0;
        do
        {
            // 이번 인덱스 강조, 나머지는 base로 돌려보냄
            for (int i = 0; i < arrows.Length; i++)
            {
                float target = (i == idx) ? highlightAlpha : baseAlpha;
                StartCoroutine(FadeTo(arrows[i], target));
            }

            // 다음 화살표로
            idx = (idx + 1) % arrows.Length;
            yield return new WaitForSeconds(interval);
        }
        while (loop);
    }

    private IEnumerator FadeTo(SpriteRenderer sr, float targetAlpha)
    {
        float start = sr.color.a;
        float elapsed = 0f;
        Color c = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(start, targetAlpha, elapsed / fadeDuration);
            sr.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        sr.color = new Color(c.r, c.g, c.b, targetAlpha);
    }
}
