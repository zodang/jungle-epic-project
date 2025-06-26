using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class InventorySlide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("슬라이드할 거리 (Top 값)")]
    [SerializeField] private float slideY = -25f;
    [Header("슬라이드 시간(초)")]
    [SerializeField] private float duration = 0.5f;

    private RectTransform _rect;
    private Vector2 _startPos;
    private Coroutine _slideCoroutine;

    void Awake()
    {
        // HorizontalLayoutGroup이 붙은 오브젝트의 RectTransform
        _rect = GetComponent<RectTransform>();
        _startPos = _rect.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 진입 시 slideY 위치로 이동
        StartSlide(_startPos.y + slideY);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 벗어날 때 원위치로 복귀
        StartSlide(_startPos.y);
    }

    private void StartSlide(float targetY)
    {
        if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
        _slideCoroutine = StartCoroutine(SlideCoroutine(targetY));
    }

    private IEnumerator SlideCoroutine(float targetY)
    {
        float elapsed = 0f;
        Vector2 from = _rect.anchoredPosition;
        Vector2 to = new Vector2(from.x, targetY);

        while (elapsed < duration)
        {
            _rect.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _rect.anchoredPosition = to;
    }
}
