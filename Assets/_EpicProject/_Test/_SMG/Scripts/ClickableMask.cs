using UnityEngine;

public class ClickableMask : MonoBehaviour
{
    private const float _zPos = -4f;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _zPos);
    }
}
