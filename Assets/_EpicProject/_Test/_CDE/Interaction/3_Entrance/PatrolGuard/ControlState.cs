using UnityEngine;

public class ControlState : FSMState
{
    private EntrancePatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    public ControlState(EntrancePatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        Debug.Log("Control State");
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
