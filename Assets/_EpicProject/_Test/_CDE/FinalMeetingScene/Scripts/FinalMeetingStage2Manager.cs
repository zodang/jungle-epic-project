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
        base.Awake();
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Ending);

        //PlayTimeline();
    }

    private void PlayTimeline()
    {
        
        _playableDirector.Play();
    }
}
