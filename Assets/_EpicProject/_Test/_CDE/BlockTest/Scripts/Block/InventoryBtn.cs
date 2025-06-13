using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBtn : MonoBehaviour
{
    private InventoryAniController _inventoryAniController;
    private Button _button;
    
    private Sequence _sequence;
    private RectTransform _rectTransform;
    private float _duration = 0.2f;
    private float _startPos = 10;
    private float _endPos = -100;

    public void Init(InventoryAniController inventoryAniController)
    {
        _inventoryAniController = inventoryAniController;
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();
        
        _button.onClick.AddListener(OnClickInventoryBtn);
    }

    public void ActivateBtn(bool isActive)
    {
        float pos = isActive ? _startPos : _endPos;
        
        _sequence = DOTween.Sequence();
        _sequence.Append(_rectTransform.DOAnchorPosY(pos, _duration));
    }

    private void OnClickInventoryBtn()
    {
        // Inventory 버튼 클릭 시
        ActivateBtn(false);
        _inventoryAniController.ToggleInventory();
    }
    
}
