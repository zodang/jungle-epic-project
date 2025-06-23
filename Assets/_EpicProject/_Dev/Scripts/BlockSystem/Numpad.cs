using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Numpad : MonoBehaviour, ISlotType, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<int> OnClickNumpad;

    public int Index { get; private set; }
    private Button _btn;

    private EngineBlock _blockToDrag;

    public void Init(int index)
    {
        Index = index;
        
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnBtnClicked);
    }

    private void OnBtnClicked()
    {
        OnClickNumpad?.Invoke(Index);
    }


    private Clickable _targetClickable;  
    public SlotType GetSlotType()
    {
        return SlotType.Numpad;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 현재 타겟에서 해당 인덱스의 블록을 가져옴
        if (_targetClickable == null) return;

        _targetClickable.BlockDictionary.TryGetValue(Index, out _blockToDrag);
        if (_blockToDrag == null) return;

        // 강제로 드래그 시작
        var visual = _blockToDrag.GetComponent<BlockVisual>();
        if (visual == null) return;

        visual.ForceBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _blockToDrag?.GetComponent<BlockVisual>()?.ForceDrag(eventData);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _blockToDrag?.GetComponent<BlockVisual>()?.ForceEndDrag(eventData);
    }
}
