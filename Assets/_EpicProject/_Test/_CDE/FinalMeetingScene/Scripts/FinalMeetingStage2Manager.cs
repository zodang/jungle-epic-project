using UnityEngine.Playables;
using UnityEngine;
using Define;

public class FinalMeetingStage2Manager : StageBaseManager
{
    private PlayableDirector _playableDirector;
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/FinalMeetingStage2";
        ChangeStageId("ending_3");
        
        base.Awake();
        _playableDirector = GetComponent<PlayableDirector>();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Ending);

        //PlayTimeline();
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");
    }

    private void PlayTimeline()
    {
        
        _playableDirector.Play();
    }

    public void LoadNextScene()
    {
        GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
    }
}
