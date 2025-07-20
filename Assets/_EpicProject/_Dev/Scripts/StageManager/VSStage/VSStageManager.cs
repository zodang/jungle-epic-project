using Define;
using UnityEngine;

public class VSStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/VSStage";
        ChangeStageId("30_cliff_1");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        ObjectPropertyController foundAxe = FindAnyObjectByType<ObjectPropertyController>();
        if (foundAxe != null)
        {
            foundAxe.Submerge();
        }
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage2);
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("30_0_enter_cliff");
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        ChangeStageSection("30_2_exit_cliff");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
