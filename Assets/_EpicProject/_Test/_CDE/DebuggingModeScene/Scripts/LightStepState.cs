public class LightStepState : FSMState
{
    private PuzzleFSM _puzzleFsm;
    private FSM<FSMState> _fsm;
    private DebugSlot _debugSlot;

    public LightStepState(PuzzleFSM puzzleFsm, FSM<FSMState> fsm, DebugSlot debugSlot)
    {
        _puzzleFsm = puzzleFsm;
        _fsm = fsm;
        _debugSlot = debugSlot;
    }
    
    public override void Enter()
    {
        
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
