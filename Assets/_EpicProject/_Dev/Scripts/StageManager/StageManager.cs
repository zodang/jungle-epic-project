using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public InputManager InputManager { get; private set; }
    public BlockFactory BlockFactory { get; private set; }
    public FlagManager FlagManager { get; private set; }
    
    private StageData _currentStage = new StageData();

    private void Awake()
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

    private void Start()
    {
        // 저장된 스테이지 정보 불러오기
        _currentStage = GameManager.Instance.SaveManager.LoadStageData();
    }

    public void SaveSceneIndex(int index)
    {
        // 변경된 스테이지 정보 저장
        _currentStage.ClearStageIndex = index;
        GameManager.Instance.SaveManager.SaveStageData(_currentStage);
    }
}
