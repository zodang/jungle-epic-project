public class LightStepState : FSMState
{
    private string[] _dialogueIdList = new[] { "step_1_1_player", "step_1_2_player", "step_1_3_player" };
    private string[] _consoleIdList = new[] { "console_light_broken", "console_light_solved"};
    
    private PuzzleFSM _puzzleFsm;
    private BrokenEmotionBlock _target;

    public LightStepState(PuzzleFSM puzzleFsm, BrokenEmotionBlock targetObj)
    {
        _puzzleFsm = puzzleFsm;
        _target = targetObj;
    }
    
    public override void Enter()
    {
        _puzzleFsm.OnBlockCorrect += ChangeToLeadDialogue;

        _target.OnLightCorrect += _puzzleFsm.ChangeToNextStep;
        _target.OnLightCorrect += ChangeToCorrectDialogue;
        
        _puzzleFsm.ChangeDialogueText(_dialogueIdList[0]);
        _puzzleFsm.ChangeConsoleText(_consoleIdList[0]);
    }

    public override void Exit()
    {
        _target.OnLightCorrect -= _puzzleFsm.ChangeToNextStep;
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
