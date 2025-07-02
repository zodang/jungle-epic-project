using Define;

public class ControlState : FSMState
{
    private PatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    public ControlState(PatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        _patrolFsm.ChangeCurrentState(PatrolStateType.Control);
        
        _patrolFsm.Agent.enabled = false;
        _patrolFsm.SetFocus();
        
        _guard.OnControlDisabled += ChangeToReturnState;

    }

    public override void Exit()
    {
        _guard.OnControlDisabled -= ChangeToReturnState;
    }
    
    private void ChangeToReturnState()
    {
        _fsm.ChangeState(new ReturnState(_guard, _patrolFsm, _fsm));
    }
}
