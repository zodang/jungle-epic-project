using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockVisual : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<ISlotType> OnDragEnd;
    
    public static event Action OnAnyBlockBeginDrag;
    public static event Action OnAnyBlockEndDrag;
    
    private ISlotType _detectedSlot;
    
    public GameObject VisualGroup;
    public GameObject EngineBlock;
    public GameObject InventoryBlock;
    public GameObject NumpadBlock;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Vector2 _dragOffset;

    private void Awake()
    {
        ChangeBlockVisual(SlotType.InventorySlot);
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnAnyBlockBeginDrag?.Invoke();
        
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 globalMousePos))
        {
            _dragOffset = (Vector2)(_rectTransform.position - globalMousePos);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // UI 중앙을 마우스 위치로 이동
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 globalMousePos))
        {
            _rectTransform.position = globalMousePos;
        }

        // Slot 감지
        if (!TryGetSlotUnderMouse(out ISlotType slot)) return;
        _detectedSlot = slot;
        ChangeBlockVisual(_detectedSlot);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke(_detectedSlot);
        OnAnyBlockEndDrag?.Invoke();
    }
    
    private bool TryGetSlotUnderMouse(out ISlotType slot)
    {
        slot = null;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            var slotInterface = result.gameObject.GetComponent<ISlotType>();
            if (slotInterface != null)
            {
                slot = slotInterface;
                return true;
            }
        }

        return false;
    }
    
    private void ChangeBlockVisual(ISlotType slot)
    {
        SlotType slotType = slot.GetSlotType();
        InventoryBlock.SetActive(slotType == SlotType.InventorySlot);
        EngineBlock.SetActive(slotType == SlotType.EngineSlot);
        NumpadBlock.SetActive(slotType == SlotType.NumpadSlot || slotType == SlotType.Numpad);
    }

    public void ChangeBlockVisual(SlotType slotType)
    {
        InventoryBlock.SetActive(slotType == SlotType.InventorySlot);
        EngineBlock.SetActive(slotType == SlotType.EngineSlot);
        NumpadBlock.SetActive(slotType == SlotType.NumpadSlot || slotType == SlotType.Numpad);
    }

    public void ShowBlockVisual(bool isActive)
    {
        VisualGroup.SetActive(isActive);
    }
    
    public void ForceBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();

        RectTransform parentRect = _rectTransform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPointerPosition
        );

        _rectTransform.anchoredPosition = localPointerPosition;
        _dragOffset = Vector2.zero;
    }

    public void ForceDrag(PointerEventData eventData)
    {
        RectTransform parentRect = _rectTransform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        _rectTransform.anchoredPosition = localPoint + _dragOffset + Vector2.up;

        // Slot Type 감지
        if (!TryGetSlotUnderMouse(out ISlotType slot)) return;

        _detectedSlot = slot;
        ChangeBlockVisual(_detectedSlot);
    }


    public void ForceEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke(_detectedSlot);
    }
}
