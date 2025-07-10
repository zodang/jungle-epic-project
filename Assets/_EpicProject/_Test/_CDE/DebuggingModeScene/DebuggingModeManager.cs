using UnityEngine;

public class DebuggingModeManager : StageBaseManager
{
    [SerializeField] private EngineController engineController;
    [SerializeField] private Clickable _brokenBlock;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/DebuggingModeStage";
        base.Awake();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
    }
    
}
