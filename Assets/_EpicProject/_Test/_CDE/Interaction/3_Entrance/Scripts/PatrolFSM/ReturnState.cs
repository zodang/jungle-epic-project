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
        _patrolFsm.SetFocus();
        _targetPoint = _patrolFsm.PatrolPositions[_patrolFsm.GetClosestPointIndex()];
        StageManager.Instance.InputManager.ActivatePlayerInput(false);
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
        
        // 현재 위치에서 타겟 포인트로 이동
        Vector3 pos = _guard.transform.position;
        Vector3 target = _targetPoint;
        Vector2 dir = (target - pos).normalized;
        _guard.SetMoveDirection(dir);

        if (!(Vector3.Distance(_guard.transform.position, _targetPoint) < 2f)) return;
        
        // targetPoint에 도달
        _waiting = true;
        _waitTimer = 0f;
    }

    public override void Exit()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _patrolFsm.UnsetFocus();
    }
}
