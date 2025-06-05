using Define;
using UnityEngine;

public class BlockUI : DraggableBlock
{
    public BlockType Type { get; private set; }

    public void Init(BlockType type)
    {
        Type = type;
    }
}