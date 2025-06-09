using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 move = InputManager.Instance.MoveInput;
        bool isMoving = move.sqrMagnitude > 0.01f;
        
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            // 수평 우선
            move.y = 0;
        }
        else
        {
            // 수직 우선
            move.x = 0;
        }

        _anim.SetBool("IsMoving", isMoving);
        _anim.SetFloat("AbsMoveX", Mathf.Abs(move.x));
        _anim.SetFloat("MoveX", move.x);
        _anim.SetFloat("MoveY", move.y);

        // 좌우 반전
        if (Mathf.Abs(move.x) > 0.01f)
        {
            _spriteRenderer.flipX = move.x > 0;
        }
    }
}
