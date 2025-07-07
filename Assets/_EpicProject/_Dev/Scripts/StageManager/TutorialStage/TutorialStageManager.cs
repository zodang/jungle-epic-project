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
        
        GameManager.Instance.AudioManager.PlayBgm(true);
     
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        Debug.Log(_goalTrigger.name);
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
