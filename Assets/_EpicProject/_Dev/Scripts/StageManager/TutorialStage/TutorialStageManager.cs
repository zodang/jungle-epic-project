using Define;
using UnityEngine;

public class TutorialStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/TutorialStage";
        ChangeStageId("Tutorial_1");
        
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
     
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Tutorial);
                
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageExit(StageId, "clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
