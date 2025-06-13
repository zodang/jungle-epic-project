using Define;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAniController : MonoBehaviour
{
    private InventoryBtn _button;
    private Animator _inventoryAni;  
    private bool _isOpen = false;

    private void Awake()
    {
        _inventoryAni = GetComponent<Animator>();
        _button = FindAnyObjectByType<InventoryBtn>();
            
        if (_button != null)
        {
            // 버튼 클릭 시 inventory 활성화
            _button.GetComponent<Button>().onClick.AddListener(ToggleInventory);
        }
        
        // Tab 누를 시 inventory 활성화
        InputManager.Instance.OnInventoryToggled += ToggleInventory;
    }

    private void ToggleInventory()
    {
        if (_inventoryAni == null) return;

        if (!_isOpen)
        {
            _inventoryAni.Play("OpenAniClip");
            _isOpen = true;
            
            AudioManager.Instance.PlaySfx(SfxType.Open);
        }
        else
        {
            _inventoryAni.Play("CloseAniClip");
            _isOpen = false;
            
            AudioManager.Instance.PlaySfx(SfxType.Close);
        }
    }
}
