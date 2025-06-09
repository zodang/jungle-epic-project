// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Core Setup")]
    [SerializeField] private GameObject dialogueBubblePrefab;
    [SerializeField] private GameObject choiceBubblePrefab;
    [SerializeField] private Transform canvasTransform;

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName = "dialogues";
    [SerializeField] private bool pauseGameDuringDialogue = true;

    public const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    public const string PLAYER_SPEAKER_ID_CONST = "당신";

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
    public DialogueUI CurrentChoiceBubbleUI { get; private set; }

    private Transform currentNpcSpeakerAnchor;
    public Transform PlayerSpeechAnchor { get; private set; }
    public Transform CurrentDialogueBubbleTargetAnchor { get; set; }
    public Transform CurrentChoiceBubbleTargetAnchor { get; set; }

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

    bool InitializeDialogueBubbleUI()
    {
        if (CurrentDialogueBubbleUI != null && CurrentDialogueBubbleUI.gameObject.activeInHierarchy) return true;
        if (CurrentDialogueBubbleUI == null) // 인스턴스가 아예 없을 때만 새로 생성
        {
            if (dialogueBubblePrefab == null || canvasTransform == null) { Debug.LogError("DM: dialogueBubblePrefab or CanvasTransform not set."); return false; }
            GameObject instance = Instantiate(dialogueBubblePrefab, canvasTransform);
            CurrentDialogueBubbleUI = instance.GetComponent<DialogueUI>();
            if (CurrentDialogueBubbleUI == null) { Debug.LogError("DM: DialogueUI component not found on dialogueBubblePrefab."); Destroy(instance); return false; }
        }
        if (!CurrentDialogueBubbleUI.enabled) { Debug.LogError("DM: Instantiated DialogueBubbleUI is not enabled (check its Awake)."); return false; }
        // CurrentDialogueBubbleUI.Show(false); // Show는 StartDialogue나 EndDialogue에서 제어
        return true;
    }

    bool InitializeChoiceBubbleUI()
    {
        if (CurrentChoiceBubbleUI != null && CurrentChoiceBubbleUI.gameObject.activeInHierarchy) return true;
        if (CurrentChoiceBubbleUI == null)
        {
            if (choiceBubblePrefab == null || canvasTransform == null) { Debug.LogError("DM: choiceBubblePrefab or CanvasTransform not set."); return false; }
            GameObject instance = Instantiate(choiceBubblePrefab, canvasTransform);
            CurrentChoiceBubbleUI = instance.GetComponent<DialogueUI>();
            if (CurrentChoiceBubbleUI == null) { Debug.LogError("DM: DialogueUI component not found on choiceBubblePrefab."); Destroy(instance); return false; }
        }
        if (!CurrentChoiceBubbleUI.enabled) { Debug.LogError("DM: Instantiated ChoiceBubbleUI is not enabled (check its Awake)."); return false; }
        // CurrentChoiceBubbleUI.Show(false);
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

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        CurrentDialogueBubbleTargetAnchor = npcSpeechAnchor;
        justStartedDialogueInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

        currentDialogueLines.Clear();
        foreach (var line in entry.lines) { currentDialogueLines.Enqueue(line); }
        CurrentChoices = entry.choices;

        CurrentDialogueBubbleUI.Show(true);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false); // 이전 선택지 UI 숨김
        TransitionToState(StartingState);
    }

    public void PrepareNextLine() { CurrentLineToShow = currentDialogueLines.Count > 0 ? currentDialogueLines.Dequeue() : null; }

    public void DisplayCurrentLineOnDialogueBubble()
    {
        if (CurrentDialogueBubbleUI == null || CurrentLineToShow == null) return;
        CurrentDialogueBubbleUI.SetMainText(CurrentLineToShow.text, true); // 타이핑 효과 사용
        CurrentDialogueBubbleUI.SetSpeakerName(CurrentLineToShow.speaker);
        if (CurrentLineToShow.speaker.Equals(PLAYER_SPEAKER_ID_CONST, System.StringComparison.OrdinalIgnoreCase))
        {
            CurrentDialogueBubbleTargetAnchor = PlayerSpeechAnchor;
        }
        else
        {
            CurrentDialogueBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }
    }

    public void DisplayChoicesOnChoiceBubble()
    {
        if (!InitializeChoiceBubbleUI()) { Debug.LogError("DM: Failed to initialize ChoiceBubbleUI."); TransitionToState(EndingState); return; }
        if (CurrentChoices == null || CurrentChoices.Count == 0) { Debug.LogWarning("DM: No choices for ChoiceBubble."); TransitionToState(EndingState); return; }

        CurrentChoiceBubbleUI.SetSpeakerName(PLAYER_SPEAKER_ID_CONST); // 또는 선택지 타이틀
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
            CurrentChoiceBubbleUI.DisplayChoicesInMainText(CurrentChoices, CurrentSelectedChoiceIndex); // 즉시 업데이트
        }
    }

    public void SelectCurrentChoice()
    {
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);
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
        if (CurrentDialogueBubbleUI != null) CurrentDialogueBubbleUI.Show(false);
        if (CurrentChoiceBubbleUI != null) CurrentChoiceBubbleUI.Show(false);
        dialogueJustEndedInputLock = true;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;
        currentNpcSpeakerAnchor = null;
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
        {
            justStartedDialogueInputLock = false;
            return;
        }
        if (currentState == null || currentState == IdleState) return;
        currentState.UpdateState(this);
        PositionDialogueBubble();
        PositionChoiceBubble();
    }

    void PositionDialogueBubble()
    {
        if (CurrentDialogueBubbleUI != null && CurrentDialogueBubbleUI.gameObject.activeInHierarchy && CurrentDialogueBubbleTargetAnchor != null && Camera.main != null)
        {
            CurrentDialogueBubbleUI.SetBubblePosition(Camera.main.WorldToScreenPoint(CurrentDialogueBubbleTargetAnchor.position));
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