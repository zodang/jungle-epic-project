using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CDEEndingSceneManager : StageBaseManager
{
    private CDEEndingStageUI _stageUI;
    private Daughter _daughter;
    
    private PlayableDirector _playableDirector;
    [SerializeField] TimelineAsset[] timelines;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/VSStage";
        base.Awake();

        _stageUI = FindAnyObjectByType<CDEEndingStageUI>();
        _daughter = FindAnyObjectByType<Daughter>();

        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        // 감정 블록 장착 시 타임라인 재생
        _daughter.OnEmotionEnabled += ChangeTimeline;

        PlayTimeline(0);
    }

    private void ChangeTimeline()
    {
        PlayTimeline(1);
    }

    private void PlayTimeline(int index)
    {
        _playableDirector.playableAsset = timelines[index]; 
        _playableDirector.Play();
    }
}
