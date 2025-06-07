using Define;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAniController : MonoBehaviour
{
    private Button _button;
    private Animator _inventoryAni;  
    private bool _isOpen = false;

    private void Awake()
    {
        _inventoryAni = GetComponent<Animator>();
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(ToggleInventory);
    }

    // SloGroup => Button�ȿ� OnCLick ���� �޼���
    public void ToggleInventory()
    {
        if (_inventoryAni == null) return;

        if (!_isOpen)
        {
            _inventoryAni.Play("OpenAniClip");
            _isOpen = true;
            
            AudioManager.instance.playSfx(SfxType.Open);
        }
        else
        {
            _inventoryAni.Play("CloseAniClip");
            _isOpen = false;
            
            AudioManager.instance.playSfx(SfxType.Close);
        }
    }
}
