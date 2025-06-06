// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Core Setup")]
    [SerializeField] private GameObject dialogueBubblePrefab; // 일반 대화용 말풍선 프리팹
    [SerializeField] private GameObject choiceBubblePrefab;   // 선택지용 말풍선 프리팹
    [SerializeField] private Transform canvasTransform;

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName = "dialogues";
    [SerializeField] private bool pauseGameDuringDialogue = true;

    public const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    public const string PLAYER_SPEAKER_ID_CONST = "당신"; // 상태 클래스에서 참조할 상수

    // --- 상태 인스턴스 ---
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
    public DialogueUI CurrentDialogueBubbleUI { get; private set; }
    public DialogueUI CurrentChoiceBubbleUI { get; private set; } // DialogueUI 재사용 가정
    private Transform currentNpcSpeakerAnchor;
    public Transform PlayerSpeechAnchor { get; private set; }
    public Transform CurrentDialogueBubbleTargetAnchor { get; set; }
    public Transform CurrentChoiceBubbleTargetAnchor { get; set; } // 선택지 말풍선용 타겟 앵커
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

        FindPlayerAnchorByName();
        // UI 인스턴스는 필요할 때 생성/초기화 (StartDialogue, DisplayChoicesOnChoiceBubble)

        TransitionToState(IdleState);
    }

    void FindPlayerAnchorByName()
    {
        GameObject playerObj = GameObject.FindWithTag(PLAYER_TAG);
        if (playerObj != null)
        {
            Transform anchor = playerObj.transform.Find(PLAYER_SPEECH_ANCHOR_NAME);
            PlayerSpeechAnchor = anchor ?? playerObj.transform;
            if (anchor == null) Debug.LogWarning($"DialogueManager: Child '{PLAYER_SPEECH_ANCHOR_NAME}' not found on Player.");
        }
        else Debug.LogError($"DialogueManager: Player object with tag '{PLAYER_TAG}' not found.");
    }

    bool InitializeDialogueBubbleUI()
    {
        if (CurrentDialogueBubbleUI != null) return true; // 이미 인스턴스가 있다면 (예: 풀링)
        if (dialogueBubblePrefab == null || canvasTransform == null) { Debug.LogError("DM: dialogueBubblePrefab or CanvasTransform not set."); return false; }

        GameObject instance = Instantiate(dialogueBubblePrefab, canvasTransform);
        CurrentDialogueBubbleUI = instance.GetComponent<DialogueUI>();
        if (CurrentDialogueBubbleUI == null) { Debug.LogError("DM: DialogueUI component not found on dialogueBubblePrefab."); Destroy(instance); return false; }
        if (!CurrentDialogueBubbleUI.enabled) { Debug.LogError("DM: Instantiated DialogueBubbleUI is not enabled (check its Awake)."); return false; }
        CurrentDialogueBubbleUI.Show(false); // 초기에는 숨김
        return true;
    }

    bool InitializeChoiceBubbleUI()
    {
        if (CurrentChoiceBubbleUI != null) return true;
        if (choiceBubblePrefab == null || canvasTransform == null) { Debug.LogError("DM: choiceBubblePrefab or CanvasTransform not set."); return false; }

        GameObject instance = Instantiate(choiceBubblePrefab, canvasTransform);
        CurrentChoiceBubbleUI = instance.GetComponent<DialogueUI>(); // DialogueUI 재사용 가정
        if (CurrentChoiceBubbleUI == null) { Debug.LogError("DM: DialogueUI component not found on choiceBubblePrefab."); Destroy(instance); return false; }
        if (!CurrentChoiceBubbleUI.enabled) { Debug.LogError("DM: Instantiated ChoiceBubbleUI is not enabled (check its Awake)."); return false; }
        CurrentChoiceBubbleUI.Show(false); // 초기에는 숨김
        return true;
    }

    public void TransitionToState(IDialogueState nextState)
    {
        currentState?.ExitState(this);
        currentState = nextState;
        currentState?.EnterState(this);
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        if (dialogueCollection == null) { Debug.LogError("DM: Dialogue collection not loaded."); return; }

        DialogueEntry entry = dialogueLoader.GetDialogueEntryById(dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"DM: Dialogue ID '{dialogueId}' not found."); TransitionToState(EndingState); return; }

        if (!InitializeDialogueBubbleUI()) { Debug.LogError("DM: InitializeDialogueBubbleUI failed."); TransitionToState(EndingState); return; }
        // 선택지 UI는 ShowingChoicesState 진입 시 초기화/표시

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        CurrentDialogueBubbleTargetAnchor = npcSpeechAnchor; // 초기 대화는 NPC 기준
        justStartedDialogueInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

        currentDialogueLines.Clear();
        foreach (var line in entry.lines) { currentDialogueLines.Enqueue(line); }
        CurrentChoices = entry.choices;

        CurrentDialogueBubbleUI.Show(true);
        TransitionToState(StartingState);
    }

    public void PrepareNextLine()
    {
        CurrentLineToShow = currentDialogueLines.Count > 0 ? currentDialogueLines.Dequeue() : null;
    }

    public void DisplayCurrentLineOnDialogueBubble()
    {
        if (CurrentDialogueBubbleUI == null || CurrentLineToShow == null) return;

        CurrentDialogueBubbleUI.SetDialogueText(CurrentLineToShow.text);
        CurrentDialogueBubbleUI.SetSpeakerName(CurrentLineToShow.speaker);

        if (CurrentLineToShow.speaker.Equals(PLAYER_SPEAKER_ID_CONST, System.StringComparison.OrdinalIgnoreCase))
        {
            if (PlayerSpeechAnchor == null) { Debug.LogError("DM: PlayerSpeechAnchor is null."); TransitionToState(EndingState); return; }
            CurrentDialogueBubbleTargetAnchor = PlayerSpeechAnchor;
        }
        else
        {
            if (currentNpcSpeakerAnchor == null) { Debug.LogError("DM: currentNpcSpeakerAnchor is null."); TransitionToState(EndingState); return; }
            CurrentDialogueBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }
    }

    public void DisplayChoicesOnChoiceBubble()
    {
        if (!InitializeChoiceBubbleUI())
        {
            Debug.LogError("DM: Failed to initialize ChoiceBubbleUI.");
            TransitionToState(EndingState); return;
        }
        if (CurrentChoices == null || CurrentChoices.Count == 0)
        {
            Debug.LogWarning("DM: No choices to display for ChoiceBubble.");
            TransitionToState(EndingState); return;
        }

        CurrentChoiceBubbleUI.SetSpeakerName(PLAYER_SPEAKER_ID_CONST); // 선택지는 플레이어 주체
        CurrentChoiceBubbleUI.UpdateChoicesVisual(CurrentChoices, CurrentSelectedChoiceIndex);

        CurrentChoiceBubbleTargetAnchor = PlayerSpeechAnchor; // 선택지 말풍선은 플레이어 기준

        CurrentChoiceBubbleUI.Show(true);
    }

    public void AdvanceDialogue()
    {
        PrepareNextLine();
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

    public void UpdateChoiceSelectionVisualOnChoiceBubble()
    {
        if (CurrentChoiceBubbleUI != null && CurrentChoices != null)
        {
            CurrentChoiceBubbleUI.UpdateChoicesVisual(CurrentChoices, CurrentSelectedChoiceIndex);
        }
    }

    public void SelectCurrentChoice()
    {
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false); // 선택지 말풍선 숨김

        if (CurrentChoices == null || CurrentSelectedChoiceIndex < 0 || CurrentSelectedChoiceIndex >= CurrentChoices.Count)
        {
            Debug.LogWarning("DM: Invalid choice selection attempt.");
            TransitionToState(EndingState); return;
        }

        string nextDialogueId = CurrentChoices[CurrentSelectedChoiceIndex].nextDialogueId;

        if (string.IsNullOrEmpty(nextDialogueId))
        {
            TransitionToState(EndingState);
        }
        else
        {
            DialogueEntry nextEntry = dialogueLoader.GetDialogueEntryById(dialogueCollection, nextDialogueId);
            if (nextEntry != null)
            {
                // 다음 대화 세그먼트 준비 (현재 NPC 앵커 유지)
                currentDialogueLines.Clear();
                foreach (var line in nextEntry.lines) { currentDialogueLines.Enqueue(line); }
                CurrentChoices = nextEntry.choices;
                // justStartedDialogueInputLock = true; // 새 세그먼트 시작 시 입력 잠금 (StartDialogue에서 처리)
                TransitionToState(StartingState); // 새 대화 시작 상태로
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
        if (CurrentDialogueBubbleUI != null) CurrentDialogueBubbleUI.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);
        dialogueJustEndedInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        currentNpcSpeakerAnchor = null; // 현재 대화 NPC 앵커 초기화
        CurrentDialogueBubbleTargetAnchor = null;
        CurrentChoiceBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        CurrentChoices = null;
        CurrentLineToShow = null;

        TransitionToState(IdleState);
    }

    void Update()
    {
        if (dialogueJustEndedInputLock) { dialogueJustEndedInputLock = false; }

        if (justStartedDialogueInputLock && currentState != null && currentState != IdleState)
        { // 수정: Idle 아닐때만
            justStartedDialogueInputLock = false;
            return; // 대화 시작 프레임에는 상태 업데이트를 통한 입력 처리 건너뜀
        }

        if (currentState == null || currentState == IdleState) return; // Idle 상태이거나 초기화 전이면 Update 로직 X

        currentState.UpdateState(this); // 현재 상태의 업데이트 로직 실행

        // 각 UI 위치 업데이트 (해당 UI가 활성화 되어 있을 때만)
        PositionDialogueBubble();
        PositionChoiceBubble();
    }

    void PositionDialogueBubble()
    {
        if (CurrentDialogueBubbleUI == null || !CurrentDialogueBubbleUI.gameObject.activeInHierarchy || CurrentDialogueBubbleTargetAnchor == null || Camera.main == null) return;
        CurrentDialogueBubbleUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentDialogueBubbleTargetAnchor.position));
    }

    void PositionChoiceBubble()
    {
        if (CurrentChoiceBubbleUI == null || !CurrentChoiceBubbleUI.gameObject.activeInHierarchy || CurrentChoiceBubbleTargetAnchor == null || Camera.main == null) return;
        CurrentChoiceBubbleUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentChoiceBubbleTargetAnchor.position));
    }
}