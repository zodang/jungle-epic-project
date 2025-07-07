using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipData _tooltipData;
    private bool _isShow = false;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isShow = true;
        TooltipUI.Instance.ShowTooltip(_tooltipData, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
        _isShow = false;
    }

    private void Update()
    {
        if (!_isShow) return;
        TooltipUI.Instance.SetTooltipPosition();
    }

    private void OnDisable()
    {
        TooltipUI.Instance.HideTooltip();
        _isShow = false;
    }
}
