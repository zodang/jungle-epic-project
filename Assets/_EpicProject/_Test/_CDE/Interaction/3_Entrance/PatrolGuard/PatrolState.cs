using UnityEngine;

public class PatrolState : FSMState
{
    private EntrancePatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    private int _currentPoint;
    private float patrolSpeed = 5f;
    
    public PatrolState(EntrancePatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        Debug.Log("Patrol State");
        
        _guard.OnControlEnabled += ChangeToControlState;
        _currentPoint = _patrolFsm.GetClosestPointIndex();
    }

    public override void Update()
    {
        if (_patrolFsm.PatrolPositions == null || _patrolFsm.PatrolPositions.Length == 0) return;

        // 현재 목표 포인트로 이동
        _guard.transform.position = Vector3.MoveTowards(
            _guard.transform.position,
            _patrolFsm.PatrolPositions[_currentPoint],
            patrolSpeed * Time.deltaTime
        );

        // 도착했다면 다음 포인트로
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
