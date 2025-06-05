using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text; // For StringBuilder
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [Header("UI Setup")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform canvasTransform;

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName = "dialogues";
    [SerializeField] private bool pauseGameDuringDialogue = true;

    // --- Constants for string literals ---
    private const string PLAYER_TAG = "Player";
    private const string PLAYER_SPEECH_ANCHOR_NAME = "PlayerSpeechAnchor";
    private const string DIALOGUE_TEXT_UI_NAME = "DialogueText";
    private const string SPEAKER_NAME_TEXT_UI_NAME = "SpeakerNameText";
    private const string PLAYER_SPEAKER_ID = "당신";

    private DialogueCollection dialogueCollection;
    private Queue<DialogueLine> currentDialogueLines;
    private List<DialogueChoice> currentChoices;

    private GameObject currentSpeechBubbleInstance;
    private TextMeshProUGUI dialogueTextUI;
    private TextMeshProUGUI speakerNameTextUI;

    private Transform currentNpcSpeakerAnchor;
    private Transform playerSpeechAnchor;
    private Transform currentBubbleTargetAnchor;

    private bool dialogueActive = false;
    private bool justStartedDialogue = false;
    private bool dialogueJustEnded = false; // 대화가 방금 종료되었는지 확인하는 플래그
    private bool isChoosing = false;
    private int currentSelectedChoiceIndex = 0;

    private StringBuilder choiceStringBuilder = new StringBuilder(); // For UpdateChoicesVisual

    public bool IsDialogueActive() => dialogueActive;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentDialogueLines = new Queue<DialogueLine>();
        LoadDialogueData();
        FindPlayerAnchor();
    }
    public bool WasDialogueJustEndedThisFrame()
    {
        return dialogueJustEnded;
    }

    void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(dialogueFileName);
        if (jsonFile != null)
        {
            dialogueCollection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
            if (dialogueCollection?.dialogues == null) // Null-conditional operator
            {
                Debug.LogError($"DialogueManager: Failed to parse JSON from '{dialogueFileName}'.");
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: Failed to load 'Resources/{dialogueFileName}.json'.");
        }
    }

    void FindPlayerAnchor()
    {
        GameObject playerObj = GameObject.FindWithTag(PLAYER_TAG);
        if (playerObj != null)
        {
            Transform anchor = playerObj.transform.Find(PLAYER_SPEECH_ANCHOR_NAME);
            playerSpeechAnchor = anchor ?? playerObj.transform; // Null-coalescing operator
            if (anchor == null)
            {
                Debug.LogWarning($"DialogueManager: '{PLAYER_SPEECH_ANCHOR_NAME}' not found. Using Player's root.");
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: Player object with tag '{PLAYER_TAG}' not found.");
        }
    }

    private bool InitializeSpeechBubble()
    {
        if (currentSpeechBubbleInstance != null) return true; // Already initialized

        if (speechBubblePrefab == null || canvasTransform == null)
        {
            Debug.LogError("DialogueManager: SpeechBubblePrefab or CanvasTransform not set.");
            return false;
        }

        currentSpeechBubbleInstance = Instantiate(speechBubblePrefab, canvasTransform);

        Transform dialogueTextObj = currentSpeechBubbleInstance.transform.Find(DIALOGUE_TEXT_UI_NAME);
        if (dialogueTextObj != null) dialogueTextUI = dialogueTextObj.GetComponent<TextMeshProUGUI>();

        Transform speakerNameTextObj = currentSpeechBubbleInstance.transform.Find(SPEAKER_NAME_TEXT_UI_NAME);
        if (speakerNameTextObj != null) speakerNameTextUI = speakerNameTextObj.GetComponent<TextMeshProUGUI>();

        if (dialogueTextUI == null)
        {
            Debug.LogError($"DialogueManager: '{DIALOGUE_TEXT_UI_NAME}' UI not found in prefab.");
            Destroy(currentSpeechBubbleInstance); // Clean up partially created instance
            return false;
        }
        if (speakerNameTextUI == null)
        {
            Debug.LogWarning($"DialogueManager: '{SPEAKER_NAME_TEXT_UI_NAME}' UI not found. Speaker name won't be shown.");
        }
        return true;
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        if (dialogueCollection?.dialogues == null)
        {
            Debug.LogError("DialogueManager: Dialogue data not loaded or invalid.");
            return;
        }

        DialogueEntry entry = dialogueCollection.dialogues.FirstOrDefault(d => d.id == dialogueId);
        if (entry == null)
        {
            Debug.LogWarning($"DialogueManager: Dialogue ID '{dialogueId}' not found.");
            EndDialogue();
            return;
        }

        if (!InitializeSpeechBubble()) { EndDialogue(); return; } // 말풍선 초기화 실패 시 종료

        this.currentNpcSpeakerAnchor = npcSpeechAnchor;
        dialogueActive = true;
        isChoosing = false; // 대화 시작 시 선택 모드 해제
        justStartedDialogue = true;
        if (pauseGameDuringDialogue) Time.timeScale = 0f;

        currentDialogueLines.Clear();
        foreach (var line in entry.lines)
        {
            currentDialogueLines.Enqueue(line);
        }
        currentChoices = entry.choices;

        currentSpeechBubbleInstance.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (!dialogueActive) return;

        if (currentDialogueLines.Count == 0)
        {
            if (currentChoices != null && currentChoices.Count > 0)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        DialogueLine currentLine = currentDialogueLines.Dequeue();

        // dialogueTextUI는 InitializeSpeechBubble에서 null 체크됨
        dialogueTextUI.text = currentLine.text;

        if (speakerNameTextUI != null)
        {
            if (!string.IsNullOrEmpty(currentLine.speaker))
            {
                speakerNameTextUI.text = currentLine.speaker;
                speakerNameTextUI.gameObject.SetActive(true);
            }
            else
            {
                speakerNameTextUI.gameObject.SetActive(false);
            }
        }

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
        // PositionSpeechBubble(); // 말풍선 위치는 Update에서 지속적으로 처리
    }

    void PositionSpeechBubble()
    {
        if (currentSpeechBubbleInstance == null || !currentSpeechBubbleInstance.activeSelf || currentBubbleTargetAnchor == null || Camera.main == null) return;
        currentSpeechBubbleInstance.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(currentBubbleTargetAnchor.position);
    }

    void ShowChoices()
    {
        // dialogueTextUI와 currentChoices는 DisplayNextLine에서 이미 확인됨
        isChoosing = true;
        currentSelectedChoiceIndex = 0;

        if (speakerNameTextUI != null)
        {
            speakerNameTextUI.text = PLAYER_SPEAKER_ID; // 또는 원하는 플레이어 표시 이름
            speakerNameTextUI.gameObject.SetActive(true);
        }

        if (playerSpeechAnchor != null)
        {
            currentBubbleTargetAnchor = playerSpeechAnchor;
            // PositionSpeechBubble(); // Update에서 처리
        }
        UpdateChoicesVisual();
    }

    void UpdateChoicesVisual()
    {
        // dialogueTextUI와 currentChoices는 ShowChoices 호출 전에 확인됨
        choiceStringBuilder.Clear();
        for (int i = 0; i < currentChoices.Count; i++)
        {
            choiceStringBuilder.Append(i == currentSelectedChoiceIndex ? "> " : "  ");
            choiceStringBuilder.AppendLine(currentChoices[i].text);
        }
        if (choiceStringBuilder.Length > 0 && choiceStringBuilder[choiceStringBuilder.Length - 1] == '\n')
        {
            choiceStringBuilder.Length--; // 마지막 줄바꿈 제거
        }
        dialogueTextUI.text = choiceStringBuilder.ToString();
    }

    public void SelectChoice(string nextDialogueId)
    {
        isChoosing = false;
        if (string.IsNullOrEmpty(nextDialogueId))
        {
            EndDialogue();
        }
        else
        {
            StartDialogue(nextDialogueId, currentNpcSpeakerAnchor);
        }
    }

    public void EndDialogue()
    {
        if (currentSpeechBubbleInstance != null) currentSpeechBubbleInstance.SetActive(false);
        dialogueActive = false;
        isChoosing = false;
        dialogueJustEnded = true; // <--- 대화 종료 시 플래그 설정!
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        currentNpcSpeakerAnchor = null;
        currentBubbleTargetAnchor = null;
        currentDialogueLines?.Clear();
        currentChoices = null;

    }

    void Update()
    {
        // 이 플래그는 항상 가장 먼저 처리되어야 함
        if (dialogueJustEnded)
        {
            dialogueJustEnded = false; // 다음 프레임에는 영향 없도록 리셋
                                       // 이 프레임에는 NPCInteraction이 새 대화를 시작하지 못하도록 하기 위함
                                       // dialogueActive는 이미 false이므로 아래 로직은 실행 안 됨
        }

        if (justStartedDialogue)
        {
            justStartedDialogue = false;
            // dialogueActive는 true인 상태지만, 이 프레임의 입력은 무시
            return;
        }

        if (!dialogueActive) return; // 이제 이 이후는 dialogueActive가 true일 때만 실행

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
            else if (currentChoices.Count > 1) { currentSelectedChoiceIndex = currentChoices.Count - 1; selectionChanged = true; } // 순환
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentSelectedChoiceIndex < currentChoices.Count - 1) { currentSelectedChoiceIndex++; selectionChanged = true; }
            else if (currentChoices.Count > 1) { currentSelectedChoiceIndex = 0; selectionChanged = true; } // 순환
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSelectedChoiceIndex >= 0 && currentSelectedChoiceIndex < currentChoices.Count)
            {
                SelectChoice(currentChoices[currentSelectedChoiceIndex].nextDialogueId);
            }
            return;
        }

        if (selectionChanged) UpdateChoicesVisual();
    }

    void HandleDialogueContinuationInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 또는 통합한 키
        {
            DisplayNextLine();
        }
    }
}