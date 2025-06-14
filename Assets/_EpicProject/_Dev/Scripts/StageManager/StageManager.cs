using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public InputManager InputManager { get; private set; }
    public BlockFactory BlockFactory { get; private set; }
    public FlagManager FlagManager { get; private set; }
    public GameObject Player { get; private set; }

    public override void Awake()
    {
        base.Awake();

        InputManager = GetComponentInChildren<InputManager>();
        BlockFactory = GetComponentInChildren<BlockFactory>();
        FlagManager = GetComponentInChildren<FlagManager>();
    }
}
