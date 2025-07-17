using Define;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI; // UI 기능을 사용하기 위해 추가

// 스킵 시 보정할 대사 정보를 담는 새로운 클래스
[System.Serializable]
public class DialogueSkipInfo
{
    [Tooltip("대사를 말하는 NPC의 TimelineDialogue 스크립트를 연결하세요.")]
    public TimelineDialogue speakerController;
    [Tooltip("해당 타임라인에서 이 NPC가 말하는 대사의 총 횟수입니다.")]
    public int dialogueCountInTimeline;
}

public class FinalMeetingStageManager : StageBaseManager
{
    // --- 기존 변수들 ---
    [SerializeField] private Daughter _daughter;
    [SerializeField] private SpriteRenderer _daughterSpriteRenderer;
    [SerializeField] private SimpleSlot _simpleSlot;
    [SerializeField] private TimelineAsset[] timelines;
    private PlayableDirector _playableDirector;
    private int _currentTimelineIndex = -1;

    // --- 스킵 기능 변수들 ---
    [Header("스킵 기능 설정")]
    [SerializeField] private KeyCode _skipKey = KeyCode.Escape;
    [SerializeField] private float _timeToSkip = 1.5f;

    [Header("스킵 안내 UI (선택 사항)")]
    [Tooltip("스킵 안내 UI의 부모 오브젝트입니다.")]
    [SerializeField] private GameObject _skipPromptUI;
    [Tooltip("진행 상태를 보여줄 채워지는(Filled) 이미지입니다.")]
    [SerializeField] private Image _skipProgressImage;

    [Header("타임라인별 대사 정보")]
    [Tooltip("첫 번째 타임라인의 대사 정보 목록입니다.")]
    [SerializeField] private DialogueSkipInfo[] _dialoguesInFirstTimeline;
    [Tooltip("두 번째 타임라인의 대사 정보 목록입니다.")]
    [SerializeField] private DialogueSkipInfo[] _dialoguesInSecondTimeline;

    private float _skipTimer = 0f;
    private bool _isSkipped = false;

    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/FinalMeetingStage";
        ChangeStageId("ending_1");

        base.Awake();

        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");

        // 감정 블록 장착 시 타임라인 재생
        _daughter.OnEmotionEnabled += WhenEmotionEnabled;

        // UI가 연결되어 있다면, 시작 시 숨깁니다.
        if (_skipPromptUI != null)
            _skipPromptUI.SetActive(false);

        PlayTimeline(0);

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage4_cutscene);

        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("stage_enter");
    }

    private void Update()
    {
        if (_playableDirector.state != PlayState.Playing || _isSkipped)
        {
            // 타임라인이 끝나면 UI를 확실히 숨깁니다.
            if (_skipPromptUI != null && _skipPromptUI.activeInHierarchy)
                _skipPromptUI.SetActive(false);
            return;
        }

        if (Input.GetKey(_skipKey))
        {
            _skipTimer += Time.unscaledDeltaTime;

            // UI 보이기 및 진행 상태 업데이트
            if (_skipPromptUI != null && !_skipPromptUI.activeInHierarchy)
                _skipPromptUI.SetActive(true);

            if (_skipProgressImage != null)
                _skipProgressImage.fillAmount = _skipTimer / _timeToSkip;

            if (_skipTimer >= _timeToSkip)
            {
                SkipCurrentTimeline();
            }
        }

        if (Input.GetKeyUp(_skipKey))
        {
            _skipTimer = 0f;

            // UI 초기화 및 숨기기
            if (_skipProgressImage != null)
                _skipProgressImage.fillAmount = 0f;

            if (_skipPromptUI != null)
                _skipPromptUI.SetActive(false);
        }
    }

    private void PlayTimeline(int index)
    {
        // 플레이어 Input 비활성화
        StageManager.Instance.InputManager.ActivatePlayerInput(false);
        _daughterSpriteRenderer.GetComponent<Collider2D>().enabled = false;

        _currentTimelineIndex = index;
        _playableDirector.playableAsset = timelines[index];
        // 새로운 타임라인을 재생하기 전에, 재생 시간을 반드시 0으로 초기화합니다.
        _playableDirector.time = 0;

        _playableDirector.Play();

        // 새로운 타임라인이 시작될 때마다 스킵 관련 변수들을 초기화
        _skipTimer = 0f;
        _isSkipped = false;
    }

    private void WhenEmotionEnabled()
    {
        // 딸 엔진 창 비활성화
        _daughter.GetComponent<Clickable>().EngineController.DeactivateSilently();
        _daughterSpriteRenderer.enabled = false;

        // 로그 시스템
        ChangeStageSection("add_emotion");

        PlayTimeline(1); // 두 번째 타임라인 재생
    }

    private void SkipCurrentTimeline()
    {
        if (_playableDirector.state != PlayState.Playing || _isSkipped) return;

        _isSkipped = true;
        Debug.Log($"타임라인 {_currentTimelineIndex} 스킵! 최종 상태로 즉시 적용합니다.");

        // [논리적 상태 보정] - 현재 타임라인에 맞는 대사 정보를 가져와 인덱스를 보정합니다.
        if (_currentTimelineIndex == 0)
        {
            foreach (var info in _dialoguesInFirstTimeline)
            {
                if (info.speakerController != null)
                    info.speakerController.AdvanceDialogueIndex(info.dialogueCountInTimeline);
            }
        }
        else if (_currentTimelineIndex == 1)
        {
            foreach (var info in _dialoguesInSecondTimeline)
            {
                if (info.speakerController != null)
                    info.speakerController.AdvanceDialogueIndex(info.dialogueCountInTimeline);
            }
        }

        // [시각적 상태 보정] - 타임라인의 마지막 모습을 즉시 적용합니다.
        _playableDirector.Stop();
        _playableDirector.time = _playableDirector.duration;
        _playableDirector.Evaluate();

        // [종료 처리] - 해당 타임라인의 종료 함수를 수동으로 호출합니다.
        if (_currentTimelineIndex == 0)
        {
            HandleFirstTimelineEnd();
        }
        else if (_currentTimelineIndex == 1)
        {
            HandleSecondTimelineEnd();
        }
    }

    public void HandleFirstTimelineEnd()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _daughterSpriteRenderer.GetComponent<Collider2D>().enabled = true;
        _simpleSlot.AddEvents();
    }

    public void HandleSecondTimelineEnd()
    {
        // 로그 시스템
        ChangeStageSection("stage_exit");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId, "clear", Time.realtimeSinceStartup - StageStartTime);

        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
    }
}
