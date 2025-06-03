using UnityEngine;

public class Slot : MonoBehaviour
{
    private Block _currentBlock;

    private void Start()
    {
        // 초기 블록 체크
        Block block = GetChildBlock();
        if (block != null)
        {
            OnBlockDrop(block);
        }
    }

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

    private Block GetChildBlock()
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>(); 
            if (block != null)
                return block;
        }
        return null;
    }
}
