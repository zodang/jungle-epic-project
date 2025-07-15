using Define;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FinalMeetingStageManager : StageBaseManager
{
    [SerializeField] private Daughter _daughter;
    [SerializeField] private SpriteRenderer _daughterSpriteRenderer;
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

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage4_cutscene);
    }

    private void PlayTimeline(int index)
    {
        // 플레이어 Input 비활성화
        StageManager.Instance.InputManager.ActivatePlayerInput(false);
        _daughterSpriteRenderer.GetComponent<Collider2D>().enabled = false;
        
        _playableDirector.playableAsset = timelines[index]; 
        _playableDirector.Play();
    }
    
    private void WhenEmotionEnabled()
    {
        // 딸 엔진 창 비활성화
        _daughter.GetComponent<Clickable>().EngineController.DeactivateSilently();
        _daughterSpriteRenderer.enabled = false;    
        
        PlayTimeline(1);
    }

    public void HandleFirstTimelineEnd()
    {
        // 플레이어 Input 활성화
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _daughterSpriteRenderer.GetComponent<Collider2D>().enabled = true;
        
        // 블록 드래그 활성화
        _simpleSlot.AddEvents();
    }
    
    public void HandleSecondTimelineEnd()
    {
        // 플레이어 Input 활성화
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        
        // 씬 전환
        GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
    }
}
