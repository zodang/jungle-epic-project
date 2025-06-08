using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EngineUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    public Action OnClickCloseBtn;
    
    [Header("Profile")]
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text serialNum;
    [SerializeField] private TMP_Text status;
    [SerializeField] private TMP_InputField noteInput;
    [SerializeField] private Image targetImg;
    
    [Header("Button")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button resetBtn;
    
    private EngineUICloseBtn _closeBtn;
    private EngineUIOpacitySlider _opacitySlider;
    
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    
    private Vector2 _offset = new Vector2(150, 0);
    private float _minOpacity = 0.4f;

    private Clickable _target;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        // opacity slider 기능 연결
        _opacitySlider = transform.GetComponentInChildren<EngineUIOpacitySlider>();
        _opacitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        _opacitySlider.GetComponent<Slider>().minValue = _minOpacity;
        
        // Button 기능 연결
        closeBtn.onClick.AddListener(WhenCloseBtnClicked);
        resetBtn.onClick.AddListener(WhenResetBtnClicked);
    }

    private void WhenCloseBtnClicked()
    {
        // Close Btn 클릭
        OnClickCloseBtn?.Invoke();
    }
    
    private void WhenResetBtnClicked()
    {
        // Reset Btn 클릭
        OnResetBtnClicked?.Invoke();
    }
    
    private void OnSliderValueChanged(float value)
    {
        // Canvas 투명도 조절
        _canvasGroup.alpha = value;
    }

    public void SetProfile(ClickableProfile profile, Clickable target)
    {
        // 프로필 설정
        _target = target;
        name.text = profile.name;
        serialNum.text = profile.serialNumber;
        status.text = profile.status;
        
        noteInput.onValueChanged.RemoveAllListeners();
        noteInput.text = profile.note;
        noteInput.onValueChanged.AddListener(newNote => { _target.UpdateNote(newNote);});

        targetImg.sprite = profile.sprite;
    }

    public void ClearProfile()
    {
        _target = null;
    }
    
    public void SetUIPosition(Clickable clickable)
    {
        // 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 우측에 UI 위치
        Vector2 targetPos = screenPos + _offset;

        // 팝업 UI 크기/캔버스 크기 가져오기
        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        // 화면 끝 계산 (스크린 좌표)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 짤림 검사
        // (1) 오른쪽 끝 넘침 → 왼쪽에 붙임
        if (targetPos.x + halfWidth > screenWidth)
            targetPos.x = screenPos.x - _offset.x - uiSize.x;

        // (2) 왼쪽 끝 넘침 → 오른쪽에 붙임
        if (targetPos.x - halfWidth < 0)
            targetPos.x = screenPos.x + _offset.x;

        // (3) 위쪽 끝 넘침 → 아래로 내림
        if (targetPos.y + halfHeight > screenHeight)
            targetPos.y = screenHeight - halfHeight - 10;

        // (4) 아래쪽 끝 넘침 → 위로 올림
        if (targetPos.y - halfHeight < 0)
            targetPos.y = halfHeight + 10;

        // UI 위치 변경
        _rectTransform.position = targetPos;
    }

    private void OnDestroy()
    {
        OnResetBtnClicked = null;
        OnClickCloseBtn = null;
    }
}
