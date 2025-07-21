using UnityEngine;

public class Axe : MonoBehaviour, IFeatureResetable, IControllable
{
    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;

    // IControllable
    private bool _enableMove;

    // IScalable
    private float _minScale = 0.3f;
    private float _maxScale = 6f;

    // IRotatable
    private float _minRotate = 0f;
    private float _maxRotate = 359f;

    // ILightAdjustable
    private float _minBright = 0.5f;
    private float _maxBright = 3f;

    private Transform _model;
    private GameObject _foot; 
    private GameObject _TwinkleLv1;
    private GameObject _TwinkleLv2;

    private Movement2D _movement2D;

    // --- [새로 추가된 부분 1] ObjectPropertyController 변수 선언 ---
    private ObjectPropertyController _propertyController;


    private void Awake()
    {
        _model = transform.GetChild(0);
        _foot = _model.GetChild(1).gameObject;
        _TwinkleLv1 = _model.GetChild(3).gameObject;
        _TwinkleLv2 = _model.GetChild(4).gameObject;

        _movement2D = GetComponent<Movement2D>();

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);

        _rotateHandler.Init(_minRotate, _maxRotate, 1f);
        _rotateHandler.OnSetValue += Rotate;

        _scaleHandler.Init(_minScale, _maxScale, 15f);
        _scaleHandler.OnSetValue += Resize;

        _lightHandler.Init(_minBright, _maxBright, 1f);
        _lightHandler.OnSetValue += Twinkle;

        // --- [새로 추가된 부분 2] 자기 자신에게 붙어있는 컴포넌트 찾아오기 ---
        _propertyController = transform.Find("Model").GetComponent<ObjectPropertyController>();
        
        ResetFeature();
    }
    
    private void Update()
    {
        if(_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput; 
        }
    }

    void Resize(float scale)
    {
        _model.localScale = new Vector3(scale, scale, scale);
    }

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0f, 0, angle);
    }

    void Twinkle(float bright)
    {
        if(bright >= 1.5f)
        {
            _TwinkleLv1.SetActive(true);
        }
        else
        {
            _TwinkleLv1.SetActive(false);
        }

        if(bright >= 2.5f)
        {
            _TwinkleLv2.SetActive(true);
        }
        else
        {
            _TwinkleLv2.SetActive(false);
        }
            
    }

    // IFeatureResetable
    public void ResetFeature()
    {
        _rotateHandler.SetValue(345f);
        _scaleHandler.SetValue(1f);
        _lightHandler.SetValue(1f);

        // DisableControl();
    }

    #region IControllable
    public void EnableControl()
    {
        _enableMove = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _foot.SetActive(true);
        _movement2D.MoveDir = Vector2.zero;

        // --- [수정된 부분] 미리 찾아둔 _propertyController 변수 사용 ---
        _propertyController.AttachController();
    }

    public void DisableControl()
    {
        _enableMove = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        _foot.SetActive(false);
        _movement2D.MoveDir = Vector2.zero;

        // --- [수정된 부분] 컨트롤이 비활성화 될 때도 호출해주는 것이 좋습니다 ---
        // _propertyController가 null이 아닐 때만 호출하도록 안전장치 추가
        if (_propertyController != null)
        {
            _propertyController.DetachController();
        }
    }
    #endregion
}
