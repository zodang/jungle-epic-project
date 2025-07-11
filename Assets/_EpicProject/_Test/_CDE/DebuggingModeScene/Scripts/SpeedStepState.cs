public class SpeedStepState : FSMState
{
    private PuzzleFSM _puzzleFsm;
    private FSM<FSMState> _fsm;

    public SpeedStepState(PuzzleFSM puzzleFsm, FSM<FSMState> fsm)
    {
        _puzzleFsm = puzzleFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
