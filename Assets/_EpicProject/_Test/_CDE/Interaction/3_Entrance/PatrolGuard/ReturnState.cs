using UnityEngine;

public class ReturnState : FSMState
{
    private EntrancePatrolGuard _guard;
    private PatrolFSM _patrolFsm;
    private FSM<FSMState> _fsm;
    
    private float _returnSpeed = 5f;
    private Vector3 _targetPoint;
    
    private bool _waiting;
    private float _waitTimer;
    private float _waitTime = 0.3f;
    
    public ReturnState(EntrancePatrolGuard guard, PatrolFSM patrolFsm, FSM<FSMState> fsm)
    {
        _guard = guard;
        _patrolFsm = patrolFsm;
        _fsm = fsm;
    }

    public override void Enter()
    {
        Debug.Log("Return State");
       
        _patrolFsm.SetFocus();
        StageManager.Instance.InputManager.ActivatePlayerInput(false);
        _targetPoint = GetClosestPatrolPoint();
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
        _guard.transform.position = Vector3.MoveTowards(
            _guard.transform.position,
            _targetPoint,
            _returnSpeed * Time.deltaTime
        );

        if (!(Vector3.Distance(_guard.transform.position, _targetPoint) < 0.1f)) return;
        
        // targetPoint에 도달
        _waiting = true;
        _waitTimer = 0f;
    }

    public override void Exit()
    {
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _patrolFsm.UnsetFocus();
    }
    
    private Vector3 GetClosestPatrolPoint()
    {
        //  가까운 patrolPoint 찾기
        var points = _patrolFsm.PatrolPositions;
        float minDist = float.MaxValue;
        Vector3 closest = points[0];
        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector3.Distance(_guard.transform.position, points[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = points[i];
            }
        }
        return closest;
    }
}
