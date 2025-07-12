public class SpeedStepState : FSMState
{
    private string[] _dialogueIdList = new[] { "step_3_1_player", "step_3_2_player", "step_3_3_player" };
    private string[] _consoleIdList = new[] { "console_speed_broken", "console_speed_solved"};
    
    private PuzzleFSM _puzzleFsm;
    private FSM<FSMState> _fsm;
    private BrokenEmotionBlock _target;

    public SpeedStepState(PuzzleFSM puzzleFsm, FSM<FSMState> fsm, BrokenEmotionBlock targetObj)
    {
        _puzzleFsm = puzzleFsm;
        _fsm = fsm;
        _target = targetObj;
    }
    
    public override void Enter()
    {
        _puzzleFsm.OnBlockCorrect += ChangeToLeadDialogue;

        _target.OnSpeedCorrect += _puzzleFsm.ChangeToNextStep;
        _target.OnSpeedCorrect += ChangeToCorrectDialogue;
        
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[0]);
        _puzzleFsm.ChangeConsoleText(_consoleIdList[0]);
    }

    public override void Exit()
    {
        _target.OnSpeedCorrect -= _puzzleFsm.ChangeToNextStep;
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
