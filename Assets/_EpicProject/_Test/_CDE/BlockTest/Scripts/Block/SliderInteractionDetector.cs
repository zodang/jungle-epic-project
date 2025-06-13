using Define;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderInteractionDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfx(SfxType.Click);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfx(SfxType.Close);
    }
}
