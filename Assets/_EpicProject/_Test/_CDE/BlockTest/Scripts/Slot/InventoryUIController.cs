using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    
    private Inventory _inventory;
    [SerializeField] private Button resetBtn;

    private RectTransform _rectTransform;
    private HorizontalLayoutGroup _layoutGroup;
    
    private readonly float _height = 200f;
    private readonly int _paddingTop = 188;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();

        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _height);
        _layoutGroup.padding.top = _paddingTop;
        
        resetBtn.onClick.AddListener(OnClickResetBtn);
    }

    private void OnClickResetBtn()
    {
        OnResetBtnClicked?.Invoke();
    }
}
