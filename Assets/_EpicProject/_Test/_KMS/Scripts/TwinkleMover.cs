using UnityEngine;

public class TwinkleMover : MonoBehaviour
{
    private float _speed;
    private float _maxY;

    public void Initialize(float speed, float maxY)
    {
        _speed = speed;
        _maxY = maxY;
    }

    void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;

        if (transform.position.y >= _maxY)
        {
            Destroy(gameObject);
        }
    }
}
