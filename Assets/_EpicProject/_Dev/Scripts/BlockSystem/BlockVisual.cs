using Define;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockVisual : MonoBehaviour, 
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, 
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<ISlotType> OnDragEnd;
    public Action OnLeftClicked;
    public Action OnRightClicked;
    
    public static event Action OnAnyBlockBeginDrag;
    public static event Action OnAnyBlockEndDrag;
    
    private ISlotType _detectedSlot;
    
    public GameObject EngineBlock;
    public GameObject InventoryBlock;
    public GameObject NumpadBlock;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Vector2 _dragOffset;

    public RectTransform _visualObject;
    private bool _isAnimating = false;
    private bool _isPointerOver = false;
    private Tween _hoverTween;

    public bool IsRaised;
    private float _yRaisedPos = 130;

    private bool _isDragged;
    private bool _isDragging;

    private void Awake()
    {
        ChangeBlockVisual(SlotType.InventorySlot);
        _visualObject = transform.GetChild(0).GetComponent<RectTransform>();
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDragged) return;
        
        // 해당 블록에 대한 우클릭 검사
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClicked?.Invoke();
        if (eventData.button == PointerEventData.InputButton.Right) OnRightClicked?.Invoke();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InventoryBlock.activeSelf) return;
        
        _isPointerOver = true;
        if (_isAnimating) return;

        _hoverTween?.Kill();
        _isAnimating = true;
        _hoverTween = _visualObject.DOAnchorPosY(_visualObject.anchoredPosition.y + 30, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _isAnimating = false;
                if (!_isPointerOver) OnPointerExit(null);
            });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InventoryBlock.activeSelf) return;
        
        _isPointerOver = false;
        if (_isAnimating) return;

        _hoverTween?.Kill();
        _isAnimating = true;
        _hoverTween = _visualObject.DOAnchorPosY(0, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _isAnimating = false;
                if (_isPointerOver) OnPointerEnter(null);
            });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragged = false;
        _isDragging = true;
        
        OnAnyBlockBeginDrag?.Invoke();
        
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();
        ChangeBlockVisual(SlotType.InventorySlot);

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 globalMousePos))
        {
            _dragOffset = _rectTransform.position - globalMousePos;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _isDragged = true;
        
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
        if (!TryGetSlotUnderMouse(out ISlotType slot))
        {
            ChangeBlockVisual(SlotType.InventorySlot);
            _detectedSlot = null;
            return;
        }
        
        _detectedSlot = slot;
        ChangeBlockVisual(_detectedSlot);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragged = false;
        _isDragging = false;
        
        _hoverTween?.Kill();
        _isAnimating = false;
        _visualObject.anchoredPosition = Vector2.zero;
        
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
        EngineBlock.SetActive(slotType == SlotType.EngineSlot || slotType == SlotType.SimpleSlot);
        NumpadBlock.SetActive(slotType == SlotType.NumpadSlot || slotType == SlotType.Numpad);
    }

    public void ChangeBlockVisual(SlotType slotType)
    {
        InventoryBlock.SetActive(slotType == SlotType.InventorySlot);
        EngineBlock.SetActive(slotType == SlotType.EngineSlot || slotType == SlotType.SimpleSlot);
        NumpadBlock.SetActive(slotType == SlotType.NumpadSlot || slotType == SlotType.Numpad);
    }
    
    public void RaiseVisual(bool raise)
    {
        _hoverTween?.Kill();
        _isAnimating = false;
        if (_isDragging) return;
        
        IsRaised = raise;

        if (IsRaised)
            _visualObject.anchoredPosition = _yRaisedPos * Vector2.up;
        else
        {
            _visualObject.anchoredPosition = Vector2.zero;
            ChangeBlockVisual(SlotType.InventorySlot);
        }
    }
}
