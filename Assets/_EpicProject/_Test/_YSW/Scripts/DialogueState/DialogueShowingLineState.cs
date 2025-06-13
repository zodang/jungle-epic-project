// DialogueShowingLineState.cs
using UnityEngine;

public class DialogueShowingLineState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        dialogueManager.DisplayCurrentLineOnActiveBubble(); // 이름 변경된 함수 호출
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    DialogueUI currentBubble = dialogueManager.GetCurrentActiveDialogueBubble(); // 헬퍼 함수 사용
        //    if (currentBubble != null && currentBubble.IsTyping())
        //    {
        //        currentBubble.CompleteTyping();
        //    }
        //    else
        //    {
        //        dialogueManager.AdvanceDialogue();
        //    }
        //}
    }
    public void ExitState(DialogueManager dialogueManager) { }
}