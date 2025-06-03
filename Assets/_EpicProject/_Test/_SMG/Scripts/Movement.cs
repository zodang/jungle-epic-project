using UnityEngine;

public class Movement : MonoBehaviour
{
    float _speed = 5f;

    public void Move(Vector2 move)
    {
        transform.Translate(move * _speed * Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
