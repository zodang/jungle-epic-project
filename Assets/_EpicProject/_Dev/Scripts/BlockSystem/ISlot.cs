using Define;
using System;
using UnityEngine;

public interface ISlot
{
    event Action<EngineBlock> OnBlockSet;
    
    int GetSlotIndex();
    SlotType GetSlotType();
    Clickable GetTargetClickable();
    Transform GetTransform();
    BlockContainerBase GetBlockContainerBase();

    void SetCurrentBlock(EngineBlock block);
    void SetTargetClickable(Clickable clickable);
    void SetBlockContainerBase(BlockContainerBase blockContainer);
}
