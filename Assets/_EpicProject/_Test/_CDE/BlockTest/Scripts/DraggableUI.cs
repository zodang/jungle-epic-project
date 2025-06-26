using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public Action<bool> OnParentChangedToHome;
    
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

    private void Start()
    {
        OnParentChangedToHome += SetParentToOriginal;
    }

    private void OnDestroy()
    {
        OnParentChangedToHome = null;
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
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // TabHome 자식으로 이동
            TabHome tabHome = result.gameObject.GetComponent<TabHome>();
            if (tabHome != null)
            {
                transform.SetParent(tabHome.transform, false);
                _rectTransform.anchoredPosition = Vector3.zero;
                OnParentChangedToHome?.Invoke(true);
                break;
            }
            else
            {
                OnParentChangedToHome?.Invoke(false);
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void SetParentToOriginal(bool isInHome)
    {
        if (!isInHome)
        {
            // 정렬 해제 시 부모 복구
            transform.SetParent(_originalParent);
            transform.SetAsLastSibling();
        }
    }
}
