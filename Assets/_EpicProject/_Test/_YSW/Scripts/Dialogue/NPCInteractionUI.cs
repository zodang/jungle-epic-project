// NpcInteraction.cs (두 개의 UI를 모두 제어하도록 수정됨)
using UnityEngine;

public class NpcInteractionUI : MonoBehaviour
{
    // [수정] 두 개의 UI를 모두 연결할 변수
    public GameObject interactionPromptCanvas; // "E키 눌러 대화" UI
    public GameObject dialogueIndicator;      // "..." 말풍선 아이콘 UI

    private bool isPlayerInRange = false;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        // [수정] UI 초기 상태 설정
        interactionPromptCanvas?.SetActive(false); // 'E키' 프롬프트는 기본적으로 숨김
        dialogueIndicator?.SetActive(true);      // '대화 가능' 아이콘은 기본적으로 보임
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 대화 중이 아닐 때만 'E키' 프롬프트를 보여줌
            if (dialogueManager != null && !dialogueManager.IsDialogueActive())
            {
                interactionPromptCanvas?.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 범위를 벗어나면 'E키' 프롬프트는 무조건 숨김
            interactionPromptCanvas?.SetActive(false);
        }
    }

    // [이름 변경 및 로직 통합] 대화 시작 시 호출될 함수
    public void OnDialogueStarted()
    {
        // 대화가 시작되면 모든 상호작용 관련 UI를 숨깁니다.
        interactionPromptCanvas?.SetActive(false);
        dialogueIndicator?.SetActive(false);
    }

    // [이름 변경 및 로직 통합] 대화 종료 시 호출될 함수
    public void OnDialogueEnded()
    {
        // 대화가 끝나면 '대화 가능' 아이콘을 다시 보여줍니다.
        dialogueIndicator?.SetActive(true);

        // 만약 플레이어가 여전히 범위 안에 있다면 'E키' 프롬프트도 다시 보여줍니다.
        if (isPlayerInRange)
        {
            interactionPromptCanvas?.SetActive(true);
        }
    }

    // Update 함수는 변경 없음
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && (dialogueManager != null && !dialogueManager.IsDialogueActive()))
        {
            // 실제 대화 시작 로직 호출
            // 예시: dialogueManager.StartDialogue("npc_id_01", this.transform);
        }
    }
}