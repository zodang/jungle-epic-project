using Define;
using UnityEngine;

public class PatrolState : FSMState
{
    private PatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    private int _currentPoint;
    
    public PatrolState(PatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        _patrolFsm.ChangeCurrentState(PatrolStateType.Patrol);
        
        _guard.OnControlEnabled += ChangeToControlState;
        _currentPoint = _patrolFsm.GetClosestPointIndex();
        
        _patrolFsm.Agent.enabled = false;
    }

    public override void Update()
    {
        if (_patrolFsm.PatrolPositions == null || _patrolFsm.PatrolPositions.Length == 0) return;

        Vector3 pos = _guard.transform.position;
        Vector3 target = _patrolFsm.PatrolPositions[_currentPoint];
        Vector2 dir = (target - pos).normalized;
        
        // 목적지로 이동
        _guard.SetMoveDirection(dir);

        // 목적지 변경
        if (Vector3.Distance(_guard.transform.position, _patrolFsm.PatrolPositions[_currentPoint]) < 0.1f)
        {
            _currentPoint = (_currentPoint + 1) % _patrolFsm.PatrolPositions.Length;
        }
    }

    public override void Exit()
    {
        _guard.OnControlEnabled -= ChangeToControlState;
    }

    private void ChangeToControlState()
    {
        _fsm.ChangeState(new ControlState(_guard, _patrolFsm, _fsm));
    }
}
