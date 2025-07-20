using Define;
using UnityEngine;

public class OpeningManager : StageBaseManager
{
    private void Awake()
    {
        stageFilePath = "StageInfos/Opening";
        ChangeStageId("00_opening");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Menu);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadNextScene;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("00_0_enter_opening");
    }

    void LoadNextScene()
    {
        // 로그 시스템
        ChangeStageSection("00_1_exit_opening");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
