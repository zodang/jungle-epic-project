using UnityEngine;
using UnityEngine.UI;

public class PopInspectorUI : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private RectTransform tooltipRect;

    private Canvas _canvas;
    public Vector2 offset = new Vector2(450, 0);

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        
        closeBtn.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
    }

    public void Show(ClickableController clickable)
    {
        SetPosition(clickable);
        // 슬롯 데이터 초기화
        
        _canvas.enabled = true;
    }
    
    public void Hide()
    {
        _canvas.enabled = false;
    }

    private void SetPosition(ClickableController clickable)
    {
        // 위치 세팅
        // 1. 월드 → 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 2. 우측에 UI 위치
        Vector2 targetPos = screenPos + offset;

        // 3. 팝업 UI 크기/캔버스 크기 가져오기
        Vector2 uiSize = tooltipRect.sizeDelta * _canvas.scaleFactor;
        Vector2 canvasSize = _canvas.GetComponent<RectTransform>().sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x / 2f;
        float halfHeight = uiSize.y / 2f;

        // 4. 화면 끝 계산 (스크린 좌표)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 5. 짤림 검사
        // (1) 오른쪽 끝 넘침 → 왼쪽에 붙임
        if (targetPos.x + halfWidth > screenWidth)
            targetPos.x = screenPos.x - offset.x - uiSize.x;

        // (2) 왼쪽 끝 넘침 → 오른쪽에 붙임
        if (targetPos.x - halfWidth < 0)
            targetPos.x = screenPos.x + offset.x;

        // (3) 위쪽 끝 넘침 → 아래로 내림
        if (targetPos.y + halfHeight > screenHeight)
            targetPos.y = screenHeight - halfHeight - 10;

        // (4) 아래쪽 끝 넘침 → 위로 올림
        if (targetPos.y - halfHeight < 0)
            targetPos.y = halfHeight + 10;

        // 6. UI 실제 위치 반영
        tooltipRect.position = targetPos;
    }
}
