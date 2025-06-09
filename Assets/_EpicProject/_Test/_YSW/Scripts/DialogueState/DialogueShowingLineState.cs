// DialogueShowingLineState.cs
using UnityEngine;

public class DialogueShowingLineState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        dialogueManager.DisplayCurrentLineOnDialogueBubble();
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isTyping = false;
            if (dialogueManager.CurrentDialogueBubbleUI != null)
            {
                isTyping = dialogueManager.CurrentDialogueBubbleUI.IsTyping();
            }
            Debug.Log($"<ShowingLineState> Space pressed. IsTyping: {isTyping}"); // <--- 이 로그 확인!

            if (isTyping)
            {
                Debug.Log("<ShowingLineState> Finishing typing effect.");
                dialogueManager.CurrentDialogueBubbleUI.CompleteTyping();
            }
            else
            {
                Debug.Log("<ShowingLineState> Advancing dialogue.");
                dialogueManager.AdvanceDialogue();
            }
        }
    }
    public void ExitState(DialogueManager dialogueManager) { }
}