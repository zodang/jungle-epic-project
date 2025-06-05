// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // --- 상태 인스턴스 ---
    private IDialogueState currentState;
    public readonly DialogueIdleState IdleState = new DialogueIdleState();
    public readonly DialogueStartingState StartingState = new DialogueStartingState();
    public readonly DialogueShowingLineState ShowingLineState = new DialogueShowingLineState();
    public readonly DialogueShowingChoicesState ShowingChoicesState = new DialogueShowingChoicesState();
    public readonly DialogueEndingState EndingState = new DialogueEndingState();
    // ---             ---

    [Header("Core Setup")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private string dialogueFileName = "dialogues";

    [Header("Dialogue Settings")]
    [SerializeField] private bool pauseGameDuringDialogue = true;

    public const string PLAYER_TAG = "Player"; // NPCInteraction에서 사용
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    // PLAYER_SPEAKER_ID는 상태 클래스에서 직접 사용하거나 DialogueManager에 상수로 둘 수 있음
    public const string PLAYER_SPEAKER_ID_CONST = "당신"; // 상태 클래스에서 참조할 상수

    private DialogueLoader dialogueLoader;
    private DialogueCollection dialogueCollection;

    // 상태 클래스에서 접근해야 하는 현재 대화 데이터
    private Queue<DialogueLine> currentDialogueLines = new Queue<DialogueLine>();
    public List<DialogueChoice> CurrentChoices { get; private set; }
    public DialogueLine CurrentLineToShow { get; private set; } // 현재 화면에 표시/표시될 대사

    public DialogueUI CurrentDialogueUI { get; private set; }

    private Transform currentNpcSpeakerAnchor; // 현재 대화 중인 NPC의 앵커
    public Transform PlayerSpeechAnchor { get; private set; }
    public Transform CurrentBubbleTargetAnchor { get; set; } // 상태에 따라 변경될 수 있음

    // 입력 잠금 플래그 (상태 머신 외부의 입력 처리와 동기화 위해 유지)
    private bool justStartedDialogueInputLock = false;
    private bool dialogueJustEndedInputLock = false;
    public int CurrentSelectedChoiceIndex { get; set; } = 0;


    public bool IsDialogueActive() => currentState != null && currentState != IdleState;
    public bool WasDialogueJustEndedThisFrame() => dialogueJustEndedInputLock;


    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader == null)
        {
            GameObject loaderObject = new GameObject("DialogueLoader_AutoCreated");
            dialogueLoader = loaderObject.AddComponent<DialogueLoader>();
            Debug.LogWarning("DialogueManager: DialogueLoader not found, created automatically.");
        }

        dialogueCollection = dialogueLoader.LoadDialogueDataFromFile(dialogueFileName);
        if (dialogueCollection == null)
        {
            Debug.LogError("DialogueManager: Failed to load dialogue collection. System disabled.");
            enabled = false; return;
        }

        FindPlayerAnchorByName(); // 함수 이름 변경 및 PlayerSpeechAnchor 할당
        InitializeDialogueUIInstance(); // UI 인스턴스 초기화

        TransitionToState(IdleState); // 초기 상태는 Idle
    }

    void FindPlayerAnchorByName() // 함수 이름 구체화
    {
        GameObject playerObj = GameObject.FindWithTag(PLAYER_TAG);
        if (playerObj != null)
        {
            Transform anchor = playerObj.transform.Find(PLAYER_SPEECH_ANCHOR_NAME);
            PlayerSpeechAnchor = anchor ?? playerObj.transform; // public 프로퍼티에 할당
            if (anchor == null) Debug.LogWarning($"DialogueManager: Child '{PLAYER_SPEECH_ANCHOR_NAME}' not found on Player.");
        }
        else Debug.LogError($"DialogueManager: Player object with tag '{PLAYER_TAG}' not found.");
    }

    bool InitializeDialogueUIInstance() // 함수 이름 구체화
    {
        if (CurrentDialogueUI != null) return true; // 이미 있으면 스킵

        if (speechBubblePrefab == null || canvasTransform == null)
        {
            Debug.LogError("DialogueManager: SpeechBubblePrefab or CanvasTransform not set.");
            return false;
        }
        GameObject speechBubbleInstance = Instantiate(speechBubblePrefab, canvasTransform);
        CurrentDialogueUI = speechBubbleInstance.GetComponent<DialogueUI>(); // public 프로퍼티에 할당
        if (CurrentDialogueUI == null)
        {
            Debug.LogError("DialogueManager: DialogueUI component not found on prefab.");
            Destroy(speechBubbleInstance); return false;
        }
        if (!CurrentDialogueUI.enabled)
        { // DialogueUI.Awake에서 실패 시
            Debug.LogError("DialogueManager: DialogueUI failed to initialize its internal elements.");
            return false;
        }
        return true;
    }

    public void TransitionToState(IDialogueState nextState)
    {
        // Debug.Log($"Transitioning from {currentState?.GetType().Name} to {nextState?.GetType().Name}");
        currentState?.ExitState(this);
        currentState = nextState;
        currentState?.EnterState(this); // null 체크 추가
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        if (dialogueCollection == null) { Debug.LogError("DM: Dialogue collection not loaded."); return; }

        DialogueEntry entry = dialogueLoader.GetDialogueEntryById(dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"DM: Dialogue ID '{dialogueId}' not found."); TransitionToState(EndingState); return; }

        if (!InitializeDialogueUIInstance()) { Debug.LogError("DM: InitializeDialogueUI failed."); TransitionToState(EndingState); return; }

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        justStartedDialogueInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

        currentDialogueLines.Clear();
        foreach (var line in entry.lines) { currentDialogueLines.Enqueue(line); }
        CurrentChoices = entry.choices;

        CurrentDialogueUI.Show(true);
        TransitionToState(StartingState);
    }

    public void PrepareNextLine()
    {
        CurrentLineToShow = currentDialogueLines.Count > 0 ? currentDialogueLines.Dequeue() : null;
    }

    public void DisplayCurrentLineOnUI()
    {
        if (CurrentDialogueUI == null || CurrentLineToShow == null)
        {
            // Debug.LogWarning("DM: Cannot display line, UI or Line is null.");
            // 이 경우 보통 AdvanceDialogue에서 다른 상태로 이미 전이되었어야 함.
            return;
        }

        CurrentDialogueUI.SetDialogueText(CurrentLineToShow.text);
        CurrentDialogueUI.SetSpeakerName(CurrentLineToShow.speaker);

        if (CurrentLineToShow.speaker.Equals(PLAYER_SPEAKER_ID_CONST, System.StringComparison.OrdinalIgnoreCase))
        {
            if (PlayerSpeechAnchor == null) { Debug.LogError("DM: PlayerSpeechAnchor is null."); TransitionToState(EndingState); return; }
            CurrentBubbleTargetAnchor = PlayerSpeechAnchor;
        }
        else
        {
            if (currentNpcSpeakerAnchor == null) { Debug.LogError("DM: currentNpcSpeakerAnchor is null."); TransitionToState(EndingState); return; }
            CurrentBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }
    }

    public void AdvanceDialogue()
    {
        PrepareNextLine(); // 다음 대사 준비 (CurrentLineToShow 업데이트)
        if (CurrentLineToShow != null)
        {
            TransitionToState(ShowingLineState);
        }
        else if (CurrentChoices != null && CurrentChoices.Count > 0)
        {
            TransitionToState(ShowingChoicesState);
        }
        else
        {
            TransitionToState(EndingState);
        }
    }

    public void UpdateChoiceSelectionVisual()
    {
        if (CurrentDialogueUI != null && CurrentChoices != null)
        {
            CurrentDialogueUI.UpdateChoicesVisual(CurrentChoices, CurrentSelectedChoiceIndex);
        }
    }

    public void SelectCurrentChoice()
    {
        if (CurrentChoices == null || CurrentSelectedChoiceIndex < 0 || CurrentSelectedChoiceIndex >= CurrentChoices.Count)
        {
            Debug.LogWarning("DM: Invalid choice selection attempt.");
            TransitionToState(EndingState); // 안전하게 종료
            return;
        }

        string nextDialogueId = CurrentChoices[CurrentSelectedChoiceIndex].nextDialogueId;

        if (string.IsNullOrEmpty(nextDialogueId))
        {
            TransitionToState(EndingState);
        }
        else
        {
            // 다음 대화 세그먼트 로드 및 시작 상태로 전이
            DialogueEntry nextEntry = dialogueLoader.GetDialogueEntryById(dialogueCollection, nextDialogueId);
            if (nextEntry != null)
            {
                // 현재 NPC 앵커는 유지 (같은 NPC와 대화 가정, 달라진다면 StartDialogue 호출 필요)
                currentDialogueLines.Clear();
                foreach (var line in nextEntry.lines) { currentDialogueLines.Enqueue(line); }
                CurrentChoices = nextEntry.choices;
                // justStartedDialogueInputLock = true; // 새 세그먼트 시작 시 입력 잠금 필요
                TransitionToState(StartingState);
            }
            else
            {
                Debug.LogWarning($"DM: Next Dialogue ID '{nextDialogueId}' not found.");
                TransitionToState(EndingState);
            }
        }
    }

    public void FinalizeDialogue()
    {
        if (CurrentDialogueUI != null) CurrentDialogueUI.Show(false);
        dialogueJustEndedInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        currentNpcSpeakerAnchor = null;
        CurrentBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        CurrentChoices = null;
        CurrentLineToShow = null;

        TransitionToState(IdleState);
    }

    void Update()
    {
        // 입력 잠금 플래그 처리
        if (dialogueJustEndedInputLock) { dialogueJustEndedInputLock = false; }

        // justStartedDialogueInputLock은 각 상태의 UpdateState 시작 부분에서 확인하거나,
        // DialogueManager의 Update에서 currentState?.UpdateState(this) 호출 전에 확인하고
        // 상태에 플래그를 전달하는 방식으로 처리할 수 있음.
        // 여기서는 간단하게, 입력 처리가 있는 상태 (ShowingLine, ShowingChoices)의 UpdateState에서
        // justStartedDialogueInputLock을 직접 확인하도록 유도.
        // 또는, 아래처럼 처리:
        if (justStartedDialogueInputLock && currentState != IdleState && currentState != null)
        {
            justStartedDialogueInputLock = false;
            // 이 프레임에는 상태 업데이트를 통한 입력 처리를 건너뛰고 싶다면 여기서 return.
            // 하지만 상태 Enter에서 대부분의 작업이 이루어지므로, 그냥 둬도 큰 문제는 없을 수 있음.
            // 일단은 상태의 Update가 호출되도록 둠.
        }

        currentState?.UpdateState(this);

        if (IsDialogueActive() && CurrentDialogueUI != null && CurrentDialogueUI.gameObject.activeInHierarchy)
        {
            PositionSpeechBubble();
        }
    }

    void PositionSpeechBubble() // private으로 변경해도 될 수 있음
    {
        if (CurrentDialogueUI == null || !CurrentDialogueUI.gameObject.activeInHierarchy || CurrentBubbleTargetAnchor == null || Camera.main == null) return;
        CurrentDialogueUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentBubbleTargetAnchor.position));
    }
}