using UnityEngine;

public class ControlMovement : MonoBehaviour
{
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(h, v, 0f);
        transform.position += move * (5f * Time.deltaTime);
    }
}
