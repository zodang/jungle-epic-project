using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class EngineUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    public Action OnClearBtnClicked;
    public Action OnClickCloseBtn;
    
    [Header("Profile Group")]
    [SerializeField] private GameObject profileGroup;
    [SerializeField] private Image profileImg;
    [SerializeField] private TMP_Text profileName;
    
    [Header("Slot Group")]
    [SerializeField] private GameObject engineSlotGroup;

    [Header("Button")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button upBtn;

    [Header("Fold")] 
    private bool _isFold = true;
    private Image _baseImg;
    
    [Header("Values")]
    private readonly float _activeDuration = 0.25f;
    private readonly float _deactiveDuration = 0.25f;
    private RectTransform _rectTransform;
    
    [Header("*Deprecated")]
    [SerializeField] private TMP_Text subGameName;
    [SerializeField] private TMP_Text engineNum;
    [SerializeField] private Button resetBtn;
    [SerializeField] private Button clearBtn;
    [SerializeField] private Transform blockContainer;
    private Image _blockContainerImg;
    [SerializeField] private TMP_Text blockContainerText;
    [SerializeField] private List<Color> containerColors;   
    [SerializeField] private List<Color> textColors;
    [SerializeField] private GameObject numpadSlot;
    private Canvas _canvas;
    private Vector2 _offset = new Vector2(-200, 0);
    
    private void Awake()
    {
        // _canvas = GetComponentInParent<Canvas>();
        // _blockContainerImg = blockContainer.GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        
        _baseImg = GetComponent<Image>();
    }

    private void Start()
    {
        // Button 기능 연결
        closeBtn.onClick.AddListener(WhenCloseBtnClicked);
        resetBtn.onClick.AddListener(WhenResetBtnClicked);
        clearBtn.onClick.AddListener(WhenClearBtnClicked);
        upBtn.onClick.AddListener(WhenUpBtnClicked);

        // ChangeBlockContainer(0);
    }

    private void OnDestroy()
    {
        OnResetBtnClicked = null;
        OnClearBtnClicked = null;
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
    
    private void WhenClearBtnClicked()
    {
        // Clear Btn 클릭
        OnClearBtnClicked.Invoke();
    }

    private void WhenUpBtnClicked()
    {
        _isFold = !_isFold;
        _baseImg.enabled = _isFold;
        numpadSlot.SetActive(_isFold);
        engineSlotGroup.SetActive(_isFold);
    }

    public void SetProfile(ClickableProfile profile)
    {
        // 엔진 프로필 변경
        if (profile == null) return;
        int engineIndex = (int.Parse(profile.id) + 1) % 10 ;

        profileName.text = profile.name;
        subGameName.text = profile.name;
        engineNum.text = $"Engine. No. {engineIndex}.";
        profileImg.sprite = profile.sprite;
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
        _rectTransform.DOKill();
        _rectTransform.localScale = Vector3.zero;
        _rectTransform.DOScale(Vector3.one, _activeDuration).SetEase(Ease.OutCubic);
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
        /*Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 기본 위치: "왼쪽" (offset.x는 무조건 +, 방향만 음수)
        Vector2 targetPos = screenPos - new Vector2(Mathf.Abs(_offset.x), _offset.y);

        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float margin = 10f;

        if (targetPos.x - halfWidth < 0)
            targetPos.x = screenPos.x + Mathf.Abs(_offset.x);

        if (targetPos.y + halfHeight > screenHeight)
            targetPos.y = screenHeight - halfHeight - margin;
        if (targetPos.y - halfHeight < 0)
            targetPos.y = halfHeight + margin;

        _rectTransform.position = targetPos;*/
    }

    #region Opacity
    
    private float _minOpacity = 0.4f;
    private CanvasGroup _canvasGroup;
    private EngineUIOpacitySlider _opacitySlider;
    
    private void InitOpacitySlider()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        // opacity slider 기능 연결
        _opacitySlider = transform.GetComponentInChildren<EngineUIOpacitySlider>();
        _opacitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        _opacitySlider.GetComponent<Slider>().minValue = _minOpacity;
    }
    
    private void OnSliderValueChanged(float value)
    {
        // Canvas 투명도 조절
        _canvasGroup.alpha = value;
    }

    #endregion
}
