using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FinalMeetingStageManager : StageBaseManager
{
    // [Header("컷신에 등장하는 배우 및 중요 오브젝트 목록")] // 헤더를 추가하면 에디터에서 보기 편합니다.
    [SerializeField] private Daughter _daughter;
    [SerializeField] private SimpleSlot _simpleSlot;

    private PlayableDirector _playableDirector;
    [SerializeField] TimelineAsset[] timelines;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/FinalMeetingStage";
        base.Awake();

        
        
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        // 감정 블록 장착 시 타임라인 재생
        _daughter.OnEmotionEnabled += WhenEmotionEnabled;
        
        PlayTimeline(0);
    }

    private void PlayTimeline(int index)
    {
        // 딸 클릭 비활성화
        _daughter.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
        
        _playableDirector.playableAsset = timelines[index]; 
        _playableDirector.Play();
    }
    
    private void WhenEmotionEnabled()
    {
        // 딸 엔진 창 비활성화
        _daughter.GetComponent<Clickable>().EngineController.DeactivateSilently();
        PlayTimeline(1);
    }

    public void HandleFirstTimelineEnd()
    {
        // 딸 클릭 활성화
        _daughter.GetComponentInChildren<CapsuleCollider2D>().enabled = true;
        
        // 블록 드래그 활성화
        _simpleSlot.AddEvents();
    }
    
    public void HandleSecondTimelineEnd()
    {
        Debug.Log("@@DE ---> 디버그 모드 씬 전환");
    }
}
