// NPCInteraction.cs
using UnityEngine;
using System.Collections.Generic; // List 사용을 위해 추가

public class NPCInteraction : MonoBehaviour
{
    [Header("Default Dialogue")]
    [Tooltip("조건에 해당하지 않을 때 사용될 기본 대화 ID")]
    public string defaultDialogueId;

    // 조건부 대화 항목을 위한 내부 클래스
    [System.Serializable] // Inspector에 표시되도록
    public class ConditionalDialogueEntry
    {
        [Tooltip("이 조건이 만족될 때 시작될 대화 ID")]
        public string dialogueId;
        [Tooltip("이 조건을 만족하기 위해 확인해야 할 플래그의 이름 (FlagManager에서 사용)")]
        public string requiredFlagName;
        [Tooltip("플래그가 이 값이어야 조건 만족 (true 또는 false)")]
        public bool requiredFlagValue = true; // 기본적으로 플래그가 true일 때 조건 만족
        [Tooltip("이 조건의 우선순위 (낮을수록 먼저 체크, 선택사항)")]
        public int priority = 0;
    }

    [Header("Conditional Dialogues (우선순위 순서대로 정렬 또는 priority 사용)")]
    public List<ConditionalDialogueEntry> conditionalDialogues = new List<ConditionalDialogueEntry>();

    [Tooltip("말풍선이 표시될 NPC의 기준점 Transform")]
    public Transform speechBubbleAnchor;

    private bool playerInRange = false;

    void Awake()
    {
        if (speechBubbleAnchor == null)
        {
            Transform anchorInChildren = transform.Find("SpeechBubbleAnchor"); // 프리팹 내 자식 이름 고정
            speechBubbleAnchor = anchorInChildren ?? transform;
            if (anchorInChildren == null)
                Debug.LogWarning($"NPCInteraction on '{gameObject.name}': Using NPC's root for speech anchor. Consider adding 'SpeechBubbleAnchor' child.");
        }

        // 우선순위에 따라 정렬 (선택 사항, Inspector에서 직접 순서 조정도 가능)
         conditionalDialogues.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG)) // DialogueManager의 상수 사용
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG))
        {
            playerInRange = false;
        }
    }

    private string GetDialogueIdBasedOnConditions()
    {
        if (StageManager.Instance.FlagManager == null)
        {
            Debug.LogWarning($"<NPCInteraction> StageManager.Instance.FlagManager is null on '{gameObject.name}'. Cannot check conditions. Returning default dialogue: {defaultDialogueId}");
            return defaultDialogueId;
        }

        // 설정된 조건부 대화들을 순서대로 확인 (리스트의 순서가 우선순위)
        foreach (var conditionEntry in conditionalDialogues)
        {
            if (string.IsNullOrEmpty(conditionEntry.requiredFlagName) || string.IsNullOrEmpty(conditionEntry.dialogueId))
            {
                // 필수 정보 누락 시 이 조건은 건너뜀 (또는 경고)
                // Debug.LogWarning($"<NPCInteraction> Conditional entry on '{gameObject.name}' is missing requiredFlagName or dialogueId.");
                continue;
            }

            // FlagManager를 통해 플래그 상태 확인
            bool flagState = StageManager.Instance.FlagManager.IsFlagSet(conditionEntry.requiredFlagName);

            // 플래그 상태가 요구되는 값과 일치하는지 확인
            if (flagState == conditionEntry.requiredFlagValue)
            {
                Debug.Log($"<NPCInteraction> Condition met for '{gameObject.name}': Flag '{conditionEntry.requiredFlagName}' is {flagState} (required: {conditionEntry.requiredFlagValue}). Using dialogue: {conditionEntry.dialogueId}");
                return conditionEntry.dialogueId;
            }
        }

        // 모든 특정 조건에 해당하지 않으면 기본 대화 ID 반환
        // Debug.Log($"<NPCInteraction> No specific conditions met for '{gameObject.name}'. Using default dialogue: {defaultDialogueId}");
        return defaultDialogueId;
    }

    public void InteractWithNPC()
    {
        string dialogueIdToStart = GetDialogueIdBasedOnConditions();

        if (StageBaseManager.Instance.DialogueManager != null && !string.IsNullOrEmpty(dialogueIdToStart) && speechBubbleAnchor != null)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(dialogueIdToStart, speechBubbleAnchor);
        }
        else
        {
            if (string.IsNullOrEmpty(dialogueIdToStart))
                Debug.LogWarning($"NPCInteraction on '{gameObject.name}': No suitable dialogue ID determined by conditions for InteractWithNPC.");
            if (StageBaseManager.Instance.DialogueManager == null) Debug.LogError($"NPCInteraction on '{gameObject.name}': StageBaseManager.Instance.DialogueManager is null.");
            if (speechBubbleAnchor == null) Debug.LogWarning($"NPCInteraction on '{gameObject.name}': SpeechBubbleAnchor is null.");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Space)) // 또는 다른 상호작용 키
        {
            if (StageBaseManager.Instance.DialogueManager != null)
            {
                if (!StageBaseManager.Instance.DialogueManager.IsDialogueActive() &&
                    !StageBaseManager.Instance.DialogueManager.WasDialogueJustEndedThisFrame())
                {
                    InteractWithNPC();
                }
            }
        }
    }
}