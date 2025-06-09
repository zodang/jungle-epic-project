using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float _speed = 5f;
    public Vector2 MoveDir { get; set; }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = MoveDir * _speed;
    }
}
