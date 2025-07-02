using Define;
using UnityEngine;

public class ReturnState : FSMState
{
    private PatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    private Vector3 _targetPoint;
    
    private bool _waiting;
    private float _waitTimer;
    private float _waitTime = 0.3f;
    
    public ReturnState(PatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        _patrolFsm.ChangeCurrentState(PatrolStateType.Return);
        
        _targetPoint = _patrolFsm.PatrolPositions[_patrolFsm.GetClosestPointIndex()];
        StageManager.Instance.InputManager.ActivatePlayerInput(false);

        _patrolFsm.Agent.enabled = true;
        _patrolFsm.Agent.SetDestination(_targetPoint);
        _patrolFsm.SetFocus();
    }

    public override void Update()
    {
        if (_waiting)
        {
            // _waitTime 동안 대기
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _waitTime)
            {
                _fsm.ChangeState(new PatrolState(_guard, _patrolFsm, _fsm));
            }
            return;
        }

        NavMeshMove();
    }

    public override void Exit()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _patrolFsm.UnsetFocus();
    }

    private void NavMeshMove()
    {
        // NavMesh 사용한 이동
        if (_patrolFsm.Agent.pathPending) return;
        if (!(_patrolFsm.Agent.remainingDistance < 0.5f)) return;

        _waiting = true;
        _waitTimer = 0f;
            
        _patrolFsm.Agent.SetDestination(_patrolFsm.Agent.transform.position);
    }

    private void RigidbodyMove()
    {
        // Movement2D 사용한 이동
        Vector3 pos = _guard.transform.position;
        Vector3 target = _targetPoint;
        Vector2 dir = (target - pos).normalized;
        _guard.SetMoveDirection(dir);

        if (!(Vector3.Distance(_guard.transform.position, _targetPoint) < 2f)) return;
        
        // targetPoint에 도달
        _waiting = true;
        _waitTimer = 0f;
    }
}
