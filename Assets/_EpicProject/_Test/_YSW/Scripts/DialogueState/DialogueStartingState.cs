// DialogueStartingState.cs
using UnityEngine;

public class DialogueStartingState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: Starting");
        // 대화 시작 시 필요한 초기화 (UI 표시 등은 StartDialogue에서 이미 처리됨)
        // 다음 보여줄 첫 대사를 준비
        dialogueManager.PrepareNextLine();

        if (dialogueManager.CurrentLineToShow != null)
        {
            dialogueManager.TransitionToState(dialogueManager.ShowingLineState);
        }
        else if (dialogueManager.CurrentChoices != null && dialogueManager.CurrentChoices.Count > 0)
        {
            dialogueManager.TransitionToState(dialogueManager.ShowingChoicesState);
        }
        else
        {
            // 보여줄 대사도, 선택지도 없으면 바로 종료 상태로
            dialogueManager.TransitionToState(dialogueManager.EndingState);
        }
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // 이 상태는 보통 즉시 다른 상태로 전이되므로 Update 로직이 필요 없을 수 있음.．
    }

    public void ExitState(DialogueManager dialogueManager) { }
}