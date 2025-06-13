// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Core Setup")]
    [SerializeField] private GameObject npcDialogueBubblePrefab;    // NPC용 말풍선 프리팹
    [SerializeField] private GameObject playerDialogueBubblePrefab; // 플레이어용 말풍선 프리팹
    [SerializeField] private GameObject choiceBubblePrefab;
    [SerializeField] private Transform canvasTransform;

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName = "dialogues";
    [SerializeField] private bool pauseGameDuringDialogue = true;

    public const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    public const string PLAYER_SPEAKER_ID_CONST = "당신"; // 또는 JSON의 "Player" ID

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
    private bool dialogueJustEndedInputLock = false;
    public int CurrentSelectedChoiceIndex { get; set; } = 0;

    public bool IsDialogueActive() => currentState != null && currentState != IdleState;
    public bool WasDialogueJustEndedThisFrame() => dialogueJustEndedInputLock;

    // 상태 클래스에서 현재 활성화된 일반 대화 UI에 접근하기 위한 헬퍼
    public DialogueUI GetCurrentActiveDialogueBubble() => activeDialogueBubbleUI;


    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader == null)
        {
            GameObject loaderObject = new GameObject("DialogueLoader_AutoCreated");
            dialogueLoader = loaderObject.AddComponent<DialogueLoader>();
            Debug.LogWarning("DM: DialogueLoader not found, created automatically.");
        }

        dialogueCollection = dialogueLoader.LoadDialogueDataFromFile(dialogueFileName);
        if (dialogueCollection == null)
        {
            Debug.LogError("DM: Failed to load dialogue collection. System disabled.");
            enabled = false; return;
        }

        FindPlayerAnchorByName();
        TransitionToState(IdleState);
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
    private DialogueUI InitializeSpecificDialogueUI(DialogueUI existingInstance, GameObject prefab, string uiName)
    {
        if (existingInstance != null) return existingInstance; // 이미 있으면 반환 (풀링 시 유용)

        if (prefab == null || canvasTransform == null) { Debug.LogError($"DM: {uiName} Prefab or CanvasTransform not set."); return null; }

        GameObject instanceGO = Instantiate(prefab, canvasTransform);
        DialogueUI uiComponent = instanceGO.GetComponent<DialogueUI>();
        if (uiComponent == null) { Debug.LogError($"DM: DialogueUI component not found on {uiName} Prefab."); Destroy(instanceGO); return null; }
        if (!uiComponent.enabled) { Debug.LogError($"DM: Instantiated {uiName} (DialogueUI) is not enabled (check its Awake)."); return null; }

        uiComponent.Show(false); // 초기에는 숨김
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
        if (dialogueCollection == null) { Debug.LogError("DM: Dialogue collection not loaded."); return; }
        DialogueEntry entry = dialogueLoader.GetDialogueEntryById(dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"DM: Dialogue ID '{dialogueId}' not found."); TransitionToState(EndingState); return; }

        // 모든 UI는 필요할 때 Ensure... 함수를 통해 초기화됨
        // 기존에 활성화된 UI가 있다면 숨김 (FinalizeDialogue에서 처리되지만, 안전을 위해)
        if (activeDialogueBubbleUI != null) activeDialogueBubbleUI.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);


        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        // CurrentActiveDialogueBubbleTargetAnchor는 DisplayCurrentLineOnActiveBubble에서 설정
        justStartedDialogueInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

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

        bool isPlayerSpeaking = CurrentLineToShow.speaker.Equals(PLAYER_SPEAKER_ID_CONST, System.StringComparison.OrdinalIgnoreCase);
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
        activeDialogueBubbleUI.SetSpeakerName(CurrentLineToShow.speaker); // 화자 이름도 Show 이후 또는 동시에
        activeDialogueBubbleUI.SetMainText(CurrentLineToShow.text, true); // 그 다음에 타이핑 효과 시작
    }

    public void DisplayChoicesOnChoiceBubble()
    {
        CurrentChoiceBubbleUI = InitializeSpecificDialogueUI(CurrentChoiceBubbleUI, choiceBubblePrefab, "ChoiceBubble");
        if (CurrentChoiceBubbleUI == null) { Debug.LogError("DM: Failed to initialize ChoiceBubbleUI."); TransitionToState(EndingState); return; }
        if (CurrentChoices == null || CurrentChoices.Count == 0) { Debug.LogWarning("DM: No choices for ChoiceBubble."); TransitionToState(EndingState); return; }

        CurrentChoiceBubbleUI.SetSpeakerName(PLAYER_SPEAKER_ID_CONST);
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
        if (activeDialogueBubbleUI != null) activeDialogueBubbleUI.Show(false);
        if (npcDialogueBubbleInstance != null) npcDialogueBubbleInstance.Show(false);
        if (playerDialogueBubbleInstance != null) playerDialogueBubbleInstance.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);

        dialogueJustEndedInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        currentNpcSpeakerAnchor = null;
        CurrentActiveDialogueBubbleTargetAnchor = null;
        CurrentChoiceBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        CurrentChoices = null;
        CurrentLineToShow = null;
        activeDialogueBubbleUI = null;

        TransitionToState(IdleState);
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

        currentState.UpdateState(this);

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
}