using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EngineUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    public Action OnClickCloseBtn;
    
    [Header("Profile")]
    [SerializeField] private TMP_Text gameName;
    [SerializeField] private Image targetImg;
    
    [Header("Button")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button resetBtn;
    
    private EngineUICloseBtn _closeBtn;
    private EngineUIOpacitySlider _opacitySlider;
    
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    [Header("Dotween")]
    private float _posX = 700f;
    private float _minPosY = -900f;
    private float _maxPosY = -245f;
    private float _activeDuration = 1f;
    private float _deactiveDuration = 0.25f;
    
    private float _minOpacity = 0.4f;

    private void Awake()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
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

    public void SetProfile(ClickableProfile profile)
    {
        // 프로필 이름 변경
        if (profile == null) return;
        gameName.text = profile.name;
        targetImg.sprite = profile.sprite;
    }

    public void ActivateEffect()
    {
        transform.SetAsLastSibling();
        
        // 초기 설정
        _rectTransform.localScale = Vector3.one;
        _rectTransform.anchoredPosition = new Vector2(_posX, _minPosY);
        
        _rectTransform.DOAnchorPos(new Vector2(_posX, _maxPosY), _activeDuration).SetEase(Ease.OutBack);
    }

    public void DeactivateEffect()
    {
        _rectTransform.DOScale(Vector3.zero, _deactiveDuration);
    }
    
    private void OnDestroy()
    {
        OnResetBtnClicked = null;
        OnClickCloseBtn = null;
    }
}
