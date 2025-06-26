using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public Action<bool> OnDragEndedInHome;
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _offset;
    private Transform _originalParent;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _originalParent = transform.parent;
    }
    
    private void OnDestroy()
    {
        OnDragEndedInHome = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(_originalParent);
        transform.SetAsLastSibling();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.position, eventData.pressEventCamera, out _offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 pointerPosition))
        {
            _rectTransform.anchoredPosition = pointerPosition - _offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool isInHome = false;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            TabHome tabHome = result.gameObject.GetComponent<TabHome>();
            if (tabHome != null)
            {
                // TabHome으로 정렬 시
                transform.SetParent(tabHome.transform, false);
                _rectTransform.anchoredPosition = Vector3.zero;
                isInHome = true;
                break;
            }
        }
        
        OnDragEndedInHome?.Invoke(isInHome);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void SetToOriginalParent()
    {
        transform.SetParent(_originalParent);
        transform.SetAsLastSibling();
    }
}
