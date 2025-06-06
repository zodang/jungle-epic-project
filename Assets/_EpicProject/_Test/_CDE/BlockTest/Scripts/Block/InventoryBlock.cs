using Define;

public class InventoryBlock : DraggableBlock
{
    public BlockType Type { get; private set; }

    public void Init(BlockType type)
    {
        Type = type;
    }
}