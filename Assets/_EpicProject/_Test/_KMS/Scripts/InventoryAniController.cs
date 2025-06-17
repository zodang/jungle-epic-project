using Define;
using UnityEngine;

public class InventoryAniController : MonoBehaviour
{
    private Animator _inventoryAni;  
    private bool _isOpen = false;
    private InventoryBtn _inventoryBtn;

    private void Awake()
    {
        _inventoryAni = GetComponent<Animator>();
        
        // Inventory 버튼 초기화
        _inventoryBtn = FindAnyObjectByType<InventoryBtn>();
        if (_inventoryBtn != null)
        {
            _inventoryBtn.Init(this);
        }
    }

    private void Start()
    {
        // Tab 누를 시 inventory 활성화
        StageManager.Instance.InputManager.OnInventoryToggled += ToggleInventory;
    }

    public void ToggleInventory()
    {
        if (_inventoryAni == null) return;

        if (!_isOpen)
        {
            _inventoryAni.Play("OpenAniClip");
            _isOpen = true;
            
            _inventoryBtn?.ActivateBtn(false);
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Open);
        }
        else
        {
            _inventoryAni.Play("CloseAniClip");
            _isOpen = false;
            
            _inventoryBtn?.ActivateBtn(true);
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        }
    }
}
