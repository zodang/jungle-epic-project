using Unity.VisualScripting;

public class CaveTestSceneManager : StageBaseManager
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

        //플레이어 무게 감지 추가
        PlayerManager.transform.AddComponent<ScaleChecker>();

    }

    protected override void OnDestroy()
    {
        base.Awake();
        // 플레이어 무게 감지 제거
        Destroy(PlayerManager.transform.AddComponent<ScaleChecker>());
    }
    
}
