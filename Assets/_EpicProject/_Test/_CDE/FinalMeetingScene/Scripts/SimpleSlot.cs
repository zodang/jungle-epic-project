using Define;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSlot : BlockContainerBase
{
    [SerializeField] private BlockType blockType;
    [SerializeField] private Clickable targetClickable;

    private EngineBlock _block;

    private void Start()
    {
        InitBlock();
        Invoke(nameof(AddEvents), 10f);
    }

    protected override void RemoveBlock(int index)
    {
        Debug.Log("@@DE : Remove Block");
    }

    private void InitBlock()
    {
        _block = StageManager.Instance.BlockFactory.CreateBlock(blockType, transform);
        RegisterBlockEvents(_block);
        _block.InitDefaultBlock(targetClickable, SlotType.SimpleSlot);
        
        // 블록 상호작용 비활성화
        _block.SetDrag(false);
    }

    private void AddEvents()
    {
        // 블록 이벤트 추가
        UnregisterClickEvents(_block);
        
        // 블록 상호작용 활성화
        _block.SetDrag(true);
        _block.AddComponent<BlockCanvasConverter>();
    }
}
