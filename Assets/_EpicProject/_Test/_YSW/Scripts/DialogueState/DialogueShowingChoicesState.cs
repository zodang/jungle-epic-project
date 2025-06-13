// DialogueShowingChoicesState.cs
using UnityEngine;

public class DialogueShowingChoicesState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        dialogueManager.CurrentSelectedChoiceIndex = 0;
        dialogueManager.DisplayChoicesOnChoiceBubble(); // 선택지는 즉시 표시됨
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        bool selectionChanged = false;
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (dialogueManager.CurrentSelectedChoiceIndex > 0)
            {
                dialogueManager.CurrentSelectedChoiceIndex--;
                selectionChanged = true;
            }
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1)
            {
                dialogueManager.CurrentSelectedChoiceIndex = dialogueManager.CurrentChoices.Count - 1;
                selectionChanged = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentSelectedChoiceIndex < dialogueManager.CurrentChoices.Count - 1)
            {
                dialogueManager.CurrentSelectedChoiceIndex++;
                selectionChanged = true;
            }
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1)
            {
                dialogueManager.CurrentSelectedChoiceIndex = 0;
                selectionChanged = true;
            }
        }
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    // 선택지 UI에는 타이핑 효과가 없으므로, 스킵 로직 불필요
        //    if (dialogueManager.CurrentChoices != null &&
        //        dialogueManager.CurrentSelectedChoiceIndex >= 0 &&
        //        dialogueManager.CurrentSelectedChoiceIndex < dialogueManager.CurrentChoices.Count)
        //    {
        //        dialogueManager.SelectCurrentChoice();
        //    }
        //    return;
        //}

        if (selectionChanged)
        {
            dialogueManager.UpdateChoiceSelectionVisualOnChoiceBubble();
        }
    }

    public void ExitState(DialogueManager dialogueManager)
    {
        // 선택지 말풍선 숨기기는 SelectCurrentChoice 또는 FinalizeDialogue에서 처리됨
    }
}