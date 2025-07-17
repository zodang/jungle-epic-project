// NpcInteractionUI.cs (최종 수정)
using UnityEngine;

public class NpcInteractionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject interactionPromptCanvas;

    [Header("Indicators")]
    [Tooltip("새로운 대화가 있음을 나타내는 아이콘 (예: 노란 느낌표)")]
    public GameObject newDialogueIndicator;
    [Tooltip("이미 본 대화임을 나타내는 아이콘 (예: 회색 말풍선)")]
    public GameObject talkedDialogueIndicator;

    private bool isPlayerInRange = false;
    private NPCInteraction npcInteraction; // 현재 대화 ID를 가져오기 위해 필수

    private void Awake()
    {
        npcInteraction = GetComponent<NPCInteraction>();
        if (npcInteraction == null)
        {
            Debug.LogError($"NpcInteractionUI on '{gameObject.name}': NPCInteraction component not found on the same GameObject!", this);
            enabled = false;
            return;
        }
        interactionPromptCanvas?.SetActive(false);
    }

    private void Start()
    {
        // 게임 시작 시 초기 인디케이터 상태 설정
        UpdateIndicatorState();
    }

    private void OnEnable()
    {
        StageBaseManager.Instance?.DialogueManager?.OnDialogueStart.AddListener(HandleDialogueStarted);
        StageBaseManager.Instance?.DialogueManager?.OnDialogueEnd.AddListener(HandleDialogueEnded);
        // TODO: 플래그가 변경될 때도 인디케이터를 업데이트해야 함!
        // FlagManager에 이벤트가 있다면 구독
        // StageBaseManager.Instance?.FlagManager?.OnFlagChanged.AddListener(HandleFlagChanged);
    }

    private void OnDisable()
    {
        StageBaseManager.Instance?.DialogueManager?.OnDialogueStart.RemoveListener(HandleDialogueStarted);
        StageBaseManager.Instance?.DialogueManager?.OnDialogueEnd.RemoveListener(HandleDialogueEnded);
        // StageBaseManager.Instance?.FlagManager?.OnFlagChanged.RemoveListener(HandleFlagChanged);
    }

    // FlagManager의 이벤트 핸들러 (FlagManager에 이벤트가 있을 경우)
    private void HandleFlagChanged(string flagName)
    {
        // 변경된 플래그가 이 NPC의 조건과 관련이 있다면 인디케이터 업데이트
        // 또는 간단하게, 플레이어가 범위 내에 있을 때만 업데이트
        if (isPlayerInRange)
        {
            UpdateIndicatorState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG))
        {
            isPlayerInRange = true;
            UpdateInteractionPrompt();
            UpdateIndicatorState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG))
        {
            isPlayerInRange = false;
            interactionPromptCanvas?.SetActive(false);
            // 범위를 벗어나면 인디케이터 숨김 (선택 사항)
            //newDialogueIndicator?.SetActive(false);
            //talkedDialogueIndicator?.SetActive(false);
        }
    }

    private void HandleDialogueStarted()
    {
        interactionPromptCanvas?.SetActive(false);
        newDialogueIndicator?.SetActive(false);
        talkedDialogueIndicator?.SetActive(false);
    }

    private void HandleDialogueEnded()
    {
        UpdateIndicatorState();
        UpdateInteractionPrompt();
    }

    private void UpdateInteractionPrompt()
    {
        if (isPlayerInRange && StageBaseManager.Instance?.DialogueManager != null &&
            !StageBaseManager.Instance.DialogueManager.IsDialogueActive())
        {
            interactionPromptCanvas?.SetActive(true);
        }
        else
        {
            interactionPromptCanvas?.SetActive(false);
        }
    }

    /// <summary>
    /// 현재 NPC가 제공할 대화를 이미 봤는지 확인하여 올바른 인디케이터를 표시합니다.
    /// </summary>
    public void UpdateIndicatorState()
    {
        if (StageBaseManager.Instance?.DialogueManager != null && StageBaseManager.Instance.DialogueManager.IsDialogueActive())
        {
            // 대화 중에는 모든 인디케이터 숨김
            newDialogueIndicator?.SetActive(false);
            talkedDialogueIndicator?.SetActive(false);
            return;
        }

        if (ProgressManager.Instance != null && npcInteraction != null)
        {
            // 1. NPC가 현재 조건에서 어떤 대화를 할지 ID를 가져온다.
            string currentDialogueId = npcInteraction.GetCurrentDialogueId();

            // 2. ProgressManager에게 이 대화 ID를 이미 봤는지 물어본다.
            bool hasSeenThisDialogue = ProgressManager.Instance.HasSeenDialogue(currentDialogueId);

            // 3. 결과에 따라 인디케이터를 설정한다.
            if (newDialogueIndicator != null) newDialogueIndicator.SetActive(!hasSeenThisDialogue);
            if (talkedDialogueIndicator != null) talkedDialogueIndicator.SetActive(hasSeenThisDialogue);
        }
        else
        {
            if (newDialogueIndicator != null) newDialogueIndicator.SetActive(true);
            if (talkedDialogueIndicator != null) talkedDialogueIndicator.SetActive(false);
            if (ProgressManager.Instance == null) Debug.LogWarning("NpcInteractionUI: ProgressManager not found.");
        }
    }

    private void Update()
    {
        // 대화 중이 아닐 때만 인디케이터 상태를 계속 확인하고 업데이트
        if (StageBaseManager.Instance?.DialogueManager != null &&
            !StageBaseManager.Instance.DialogueManager.IsDialogueActive())
        {
            UpdateIndicatorState();
        }
    }
}