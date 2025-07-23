using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialHandle : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action<float> OnValueChanged;
    
    public event Action OnControlStarted;
    public event Action OnControlEnd;

    private RectTransform _rectTransform;
    private Vector2 _prevMouseDir;
    private Vector2 _centerScreenPos;

    private float _totalRotation;
    private int _snapDivision = 24;
    private int _lastSnapIndex = -1;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // EngineBlock 클릭 방지를 위한 return
        return;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //GameManager.Instance.AudioManager.PlaySfx(SfxType.Click);

        _centerScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, _rectTransform.position);
        _prevMouseDir = (eventData.position - _centerScreenPos).normalized;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMouseDir = (eventData.position - _centerScreenPos).normalized;
        
        // 두 벡터 사이의 각도 계산
        float angle = Vector2.SignedAngle(_prevMouseDir, currentMouseDir); // +면 반시계, -면 시계
        _totalRotation += angle;
        _prevMouseDir = currentMouseDir;

        // 회전값을 0~360 범위로 정규화
        float visualAngle = (_totalRotation % 360f + 360f) % 360f;

        // Snap 처리
        float snapStep = 360f / _snapDivision;
        int snapIndex = Mathf.RoundToInt(visualAngle / snapStep);
        float snappedAngle = snapIndex * snapStep;

        if (snapIndex != _lastSnapIndex)
        {
            _lastSnapIndex = snapIndex;
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Dial);
        }

        _rectTransform.localEulerAngles = new Vector3(0, 0, snappedAngle);

        float normalized = visualAngle / 360f;
        OnValueChanged?.Invoke(normalized);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetRotationByValue(float normalized)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        float angle = Mathf.Lerp(0, 360, normalized);
        _rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        _totalRotation = angle;
        _lastSnapIndex = Mathf.RoundToInt(angle / (360f / _snapDivision));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnControlStarted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnControlEnd?.Invoke();
    }
}