using Define;
using System.Collections;
using UnityEngine;

public class TutorialStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    private BlockSystemHelper _blockSystemHelper;
    private EngineBlock[] _blocks;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/TutorialStage";
        ChangeStageId("10_tutorial_1");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
     
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Tutorial);
                
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("10_0_enter_tutorial");
        
        // Block System Helper 세팅
        StartCoroutine(SetBlockSystemHelper());
    }

    private IEnumerator SetBlockSystemHelper()
    {
        yield return new WaitForSeconds(1.0f);
        
        _blockSystemHelper = FindAnyObjectByType<BlockSystemHelper>();
        _blocks = FindObjectsByType<EngineBlock>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var block in _blocks)
        {
            block.OnBlockDragStarted += _blockSystemHelper.StartBlinking;
            block.OnBlockDragEnd += _blockSystemHelper.StopBlinking;
        }
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        ChangeStageSection("10_3_exit_tutorial");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
