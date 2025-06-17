using Define;

public class EngineSlot : Slot
{
    public int SlotIndex { get; private set; }
    public EngineBlock CurrentBlock { get; private set; }

    private EngineController _engineController;

    public BlockType? CurrentBlockType
    {
        get
        {
            // 현재 블록이 없다면 null 반환
            if (CurrentBlock == null) return null;
            return CurrentBlock.Type;
        }
    }

    private void Awake()
    {
        _engineController = GetComponentInParent<EngineController>();
    }

    public void SetSlotIndex(int index)
    {
        SlotIndex = index;
    }
    
    public void SetBlock(EngineBlock block)
    {
        CurrentBlock = block;
        block.SetOwner(_engineController.CurrentTarget);
    }

    public void ClearBlock()
    {
        if (CurrentBlock != null)
        {
            Destroy(CurrentBlock.gameObject);
            CurrentBlock = null;
        }
    }

    public override void OnBlockDrop(DraggableBlock draggableBlock, Slot endSlot)
    {
        if (draggableBlock is InventoryBlock inventoryBlock)
        {
            DropInventoryToEngine(inventoryBlock);
            return;
        }
        
        if (draggableBlock is EngineBlock engineBlock)
        {
            DropEngineToEngine(engineBlock, endSlot);
            return;
        }
        
        base.OnBlockDrop(draggableBlock, endSlot);
    }

    private void DropInventoryToEngine(InventoryBlock inventoryBlock)
    {
        // 인벤토리 → 엔진
        var clickable = _engineController.CurrentTarget;
        if (clickable == null) return;

        // Engine Block 생성
        var block = StageManager.Instance.BlockFactory.CreateFeatureBlock(inventoryBlock.Type, transform);
        
        // Slot에 Block 위치 조정
        base.OnBlockDrop(block, this);
        
        // 기능 활성화
        var feature = clickable.GetComponent(block.RequiredFeatureType);
        block.Activate(feature);
        
        // Slot에 등록
        SetBlock(block);

        // Clickable에 Block 추가 후 UI 삭제
        clickable.AddBlockToClickable(inventoryBlock.Type, SlotIndex);
        
        Destroy(inventoryBlock.gameObject);
    }
    
    private void DropEngineToEngine(EngineBlock engineBlock, Slot endSlot)
    {
        if (engineBlock.PrevSlot == null)
        {
            // 기본 블록
            base.OnBlockDrop(engineBlock, endSlot);
            return;
        }
            
        var prevController = engineBlock.PrevSlot.GetComponentInParent<EngineController>();
        var prevClickable = prevController?.CurrentTarget;
        var thisClickable = _engineController.CurrentTarget;
        
        if (prevClickable == thisClickable)
        {
            // 엔진 → 같은 엔진
            base.OnBlockDrop(engineBlock, endSlot);
            return;
        }

        // 기존 Block 기능 비활성화 및 Block 삭제
        engineBlock.PrevSlot.OnBlockRemoved();
        prevClickable.RemoveBlockFromClickable(engineBlock.Type);
        engineBlock.Deactivate(engineBlock);
        Destroy(engineBlock.gameObject);

        // 새로운 타겟의 기능 활성화 및 Block 생성
        var newBlock = StageManager.Instance.BlockFactory.CreateFeatureBlock(engineBlock.Type, transform);
        var feature = thisClickable.GetComponent(newBlock.RequiredFeatureType);
        newBlock.Activate(feature);

        // 슬롯에 등록
        SetBlock(newBlock);

        // 클릭 가능한 대상에 블록 기록
        thisClickable.AddBlockToClickable(engineBlock.Type, SlotIndex);
    }
}
