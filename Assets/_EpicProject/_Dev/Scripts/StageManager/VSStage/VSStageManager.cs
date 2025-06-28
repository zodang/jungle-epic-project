public class VSStageManager : StageBaseManager
{
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/VSStage";
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        GameManager.Instance.AudioManager.PlayBgm(true);
        ObjectPropertyController foundAxe = FindAnyObjectByType<ObjectPropertyController>();
        if (foundAxe != null)
        {
            foundAxe.Submerge();
        }
    }
}
