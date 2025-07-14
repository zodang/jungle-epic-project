using Unity.VisualScripting;
using UnityEngine;

public class BowlingPinHitHandler : MonoBehaviour
{
    public bool IsHit { get; private set; }

    [SerializeField] private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        IsHit = false;

        if (_rigidbody2D.IsUnityNull())
        {
            _rigidbody2D = GetComponentInParent<Rigidbody2D>();
            if (_rigidbody2D.IsUnityNull())
            {
                ComponentHelper.TryGetOrAddComponent<Rigidbody2D>(ref _rigidbody2D, gameObject);
            }
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hit"))
        {
            if (IsHit) return;

            ScaleHandler scaleHandler = collision.GetComponentInParent<ScaleHandler>();
            if (scaleHandler.IsUnityNull() || scaleHandler.CurrentScale < 2f) return;

            IsHit = true;

            LaunchAway(_rigidbody2D, collision.transform);
            
            //Invoke("leafSetActiveFalse", 5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Hit"))
        {
            if (IsHit) return;

            ScaleHandler scaleHandler = collision.collider.GetComponentInParent<ScaleHandler>();
            if (scaleHandler.IsUnityNull() || scaleHandler.CurrentScale < 2f) return;

            IsHit = true;

            LaunchAway(_rigidbody2D, collision.transform);

            Invoke("SetActiveFalse", 5f);
        }
    }

    void LaunchAway(Rigidbody2D pin, Transform ball)
    {
        Collider2D[] collider2Ds = pin.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].isTrigger = true;
        }
        Movement2D movement2D = pin.GetComponentInParent<Movement2D>();
        if (!movement2D.IsUnityNull())
        {
            movement2D.enabled = false;
        }
        //pin.GetComponent<Collider2D>().isTrigger = true;

        pin.bodyType = RigidbodyType2D.Dynamic;
        pin.gravityScale = 0f;
        pin.freezeRotation = false;
        pin.AddForce((pin.transform.position - ball.position).normalized * 7f, ForceMode2D.Impulse);
        pin.AddTorque(10f, ForceMode2D.Impulse);
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
