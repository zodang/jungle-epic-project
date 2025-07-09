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

    public event Action<EngineBlock, ISlotType> OnBlockDragEnd;
    public event Action<EngineBlock> OnBlockLeftClick;
    public event Action<EngineBlock> OnBlockRightClick;

    public Clickable PrevTarget { get; private set; }
    public object PrevFeature { get; private set; }
    public int PrevSlotIndex { get; private set; } = -1;
    public SlotType CurrentSlotType { get; private set; }

    public InventorySlot InventorySlot { get; private set; }
    public SimpleSlot SimpleSlot { get; private set; }

    protected virtual void Awake()
    {
        _visual = GetComponent<BlockVisual>();
        InventorySlot = FindAnyObjectByType<InventorySlot>();

        // SimpleSlot 사용하는 Block일 시 사용
        SimpleSlot = GetComponentInParent<SimpleSlot>();
    }
    
    private void Start()
    {
        if (_visual == null) return;
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
    
    public void ChangeTargetInfo(Clickable target, object feature, int slotIndex, SlotType slotType)
    {
        PrevTarget = target;
        PrevFeature = feature;
        PrevSlotIndex = slotIndex;
        CurrentSlotType = slotType;
    }

    public void InitDefaultBlock(Clickable target, SlotType type, int index = -1)
    {
        PrevTarget = target;
        PrevFeature = target.GetComponent(RequiredFeatureType);
        PrevSlotIndex = index;
        CurrentSlotType = type;
        
        if (PrevFeature != null)
        {
            Activate(PrevFeature);
        }
        
        _visual.ChangeBlockVisual(type);
    }
    
    private void WhenDragEnd(ISlotType slot)
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
            DeactivateEngineBlock();
        }
        else
        {
            ActivateEngineBlock();
        }
    }

    public void ActivateEngineBlock()
    {
        if (CurrentSlotType is SlotType.EngineSlot) return;
        
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
        _visual.RaiseVisual(true);
    }
    
    public void DeactivateEngineBlock()
    {
        if (CurrentSlotType is SlotType.EngineSlot) return;
        
        _visual.ChangeBlockVisual(SlotType.InventorySlot);
        _visual.RaiseVisual(false);
    }

    public void SetDrag(bool isAble)
    {
        _visual.EnableMouseInteraction(isAble);
    }
}
