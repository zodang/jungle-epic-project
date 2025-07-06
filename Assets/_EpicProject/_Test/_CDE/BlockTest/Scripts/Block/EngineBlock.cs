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
    private int _prevSlotIndex = -1;

    private BlockVisual _visual;
    private InventorySlot _inventorySlot;
    private SlotType _currentSlotType;

    protected virtual void Awake()
    {
        _visual = GetComponent<BlockVisual>();
        _inventorySlot = FindAnyObjectByType<InventorySlot>();
    }
    
    private void Start()
    {
        _visual.OnDragEnd += WhenDragEnd;
        _visual.OnLeftClicked += ToggleEngineBlock;
        _visual.OnRightClicked += DropToInventorySlot;
    }

    private void OnDestroy()
    {
        _visual.OnDragEnd -= WhenDragEnd;
        _visual.OnLeftClicked -= ToggleEngineBlock;
        _visual.OnRightClicked -= DropToInventorySlot;
    }
    
    public void InitDefaultBlock(Clickable target, SlotType type, int index = -1)
    {
        _prevTarget = target;
        _prevFeature = target.GetComponent(RequiredFeatureType);
        _prevSlotIndex = index;
        _currentSlotType = type;
        
        if (_prevFeature != null)
        {
            Activate(_prevFeature);
        }
        
        _visual.ChangeBlockVisual(type);
    }
    
    private void WhenDragEnd(ISlotType slot)
    {
        SlotType slotType = slot?.GetSlotType() ?? SlotType.None;
        
        switch (slotType)
        {
            case SlotType.InventorySlot:
                WhenDroppedInventorySlot(slot);
                break;
            case SlotType.EngineSlot:
                WhenDroppedEngineSlot(slot);
                break;
            case SlotType.None:
                WhenDroppedNone();
                break;
        }
    }

    private void ToggleEngineBlock()
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
        if (_currentSlotType is SlotType.EngineSlot) return;
        
        _visual.ChangeBlockVisual(SlotType.EngineSlot);
        _visual.RaiseVisual(true);
    }
    
    public void DeactivateEngineBlock()
    {
        if (_currentSlotType is SlotType.EngineSlot) return;
        
        _visual.ChangeBlockVisual(SlotType.InventorySlot);
        _visual.RaiseVisual(false);
    }

    public void DropToInventorySlot()
    {
        if (_currentSlotType is SlotType.InventorySlot) return;
        
        // 인벤토리로 블록 이동
        if (_inventorySlot == null) return;
        WhenDroppedInventorySlot(_inventorySlot);
        _visual.ChangeBlockVisual(SlotType.InventorySlot);
    }
    
    private void WhenDroppedInventorySlot(ISlotType slot)
    {
        InventorySlot inventorySlot = slot as InventorySlot;
        Clickable newTarget = slot.GetTargetClickable();
        
        // 이전 Target의 기능 비활성화
        if (_prevTarget != null && _prevFeature != null)
        {
            _prevTarget.RemoveBlock(_prevSlotIndex);
            Deactivate(_prevFeature);
        }
        
        object newFeature = newTarget.GetComponent(RequiredFeatureType);
        if (newFeature == null) return;

        // Inventory에 Block 추가
        Inventory inventory = inventorySlot.GetInventory(); 
        inventory.AddBlock(this);
        
        // 새로운 Target의 기능 활성화
        Activate(newFeature);
        inventorySlot.SetBlockPositionToInventory(this);

        _prevTarget = newTarget;
        _prevFeature = newFeature;
        _prevSlotIndex = -1;
        _currentSlotType = SlotType.InventorySlot;
    }
    
    private void WhenDroppedEngineSlot(ISlotType slot)
    {
        EngineSlot engineSlot = slot as EngineSlot;
        int targetIndex = engineSlot.Index;
        
        // Prev Target의 기능 비활성화
        if (_prevTarget != null && _prevFeature != null)
        {
            _prevTarget.RemoveBlock(_prevSlotIndex);
            Deactivate(_prevFeature);
        }
        
        // New Target 기능 활성화
        Clickable newTarget = slot.GetTargetClickable();
        object newFeature = newTarget.GetComponent(RequiredFeatureType);
        
        Activate(newFeature);
        newTarget.EngineController.EngineSlotList[targetIndex].SetBlock(this);
        
        var (movedBlock, movedBlockIndex) = newTarget.TryAddOrMoveOrReplaceBlock(targetIndex, this);

        // Target Index에 Block 있을 때
        if (movedBlock != null)
        {
            if (movedBlockIndex >= 0)
            {
                // 빈 슬롯으로 이동
                newTarget.EngineController.EngineSlotList[movedBlockIndex].SetBlock(movedBlock);
            }
            else
            {
                movedBlock.Deactivate(movedBlock.RequiredFeatureType);

                // 인벤토리로 이동
                var inventory = StageBaseManager.Instance.PlayerManager.Inventory;
                inventory.AddBlock(movedBlock);
                movedBlock.InitDefaultBlock(_inventorySlot.GetTargetClickable(), SlotType.InventorySlot, -1);
                movedBlock.transform.SetParent(_inventorySlot.transform, false);
                
            }
        }

        _prevTarget = newTarget;
        _prevFeature = newFeature;
        _prevSlotIndex = targetIndex;
        _currentSlotType = SlotType.EngineSlot;
    }
    
    private void WhenDroppedNone()
    {
        // 기존 부모로 이동
        if (_currentSlotType == SlotType.InventorySlot)
        {
            _visual.transform.SetParent(_inventorySlot.transform, false);
            _visual.transform.localPosition = Vector3.zero;
            _visual.ChangeBlockVisual(SlotType.InventorySlot);
        }
        else if (_currentSlotType == SlotType.EngineSlot && _prevTarget != null && _prevSlotIndex >= 0)
        {
            var engineSlot = _prevTarget.EngineController.EngineSlotList[_prevSlotIndex];
            engineSlot.SetBlock(this);
            _visual.ChangeBlockVisual(SlotType.EngineSlot);
        }
    }
}
