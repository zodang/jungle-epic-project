using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipData _tooltipData;
    private bool _isShow = false;
    private bool _isPointerOver;
    
    private Coroutine _showTooltipCoroutine;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 드래그 중일 때 return
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
            
        _isShow = true;
        _isPointerOver = true;
        _showTooltipCoroutine = StartCoroutine(DelayTooltip(eventData.position));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_showTooltipCoroutine != null)
        {
            StopCoroutine(_showTooltipCoroutine);
            _showTooltipCoroutine = null;
        }
        
        TooltipUI.Instance.HideTooltip();
        _isShow = false;
        _isPointerOver = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TooltipUI.Instance.HideTooltip();
            _isPointerOver = false;
            
            if (_showTooltipCoroutine != null)
            {
                StopCoroutine(_showTooltipCoroutine);
                _showTooltipCoroutine = null;
            }
        }

        // 툴팁 위치 계속 추적
        if (_isPointerOver)
        {
            TooltipUI.Instance.SetTooltipPosition();
        }
    }

    private void OnDisable()
    {
        if (_showTooltipCoroutine != null)
        {
            StopCoroutine(_showTooltipCoroutine);
            _showTooltipCoroutine = null;
        }
        
        TooltipUI.Instance.HideTooltip();
        _isShow = false;
    }

    private IEnumerator DelayTooltip(Vector2 pos)
    {
        yield return new WaitForSeconds(0.5f);
        if (_isPointerOver)
        {
            TooltipUI.Instance.ShowTooltip(_tooltipData, pos);
        }
    }
}
