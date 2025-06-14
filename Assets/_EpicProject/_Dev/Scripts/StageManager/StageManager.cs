using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public InputManager InputManager { get; private set; }
    public BlockFactory BlockFactory { get; private set; }
    public FlagManager FlagManager { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputManager = GetComponentInChildren<InputManager>();
        BlockFactory = GetComponentInChildren<BlockFactory>();
        FlagManager = GetComponentInChildren<FlagManager>();
    }
}
