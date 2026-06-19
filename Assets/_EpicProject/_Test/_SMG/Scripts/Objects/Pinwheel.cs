using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pinwheel : MonoBehaviour, IFeatureResetable, IControllable, IWindEmitter
{
    private Transform _visualRoot;
    private Collider2D foot;
    private Transform _fan;

    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;

    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;

    [Header("Rotate Setting")]
    public float DefaultRotate;
    public float MinRotate = 0f;
    public float MaxRotate = 359f;

    [Header("Scale Setting")]
    public float DefaultScale = 1f;
    public float MinScale = 0.5f;
    public float MaxScale = 5.4f;

    public event Action<bool> OnControlEnabled;

    // Fan
    float FanPower;
    float _fanSpeed;
    float FanLv1Threshold = 0.3f;
    float FanLv2Threshold = 12f;
    float FanLv3Threshold = 20f;
    [SerializeField] PinwheelWindZone WindZoneLv1;
    [SerializeField] PinwheelWindZone WindZoneLv2;
    [SerializeField] PinwheelWindZone WindZoneLv3;
    private float _prevRotate;

    private float _decayDelay;
    private bool _prevWindEnable;


    private void Awake()
    {
        _movement2D = GetComponent<Movement2D>();
        if (!_movement2D.IsUnityNull())
        {
            _movement2D.MultiplySpeed(0.7f);
        }

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0f;

        _visualRoot = transform.GetChild(0);
        foot = GetComponentInChildren<FootTag>().GetComponent<Collider2D>();
        _fan = _visualRoot.GetChild(0).GetChild(0);

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        _rotateHandler.Init(MinRotate, MaxRotate, 1f);
        _rotateHandler.OnSetValue += SetRotate;

        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        _scaleHandler.Init(MinScale, MaxScale, 1f);
        _scaleHandler.OnSetValue += SetScale;

        ResetFeature();
        DisableControl();

        _prevWindEnable = !(transform.position.y < 2.4f);
    }

    void Update()
    {
        if (_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        }

        bool windEnable = transform.position.y < 2.4f;
        if (_prevWindEnable != windEnable)
        {
            _prevWindEnable = windEnable;
            WindZoneLv1.GetComponent<Collider2D>().enabled = windEnable;
            WindZoneLv2.GetComponent<Collider2D>().enabled = windEnable;
            WindZoneLv3.GetComponent<Collider2D>().enabled = windEnable;
        }

        if (Mathf.Abs(_fanSpeed) < 0.02f)
        {
            _fanSpeed = 0f;
            return;
        }

        if (_fanSpeed > 6f * _visualRoot.localScale.x)
        {
            _fanSpeed = 6f * _visualRoot.localScale.x;
        }
        else if (_fanSpeed < -6f * _visualRoot.localScale.x)
        {
            _fanSpeed = -6f * _visualRoot.localScale.x;
        }

        if (_decayDelay > 0)
        {
            _decayDelay -= Time.deltaTime;
            _fanSpeed -= ((_fanSpeed > 0) ? 1f : -1f) * Time.deltaTime;
            //FanPower -= 1f * Time.deltaTime;
        }
        else
        {
            _fanSpeed -= ((_fanSpeed > 0) ? 10f : -10f) * Time.deltaTime;
            //FanPower -= 10f * Time.deltaTime;
        }
        
        FanPower = Mathf.Abs(_fanSpeed);

        PinwheelWindZone windZone;
        if (FanPower >= FanLv3Threshold)
        {
            windZone = WindZoneLv3;
        }
        else if (FanPower >= FanLv2Threshold)
        {
            windZone = WindZoneLv2;
        }
        else
        {
            windZone = WindZoneLv1;
        }

        if (FanPower >= FanLv3Threshold)
        {
            WindZoneLv1.gameObject.SetActive(false);
            WindZoneLv2.gameObject.SetActive(false);
            WindZoneLv3.gameObject.SetActive(true);
        }
        else if (FanPower >= FanLv2Threshold)
        {
            WindZoneLv1.gameObject.SetActive(false);
            WindZoneLv2.gameObject.SetActive(true);
            WindZoneLv3.gameObject.SetActive(false);
        }
        else if (FanPower >= FanLv1Threshold)
        {
            WindZoneLv1.gameObject.SetActive(true);
            WindZoneLv2.gameObject.SetActive(false);
            WindZoneLv3.gameObject.SetActive(false);
        }
        else
        {
            WindZoneLv1.gameObject.SetActive(false);
            WindZoneLv2.gameObject.SetActive(false);
            WindZoneLv3.gameObject.SetActive(false);
        }

        if (FanPower >= FanLv1Threshold)
        {
            windZone.ApplyAnimator(FanPower, FanLv1Threshold);
            windZone.ApplyAlpha(FanPower, FanLv1Threshold);
        }
    }


    public void ResetFeature()
    {
        _rotateHandler.SetValue(DefaultRotate);
        _scaleHandler.SetValue(DefaultScale);
    }

    public void EnableControl()
    {
        SetEnableControl(true);
    }

    public void DisableControl()
    {
        SetEnableControl(false);
    }

    void SetEnableControl(bool enable)
    {
        _enableMove = enable;
        _movement2D.MoveDir = Vector2.zero;
        _rigidbody2D.bodyType = enable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        foot.enabled = enable;
        OnControlEnabled?.Invoke(enable);
    }


    void SetRotate(float rotate)
    {
        float deltaAngle = Mathf.DeltaAngle(_prevRotate, rotate);
        _decayDelay = 1f;

        _fanSpeed += deltaAngle * 0.002f * _visualRoot.localScale.x;
        _fan.localEulerAngles = new Vector3(0, 0, rotate);
        _prevRotate = rotate;
    }

    void SetScale(float scale)
    {
        _visualRoot.localScale = new Vector3(scale, scale, 1f);
    }

    public float GetWindPower()
    {
        return FanPower;
    }
}
