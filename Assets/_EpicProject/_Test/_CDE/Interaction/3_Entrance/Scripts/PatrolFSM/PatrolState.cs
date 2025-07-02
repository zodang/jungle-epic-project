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
        
        _patrolFsm.Agent.enabled = false;
        _currentPoint = _patrolFsm.GetClosestPointIndex();
        
        _guard.OnControlEnabled += ChangeToControlState;
    }

    public override void Update()
    {
        if (_patrolFsm.PatrolPositions == null || _patrolFsm.PatrolPositions.Length == 0) return;

        Vector3 pos = _guard.transform.position;
        Vector3 target = _patrolFsm.PatrolPositions[_currentPoint];
        Vector2 dir = new Vector2(target.x - pos.x, 0f).normalized; // y는 무시!
        _guard.SetMoveDirection(dir);

        // 목적지 변경
        if (Vector3.Distance(_guard.transform.position, _patrolFsm.PatrolPositions[_currentPoint]) < 1f)
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
