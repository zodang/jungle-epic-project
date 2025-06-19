public interface IDialogueState
{
    /// <summary>
    /// 이 상태에 진입했을 때 호출됩니다.
    /// </summary>
    void EnterState(DialogueManager dialogueManager);

    /// <summary>
    /// 이 상태가 활성화되어 있는 동안 매 프레임 호출됩니다.
    /// </summary>
    void UpdateState(DialogueManager dialogueManager);

    /// <summary>
    /// 이 상태에서 빠져나갈 때 호출됩니다.
    /// </summary>
    void ExitState(DialogueManager dialogueManager);
}