using Define;
using UnityEngine;

public class EntranceSceneManager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/EntranceStage";
        ChangeStageId("40_entrance_1");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage3);
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("40_0_enter_entrance");
    }
    
    private void OnGoalTriggered()
    {
        // 충돌 감지 제거
        foreach (var detectionRange in FindObjectsByType<DetectionRange>(FindObjectsSortMode.None))
        {
            Destroy(detectionRange.gameObject);  
        }
        
        // 로그 시스템
        ChangeStageSection("40_2_exit_entrance");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
