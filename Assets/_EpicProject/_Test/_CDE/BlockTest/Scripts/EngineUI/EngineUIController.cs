using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

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
    [SerializeField] private Button upBtn;
    
    [Header("Block Container")]
    [SerializeField] private Transform blockContainer;
    [SerializeField] private List<Color> containerColors;   
    [SerializeField] private TMP_Text blockContainerText;
    [SerializeField] private List<Color> textColors;

    [Header("Fold")] 
    private bool _isFold = true;
    [SerializeField] private Image baseImg;
    [SerializeField] private GameObject numpadSlot;
    [SerializeField] private GameObject engineSlot;
    
    [Header("DOTween Values")]
    private float _posX = 700f;
    private float _minPosY = -900f;
    private float _maxPosY = -245f;
    private float _activeDuration = 1f;
    private float _deactiveDuration = 0.25f;
    
    [Header("Values")]
    private float _minOpacity = 0.4f;
    private Vector2 _offset = new Vector2(100, 0);
    
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private EngineUIOpacitySlider _opacitySlider;
    private Image _blockContainerImg;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _blockContainerImg = blockContainer.GetComponent<Image>();
    }

    private void Start()
    {
        // opacity slider 기능 연결
        _opacitySlider = transform.GetComponentInChildren<EngineUIOpacitySlider>();
        _opacitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        _opacitySlider.GetComponent<Slider>().minValue = _minOpacity;
        
        // Button 기능 연결
        closeBtn.onClick.AddListener(WhenCloseBtnClicked);
        resetBtn.onClick.AddListener(WhenResetBtnClicked);
        upBtn.onClick.AddListener(WhenUpBtnClicked);

        ChangeBlockContainer(0);
    }

    private void OnDestroy()
    {
        OnResetBtnClicked = null;
        OnClickCloseBtn = null;
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

    private void WhenUpBtnClicked()
    {
        _isFold = !_isFold;
        baseImg.enabled = _isFold;
        numpadSlot.SetActive(_isFold);
        engineSlot.SetActive(_isFold);
    }
    
    private void OnSliderValueChanged(float value)
    {
        // Canvas 투명도 조절
        _canvasGroup.alpha = value;
    }

    public void SetProfile(ClickableProfile profile)
    {
        // 엔진 프로필 변경
        if (profile == null) return;
        
        gameName.text = profile.name;
        targetImg.sprite = profile.sprite;
    }
    
    public void SetBlockPositionToEngine(EngineBlock block)
    {
        // Block Container 가운데로 배치
        RectTransform rectTransform = block.GetComponent<RectTransform>();
        rectTransform.SetParent(blockContainer, false);
        
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void ChangeBlockContainer(int index)
    {
        _blockContainerImg.color = containerColors[index];
        blockContainerText.color = textColors[index];
        blockContainerText.text = $"{index + 1}";
    }
    
    public void ActivateEffect()
    {
        transform.SetAsLastSibling();
        _rectTransform.localScale = Vector3.one;
    }

    public void DeactivateEffect(Clickable target)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_rectTransform.DOScale(Vector3.zero, _deactiveDuration));
        sequence.OnComplete(() =>
        {
            SetPosition(target);
            _rectTransform.localScale = Vector3.one;
            gameObject.SetActive(false);
        });
    }

    public void SetPosition(Clickable clickable)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 기본 위치 오른쪽
        Vector2 targetPos = screenPos + new Vector2(Mathf.Abs(_offset.x), _offset.y);

        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // targetPos 설정
        if (targetPos.x + halfWidth > screenWidth) targetPos.x = screenPos.x - Mathf.Abs(_offset.x) - uiSize.x;
        if (targetPos.x - halfWidth < 0) targetPos.x = screenPos.x + Mathf.Abs(_offset.x);
        if (targetPos.y + halfHeight > screenHeight) targetPos.y = screenHeight - halfHeight - 10;
        if (targetPos.y - halfHeight < 0) targetPos.y = halfHeight + 10;

        _rectTransform.position = targetPos;
    }
}
