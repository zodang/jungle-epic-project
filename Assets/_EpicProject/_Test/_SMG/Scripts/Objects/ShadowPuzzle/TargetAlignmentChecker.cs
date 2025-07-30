using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum ShadowStatueType
{
    Cube,
    Pyramid,
    Sphere
}
public class TargetAlignmentChecker : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private List<Vector3> _rotationAnswers;
    private List<Transform> _targets; 

    [Header("Offset")]
    public Vector3 _offsetPosition;
    public Vector3 _offsetRotation;
    public Vector3 _offsetScale;

    [Header("Range")]
    public Vector3 _rangePosition = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 _rangeRotation = new Vector3(0f, 0f, 10f);
    public Vector3 _rangeScale = new Vector3(0.15f, 0.15f, 0.15f);

    [Header("State")]
    public Color AlignedColor = new Color(0f, 1f, 0f);
    public Color UnalignedColor = new Color(0f, 0f, 0f);
    private SpriteRenderer _slot;
    public bool IsAligned { get; private set; }
    private bool _prevIsAligned;
    public bool IsAlignedPosition { get; private set; }
    public bool IsAlignedRotation { get; private set; }
    public bool IsAlignedScale { get; private set; }

    public UnityEvent<bool> OnIsAligend;

    private void Start()
    {
        if (_rotationAnswers.Count < 1)
            _rotationAnswers.Add(Vector3.zero);

        TryGetComponent<SpriteRenderer>(out _slot);

        _prevIsAligned = true;
    }

    private void Update()
    {
        if (_target.IsUnityNull()) return;

        Vector3 goalPos = transform.position + _offsetPosition;
        //bool isAligendPosition =
        IsAlignedPosition =
            Mathf.Abs(goalPos.x - _target.position.x) <= _rangePosition.x &&
            Mathf.Abs(goalPos.y - _target.position.y) <= _rangePosition.y;// &&
            //Mathf.Abs(goalPos.z - _target.position.z) <= _rangePosition.z;

        IsAlignedRotation = false;
        for (int i = 0; i < _rotationAnswers.Count; i++)
        {
            Vector3 goalRot = transform.eulerAngles + _offsetRotation + _rotationAnswers[i];
            bool isAlignedRotation = Mathf.Abs(Mathf.DeltaAngle(goalRot.z, _target.eulerAngles.z)) <= _rangeRotation.z;

            if (isAlignedRotation)
            {
                IsAlignedRotation = true;
                break;
            }
        }

        Vector3 goalScale = transform.localScale + _offsetScale;
        //bool isAligendScale =
        IsAlignedScale =
            Mathf.Abs(goalScale.x - _target.localScale.x) <= _rangeScale.x &&
            Mathf.Abs(goalScale.y - _target.localScale.y) <= _rangeScale.y;// &&
            //Mathf.Abs(goalScale.z - _target.localScale.z) <= _rangeScale.z;

        IsAligned = IsAlignedPosition && IsAlignedRotation && IsAlignedScale;
        if(_prevIsAligned != IsAligned)
        {
            _prevIsAligned = IsAligned;
            if(!_slot.IsUnityNull())
            {
                _slot.color = IsAligned ? AlignedColor : UnalignedColor;
            }
            OnIsAligend?.Invoke(IsAligned);
        }
        
    }

    private void OnValidate()
    {
        _rangePosition = new Vector3(
            Mathf.Abs(_rangePosition.x),
            Mathf.Abs(_rangePosition.y),
            Mathf.Abs(_rangePosition.z)
            );

        _rangeRotation = new Vector3(
            Mathf.Abs(_rangeRotation.x),
            Mathf.Abs(_rangeRotation.y),
            Mathf.Abs(_rangeRotation.z)
            );

        _rangeScale = new Vector3(
            Mathf.Abs(_rangeScale.x),
            Mathf.Abs(_rangeScale.y),
            Mathf.Abs(_rangeScale.z)
            );
    }
}
