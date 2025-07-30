using UnityEngine;
using UnityEngine.EventSystems;

public class VisualNovelNextButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool GetKey { get; private set; }
    public bool GetKeyDown { get; private set; }
    public bool GetKeyUp { get; private set; }

    private void LateUpdate()
    {
        GetKeyDown = false;
        GetKeyUp = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetKeyDown = true;
        GetKey = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetKeyUp = true;
        GetKey = false;
    }
}
