using Define;
using UnityEngine;

public class OpeningManager : StageBaseManager
{
    private void Awake()
    {
        stageFilePath = "StageInfos/Opening";
        ChangeStageId("opening_1");
        
        base.Awake();
    }

    void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Menu);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadNextScene;
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");
    }

    void LoadNextScene()
    {
        // 로그 시스템
        ChangeStageSection("stage_exit");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
