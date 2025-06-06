// DialogueShowingLineState.cs
using UnityEngine;

public class DialogueShowingLineState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: ShowingLine");
        dialogueManager.DisplayCurrentLineOnDialogueBubble(); // 일반 대화 말풍선에 표시
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 입력은 여기서 직접 처리 (InputHandler 분리 전)
        {
            dialogueManager.AdvanceDialogue();
        }
    }

    public void ExitState(DialogueManager dialogueManager) { }
}