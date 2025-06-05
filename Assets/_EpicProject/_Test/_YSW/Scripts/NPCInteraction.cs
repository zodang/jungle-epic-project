// NPCInteraction.cs
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Tooltip("이 NPC와 처음 대화할 때 시작될 대화의 ID (JSON 파일에 정의된 ID)")]
    public string initialDialogueId;

    [Tooltip("말풍선이 표시될 NPC의 기준점 Transform (보통 NPC 머리 위 빈 오브젝트)")]
    public Transform speechBubbleAnchor; // NPC 말풍선 앵커

    private bool playerInRange = false; // 플레이어가 상호작용 범위 내에 있는지 여부

    void Awake()
    {
        // speechBubbleAnchor가 할당되지 않았거나, 할당되었지만 비활성화된 경우 등 처리
        if (speechBubbleAnchor == null)
        {
            // 자식 중에 "SpeechBubbleAnchor" (또는 유사한 이름)를 찾아본다.
            Transform anchorInChildren = transform.Find("SpeechBubbleAnchor");
            if (anchorInChildren != null)
            {
                speechBubbleAnchor = anchorInChildren;
            }
            else
            {
                // 그래도 없으면 NPC 자신의 Transform을 사용하고 경고를 남긴다.
                speechBubbleAnchor = transform;
                Debug.LogWarning($"NPCInteraction on '{gameObject.name}': SpeechBubbleAnchor not set and 'SpeechBubbleAnchor' child not found. Using NPC's root transform. This might not be visually ideal.");
            }
        }
    }

    // 플레이어가 상호작용을 시도할 때 호출될 수 있는 함수 (예: 플레이어 스크립트에서 호출)
    // 또는 아래 OnTrigger/Update 로직을 통해 자체적으로 호출
    public void InteractWithNPC()
    {
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(initialDialogueId) && speechBubbleAnchor != null)
        {
            // DialogueManager에게 대화 시작 요청
            DialogueManager.Instance.StartDialogue(initialDialogueId, speechBubbleAnchor);
        }
        else
        {
            if (DialogueManager.Instance == null)
                Debug.LogError($"NPCInteraction on '{gameObject.name}': DialogueManager.Instance is null.");
            if (string.IsNullOrEmpty(initialDialogueId))
                Debug.LogWarning($"NPCInteraction on '{gameObject.name}': InitialDialogueId is not set.");
            if (speechBubbleAnchor == null)
                Debug.LogWarning($"NPCInteraction on '{gameObject.name}': SpeechBubbleAnchor is null.");
        }
    }

    // 플레이어가 NPC의 상호작용 범위(Collider2D Trigger)에 들어왔을 때
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 충돌한 오브젝트의 태그가 "Player"인지 확인
        {
            playerInRange = true;
            // 여기에 "E키를 눌러 대화" 같은 UI 힌트를 표시하는 로직 추가 가능
            // 예: UIManager.Instance.ShowInteractionPrompt(true);
            Debug.Log($"Player entered interaction range of {gameObject.name}.");
        }
    }

    // 플레이어가 NPC의 상호작용 범위에서 벗어났을 때
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // UI 힌트 숨기기
            // 예: UIManager.Instance.ShowInteractionPrompt(false);
            Debug.Log($"Player exited interaction range of {gameObject.name}.");

            // 만약 플레이어가 범위를 벗어날 때 진행 중인 대화가 있다면 강제 종료할 수도 있음 (선택 사항)
            // if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActiveWith(this.speechBubbleAnchor)) // 이런 확인 함수 필요
            // {
            //     DialogueManager.Instance.EndDialogue();
            // }
        }
    }

    // 매 프레임 호출 (플레이어 입력 감지용)
    void Update()
    {
        if (playerInRange) // 플레이어가 범위 내에 있을 때만 로그를 남기도록 수정
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("<NPCInteraction> Space key pressed while playerInRange."); // 스페이스바 입력 감지 로그

                if (DialogueManager.Instance != null)
                {
                    bool isDialogueActive = DialogueManager.Instance.IsDialogueActive();
                    bool wasDialogueJustEnded = DialogueManager.Instance.WasDialogueJustEndedThisFrame();
                    Debug.Log($"<NPCInteraction> Checking conditions: IsDialogueActive={isDialogueActive}, WasDialogueJustEndedThisFrame={wasDialogueJustEnded}");

                    if (!isDialogueActive && !wasDialogueJustEnded)
                    {
                        Debug.Log("<NPCInteraction> Conditions met. Calling InteractWithNPC().");
                        InteractWithNPC();
                    }
                    else
                    {
                        Debug.Log("<NPCInteraction> Conditions NOT met. Not calling InteractWithNPC().");
                        if (isDialogueActive) Debug.Log("<NPCInteraction> Reason: Dialogue is already active.");
                        if (wasDialogueJustEnded) Debug.Log("<NPCInteraction> Reason: Dialogue just ended this frame.");
                    }
                }
                else
                {
                    Debug.LogError("<NPCInteraction> DialogueManager.Instance is null!");
                }
            }
        }
    }
}