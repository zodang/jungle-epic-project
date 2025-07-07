using Define;
using UnityEngine;

public class TutorialStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/TutorialStage";
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
     
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Tutorial);
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
