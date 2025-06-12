using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EngineUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    public Action OnClickCloseBtn;
    
    [Header("Profile")]
    [SerializeField] private TMP_Text gameName;
    [SerializeField] private TMP_Text serialNum;
    [SerializeField] private TMP_Text status;
    [SerializeField] private Image targetImg;
    
    [Header("Button")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button resetBtn;
    
    private EngineUICloseBtn _closeBtn;
    private EngineUIOpacitySlider _opacitySlider;
    
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    
    private Vector2 _offset = new Vector2(-300, 0);
    private float _minOpacity = 0.4f;


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
        if (profile == null) return;

        // 프로필 설정
        gameName.text = profile.name;
        serialNum.text = profile.serialNumber;
        status.text = profile.status;

        targetImg.sprite = profile.sprite;
    }
    
    public void SetUIPosition(Clickable clickable)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 기본 위치는 왼쪽 (Offset 적용)
        Vector2 targetPos = screenPos + _offset;

        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // (1) 오른쪽 화면을 벗어나면 → 왼쪽으로 붙임
        if (targetPos.x + halfWidth > screenWidth)
            targetPos.x = screenPos.x - Mathf.Abs(_offset.x) - uiSize.x;

        // (2) 왼쪽 화면을 벗어나면 → 오른쪽으로 붙임
        if (targetPos.x - halfWidth < 0)
            targetPos.x = screenPos.x + Mathf.Abs(_offset.x);

        // (3) 위쪽 화면을 벗어나면 → 아래로 내림
        if (targetPos.y + halfHeight > screenHeight)
            targetPos.y = screenHeight - halfHeight - 10;

        // (4) 아래쪽 화면을 벗어나면 → 위로 올림
        if (targetPos.y - halfHeight < 0)
            targetPos.y = halfHeight + 10;

        _rectTransform.position = targetPos;
    }

    private void OnDestroy()
    {
        OnResetBtnClicked = null;
        OnClickCloseBtn = null;
    }
}
