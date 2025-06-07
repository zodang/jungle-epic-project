using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<float> OnValueChanged; // 0~1 정규화 값
    
    private readonly float _dialSensitivity = 0.5f;
    private RectTransform _rectTransform;

    private float _previousAngle;
    private float _totalRotation;
    
    private int _snapDivision = 12; // 360를 12개 구간으로 나눔
    private int _lastSnapIndex = -1;

    public SFXEventChannelSO _sfxEventChannel;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, _rectTransform.position);
        Vector2 fromCenter = eventData.position - center;
        float currentAngle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;

        float delta = Mathf.DeltaAngle(_previousAngle, currentAngle) * _dialSensitivity;
        _totalRotation += delta;
        _previousAngle = currentAngle;

        // Dial Handle 회전
        float visualAngle = (_totalRotation % 360f + 360f) % 360f;
        
        float snapStep = 360f / _snapDivision;
        int snapIndex = Mathf.RoundToInt(visualAngle / snapStep);
        float snappedAngle = snapIndex * snapStep;
        
        if (snapIndex != _lastSnapIndex)
        {
            // Snap 및 사운드 재생
            _lastSnapIndex = snapIndex;
            _sfxEventChannel.RaiseEvent(Sfx.DialTick);
        }
        
        // Snap 값 대로 Dial 회전
        _rectTransform.localEulerAngles = new Vector3(0, 0, snappedAngle);

        // 실제 회전 값 전달 
        float normalized = ((-visualAngle % 360f) + 360f) % 360f / 360f;
        OnValueChanged?.Invoke(normalized);
    }

    public void SetRotationByValue(float normalized)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
        
        float angle = Mathf.Lerp(0, 360, normalized);
        _rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    
    public void OnBeginDrag(PointerEventData eventData) { }
}
