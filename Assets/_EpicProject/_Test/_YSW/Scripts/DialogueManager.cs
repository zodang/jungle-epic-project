using UnityEngine;
using UnityEngine.UI; // Unity UI 기본 사용 시 (Image 등)
using TMPro;          // TextMeshPro 사용 시
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    [Header("UI Setup")]
    public GameObject speechBubblePrefab;   // 말풍선 UI 프리팹
    public Transform canvasTransform;       // 말풍선을 표시할 UI 캔버스 (ScreenSpace-Overlay 권장)

    [Header("Dialogue Settings")]
    public string dialogueFileName = "dialogues"; // Resources 폴더 내 JSON 파일 이름 (확장자 제외)
    public bool pauseGameDuringDialogue = true;   // 대화 중 게임 시간 정지 여부

    // 내부 상태 변수
    private DialogueCollection dialogueCollection;
    private Queue<DialogueLine> currentDialogueLines;
    private List<DialogueChoice> currentChoices;
    private DialogueEntry currentFullDialogueEntry;

    private GameObject currentSpeechBubbleInstance;
    private TextMeshProUGUI dialogueTextUI; // 말풍선 안의 TextMeshProUGUI 컴포넌트

    private Transform currentNpcSpeakerAnchor;    // 대화를 시작한 NPC의 말풍선 기준점
    private Transform playerSpeechAnchor;         // 플레이어의 말풍선 기준점
    private Transform currentBubbleTargetAnchor;  // 현재 말풍선이 따라다닐 실제 기준점

    private bool dialogueActive = false;
    private bool justStartedDialogue = false; // 대화가 방금 시작되었는지 확인하는 플래그

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지 (필요에 따라)
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

    void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(dialogueFileName);
        if (jsonFile != null)
        {
            dialogueCollection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
            if (dialogueCollection == null || dialogueCollection.dialogues == null)
            {
                Debug.LogError($"DialogueManager: Failed to parse JSON data from '{dialogueFileName}'. Check JSON structure and C# classes.");
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: Failed to load dialogue data from 'Resources/{dialogueFileName}.json'. File not found.");
        }
    }

    void FindPlayerAnchor()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // 플레이어 자식 중 "PlayerSpeechAnchor" 이름의 오브젝트를 우선적으로 찾음
            Transform anchor = playerObj.transform.Find("PlayerSpeechAnchor");
            if (anchor != null)
            {
                playerSpeechAnchor = anchor;
            }
            else
            {
                playerSpeechAnchor = playerObj.transform; // 없으면 플레이어 루트 Transform 사용
                Debug.LogWarning("DialogueManager: 'PlayerSpeechAnchor' not found as a child of Player. Using Player's root transform. Consider adding a dedicated anchor.");
            }
        }
        else
        {
            Debug.LogError("DialogueManager: Player object with tag 'Player' not found. Player speech bubble positioning will fail.");
        }
    }

    public void StartDialogue(string dialogueId, Transform npcSpeechAnchor)
    {
        if (dialogueCollection == null || dialogueCollection.dialogues == null)
        {
            Debug.LogError("DialogueManager: Dialogue data not loaded or is invalid. Cannot start dialogue.");
            return;
        }

        currentFullDialogueEntry = dialogueCollection.dialogues.FirstOrDefault(d => d.id == dialogueId);

        if (currentFullDialogueEntry != null)
        {
            this.currentNpcSpeakerAnchor = npcSpeechAnchor;
            dialogueActive = true;
            justStartedDialogue = true; // 플래그 설정

            if (pauseGameDuringDialogue) Time.timeScale = 0f;

            currentDialogueLines.Clear();
            foreach (var line in currentFullDialogueEntry.lines)
            {
                currentDialogueLines.Enqueue(line);
            }
            currentChoices = currentFullDialogueEntry.choices;

            if (currentSpeechBubbleInstance == null && speechBubblePrefab != null && canvasTransform != null)
            {
                currentSpeechBubbleInstance = Instantiate(speechBubblePrefab, canvasTransform);
                dialogueTextUI = currentSpeechBubbleInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (dialogueTextUI == null)
                {
                    Debug.LogError("DialogueManager: SpeechBubblePrefab does not have a TextMeshProUGUI component in its children.");
                    EndDialogue(); // 필수 UI 없으면 진행 불가
                    return;
                }
            }

            if (currentSpeechBubbleInstance != null)
            {
                currentSpeechBubbleInstance.SetActive(true);
            }
            else
            {
                Debug.LogError("DialogueManager: Failed to create or find speech bubble instance.");
                EndDialogue(); // 말풍선 없으면 진행 불가
                return;
            }

            DisplayNextLine();
        }
        else
        {
            Debug.LogWarning($"DialogueManager: Dialogue with ID '{dialogueId}' not found.");
            // ID 못 찾으면 대화 즉시 종료 또는 다른 처리
            EndDialogue();
        }
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

        if (dialogueTextUI == null)
        {
            Debug.LogError("DialogueManager: dialogueTextUI is null. Cannot display text.");
            EndDialogue();
            return;
        }
        dialogueTextUI.text = currentLine.text;

        if (currentLine.speaker.Equals("Player", System.StringComparison.OrdinalIgnoreCase))
        {
            if (playerSpeechAnchor == null)
            {
                Debug.LogError("DialogueManager: PlayerSpeechAnchor is null."); EndDialogue(); return;
            }
            currentBubbleTargetAnchor = playerSpeechAnchor;
        }
        else
        {
            if (currentNpcSpeakerAnchor == null)
            {
                Debug.LogError("DialogueManager: currentNpcSpeakerAnchor is null."); EndDialogue(); return;
            }
            currentBubbleTargetAnchor = currentNpcSpeakerAnchor;
        }

        if (currentSpeechBubbleInstance != null && currentSpeechBubbleInstance.activeSelf)
        {
            PositionSpeechBubble();
        }
    }

    void PositionSpeechBubble()
    {
        if (currentSpeechBubbleInstance == null || currentBubbleTargetAnchor == null || Camera.main == null) return;

        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(currentBubbleTargetAnchor.position);
        RectTransform bubbleRect = currentSpeechBubbleInstance.GetComponent<RectTransform>();

        // 캔버스가 ScreenSpace-Overlay이고 말풍선 Pivot이 (0.5, 0) [하단 중앙] 등으로 잘 설정되어 있다면
        // targetScreenPos를 바로 사용하거나 약간의 Y 오프셋만 주면 됩니다.
        // 말풍선 디자인과 앵커 위치에 따라 오프셋 조정 필요.
        bubbleRect.position = targetScreenPos; // 예: + new Vector3(0, 30, 0); // 약간 위로
    }

    void ShowChoices()
    {
        // 이 부분은 실제 선택지 UI (버튼 등)를 생성하고 표시하는 로직으로 대체되어야 합니다.
        if (dialogueTextUI != null && currentChoices != null)
        {
            string choicePrompt = "선택하세요:\n"; // 또는 다른 안내 문구
            for (int i = 0; i < currentChoices.Count; i++)
            {
                choicePrompt += $"{i + 1}. {currentChoices[i].text}\n";
            }
            dialogueTextUI.text = choicePrompt; // 임시로 말풍선에 선택지 텍스트 나열
            // 실제 UI 버튼을 생성하고, 각 버튼 클릭 시 SelectChoice(choice.nextDialogueId) 호출하도록 연결
        }
        // 현재는 다음 스페이스/클릭 입력을 기다리지 않고, Update에서 숫자키 입력을 통해 SelectChoice가 호출됨.
    }

    public void SelectChoice(string nextDialogueId)
    {
        // 선택지 UI를 숨기거나 제거하는 로직 (예: ClearChoiceButtons();)

        if (string.IsNullOrEmpty(nextDialogueId))
        {
            EndDialogue();
        }
        else
        {
            // 다음 대화는 현재 NPC와 계속한다고 가정.
            StartDialogue(nextDialogueId, currentNpcSpeakerAnchor);
        }
    }

    public void EndDialogue()
    {
        if (currentSpeechBubbleInstance != null)
        {
            currentSpeechBubbleInstance.SetActive(false);
        }
        dialogueActive = false;
        if (pauseGameDuringDialogue) Time.timeScale = 1f;

        // 상태 변수 초기화
        currentNpcSpeakerAnchor = null;
        currentBubbleTargetAnchor = null;
        currentDialogueLines.Clear();
        currentChoices = null;
        currentFullDialogueEntry = null;
    }

    void Update()
    {
        if (!dialogueActive) return; // 대화 중이 아니면 아무것도 안 함

        // 대화가 방금 시작된 프레임이라면, 입력 처리를 건너뛴다.
        if (justStartedDialogue)
        {
            justStartedDialogue = false; // 다음 프레임부터는 정상 처리하도록 플래그 해제
            return;
        }

        if (currentSpeechBubbleInstance != null && currentSpeechBubbleInstance.activeSelf && currentBubbleTargetAnchor != null)
        {
            PositionSpeechBubble(); // 말풍선 위치 지속 업데이트
        }

        // 대화 넘기기 입력 처리 (NPCInteraction에서 사용한 키와 동일하게)
        if (Input.GetKeyDown(KeyCode.Space)) // E 키로 대화 넘기기 (또는 Space 등 원하는 키)
        {
            // 선택지 UI가 활성화된 상태가 아닐 때만 대화 넘기기 (선택지 UI 구현 시 중요)
            // if (!IsChoiceUiActive()) // 이런 함수가 필요할 수 있음
            // {
            DisplayNextLine();
            // }
        }
        // 임시: 선택지를 보여주고 있을 때 숫자키로 선택 (실제 UI 버튼으로 대체 필요)
        else if (currentDialogueLines.Count == 0 && currentChoices != null && currentChoices.Count > 0)
        {
            for (int i = 0; i < currentChoices.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    SelectChoice(currentChoices[i].nextDialogueId);
                    break;
                }
            }
        }
    }
}