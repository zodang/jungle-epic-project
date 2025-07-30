using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    
    private Inventory _inventory;
    [SerializeField] private Button resetBtn;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        resetBtn.onClick.AddListener(OnClickResetBtn);
    }

    private void OnClickResetBtn()
    {
        OnResetBtnClicked?.Invoke();
    }
}
