using Define;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EngineUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    public Action OnTabBtnClicked;
    public Action OnClearBtnClicked;
    public Action OnClickCloseBtn;

    [Header("Profile Group")] 
    [SerializeField] private GameObject profileGroup;
    [SerializeField] private Image profileImg;
    [SerializeField] private TMP_Text profileName;

    [Header("Slot Group")] 
    [SerializeField] private GameObject engineSlotGroup;

    [Header("Fold")] 
    private bool _isFold = true;
    private Image _baseImg;

    [Header("Values")]
    private RectTransform _rectTransform;

    [SerializeField] private Button resetBtn;
    [SerializeField] private Button tabBtn;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _baseImg = GetComponent<Image>();
    }

    private void Start()
    {
        // Button 기능 연결
        resetBtn.onClick.AddListener(WhenResetBtnClicked);
        tabBtn.onClick.AddListener(WhenTabBtnClicked);
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

    private void WhenTabBtnClicked()
    {
        OnTabBtnClicked?.Invoke();
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
        engineSlotGroup.SetActive(_isFold);
        profileGroup.SetActive(_isFold);
    }

    public void SetProfile(ClickableProfile profile)
    {
        // 엔진 프로필 변경
        if (profile == null) return;

        profileName.text = profile.name;
        profileImg.sprite = profile.sprite;
    }
    
    public void DisableTabBtn()
    {
        tabBtn.gameObject.SetActive(false);
    }

    #region Dotween

    [Header("Dotween")] 
    [SerializeField] private float _outsidePos = 0;
    private float _insidePos = -300;
    private float _duration = 0.4f;
    private Sequence _sequence;

    public void ActivateEffect(EngineActivationType type)
    {
        transform.SetAsLastSibling();
        _rectTransform.DOKill();

        if (type == EngineActivationType.LeftToRightType)
        {
            _rectTransform.DOAnchorPosX(_insidePos, 0);
        }
        else
        {
            _rectTransform.DOAnchorPosY(_insidePos, 0);
        }

        ActivateSequence(true, type);
    }

    public void DeactivateEffect(EngineActivationType type)
    {
        _rectTransform.DOKill();
        ActivateSequence(false, type);
    }

    private void ActivateSequence(bool isActive, EngineActivationType type)
    {
        float endPos = isActive ? _outsidePos : _insidePos;

        _sequence = DOTween.Sequence().SetAutoKill(false);
        if (type == EngineActivationType.LeftToRightType)
        {
            _sequence.Append(_rectTransform.DOAnchorPosX(endPos, _duration)).SetEase(Ease.InQuad);
        }
        else
        {
            _sequence.Append(_rectTransform.DOAnchorPosY(endPos, 1)).SetEase(Ease.InQuad);
        }
    }

    #endregion
}
