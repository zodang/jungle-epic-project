// DialogueShowingLineState.cs
using UnityEngine;

public class DialogueShowingLineState : IDialogueState
{
    public void EnterState(DialogueManager dialogueManager)
    {
        // Debug.Log("DM State: ShowingLine");
        // 현재 준비된 대사를 UI에 표시
        dialogueManager.DisplayCurrentLineOnUI();
    }

    public void UpdateState(DialogueManager dialogueManager)
    {
        // 다음 대사로 넘어가는 입력 처리 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 입력 잠금 플래그 확인 (DialogueManager의 Update에서 justStartedInputLock 처리)
            // if (dialogueManager.justStartedDialogueInputLock) return; // 이 로직은 DM.Update로 이동

            dialogueManager.AdvanceDialogue(); // 다음 대사/선택지/종료로 진행
        }
    }

    public void ExitState(DialogueManager dialogueManager) { }
}