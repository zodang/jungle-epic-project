using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            var clickable = hit.collider != null
                ? hit.collider.GetComponent<IClickable>()
                : null;

            if (clickable != null)
            {
                clickable.OnClicked();
            }
            else
            {
                // 팝업을 닫음
            }
        }
    }
}
