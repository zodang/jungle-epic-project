using Define;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Slot PrevSlot { get; private set; }
    
    private RectTransform _rectTransform;
    private Vector3 _originalAnchorPos;
    private Transform _originalParent;
    
    private Canvas _canvas;

    public virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetPrevSlot(Slot slot)
    {
        PrevSlot = slot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalAnchorPos = _rectTransform.anchoredPosition;
        _originalParent = _rectTransform.parent;
        PrevSlot = _originalParent.GetComponent<Slot>();
        
        _rectTransform.SetParent(_canvas.transform);
        
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Click);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector2 screenPosition = eventData.position;
        Camera eventCamera = eventData.pressEventCamera;
        Vector3 worldPosition;

        // 스크린 좌표를 월드 좌표로 변환
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect, screenPosition, eventCamera, out worldPosition);
        transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Slot nextSlot = null;
        foreach (var result in results)
        {
            nextSlot = result.gameObject.GetComponent<Slot>();
            if (nextSlot != null) break;
        }
        
        // 새 slot으로 이동
        if (nextSlot != null && nextSlot.CanDrop())
        {
            PrevSlot?.OnBlockRemoved();
            nextSlot.OnBlockDrop(this, nextSlot);
            PrevSlot = nextSlot;
        }
        
        // 기존 slot으로 이동
        else
        {
            _rectTransform.SetParent(_originalParent, false);
            _rectTransform.anchoredPosition = _originalAnchorPos;
        }

        GameManager.Instance.AudioManager.PlaySfx(SfxType.Put);
    }
}
