// DialogueEndingState.cs
using UnityEngine;

public class DialogueEndingState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: Ending");
        dialogueManager.FinalizeDialogue(); // 모든 정리 작업 및 Idle 상태로 전이
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // 이 상태는 즉시 Idle 상태로 전이되므로 Update 로직이 필요 없음.．
    }

    public void ExitState(DialogueManager dialogueManager) { }
}