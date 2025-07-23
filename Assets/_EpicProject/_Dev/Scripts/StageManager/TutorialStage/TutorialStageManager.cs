using Define;
using UnityEngine;

public class TutorialStageManager : StageBaseManager
{   
    

    private TriggerArea _goalTrigger;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/TutorialStage";
        ChangeStageId("10_tutorial_1");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
     
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Tutorial);
                
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("10_0_enter_tutorial");
    }

    private void OnGoalTriggered()
    {
        // 로그 시스템
        ChangeStageSection("10_3_exit_tutorial");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
        if(!AchievementStatusManager._isStartAchievementUnlocked)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_STORY_START");
            AchievementStatusManager._isStartAchievementUnlocked = true;
            Debug.Log("도전과제 '게임 시작'이 완료되었습니다.");
        }
        
    }
}
