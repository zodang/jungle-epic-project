using Define;
using UnityEngine;

public class VSStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/VSStage";
        base.Awake();

        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage1);
        ObjectPropertyController foundAxe = FindAnyObjectByType<ObjectPropertyController>();
        if (foundAxe != null)
        {
            foundAxe.Submerge();
        }
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadScene();
    }
}
