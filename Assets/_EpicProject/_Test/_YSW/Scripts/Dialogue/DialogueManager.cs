// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [Header("Core Setup")]
    [SerializeField] private GameObject npcDialogueBubblePrefab;    // NPC용 말풍선 프리팹
    [SerializeField] private GameObject playerDialogueBubblePrefab; // 플레이어용 말풍선 프리팹
    [SerializeField] private GameObject choiceBubblePrefab;
    [SerializeField] private Transform _canvasTransform;

    [Header("Dialogue Events")]
    public UnityEvent OnDialogueStart = new UnityEvent(); // 초기화 확실히
    public UnityEvent OnDialogueEnd = new UnityEvent();   // 초기화 확실히

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName = "dialogues";
    [SerializeField] private bool pauseGameDuringDialogue = true;

    [Header("Interaction Settings")] // 쿨타임 설정을 위한 헤더 추가
    [SerializeField] private float dialogueEndCooldown = 0.5f; // 대화 종료 후 재시작까지의 쿨타임 (초)

    public const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    private static readonly Dictionary<string, string> PLAYER_DISPLAY_NAMES = new Dictionary<string, string>
    {
        // Localization: 화자 이름 비교
        { "ko", "당신" },
        { "en", "You" },
        { "zh-Hans", "你" },
    };

    private IDialogueState currentState;
    public readonly DialogueIdleState IdleState = new DialogueIdleState();
    public readonly DialogueStartingState StartingState = new DialogueStartingState();
    public readonly DialogueShowingLineState ShowingLineState = new DialogueShowingLineState();
    public readonly DialogueShowingChoicesState ShowingChoicesState = new DialogueShowingChoicesState();
    public readonly DialogueEndingState EndingState = new DialogueEndingState();

    private DialogueLoader dialogueLoader;
    private DialogueCollection dialogueCollection;
    private Queue<DialogueLine> currentDialogueLines = new Queue<DialogueLine>();
    public List<DialogueChoice> CurrentChoices { get; private set; }
    public DialogueLine CurrentLineToShow { get; private set; }

    // UI 인스턴스 참조
    private DialogueUI npcDialogueBubbleInstance;
    private DialogueUI playerDialogueBubbleInstance;
    private DialogueUI activeDialogueBubbleUI; // 현재 활성화된 일반 대화 UI (NPC 또는 플레이어)
    public DialogueUI CurrentChoiceBubbleUI { get; private set; }

    private Transform currentNpcSpeakerAnchor;
    public Transform PlayerSpeechAnchor { get; private set; }
    public Transform CurrentActiveDialogueBubbleTargetAnchor { get; set; } // 이름 변경: 현재 활성 대화 말풍선 타겟
    public Transform CurrentChoiceBubbleTargetAnchor { get; set; }

    private bool justStartedDialogueInputLock = false;
    private bool dialogueJustEndedInputLock = false; // 이 변수를 아래 float 변수로 대체
    private float dialogueEndTime = -1f; // 대화가 종료된 시간을 기록 (-1은 아직 종료되지 않았음을 의미)
    public int CurrentSelectedChoiceIndex { get; set; } = 0;

    public bool IsDialogueActive() => currentState != null && currentState != IdleState;
    //public bool WasDialogueJustEndedThisFrame() => dialogueJustEndedInputLock;

    // 상태 클래스에서 현재 활성화된 일반 대화 UI에 접근하기 위한 헬퍼
    public DialogueUI GetCurrentActiveDialogueBubble() => activeDialogueBubbleUI;

    void Awake()
    {
        dialogueLoader = GetComponent<DialogueLoader>();
        if (dialogueLoader == null)
        {
            GameObject loaderObject = new GameObject("DialogueLoader_AutoCreated");
            dialogueLoader = loaderObject.AddComponent<DialogueLoader>();
            Debug.LogWarning("DM: DialogueLoader not found, created automatically.");
        }
        
        /*
        // 씬 메니저에서 LoadDialogue 호출
        dialogueCollection = dialogueLoader.LoadDialogueDataFromFile(dialogueFileName);
        if (dialogueCollection == null)
        {
            Debug.LogError("DM: Failed to load dialogue collection. System disabled.");
            enabled = false; return;
        }*/
        
        FindPlayerAnchorByName();
        TransitionToState(IdleState);
    }

    public void LoadDialogue(string path)
    {
        dialogueFileName = path;
        
        dialogueCollection = dialogueLoader.LoadDialogueDataFromFile(dialogueFileName);
        if (dialogueCollection == null)
        {
            Debug.LogError("DM: Failed to load dialogue collection. System disabled.");
            enabled = false; return;
        }
    }

    void FindPlayerAnchorByName()
    {
        GameObject playerObj = GameObject.FindWithTag(PLAYER_TAG);
        if (playerObj != null)
        {
            Transform anchor = playerObj.transform.Find(PLAYER_SPEECH_ANCHOR_NAME);
            PlayerSpeechAnchor = anchor ?? playerObj.transform;
            if (anchor == null) Debug.LogWarning($"DM: Child '{PLAYER_SPEECH_ANCHOR_NAME}' not found on Player.");
        }
        else Debug.LogError($"DM: Player object with tag '{PLAYER_TAG}' not found.");
    }


    // 특정 DialogueUI 인스턴스를 초기화하는 헬퍼 함수
    private DialogueUI InitializeSpecificDialogueUI(DialogueUI existingInstance, GameObject prefab, string uiNameForLog) // 로그용 이름 추가
    {
        DialogueUI uiComponent = existingInstance; // 기존 인스턴스를 먼저 사용 시도

        if (uiComponent == null) // 인스턴스가 아직 없으면 새로 생성
        {
            if (prefab == null || _canvasTransform == null)
            {
                Debug.LogError($"DM: {uiNameForLog} Prefab or CanvasTransform not set.");
                return null;
            }

            GameObject instanceGO = Instantiate(prefab, _canvasTransform);
            uiComponent = instanceGO.GetComponent<DialogueUI>();

            if (uiComponent == null)
            {
                Debug.LogError($"DM: DialogueUI component not found on {uiNameForLog} Prefab.");
                Destroy(instanceGO); // 컴포넌트 없으면 파괴
                return null;
            }

            // ***** 이벤트 리스너 등록 (새로 생성된 인스턴스에 대해서만) *****
            if (uiComponent.onNextActionRequested == null) // 안전장치: UnityEvent가 null이면 생성
            {
                uiComponent.onNextActionRequested = new UnityEvent();
            }
            uiComponent.onNextActionRequested.AddListener(ProcessNextActionInput);
            Debug.Log($"<DM> Added onNextActionRequested listener to NEW {uiNameForLog} ({uiComponent.gameObject.name})");
            // ***********************************************************
        }
        // enabled 체크는 uiComponent에 대해 수행
        if (!uiComponent.enabled)
        {
            Debug.LogError($"DM: Instantiated/Existing {uiNameForLog} (DialogueUI) is not enabled (check its Awake).");
            return null; // DialogueUI의 Awake에서 문제가 있었다면 사용 불가
        }

        uiComponent.Show(false); // 초기에는 숨김 (또는 Show는 실제 사용 시점에만)
        return uiComponent;
    }


    public void TransitionToState(IDialogueState nextState)
    {
        currentState?.ExitState(this);
        currentState = nextState;
        currentState?.EnterState(this);
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        // ProgressManager가 있다면, 시작되는 이 '대화 ID'를 "봤음"으로 기록
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.MarkDialogueAsSeen(dialogueId);
        }

        if (dialogueCollection == null) { Debug.LogError("DM: Dialogue collection not loaded."); return; }
        DialogueEntry entry = dialogueLoader.GetDialogueEntryById(dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"DM: Dialogue ID '{dialogueId}' not found."); TransitionToState(EndingState); return; }

        // 모든 UI는 필요할 때 Ensure... 함수를 통해 초기화됨
        // 기존에 활성화된 UI가 있다면 숨김 (FinalizeDialogue에서 처리되지만, 안전을 위해)
        if (activeDialogueBubbleUI != null) activeDialogueBubbleUI.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);

        OnDialogueStart.Invoke();

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        // CurrentActiveDialogueBubbleTargetAnchor는 DisplayCurrentLineOnActiveBubble에서 설정
        justStartedDialogueInputLock = true;
        if (pauseGameDuringDialogue) GameManager.Instance.TimeScaleManager.RequestPauseGame();

        currentDialogueLines.Clear();
        foreach (var line in entry.lines) { currentDialogueLines.Enqueue(line); }
        CurrentChoices = entry.choices;

        // 첫 대사는 StartingState -> ShowingLineState -> DisplayCurrentLineOnActiveBubble에서 처리
        TransitionToState(StartingState);
    }

    public void PrepareNextLine() { CurrentLineToShow = currentDialogueLines.Count > 0 ? currentDialogueLines.Dequeue() : null; }

    public void DisplayCurrentLineOnActiveBubble()
    {
        if (CurrentLineToShow == null)
        {
            Debug.LogWarning("DM: CurrentLineToShow is null, cannot display.");
            AdvanceDialogue();
            return;
        }
        
        // Localization: 현재 언어 설정에 따른 DialogueLine의 speaker와 text 데이터 추출
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
        string speaker = "";
        string text = "";
        
        // [2] speaker와 text 안전하게 꺼내기 (딕셔너리에 해당 언어 없으면 빈 문자열 fallback)
        if (CurrentLineToShow.speaker != null && CurrentLineToShow.speaker.TryGetValue(lang, out var spk))
            speaker = spk;
        if (CurrentLineToShow.text != null && CurrentLineToShow.text.TryGetValue(lang, out var txt))
            text = txt;
        
        string playerDisplayName = PLAYER_DISPLAY_NAMES.ContainsKey(lang) ? PLAYER_DISPLAY_NAMES[lang] : "Player";
        bool isPlayerSpeaking = speaker.Equals(playerDisplayName, System.StringComparison.OrdinalIgnoreCase);
        
        DialogueUI targetUI = null;

        // 이전에 활성화된 말풍선이 현재 화자와 다른 타입이면 숨김
        if (activeDialogueBubbleUI != null)
        {
            if ((isPlayerSpeaking && activeDialogueBubbleUI != playerDialogueBubbleInstance) ||
                (!isPlayerSpeaking && activeDialogueBubbleUI != npcDialogueBubbleInstance))
            {
                activeDialogueBubbleUI.Show(false);
            }
        }

        if (isPlayerSpeaking)
        {
            playerDialogueBubbleInstance = InitializeSpecificDialogueUI(playerDialogueBubbleInstance, playerDialogueBubblePrefab, "PlayerDialogueBubble");
            targetUI = playerDialogueBubbleInstance;
            if (targetUI != null) CurrentActiveDialogueBubbleTargetAnchor = PlayerSpeechAnchor;
        }
        else
        {
            npcDialogueBubbleInstance = InitializeSpecificDialogueUI(npcDialogueBubbleInstance, npcDialogueBubblePrefab, "NpcDialogueBubble");
            targetUI = npcDialogueBubbleInstance;
            if (targetUI != null) CurrentActiveDialogueBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }

        activeDialogueBubbleUI = targetUI;

        if (activeDialogueBubbleUI == null) { Debug.LogError("DM: Failed to set activeDialogueBubbleUI."); TransitionToState(EndingState); return; }
        if (CurrentActiveDialogueBubbleTargetAnchor == null) { Debug.LogError("DM: Target anchor for active dialogue bubble is null."); TransitionToState(EndingState); return; }

        // ****** 순서 변경: Show(true)를 먼저 호출! ******
        activeDialogueBubbleUI.Show(true);
        activeDialogueBubbleUI.SetSpeakerName(speaker); // 화자 이름도 Show 이후 또는 동시에
        activeDialogueBubbleUI.SetMainText(text, true); // 그 다음에 타이핑 효과 시작
    }

    public void DisplayChoicesOnChoiceBubble()
    {
        CurrentChoiceBubbleUI = InitializeSpecificDialogueUI(CurrentChoiceBubbleUI, choiceBubblePrefab, "ChoiceBubble");
        if (CurrentChoiceBubbleUI == null) { Debug.LogError("DM: Failed to initialize ChoiceBubbleUI."); TransitionToState(EndingState); return; }
        if (CurrentChoices == null || CurrentChoices.Count == 0) { Debug.LogWarning("DM: No choices for ChoiceBubble."); TransitionToState(EndingState); return; }
        
        // Localization: 화자 이름 비교
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
        string playerDisplayName = PLAYER_DISPLAY_NAMES.ContainsKey(lang) ? PLAYER_DISPLAY_NAMES[lang] : "Player";
        CurrentChoiceBubbleUI.SetSpeakerName(playerDisplayName);
        
        CurrentChoiceBubbleUI.DisplayChoicesInMainText(CurrentChoices, CurrentSelectedChoiceIndex); // 선택지는 즉시 표시
        CurrentChoiceBubbleTargetAnchor = PlayerSpeechAnchor;
        CurrentChoiceBubbleUI.Show(true);
    }

    public void AdvanceDialogue()
    {
        PrepareNextLine();
        if (CurrentLineToShow != null) { TransitionToState(ShowingLineState); }
        else if (CurrentChoices != null && CurrentChoices.Count > 0) { TransitionToState(ShowingChoicesState); }
        else { TransitionToState(EndingState); }
    }

    public void UpdateChoiceSelectionVisualOnChoiceBubble()
    {
        if (CurrentChoiceBubbleUI != null && CurrentChoices != null)
        {
            CurrentChoiceBubbleUI.DisplayChoicesInMainText(CurrentChoices, CurrentSelectedChoiceIndex);
        }
    }

    public void SelectCurrentChoice()
    {
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false); // 먼저 선택지 UI 숨김

        if (CurrentChoices == null || CurrentSelectedChoiceIndex < 0 || CurrentSelectedChoiceIndex >= CurrentChoices.Count) { Debug.LogWarning("DM: Invalid choice."); TransitionToState(EndingState); return; }
        string nextDialogueId = CurrentChoices[CurrentSelectedChoiceIndex].nextDialogueId;
        if (string.IsNullOrEmpty(nextDialogueId)) { TransitionToState(EndingState); }
        else
        {
            DialogueEntry nextEntry = dialogueLoader.GetDialogueEntryById(dialogueCollection, nextDialogueId);
            if (nextEntry != null)
            {
                currentDialogueLines.Clear();
                foreach (var line in nextEntry.lines) { currentDialogueLines.Enqueue(line); }
                CurrentChoices = nextEntry.choices;
                TransitionToState(StartingState);
            }
            else { Debug.LogWarning($"DM: Next Dialogue ID '{nextDialogueId}' not found."); TransitionToState(EndingState); }
        }
    }

    public void FinalizeDialogue()
    {

        OnDialogueEnd.Invoke(); // 대화 종료 이벤트를 여기서 발생시킵니다.
        if (activeDialogueBubbleUI != null) activeDialogueBubbleUI.Show(false);
        if (npcDialogueBubbleInstance != null) npcDialogueBubbleInstance.Show(false);
        if (playerDialogueBubbleInstance != null) playerDialogueBubbleInstance.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);

        // dialogueJustEndedInputLock = true; // 이 줄 대신 아래 줄 사용
        dialogueEndTime = Time.unscaledTime; // <<== 대화 종료 시점의 실제 시간 기록
        if (pauseGameDuringDialogue) GameManager.Instance.TimeScaleManager.RequestResumeGame();

        currentNpcSpeakerAnchor = null;
        CurrentActiveDialogueBubbleTargetAnchor = null;
        CurrentChoiceBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        CurrentChoices = null;
        CurrentLineToShow = null;
        activeDialogueBubbleUI = null;

        TransitionToState(IdleState);
    }

    // WasDialogueJustEndedThisFrame() 함수를 새로운 쿨타임 확인 함수로 변경
    public bool IsInDialogueCooldown()
    {
        // dialogueEndTime이 기록되어 있고 (즉, 대화가 끝난 적이 있고),
        // 현재 시간과 대화 종료 시간의 차이가 쿨타임보다 작다면
        // 아직 쿨타임 중입니다.
        if (dialogueEndTime > 0 && Time.unscaledTime < dialogueEndTime + dialogueEndCooldown)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (dialogueJustEndedInputLock) { dialogueJustEndedInputLock = false; }
        if (justStartedDialogueInputLock && currentState != null && currentState != IdleState)
        {
            justStartedDialogueInputLock = false;
            return;
        }
        if (currentState == null || currentState == IdleState) return;

        // E키 또는 스페이스바 입력을 감지하여 공통 입력 처리 함수 호출
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            ProcessNextActionInput();
        }

        currentState.UpdateState(this); // 상태별 특수 입력 처리 (방향키 등)

        PositionActiveDialogueBubble();
        PositionChoiceBubble();
    }



    void PositionActiveDialogueBubble()
    {
        if (activeDialogueBubbleUI != null && activeDialogueBubbleUI.gameObject.activeInHierarchy && CurrentActiveDialogueBubbleTargetAnchor != null && Camera.main != null)
        {
            activeDialogueBubbleUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentActiveDialogueBubbleTargetAnchor.position));
        }
    }
    void PositionChoiceBubble()
    {
        if (CurrentChoiceBubbleUI != null && CurrentChoiceBubbleUI.gameObject.activeInHierarchy && CurrentChoiceBubbleTargetAnchor != null && Camera.main != null)
        {
            CurrentChoiceBubbleUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentChoiceBubbleTargetAnchor.position));
        }
    }

    // 스페이스바, E키, Next 버튼 클릭 시 모두 호출되는 공통 함수
    public void ProcessNextActionInput()
    {
        if (!IsDialogueActive() || justStartedDialogueInputLock || dialogueJustEndedInputLock) return;

        if (currentState == ShowingLineState)
        {
            DialogueUI currentBubble = GetCurrentActiveDialogueBubble();
            if (currentBubble != null && currentBubble.IsTyping())
            {
                // 타이핑 중일 때 누르면 효과 스킵(완료)
                currentBubble.CompleteTyping();
            }
            else
            {
                // 타이핑이 끝나면 다음 대사로
                AdvanceDialogue();
            }
        }
        else if (currentState == ShowingChoicesState)
        {
            // 선택지 화면에서는 선택 확정 역할
            if (CurrentChoices != null && CurrentSelectedChoiceIndex >= 0 && CurrentSelectedChoiceIndex < CurrentChoices.Count)
            {
                SelectCurrentChoice();
            }
        }
    }

    // E 키를 짧게 눌렀을 때의 처리
    private void ProcessEKeyInputAction()
    {
        if (!IsDialogueActive() || justStartedDialogueInputLock || IsInDialogueCooldown()) return;

        if (currentState == ShowingLineState)
        {
            DialogueUI currentBubble = GetCurrentActiveDialogueBubble();
            if (currentBubble != null && !currentBubble.IsTyping())
            {
                AdvanceDialogue();
            }
        }
        else if (currentState == ShowingChoicesState)
        {
            if (CurrentChoices != null && CurrentSelectedChoiceIndex >= 0 && CurrentSelectedChoiceIndex < CurrentChoices.Count)
            {
                SelectCurrentChoice();
            }
        }
    }
}