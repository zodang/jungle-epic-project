using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderInteractionDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public event Action OnControlStarted;
    public event Action OnControlEnd;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        //GameManager.Instance.AudioManager.PlaySfx(SfxType.Click);
        OnControlStarted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        
        OnControlEnd?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // EngineBlock 클릭 방지를 위한 return
        return;
    }
}
