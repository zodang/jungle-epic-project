using Define;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderInteractionDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Click);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
    }
}
