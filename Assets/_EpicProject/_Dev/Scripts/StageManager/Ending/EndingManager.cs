using Define;
using UnityEngine;

public class EndingManager : StageBaseManager
{


    private void Awake()
    {
        stageFilePath = "StageInfos/Ending";
        ChangeStageId("60_ending");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.AudioManager.ContinueBgm(BgmType.Ending);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadMenuScene;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("60_0_enter_ending");

        if (!AchievementStatusManager._isHappyEndingAchievementUnlocked)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_STORY_ENDING_HAPPY");
            AchievementStatusManager._isHappyEndingAchievementUnlocked = true;
            Debug.Log("도전과제 '행복한 결말'이 완료되었습니다.");
        }
    }

    void LoadMenuScene()
    {
        // 로그 시스템
        ChangeStageSection("60_1_exit_ending");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadScene(1);
    }
}
