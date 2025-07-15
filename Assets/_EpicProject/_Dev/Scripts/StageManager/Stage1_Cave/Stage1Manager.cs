using Define;
using UnityEngine;

public class Stage1Manager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    private LogSectionTrigger _logSectionTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/CaveStage";
        ChangeStageId("cave_1");
        
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);

        _logSectionTrigger = FindAnyObjectByType<LogSectionTrigger>();
        _logSectionTrigger.OnSectionTriggered += CheckPassCrack;
        
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage1);
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.FadeManager.LoadNextScene();
    }

    private void CheckPassCrack()
    {
        // 로그 시스템
        ChangeStageSection("pass_crack");
    }

    protected override void OnDestroy()
    {
        _logSectionTrigger.OnSectionTriggered -= CheckPassCrack;
        base.OnDestroy();
    }
}
