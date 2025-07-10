using Define;
using System.Collections.Generic;
using UnityEngine;

public class ToolBoxSlot : BlockContainerBase, ISlot
{
    [SerializeField] private List<BlockType> defaultBlockList;

    private void Start()
    {
        for (int i = 0; i < defaultBlockList.Count; i++)
        {
            BlockType type = defaultBlockList[i];

            EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(type, transform);
            RegisterBlockEvents(block);
            
            block.InitDefaultBlock(null, this);
        }
    }
    

    public SlotType GetSlotType()
    {
        return SlotType.ToolBoxSlot;
    }

    public Clickable GetTargetClickable()
    {
        /*사용하지 않음*/
        return null;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetBlockPosition(EngineBlock block)
    {
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.ToolBoxSlot);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        /*사용하지 않음*/
    }
    
    protected override void RemoveBlock(int index)
    {
        /*사용하지 않음*/
    }
}
