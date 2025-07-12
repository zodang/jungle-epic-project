using System;
using Unity.VisualScripting;
using UnityEngine;

public class Vane : MonoBehaviour, IFeatureResetable, IControllable, IWindEmitter
{
    private Transform _visualRoot;
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
    [SerializeField] float FanPower;
    float FanLv1Threshold = 3.6f;
    float FanLv2Threshold = 21f;
    float FanLv3Threshold = 28f;
    [SerializeField] GameObject WindZoneLv1;
    [SerializeField] GameObject WindZoneLv2;
    [SerializeField] GameObject WindZoneLv3;
    private float _prevRotate;
    

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
        _fan = _visualRoot.GetChild(0).GetChild(0);

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        _rotateHandler.Init(MinRotate, MaxRotate, 1f);
        _rotateHandler.OnSetValue += SetRotate;

        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        _scaleHandler.Init(MinScale, MaxScale, 1f);
        _scaleHandler.OnSetValue += SetScale;

        ResetFeature();
        DisableControl();
    }

    void Update()
    {
        if (_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        }

        FanPower = Mathf.Clamp(FanPower - 3f * Time.deltaTime, 0f, 32f);

        int lv = 0;
        GameObject windZone;
        if (FanPower >= FanLv3Threshold)
        {
            lv = 3;
            windZone = WindZoneLv3;
        }
        else if (FanPower >= FanLv2Threshold)
        {
            lv = 2;
            windZone = WindZoneLv2;
        }
        else
        {
            lv = 1;
            windZone = WindZoneLv1;
        }

        if (FanPower >= FanLv3Threshold)
        {
            WindZoneLv3.SetActive(true);
            WindZoneLv1.SetActive(false);
            WindZoneLv2.SetActive(false);
        }
        else if (FanPower >= FanLv2Threshold)
        {
            WindZoneLv2.SetActive(true);
            WindZoneLv1.SetActive(false);
            WindZoneLv3.SetActive(false);
        }
        else if (FanPower >= FanLv1Threshold)
        {
            WindZoneLv1.SetActive(true);
            WindZoneLv2.SetActive(false);
            WindZoneLv3.SetActive(false);
        }
        else
        {
            WindZoneLv1.SetActive(false);
            WindZoneLv2.SetActive(false);
            WindZoneLv3.SetActive(false);
        }

        Animator[] animators = windZone.GetComponentsInChildren<Animator>();
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].speed = FanPower / 18f;
        }

        SpriteRenderer[] spriteRenderers = windZone.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = spriteRenderers[i].color;
            color.a = Mathf.Clamp01((FanPower-3f) / 9f);
            spriteRenderers[i].color = color;
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
        OnControlEnabled?.Invoke(enable);
    }

    
    void SetRotate(float rotate)
    {
        FanPower += Mathf.DeltaAngle(_prevRotate, rotate) * 0.01f;
        _fan.localEulerAngles = new Vector3(0, 0, -rotate);
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


