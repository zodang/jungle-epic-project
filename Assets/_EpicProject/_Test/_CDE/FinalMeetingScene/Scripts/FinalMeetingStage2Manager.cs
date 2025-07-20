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
        ChangeStageId("54_meeting_2");
        
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
        ChangeStageSection("54_0_enter_meeting2");
    }

    private void PlayTimeline()
    {
        
        _playableDirector.Play();
    }

    public void LoadNextScene()
    {
        // 로그 시스템
        ChangeStageSection("54_1_exit_meeting2");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);
        
        GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
    }
}
