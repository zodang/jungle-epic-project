using Define;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    [Header("Feature Block")]
    public ControlBlock ControlBlockPrefab;
    public ScaleBlock ScaleBlockPrefab;
    public RotateBlock RotateBlockPrefab;
    public LightAdustFeatureBlock LightBLockPrefab;
    
    [Header("Block UI")]
    public BlockUI ControlUI;
    public BlockUI ScaleUI;
    public BlockUI RotateUI;
    public BlockUI LightUI;

    public FeatureBlock CreateFeatureBlock(BlockType type, Transform parent = null)
    {
        FeatureBlock featureBlock = null;

        switch (type)
        {
            case BlockType.PlayerControl:
                featureBlock = Instantiate(ControlBlockPrefab, parent);
                break;
            case BlockType.Scale:
                featureBlock = Instantiate(ScaleBlockPrefab, parent);
                break;
            case BlockType.Rotate:
                featureBlock = Instantiate(RotateBlockPrefab, parent);
                break;
            case BlockType.Light:
                featureBlock = Instantiate(LightBLockPrefab, parent);
                break;
        }

        return featureBlock;
    }
    
    public BlockUI CreateBlockUI(BlockType type, Transform parent = null)
    {
        BlockUI blockUI = null;
        switch (type)
        {
            case BlockType.PlayerControl:
                blockUI = Instantiate(ControlUI, parent);
                break;
            case BlockType.Scale:
                blockUI = Instantiate(ScaleUI, parent);
                break;
            case BlockType.Rotate:
                blockUI = Instantiate(RotateUI, parent);
                break;
            case BlockType.Light:
                blockUI = Instantiate(LightUI, parent);
                break;
        }

        if (blockUI != null)
        {
            blockUI.Init(type);
        }

        return blockUI;
    }
}
