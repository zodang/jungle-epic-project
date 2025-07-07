using Define;
using UnityEngine;

public class EntranceSceneManager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/EntranceStage";
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage3);
    }
    
    private void OnGoalTriggered()
    {
        // GameManager.Instance.FadeManager.LoadNextScene();
    }
}
