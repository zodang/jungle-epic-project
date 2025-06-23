using Define;
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
    
    [Header("Block Container")]
    [SerializeField] private Transform blockContainer;
    [SerializeField] private List<Color> containerColors;   
    [SerializeField] private TMP_Text blockContainerText;
    [SerializeField] private List<Color> textColors;   
    
    private EngineUICloseBtn _closeBtn;
    private EngineUIOpacitySlider _opacitySlider;
    private Image _blockContainerImg;
    
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    
    [Header("Slot Icon")]
    [SerializeField] private List<Image> iconImageList;

    [Header("Dotween")]
    private float _posX = 700f;
    private float _minPosY = -900f;
    private float _maxPosY = -245f;
    private float _activeDuration = 1f;
    private float _deactiveDuration = 0.25f;
    
    private float _minOpacity = 0.4f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _blockContainerImg = blockContainer.GetComponent<Image>();

        // opacity slider 기능 연결
        _opacitySlider = transform.GetComponentInChildren<EngineUIOpacitySlider>();
        _opacitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        _opacitySlider.GetComponent<Slider>().minValue = _minOpacity;
        
        // Button 기능 연결
        closeBtn.onClick.AddListener(WhenCloseBtnClicked);
        resetBtn.onClick.AddListener(WhenResetBtnClicked);

        ChangeBlockContainer(0);
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

    public void SetSlotIcon(Dictionary<int, BlockType> slotBlockMap)
    {
        // Slot Btn 아이콘 변경
        for (int i = 0; i < iconImageList.Count; i++)
        {
            if (slotBlockMap.TryGetValue(i, out var blockType))
            {
                var icon = StageManager.Instance.BlockFactory.GetIcon(blockType);
                iconImageList[i].sprite = icon;
                iconImageList[i].enabled = icon != null;
            }
            else
            {
                iconImageList[i].sprite = null;
                iconImageList[i].enabled = false;
            }
        }
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
