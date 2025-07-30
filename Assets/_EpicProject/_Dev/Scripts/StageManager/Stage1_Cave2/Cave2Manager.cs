using Define;
using UnityEngine;

public class Cave2Manager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    private LogSectionTrigger _logSectionTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/Cave2Stage";
        ChangeStageId("21_cave_2");

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage1);

        // 로그 시스템
        _logSectionTrigger = FindAnyObjectByType<LogSectionTrigger>();
        _logSectionTrigger.OnSectionTriggered += CheckEndDialogue;
        
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("21_0_enter_cave2");
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        ChangeStageSection("21_2_exit_cave2");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId, "clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }

    private void CheckEndDialogue()
    {
        // 로그 시스템
        ChangeStageSection("21_1_end_dialogue");
    }

    protected override void OnDestroy()
    {
        _logSectionTrigger.OnSectionTriggered -= CheckEndDialogue;
        base.OnDestroy();
    }
}
