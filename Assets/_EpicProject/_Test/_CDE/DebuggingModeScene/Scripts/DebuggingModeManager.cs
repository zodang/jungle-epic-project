using Define;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DebuggingModeManager : StageBaseManager
{
    [SerializeField] private Clickable _brokenBlock;
    private ClickableOutline _brokenBlockOutline;
    
    private PlayableDirector _playableDirector;
    private PuzzleFSM _puzzleFSM;

    
    [Header("제어할 대상")]
    [SerializeField] private List<NPCInteraction> _npcsToBlock = new List<NPCInteraction>();

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/DebuggingModeStage";
        ChangeStageId("53_debug");
        
        base.Awake();
        
        _playableDirector = GetComponent<PlayableDirector>();
        _puzzleFSM = GetComponent<PuzzleFSM>();
        _brokenBlockOutline = _brokenBlock.GetComponentInChildren<ClickableOutline>();
    }

    protected override void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        base.Start();

        _brokenBlock.OnClickAction += WhenBlockClicked;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("53_0_enter_debug");

        GameManager.Instance.AudioManager.ContinueBgm(BgmType.Stage4_cutscene);
    }

    protected override void OnDestroy()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        
        _brokenBlock.OnClickAction -= WhenBlockClicked;
        base.Awake();
    }

    private void WhenBlockClicked()
    {   
        // 대화 종료
        StageBaseManager.Instance.DialogueManager.FinalizeDialogue();
        if (_npcsToBlock != null && _npcsToBlock.Count > 0)
        {
            foreach (NPCInteraction npc in _npcsToBlock)
            {
                if (npc != null)
                {
                    npc.CanProcessInput = false;
                }
            }
        }

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
