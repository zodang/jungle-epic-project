using UnityEngine;

public class TempTirgger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"{name}.OnCollisionEnter2D: {collision.gameObject.name}");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"{name}.OnCollisionExit2D: {collision.gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"{name}.OnTriggerEnter2D: {collision.gameObject.name}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"{name}.OnTriggerExit2D: {collision.gameObject.name}");
    }
}
