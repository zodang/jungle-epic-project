using UnityEngine;
using UnityEngine.Playables;

public class DebuggingModeManager : StageBaseManager
{
    [SerializeField] private Clickable _brokenBlock;
    
    private PlayableDirector _playableDirector;
    private PuzzleFSM _puzzleFSM;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/DebuggingModeStage";
        base.Awake();
        
        _playableDirector = GetComponent<PlayableDirector>();
        _puzzleFSM = GetComponent<PuzzleFSM>();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        _brokenBlock.OnClickAction += WhenBlockClicked;
    }

    protected override void OnDestroy()
    {
        _brokenBlock.OnClickAction -= WhenBlockClicked;
        base.Awake();
    }

    private void WhenBlockClicked()
    {
        // 엔진창 비활성화 기능 제거
        FindAnyObjectByType<EngineManager>().DisableEngineDeactivate();
        _brokenBlock.EngineController.DisableEngineDeactivate();
        
        // 오브젝트 클릭 이벤트 제거
        _brokenBlock.OnClickAction -= WhenBlockClicked;
        
        _playableDirector.Play();
    }

    public void WhenTimelineEnd()
    {
        _puzzleFSM.StartPuzzle();
    }
    
}
