// DialogueShowingChoicesState.cs
using UnityEngine;

public class DialogueShowingChoicesState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: ShowingChoices");
        dialogueManager.CurrentSelectedChoiceIndex = 0; // 선택지 인덱스 초기화

        // 일반 대화 말풍선은 이전 내용을 유지 (숨기거나 변경하지 않음)
        // 선택지 전용 말풍선에 선택지 표시
        dialogueManager.DisplayChoicesOnChoiceBubble();
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        bool selectionChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (dialogueManager.CurrentSelectedChoiceIndex > 0)
            {
                dialogueManager.CurrentSelectedChoiceIndex--;
                selectionChanged = true;
            }
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1)
            { // 순환
                dialogueManager.CurrentSelectedChoiceIndex = dialogueManager.CurrentChoices.Count - 1;
                selectionChanged = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentSelectedChoiceIndex < dialogueManager.CurrentChoices.Count - 1)
            {
                dialogueManager.CurrentSelectedChoiceIndex++;
                selectionChanged = true;
            }
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1)
            { // 순환
                dialogueManager.CurrentSelectedChoiceIndex = 0;
                selectionChanged = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueManager.SelectCurrentChoice(); // 선택 확정
            return;
        }

        if (selectionChanged)
        {
            dialogueManager.UpdateChoiceSelectionVisualOnChoiceBubble(); // 선택지 말풍선 UI 업데이트
        }
    }

    public void ExitState(DialogueManager dialogueManager)
    {
        // Debug.Log("Exiting ShowingChoices State");
        // 선택지 말풍선 숨기기는 SelectCurrentChoice 또는 FinalizeDialogue에서 처리됨
        // (SelectCurrentChoice에서 다음 상태로 넘어가기 전에 숨기거나,
        //  FinalizeDialogue에서 모든 UI를 숨길 때 함께 처리)
        // 좀 더 명확하게 하려면 여기서도 숨기는 호출을 할 수 있지만, 중복 호출될 수 있음.
        // 현재는 SelectCurrentChoice와 FinalizeDialogue에서 처리하는 것으로 가정.
        if (dialogueManager.CurrentChoiceBubbleUI != null && dialogueManager.CurrentChoiceBubbleUI.gameObject.activeInHierarchy)
        {
            // dialogueManager.CurrentChoiceBubbleUI.Show(false); // 여기서 바로 숨길 수도 있음
        }
    }
}