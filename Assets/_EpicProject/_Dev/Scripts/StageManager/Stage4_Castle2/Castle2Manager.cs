using Define;
using UnityEngine;

public class Castle2Manager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/Castle2Stage";
        ChangeStageId("51_castle_2");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage4);
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("51_0_enter_castle2");
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        ChangeStageSection("51_3_exit_castle2");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
