using Define;
using UnityEngine;
using UnityEngine.Playables;

public class DebuggingModeManager : StageBaseManager
{
    [SerializeField] private Clickable _brokenBlock;
    private ClickableOutline _brokenBlockOutline;
    
    private PlayableDirector _playableDirector;
    private PuzzleFSM _puzzleFSM;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/DebuggingModeStage";
        ChangeStageId("ending_2");
        
        base.Awake();
        
        _playableDirector = GetComponent<PlayableDirector>();
        _puzzleFSM = GetComponent<PuzzleFSM>();
        _brokenBlockOutline = _brokenBlock.GetComponentInChildren<ClickableOutline>();
    }

    private void Start()
    {
        _brokenBlock.OnClickAction += WhenBlockClicked;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage4_cutscene);
    }

    protected override void OnDestroy()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        
        _brokenBlock.OnClickAction -= WhenBlockClicked;
        base.Awake();
    }

    private void WhenBlockClicked()
    {
        // 플레이어 Input 비활성화
        StageManager.Instance.InputManager.ActivatePlayerInput(false);
        
        // 감정블록 아웃라인 비활성화
        _brokenBlockOutline.SetOutline(false);
        _brokenBlockOutline.enabled = false;
        
        // 감정블록 sorting layer 변경
        _brokenBlock.GetComponent<BrokenEmotionBlock>().ChangeBlockRender();
        
        _playableDirector.Play();

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage4_debug);
    }

    public void WhenTimelineEnd()
    {
        _puzzleFSM.StartPuzzle();
    }
    
}
