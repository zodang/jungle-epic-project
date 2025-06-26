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
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        
    }

    private void Start()
    {
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        GameManager.Instance.AudioManager.PlayBgm(true);
        Invoke(nameof(CollectBlock), .05f);
    }

    private void CollectBlock()
    {
        FindAnyObjectByType<Inventory>().Collect(BlockType.PlayerControl);
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadScene();
    }
}
