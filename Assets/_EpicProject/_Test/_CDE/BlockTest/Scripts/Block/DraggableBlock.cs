using Define;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableBlock : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ★ 드래그 시작/종료 이벤트 선언
    public static event Action OnAnyBlockBeginDrag;
    public static event Action OnAnyBlockEndDrag;

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
        // 드래그 시작 신호 발행
        OnAnyBlockBeginDrag?.Invoke();

        _originalAnchorPos = _rectTransform.anchoredPosition;
        _originalParent = _rectTransform.parent;
        PrevSlot = _originalParent.GetComponent<Slot>();
        _rectTransform.SetParent(_canvas.transform);

        GameManager.Instance.AudioManager.PlaySfx(SfxType.Click);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 기존 드래그 로직…
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector2 screenPosition = eventData.position;
        Camera eventCamera = eventData.pressEventCamera;
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect, screenPosition, eventCamera, out worldPosition);
        transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 기존 드롭 로직…
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Slot nextSlot = null;
        foreach (var result in results)
        {
            nextSlot = result.gameObject.GetComponent<Slot>();
            if (nextSlot != null) break;
        }

        if (nextSlot != null && nextSlot.CanDrop())
        {
            PrevSlot?.OnBlockRemoved();
            nextSlot.OnBlockDrop(this, nextSlot);
            PrevSlot = nextSlot;
        }
        else
        {
            _rectTransform.SetParent(_originalParent, false);
            _rectTransform.anchoredPosition = _originalAnchorPos;
        }

        GameManager.Instance.AudioManager.PlaySfx(SfxType.Put);

        // 드래그 종료 신호 발행
        OnAnyBlockEndDrag?.Invoke();
    }
}
