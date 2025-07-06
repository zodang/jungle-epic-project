using Define;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Numpad : MonoBehaviour, ISlotType, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<int> OnClickNumpad;
    public int Index { get; private set; }

    private Image _iconImg; 
    private TMP_Text _numText; 

    private Clickable _targetClickable;  
    private EngineBlock _blockToDrag;

    // 드래그 판별용
    private Vector2 _pointerDownPos;
    private bool _isDragging = false;
    private float _dragThreshold = 10f; // px 이상 움직이면 드래그로 판정

    public void Init(int index)
    {
        Index = index;

        _iconImg = GetComponentInChildren<NumpadIcon>().GetComponent<Image>();
        _numText = GetComponentInChildren<TMP_Text>();

        ChangeVisual();
    }

    public void ChangeVisual()
    {
        /*if (_targetClickable.BlockDictionary.TryGetValue(Index, out var block) && block != null)
        {
            var icon = StageManager.Instance.BlockFactory.GetIcon(block.Type);
            _iconImg.sprite = icon;
            _iconImg.enabled = true;
            _numText.enabled = false;
        }
        else
        {
            _iconImg.enabled = false;
            _numText.enabled = true;
        }*/
    }

    public SlotType GetSlotType() => SlotType.Numpad;

    public Clickable GetTargetClickable() => _targetClickable;

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownPos = eventData.position;
        _isDragging = false;

        // 드래그 준비
        if (_targetClickable == null) return;
        // _targetClickable.EngineController.BlockDictionary.TryGetValue(Index, out _blockToDrag);
        
        // 클릭 처리
        OnClickNumpad?.Invoke(Index);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_blockToDrag == null) return;

        if (!_isDragging && Vector2.Distance(_pointerDownPos, eventData.position) > _dragThreshold)
        {
            _isDragging = true;

            var visual = _blockToDrag.GetComponent<BlockVisual>();
            // visual?.ForceBeginDrag(eventData);
        }

        if (_isDragging)
        {
            // _blockToDrag.GetComponent<BlockVisual>()?.ForceDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_blockToDrag == null) return;

        if (_isDragging)
        {
            // _blockToDrag.GetComponent<BlockVisual>()?.ForceEndDrag(eventData);
        }
        else
        {
            // 클릭 처리
            OnClickNumpad?.Invoke(Index);
        }

        _isDragging = false;
        _blockToDrag = null;
    }
}