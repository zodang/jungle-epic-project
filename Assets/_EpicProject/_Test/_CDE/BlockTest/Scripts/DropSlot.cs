using UnityEngine;

public class DropSlot : MonoBehaviour
{
    private Block _currentBlock;

    public bool CanDrop()
    {
        return _currentBlock == null;
    }
    
    public void OnBlockDrop(Block block)
    {
        _currentBlock = block;
    }

    public void OnBlockRemoved()
    {
        _currentBlock = null;
    }
}
