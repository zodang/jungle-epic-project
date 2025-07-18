// NpcInteractionUI.cs
using UnityEngine;

public class NpcInteractionUI : MonoBehaviour
{
    // ... (interactionPromptCanvas, dialogueIndicator 변수) ...
    public GameObject interactionPromptCanvas;
    public GameObject dialogueIndicator;


    private bool isPlayerInRange = false;
    // private DialogueManager dialogueManager; // 이제 StageBaseManager를 통해 접근
    private NPCInteraction npcInteraction;

    private void Awake()
    {
        npcInteraction = GetComponent<NPCInteraction>();
    }

    private void Start()
    {
        if (StageBaseManager.Instance == null || StageBaseManager.Instance.DialogueManager == null) // <--- StageBaseManager 통해 접근
        {
            Debug.LogError($"NpcInteractionUI on '{gameObject.name}': StageBaseManager.Instance or its DialogueManager not found!");
            enabled = false;
            return;
        }

        if (npcInteraction == null)
        {
            Debug.LogError($"NpcInteractionUI on '{gameObject.name}': NPCInteraction component not found!");
            enabled = false;
            return;
        }

        interactionPromptCanvas?.SetActive(false);
        dialogueIndicator?.SetActive(true);
    }

    private void OnEnable()
    {
        if (StageBaseManager.Instance != null && StageBaseManager.Instance.DialogueManager != null)
        {
            StageBaseManager.Instance.DialogueManager.OnDialogueStart.AddListener(HandleDialogueStarted);
            StageBaseManager.Instance.DialogueManager.OnDialogueEnd.AddListener(HandleDialogueEnded);
        }
    }

    private void OnDisable()
    {
        // StageBaseManager나 DialogueManager가 먼저 파괴될 수 있으므로 null 체크 필요
        if (StageBaseManager.Instance != null && StageBaseManager.Instance.DialogueManager != null)
        {
            StageBaseManager.Instance.DialogueManager.OnDialogueStart.RemoveListener(HandleDialogueStarted);
            StageBaseManager.Instance.DialogueManager.OnDialogueEnd.RemoveListener(HandleDialogueEnded);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG))
        {
            isPlayerInRange = true;
            UpdateInteractionPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(DialogueManager.PLAYER_TAG))
        {
            isPlayerInRange = false;
            interactionPromptCanvas?.SetActive(false);
        }
    }

    private void HandleDialogueStarted()
    {
        interactionPromptCanvas?.SetActive(false);
        dialogueIndicator?.SetActive(false);
    }

    private void HandleDialogueEnded()
    {
        dialogueIndicator?.SetActive(true);
        UpdateInteractionPrompt();
    }

    private void UpdateInteractionPrompt()
    {
        // DialogueManager는 StageBaseManager를 통해 접근
        if (isPlayerInRange && StageBaseManager.Instance != null && StageBaseManager.Instance.DialogueManager != null &&
            !StageBaseManager.Instance.DialogueManager.IsDialogueActive())
        {
            interactionPromptCanvas?.SetActive(true);
        }
        else
        {
            interactionPromptCanvas?.SetActive(false);
        }
    }


}