using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public Action OnResetBtnClicked;
    
    private Inventory _inventory;
    [SerializeField] private Button resetBtn;

    private void Awake()
    {
        resetBtn.onClick.AddListener(OnClickResetBtn);
    }

    private void OnClickResetBtn()
    {
        OnResetBtnClicked?.Invoke();
    }
}
