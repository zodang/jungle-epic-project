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
        
        _guard.OnControlEnabled += ChangeToControlState;
        
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
        UpdateAnimationByNavMesh();
    }

    public override void Exit()
    {
        _guard.OnControlEnabled -= ChangeToControlState;
        
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        _patrolFsm.UnsetFocus();
    }
    
    private void ChangeToControlState()
    {
        _fsm.ChangeState(new ControlState(_guard, _patrolFsm, _fsm));
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
    
    private void UpdateAnimationByNavMesh()
    {
        Vector3 move = _patrolFsm.Agent.velocity;
        bool isMoving = move.sqrMagnitude > 0.01f;

        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            move.y = 0;
        }
        else
        {
            move.x = 0;
        }

        _patrolFsm.Animator.SetBool("IsMoving", isMoving);
        _patrolFsm.Animator.SetFloat("AbsMoveX", Mathf.Abs(move.x));
        _patrolFsm.Animator.SetFloat("MoveX", move.x);
        _patrolFsm.Animator.SetFloat("MoveY", move.y);

        // 좌우 반전
        _patrolFsm.SpriteRenderer.flipX = move.x > 0;
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
