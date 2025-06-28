using UnityEngine;

public class ClickableMaskBypass : MonoBehaviour
{
    private const float _zPos = -8f;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _zPos);
    }
}
