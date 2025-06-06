using UnityEngine;

public class InventoryAniController : MonoBehaviour
{
    [SerializeField] private Animator _inventoryAni;  
    private bool _isOpen = false;

    private void Awake()
    {
        if (_inventoryAni == null);
    }

    // SloGroup => Button안에 OnCLick 연결 메서드
    public void ToggleInventory()
    {
        if (_inventoryAni == null) return;

        if (!_isOpen)
        {
            _inventoryAni.Play("OpenAniClip");
            _isOpen = true;
        }
        else
        {
            _inventoryAni.Play("CloseAniClip");
            _isOpen = false;
        }
    }
}
