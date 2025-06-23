using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class Flik : MonoBehaviour
{
    [Header("한 사이클 총 시간")]
    [SerializeField] private float cycleDuration = 1f;
    [Header("알파 최소/최대")]
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.5f;

    private RawImage _image;

    void Awake()
    {
        _image = GetComponent<RawImage>();
    }

    void OnEnable()
    {
        StartCoroutine(Flicker());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Flicker()
    {
        float half = cycleDuration * 0.5f;
        while (true)
        {
            // 투명 → 불투명
            for (float t = 0f; t < half; t += Time.unscaledDeltaTime)
            {
                _image.color = new Color(1, 1, 1, Mathf.Lerp(minAlpha, maxAlpha, t / half));
                yield return null;
            }
            // 불투명 → 투명
            for (float t = 0f; t < half; t += Time.unscaledDeltaTime)
            {
                _image.color = new Color(1, 1, 1, Mathf.Lerp(maxAlpha, minAlpha, t / half));
                yield return null;
            }
        }
    }
}
