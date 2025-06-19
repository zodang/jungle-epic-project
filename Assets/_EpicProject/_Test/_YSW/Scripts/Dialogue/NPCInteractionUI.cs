// NpcInteraction.cs
using UnityEngine;

public class NpcInteractionUI : MonoBehaviour
{
    public GameObject interactionPromptCanvas;
    private bool isPlayerInRange = false;
    private DialogueManager dialogueManager; // DialogueManager 참조 (선택적)

    private void Awake()
    {
        if (interactionPromptCanvas != null)
        {
            interactionPromptCanvas.SetActive(false);
        }
        // DialogueManager를 찾아두면 IsDialogueActive 같은 상태를 직접 확인할 때 유용합니다.
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 대화 중이 아닐 때만 프롬프트를 보여줍니다.
            if (dialogueManager != null && !dialogueManager.IsDialogueActive())
            {
                interactionPromptCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionPromptCanvas.SetActive(false);
        }
    }

    // [이벤트 수신 함수 1] 대화 시작 시 호출됨
    public void HidePrompt()
    {
        if (interactionPromptCanvas != null)
        {
            interactionPromptCanvas.SetActive(false);
        }
    }

    // [이벤트 수신 함수 2] 대화 종료 시 호출됨
    public void ShowPromptIfInRange()
    {
        // 대화가 끝났을 때, 플레이어가 여전히 범위 안에 있다면 프롬프트를 다시 보여줍니다.
        if (isPlayerInRange && interactionPromptCanvas != null)
        {
            interactionPromptCanvas.SetActive(true);
        }
    }

    private void Update()
    {
        // E키 입력은 플레이어가 범위 안에 있고, 대화 중이 아닐 때만 감지합니다.
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && (dialogueManager != null && !dialogueManager.IsDialogueActive()))
        {
            // 실제 대화 시작 로직 호출
            // 예시: dialogueManager.StartDialogue("npc_id_01", this.transform);
        }
    }
}