// DialogueIdleState.cs
using UnityEngine;

public class DialogueIdleState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: Idle");
        if (dialogueManager.CurrentDialogueUI != null)
        {
            dialogueManager.CurrentDialogueUI.Show(false);
        }
        // DialogueManager의 IsDialogueActive()가 이 상태를 반영하도록 함
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // NPCInteraction이 DialogueManager.StartDialogue()를 호출하면
        // DialogueManager 내부에서 상태가 변경됨.
        // 이 상태에서는 별도의 Update 로직이 필요 없음.
    }

    public void ExitState(DialogueManager dialogueManager)
    {
        // 예를 들어, Idle 상태에서 벗어날 때 특정 UI를 활성화해야 한다면 여기서 처리
    }
}