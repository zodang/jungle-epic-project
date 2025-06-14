using Define;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAniController : MonoBehaviour
{
    private Button _toggleBtn;
    private Animator _inventoryAni;  
    private bool _isOpen = false;
    private InventoryBtn _inventoryBtn;

    private void Awake()
    {
        _inventoryAni = GetComponent<Animator>();
        _toggleBtn = GetComponentInChildren<Button>();
        
        // Toggle 버튼 클릭 시 inventory 활성화
        _toggleBtn.onClick.AddListener(ToggleInventory);
        
        // Tab 누를 시 inventory 활성화
        StageManager.Instance.InputManager.OnInventoryToggled += ToggleInventory;
        
        // Inventory 버튼 초기화
        _inventoryBtn = FindAnyObjectByType<InventoryBtn>();
        if (_inventoryBtn != null)
        {
            _inventoryBtn.Init(this);
        }
    }

    public void ToggleInventory()
    {
        if (_inventoryAni == null) return;

        if (!_isOpen)
        {
            _inventoryAni.Play("OpenAniClip");
            _isOpen = true;
            
            _inventoryBtn?.ActivateBtn(false);
            AudioManager.Instance.PlaySfx(SfxType.Open);
        }
        else
        {
            _inventoryAni.Play("CloseAniClip");
            _isOpen = false;
            
            _inventoryBtn?.ActivateBtn(true);
            AudioManager.Instance.PlaySfx(SfxType.Close);
        }
    }
}
