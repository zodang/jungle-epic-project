using System;
using Define;
using UnityEngine;

public abstract class EngineBlock : MonoBehaviour
{
    // 블록 타입 정보
    public abstract BlockType Type { get; } 
    
    // 기능 블록이 적용되기 위해 요구하는 Feature의 타입
    public abstract Type RequiredFeatureType { get; }
    
    // 기능 세팅 및 발동
    public abstract void Activate(object feature);
    
    // 기능 종료 및 정리
    public abstract void Deactivate(object feature);

    // UI 초기화 (optional)
    public virtual void ResetUI() { }
    
    private BlockVisual _visual;

    public event Action<EngineBlock, ISlot> OnBlockDragStarted;
    public event Action<EngineBlock, ISlot> OnBlockDragEnd;
    public event Action<EngineBlock> OnBlockLeftClick;
    public event Action<EngineBlock> OnBlockRightClick;

    public Clickable CurrentTarget { get; private set; }
    public object CurrentFeature { get; private set; }
    public int CurrentSlotIndex { get; private set; } = -1;
    public ISlot CurrentSlot { get; private set; }

    protected virtual void Awake()
    {
        _visual = GetComponent<BlockVisual>();
    }
    
    private void Start()
    {
        if (_visual == null) return;
        _visual.OnDragStart += WhenDragStarted;
        _visual.OnDragEnd += WhenDragEnd;
        _visual.OnLeftClicked += WhenBlockLeftClicked;
        _visual.OnRightClicked += WhenBlockRightClicked;
    }

    private void OnDestroy()
    {
        if (_visual == null) return;
        _visual.OnDragEnd -= WhenDragEnd;
        _visual.OnLeftClicked -= WhenBlockLeftClicked;
        _visual.OnRightClicked -= WhenBlockRightClicked;
    }

    public void SetVisualState(SlotType slotType, bool isRaised = false)
    {
        _visual.ChangeBlockVisual(slotType);
    }
    
    public void ChangeTargetInfo(Clickable target, object feature, int slotIndex, ISlot slot)
    {
        CurrentTarget = target;
        CurrentFeature = feature;
        CurrentSlotIndex = slotIndex;
        CurrentSlot = slot;
    }

    public void InitDefaultBlock(Clickable target, ISlot slot, int index = -1)
    {
        CurrentTarget = target;
        CurrentFeature = target?.GetComponent(RequiredFeatureType);
        CurrentSlotIndex = index;
        CurrentSlot = slot;

        if (CurrentFeature != null)
        {
            Activate(CurrentFeature);
        }
        
        _visual.ChangeBlockVisual(slot.GetSlotType());
    }
    
    private void WhenDragStarted()
    {
        OnBlockDragStarted?.Invoke(this, CurrentSlot);
    }
    
    private void WhenDragEnd(ISlot slot)
    {
        OnBlockDragEnd?.Invoke(this, slot);
    }
    
    private void WhenBlockLeftClicked()
    {
        OnBlockLeftClick?.Invoke(this);
    }

    private void WhenBlockRightClicked()
    {
        OnBlockRightClick?.Invoke(this);
    }

    public void ToggleEngineBlock()
    {
        if (_visual.IsRaised)
        {
            DropEngineBlock();
        }
        else
        {
            RaiseEngineBlock();
        }
    }

    public void RaiseEngineBlock()
    {
        _visual.RaiseVisual(true);
    }
    
    public void DropEngineBlock()
    {
        _visual.RaiseVisual(false);
    }

    public void SetInteraction(bool canClick, bool canHover, bool canDrag)
    {
        _visual.ActivateClickEvent(canClick);
        _visual.ActivateHoverEvent(canHover);
        _visual.ActivateDragEvent(canDrag);
    }
}
