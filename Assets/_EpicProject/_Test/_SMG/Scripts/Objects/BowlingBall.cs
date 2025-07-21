using UnityEngine;

public class BowlingBall : MonoBehaviour, IFeatureResetable, IControllable
{
    private Transform _visualRoot;

    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;

    [Header("Setting/Rotate")]
    public float DefaultRotate = 0f;
    private float MinRotate = 0f;
    private float MaxRotate = 359f;
    private RotateHandler _rotateHandler;

    [Header("Setting/Scale")]
    public float DefaultScale = 1f;
    public float MinScale = 1f;
    public float MaxScale = 4;
    private ScaleHandler _scaleHandler;

    

    private void Awake()
    {
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;

        _visualRoot = transform.GetChild(0);

        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);

        _scaleHandler.Init(MinScale, MaxScale, DefaultScale);
        _scaleHandler.OnSetValue += SetScale;

        _rotateHandler.Init(MinRotate, MaxRotate, DefaultScale);
        _rotateHandler.OnSetValue += SetRotate;

        DisableControl();
        ResetFeature();
    }

    void Update()
    {
        if (_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        }
        else
        {
            if(_dynamicDelta > 0f)
            {
                _dynamicDelta -= Time.deltaTime;
            }
            else
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    public void ResetFeature()
    {
        _rotateHandler.SetValue(DefaultRotate);
        _scaleHandler.SetValue(DefaultScale);
    }

    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        //foot.SetActive(true);
        _movement2D.MoveDir = Vector3.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        //foot.SetActive(false);
        _movement2D.MoveDir = Vector3.zero;
    }

    float _dynamicDelta;

    void SetScale(float scale)
    {
        _visualRoot.localScale = new Vector3(scale, scale, 1f);
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _dynamicDelta = 0.3f;
    }

    void SetRotate(float rotate)
    {
        _visualRoot.localEulerAngles = new Vector3(0, 0, rotate);
    }
}
