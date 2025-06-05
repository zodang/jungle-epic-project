// DialogueManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Core Setup")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private string dialogueFileName = "dialogues";

    [Header("Dialogue Settings")]
    [SerializeField] private bool pauseGameDuringDialogue = true;

    public const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    private const string PLAYER_SPEAKER_ID = "´ç½Å";

    private DialogueLoader dialogueLoader;
    private DialogueCollection dialogueCollection;

    private Queue<DialogueLine> currentDialogueLines;
    private List<DialogueChoice> currentChoices;
    private DialogueUI currentDialogueUI;

    private Transform currentNpcSpeakerAnchor;
    private Transform playerSpeechAnchor;
    private Transform currentBubbleTargetAnchor;

    private bool dialogueActive = false;
    private bool justStartedDialogue = false;
    private bool isChoosing = false;
    private int currentSelectedChoiceIndex = 0;
    private bool dialogueJustEnded = false;

    public bool IsDialogueActive() => dialogueActive;
    public bool WasDialogueJustEndedThisFrame() => dialogueJustEnded;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader == null)
        {
            GameObject loaderObject = new GameObject("DialogueLoader_AutoCreated");
            dialogueLoader = loaderObject.AddComponent<DialogueLoader>();
            Debug.LogWarning("DialogueManager: DialogueLoader not found, created one automatically. Consider adding it to the scene manually.");
        }

        currentDialogueLines = new Queue<DialogueLine>();
        dialogueCollection = dialogueLoader.LoadDialogueDataFromFile(dialogueFileName);
        if (dialogueCollection == null)
        {
            Debug.LogError("DialogueManager: Failed to load dialogue collection in Awake. Dialogue system will be disabled.");
            enabled = false;
            return;
        }
        FindPlayerAnchor();
    }

    void FindPlayerAnchor()
    {
        GameObject playerObj = GameObject.FindWithTag(PLAYER_TAG);
        if (playerObj != null)
        {
            Transform anchor = playerObj.transform.Find(PLAYER_SPEECH_ANCHOR_NAME);
            playerSpeechAnchor = anchor ?? playerObj.transform;
            if (anchor == null) Debug.LogWarning($"DialogueManager: Child '{PLAYER_SPEECH_ANCHOR_NAME}' not found on Player. Using Player's root.");
        }
        else Debug.LogError($"DialogueManager: Player object with tag '{PLAYER_TAG}' not found.");
    }

    private bool InitializeDialogueUI()
    {
        if (currentDialogueUI != null) return true;

        if (speechBubblePrefab == null || canvasTransform == null)
        {
            Debug.LogError("DialogueManager: SpeechBubblePrefab or CanvasTransform not set in Inspector.");
            return false;
        }

        GameObject speechBubbleInstance = Instantiate(speechBubblePrefab, canvasTransform);
        currentDialogueUI = speechBubbleInstance.GetComponent<DialogueUI>();

        if (currentDialogueUI == null)
        {
            Debug.LogError("DialogueManager: DialogueUI component not found on the instantiated SpeechBubblePrefab.");
            Destroy(speechBubbleInstance);
            return false;
        }

        if (!currentDialogueUI.enabled)
        {
            Debug.LogError("DialogueManager: Instantiated DialogueUI is not enabled. Check DialogueUI.Awake for errors (e.g., missing child UI elements).");
            return false;
        }
        return true;
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        if (dialogueCollection == null) { Debug.LogError("<DialogueManager> Dialogue collection is not loaded."); return; }

        DialogueEntry entry = dialogueLoader.GetDialogueEntryById(dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"<DialogueManager> Dialogue ID '{dialogueId}' not found."); EndDialogue(); return; }

        if (!InitializeDialogueUI()) { Debug.LogError("<DialogueManager> InitializeDialogueUI failed."); EndDialogue(); return; }

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        dialogueActive = true;
        isChoosing = false;
        justStartedDialogue = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

        currentDialogueLines.Clear();
        foreach (var line in entry.lines) { currentDialogueLines.Enqueue(line); }
        currentChoices = entry.choices;

        currentDialogueUI.Show(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (!dialogueActive) return;
        if (currentDialogueUI == null) { Debug.LogError("DialogueManager: currentDialogueUI is null."); EndDialogue(); return; }

        if (currentDialogueLines.Count == 0)
        {
            if (currentChoices != null && currentChoices.Count > 0) { ShowChoices(); }
            else { EndDialogue(); }
            return;
        }

        DialogueLine currentLine = currentDialogueLines.Dequeue();

        currentDialogueUI.SetDialogueText(currentLine.text);
        currentDialogueUI.SetSpeakerName(currentLine.speaker);

        if (currentLine.speaker.Equals(PLAYER_SPEAKER_ID, System.StringComparison.OrdinalIgnoreCase))
        {
            if (playerSpeechAnchor == null) { Debug.LogError("DialogueManager: PlayerSpeechAnchor is null."); EndDialogue(); return; }
            currentBubbleTargetAnchor = playerSpeechAnchor;
        }
        else
        {
            if (currentNpcSpeakerAnchor == null) { Debug.LogError("DialogueManager: currentNpcSpeakerAnchor is null."); EndDialogue(); return; }
            currentBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }
    }

    void PositionSpeechBubble()
    {
        if (currentDialogueUI == null || !currentDialogueUI.gameObject.activeInHierarchy || currentBubbleTargetAnchor == null || Camera.main == null) return;
        currentDialogueUI.SetBubblePosition(Camera.main.WorldToScreenPoint(currentBubbleTargetAnchor.position));
    }

    void ShowChoices()
    {
        if (currentDialogueUI == null || currentChoices == null || currentChoices.Count == 0)
        {
            Debug.LogError("DialogueManager: Cannot show choices - UI or choices data missing."); EndDialogue(); return;
        }

        isChoosing = true;
        currentSelectedChoiceIndex = 0;

        currentDialogueUI.SetSpeakerName(PLAYER_SPEAKER_ID);

        if (playerSpeechAnchor != null) currentBubbleTargetAnchor = playerSpeechAnchor;

        currentDialogueUI.UpdateChoicesVisual(currentChoices, currentSelectedChoiceIndex);
    }

    public void SelectChoice(string nextDialogueId)
    {
        isChoosing = false;
        if (string.IsNullOrEmpty(nextDialogueId)) { EndDialogue(); }
        else { StartDialogue(nextDialogueId, currentNpcSpeakerAnchor); }
    }

    public void EndDialogue()
    {
        if (currentDialogueUI != null) currentDialogueUI.Show(false);
        dialogueActive = false;
        isChoosing = false;
        dialogueJustEnded = true;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        currentNpcSpeakerAnchor = null;
        currentBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        currentChoices = null;
    }

    void Update()
    {
        if (dialogueJustEnded) { dialogueJustEnded = false; }
        if (justStartedDialogue) { justStartedDialogue = false; return; }
        if (!dialogueActive) return;

        PositionSpeechBubble();

        if (isChoosing)
        {
            HandleChoiceInput();
        }
        else
        {
            HandleDialogueContinuationInput();
        }
    }

    void HandleChoiceInput()
    {
        bool selectionChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentSelectedChoiceIndex > 0) { currentSelectedChoiceIndex--; selectionChanged = true; }
            else if (currentChoices != null && currentChoices.Count > 1) { currentSelectedChoiceIndex = currentChoices.Count - 1; selectionChanged = true; }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentChoices != null && currentSelectedChoiceIndex < currentChoices.Count - 1) { currentSelectedChoiceIndex++; selectionChanged = true; }
            else if (currentChoices != null && currentChoices.Count > 1) { currentSelectedChoiceIndex = 0; selectionChanged = true; }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentChoices != null && currentSelectedChoiceIndex >= 0 && currentSelectedChoiceIndex < currentChoices.Count)
            {
                SelectChoice(currentChoices[currentSelectedChoiceIndex].nextDialogueId);
            }
            return;
        }

        if (selectionChanged && currentDialogueUI != null)
        {
            currentDialogueUI.UpdateChoicesVisual(currentChoices, currentSelectedChoiceIndex);
        }
    }

    void HandleDialogueContinuationInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }
}