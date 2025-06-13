// DialogueIdleState.cs (더 간결한 버전)
using UnityEngine;

public class DialogueIdleState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: Idle. All UIs should have been hidden by FinalizeDialogue.");
        // FinalizeDialogue에서 모든 UI 숨김 처리를 하므로, 여기서는 특별히 할 일이 없을 수 있음.
        // 만약 여기서도 확실하게 숨기고 싶다면 이전 코드처럼 UI 참조를 가져와 Show(false) 호출.
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // Idle 상태에서는 Update 로직 없음.
    }

    public void ExitState(DialogueManager dialogueManager)
    {
        // Debug.Log("Exiting Idle State");
    }
}