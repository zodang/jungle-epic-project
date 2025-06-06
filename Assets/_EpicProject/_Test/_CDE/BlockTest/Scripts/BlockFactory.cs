using Define;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    [Header("Engine Block")]
    public ControlBlock ControlEngine;
    public ScaleBlock ScaleEngine;
    public RotateBlock RotateEngine;
    public LightAdustFeatureBlock LightEngine;
    
    [Header("Inventory Block")]
    public InventoryBlock ControlInventory;
    public InventoryBlock ScaleInventory;
    public InventoryBlock RotateInventory;
    public InventoryBlock LightInventory;

    public FeatureBlock CreateFeatureBlock(BlockType type, Transform parent = null)
    {
        FeatureBlock featureBlock = null;

        switch (type)
        {
            case BlockType.PlayerControl:
                featureBlock = Instantiate(ControlEngine, parent);
                break;
            case BlockType.Scale:
                featureBlock = Instantiate(ScaleEngine, parent);
                break;
            case BlockType.Rotate:
                featureBlock = Instantiate(RotateEngine, parent);
                break;
            case BlockType.Light:
                featureBlock = Instantiate(LightEngine, parent);
                break;
        }

        return featureBlock;
    }
    
    public InventoryBlock CreateBlockUI(BlockType type, Transform parent = null)
    {
        InventoryBlock blockUI = null;
        switch (type)
        {
            case BlockType.PlayerControl:
                blockUI = Instantiate(ControlInventory, parent);
                break;
            case BlockType.Scale:
                blockUI = Instantiate(ScaleInventory, parent);
                break;
            case BlockType.Rotate:
                blockUI = Instantiate(RotateInventory, parent);
                break;
            case BlockType.Light:
                blockUI = Instantiate(LightInventory, parent);
                break;
        }

        if (blockUI != null)
        {
            blockUI.Init(type);
        }

        return blockUI;
    }
}
