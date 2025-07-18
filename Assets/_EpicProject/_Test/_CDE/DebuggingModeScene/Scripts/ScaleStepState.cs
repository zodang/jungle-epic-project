public class ScaleStepState : FSMState
{
    private string[] _dialogueIdList = new[] { "step_2_1_player", "step_2_2_player", "step_2_3_player" };
    private string[] _consoleIdList = new[] { "console_scale_broken", "console_scale_solved"};
    
    private PuzzleFSM _puzzleFsm;
    private BrokenEmotionBlock _target;

    public ScaleStepState(PuzzleFSM puzzleFsm, BrokenEmotionBlock targetObj)
    {
        _puzzleFsm = puzzleFsm;
        _target = targetObj;
    }
    
    public override void Enter()
    {
        _puzzleFsm.OnBlockCorrect += ChangeToLeadDialogue;

        _target.OnScaleCorrect += _puzzleFsm.ChangeToNextStep;
        _target.OnScaleCorrect += ChangeToCorrectDialogue;
        
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[0]);
        _puzzleFsm.ChangeConsoleText(_consoleIdList[0]);
    }

    public override void Exit()
    {
        _target.OnScaleCorrect -= _puzzleFsm.ChangeToNextStep;
        _puzzleFsm.OnBlockCorrect -= ChangeToLeadDialogue;
        
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[2]);
        _puzzleFsm.ChangeConsoleText(_consoleIdList[1]);
    }

    private void ChangeToLeadDialogue()
    {
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[1]);
    }

    private void ChangeToCorrectDialogue()
    {
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[2]);
    }
}
