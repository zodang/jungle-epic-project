using Define;
using UnityEngine;

public class Stage1Manager : StageBaseManager
{
    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        stageFilePath = "StageInfos/CaveStage";
        base.Awake();

        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadScene();
    }
}
