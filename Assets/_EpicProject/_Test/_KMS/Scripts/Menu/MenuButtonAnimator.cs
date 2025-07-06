using UnityEngine;
using UnityEngine.UI;          // LayoutRebuilder
using DG.Tweening;
using System.Collections;

public class MenuButtonLocalAnimator : MonoBehaviour
{
    [Header("위→아래 순서대로 할당")]
    [SerializeField] private RectTransform[] Buttons;

    [Header("초기 숨김 지연(초)")]
    [SerializeField] private float initialDelay = 0.5f;

    [Header("이동·페이드 시간(초)")]
    [SerializeField] private float duration = 0.5f;

    [Header("버튼 간 딜레이(초)")]
    [SerializeField] private float interval = 0.5f;

    [Header("시작 위치 오프셋 (왼쪽으로)")]
    [SerializeField] private Vector2 startOffset = new Vector2(-100f, 0f);

    private Vector3[] _finalPositions;
    private CanvasGroup[] _canvasGroups;

    private IEnumerator Start()
    {
        // 1) 강제로 레이아웃 재계산
        var parentRect = GetComponent<RectTransform>();
        if (parentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

        // 2) 한 프레임 대기 → 이제 버튼들의 위치가 확실히 계산됨
        yield return new WaitForEndOfFrame();

        // 3) 최종 위치 저장 & 초기 숨김 세팅
        int n = Buttons.Length;
        _finalPositions = new Vector3[n];
        _canvasGroups = new CanvasGroup[n];

        for (int i = 0; i < n; i++)
        {
            var btn = Buttons[i];
            // 위치 저장
            _finalPositions[i] = btn.localPosition;
            // CanvasGroup 세팅
            var cg = btn.GetComponent<CanvasGroup>()
                     ?? btn.gameObject.AddComponent<CanvasGroup>();
            _canvasGroups[i] = cg;

            // 즉시 시작 오프셋 & 완전 투명
            btn.localPosition = _finalPositions[i] + new Vector3(startOffset.x, startOffset.y, 0f);
            cg.alpha = 0f;
        }

        // 4) 초기 지연 — 이 동안 버튼은 숨겨진 상태
        yield return new WaitForSeconds(initialDelay);

        // 5) 차례대로 페이드인+슬라이드인
        for (int i = 0; i < n; i++)
        {
            float delay = i * interval;

            _canvasGroups[i]
                .DOFade(1f, duration)
                .SetDelay(delay)
                .SetEase(Ease.OutQuad);

            Buttons[i]
                .DOLocalMove(_finalPositions[i], duration)
                .SetDelay(delay)
                .SetEase(Ease.OutQuad);
        }
    }
}
