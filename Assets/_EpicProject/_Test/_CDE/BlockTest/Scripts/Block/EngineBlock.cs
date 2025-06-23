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
    
    
    private Clickable _prevTarget;
    private object _prevFeature;

    private BlockVisual _visual;

    protected virtual void Awake()
    {
        _visual = GetComponent<BlockVisual>();
    }
    
    private void Start()
    {
        _visual.OnDragEnd += WhenDragEnd;
    }

    private void OnDestroy()
    {
        _visual.OnDragEnd -= WhenDragEnd;
    }
    
    public void InitDefaultBlock(Clickable target)
    {
        _prevTarget = target;
        _prevFeature = target.GetComponent(RequiredFeatureType);

        if (_prevFeature != null)
        {
            Activate(_prevFeature);
        }
        
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
    }
    
    public void ShowBlockVisual(bool isActive)
    {
        _visual.ShowBlockVisual(isActive);
    }
    
    private void WhenDragEnd(ISlotType slot)
    {
        SlotType slotType = slot.GetSlotType();
        
        switch (slotType)
        {
            case SlotType.InventorySlot:
                WhenDroppedInventorySlot(slot);
                break;
            case SlotType.EngineSlot:
                WhenDroppedEngineSlot(slot);
                break;
            case SlotType.NumpadSlot:
                WhenDroppedNumpadSlot(slot);
                break;
            case SlotType.Numpad:
                WhenDroppedNumpad(slot);
                break;
            case SlotType.None:
                WhenDroppedNone();
                break;
        }
    }
    
    private void WhenDroppedInventorySlot(ISlotType slot)
    {
        InventorySlot inventorySlot = slot as InventorySlot;
        
        if (_prevTarget != null && _prevFeature != null)
        {
            // 기능 비활성화
            _prevTarget.RemoveBlock();
            Deactivate(_prevFeature);
        }

        Inventory inventory = inventorySlot.GetInventory(); 
        inventory.AddBlock(this);
        
        inventorySlot.SetBlockPositionToInventory(this);


        _prevTarget = null;
        _prevFeature = null;
    }
    
    private void WhenDroppedEngineSlot(ISlotType slot)
    {
        Clickable newTarget = slot.GetTargetClickable();
        
        // 이전 Target의 기능 비활성화
        if (_prevTarget != null && _prevFeature != null)
        {
            _prevTarget.RemoveBlock();
            Deactivate(_prevFeature);
        }
        
        object newFeature = newTarget.GetComponent(RequiredFeatureType);
        if (newFeature == null) return;

        // Target에 Block 추가
        var (canAdd, index) = newTarget.TryAddBlock(this);
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
        
        // Block 추가 실패 시 복귀
        if (!canAdd)
        {
            ReturnToPrevious();
            return;
        }
        
        // 새로운 Target의 기능 활성화
        Activate(newFeature);

        _prevTarget = newTarget;
        _prevFeature = newFeature;
        
        newTarget.EngineController.ShowBlock(index);
    }
    
    private void WhenDroppedNumpadSlot(ISlotType slot)
    {
        Clickable newTarget = slot.GetTargetClickable();

        // 이전 Target의 기능 비활성화
        if (_prevTarget != null && _prevFeature != null)
        {
            _prevTarget.RemoveBlock();
            Deactivate(_prevFeature);
        }

        object newFeature = newTarget.GetComponent(RequiredFeatureType);
        if (newFeature == null) return;

        // Target에 Block 추가
        var (canAdd, index) = newTarget.TryAddBlock(this);
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
        
        // Block 추가 실패 시 복귀
        if (!canAdd)
        {
            ReturnToPrevious();
            return;
        }
        
        // 새로운 Target의 기능 활성화
        Activate(newFeature);

        _prevTarget = newTarget;
        _prevFeature = newFeature;
        
        newTarget.EngineController.ShowBlock(index);
    }
    
    private void WhenDroppedNumpad(ISlotType slot)
    {
        if (slot is not Numpad numpad) return;
        
        Clickable newTarget = slot.GetTargetClickable();
        
        // 이전 Target의 기능 비활성화
        if (_prevTarget != null && _prevFeature != null)
        {
            _prevTarget.RemoveBlock();
            Deactivate(_prevFeature);
        }
        object newFeature = newTarget.GetComponent(RequiredFeatureType);
        if (newFeature == null) return;
        
        // Target에 Block 추가
        var (canAdd, index) = newTarget.TryAddBlock(numpad.Index, this);
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
        
        // Block 추가 실패 시 복귀
        if (!canAdd)
        {
            ReturnToPrevious();
            return;
        }
        
        // 새로운 Target의 기능 활성화
        Activate(newFeature);

        _prevTarget = newTarget;
        _prevFeature = newFeature;
        
        newTarget.EngineController.ShowBlock(index);
    }
    
    private void WhenDroppedNone()
    {
        Debug.LogWarning("Dropped None");
    }
    
    private void ReturnToPrevious()
    {
        Debug.LogWarning("Return To Previous");
    }
}
