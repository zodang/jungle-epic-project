using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Vector3 _originalAnchorPos;
    private Transform _originalParent;
    private Slot _prevSlot;
    
    private Canvas _canvas;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalAnchorPos = _rectTransform.anchoredPosition;
        _originalParent = _rectTransform.parent;
        _prevSlot = _originalParent.GetComponent<Slot>();
        
        _rectTransform.SetParent(_canvas.transform);
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
        
        bool canDrop = (nextSlot != null) && (nextSlot.CanDrop());
        
        // 새 slot으로 이동
        if (canDrop)
        {
            _rectTransform.SetParent(nextSlot.transform,false);
            _rectTransform.anchoredPosition = Vector3.zero;
            
            nextSlot.OnBlockDrop(this);
            _prevSlot.OnBlockRemoved();
        }
        
        // 기존 slot으로 이동
        else
        {
            _rectTransform.SetParent(_originalParent, false);
            _rectTransform.anchoredPosition = _originalAnchorPos;
        }
    }
}
    
