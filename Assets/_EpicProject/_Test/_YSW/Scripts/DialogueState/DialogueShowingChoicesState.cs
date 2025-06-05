// DialogueShowingChoicesState.cs
using UnityEngine;

public class DialogueShowingChoicesState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: ShowingChoices");
        dialogueManager.CurrentSelectedChoiceIndex = 0; // 선택지 인덱스 초기화

        if (dialogueManager.CurrentDialogueUI != null)
        {
            dialogueManager.CurrentDialogueUI.SetSpeakerName(DialogueManager.PLAYER_SPEAKER_ID_CONST); // 상수 사용
            dialogueManager.CurrentDialogueUI.UpdateChoicesVisual(dialogueManager.CurrentChoices, dialogueManager.CurrentSelectedChoiceIndex);
        }

        if (dialogueManager.PlayerSpeechAnchor != null) // DialogueManager에 PlayerSpeechAnchor getter 필요
        {
            dialogueManager.CurrentBubbleTargetAnchor = dialogueManager.PlayerSpeechAnchor;
        }
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // 입력 잠금 플래그 확인 (DialogueManager의 Update에서 justStartedInputLock 처리)
        // if (dialogueManager.justStartedDialogueInputLock) return;

        bool selectionChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (dialogueManager.CurrentSelectedChoiceIndex > 0)
            {
                dialogueManager.CurrentSelectedChoiceIndex--;
                selectionChanged = true;
            }
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1) // 순환
            {
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
            else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 1) // 순환
            {
                dialogueManager.CurrentSelectedChoiceIndex = 0;
                selectionChanged = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueManager.SelectCurrentChoice(); // 선택 확정
            return; // 입력 처리 후 종료
        }

        if (selectionChanged)
        {
            dialogueManager.UpdateChoiceSelectionVisual();
        }
    }

    public void ExitState(DialogueManager dialogueManager)
    {
        // 선택지 UI 정리 (만약 별도의 UI 요소들을 동적으로 생성했다면)
        // if (dialogueManager.CurrentDialogueUI != null)
        // {
        //     dialogueManager.CurrentDialogueUI.ClearChoiceDisplayElements();
        // }
    }
}