using Define;
using UnityEngine;

public class SimpleSlot : BlockContainerBase
{
    [SerializeField] private BlockType blockType;
    [SerializeField] private Clickable targetClickable;

    private void Start()
    {
        InitBlock();
    }

    protected override void RemoveBlock(int index)
    {
        Debug.Log("@@DE : Remove Block");
    }

    private void InitBlock()
    {
        EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(blockType, transform);
        RegisterBlockEvents(block);
        block.InitDefaultBlock(targetClickable, SlotType.SimpleSlot);

        if (blockType == BlockType.Emotion)
        {
            // 감정 블록 시 클릭 이벤트 제거
            UnregisterClickEvents(block);
        }
    }
}
