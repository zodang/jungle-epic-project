using Define;
using UnityEngine;

public class EndingManager : StageBaseManager
{
    private void Awake()
    {
        stageFilePath = "StageInfos/Ending";
        ChangeStageId("ending_4");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.AudioManager.ContinueBgm(BgmType.Ending);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadMenuScene;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");
    }

    void LoadMenuScene()
    {
        // 로그 시스템
        ChangeStageSection("stage_exit");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadScene(1);
    }
}
