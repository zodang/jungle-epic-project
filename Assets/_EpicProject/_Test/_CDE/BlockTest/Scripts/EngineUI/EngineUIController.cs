using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [Header("Fold")] 
    private bool _isFold = true;
    private Image _baseImg;

    [Header("Values")] 
    private readonly float _activeDuration = 0.25f;
    private readonly float _deactiveDuration = 0.25f;
    private RectTransform _rectTransform;

    [SerializeField] private Button resetBtn;
    [SerializeField] private Button clearBtn;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _baseImg = GetComponent<Image>();
    }

    private void Start()
    {
        // Button 기능 연결
        resetBtn.onClick.AddListener(WhenResetBtnClicked);
        clearBtn.onClick.AddListener(WhenClearBtnClicked);
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
            _rectTransform.localScale = Vector3.one;
            gameObject.SetActive(false);
        });
    }
}
