using UnityEngine;

public class CaveFairy : MonoBehaviour, IFeatureResetable, IControllable
{
    [SerializeField] private Transform model;
    [SerializeField] private GameObject foot;
    [SerializeField] private Transform light;

    // IFeatureResettable
    
    

    [Header("Setting/Light")]
    public float DefaultLight = 0f;
    public float MinBright = 0f;
    public float MaxBright = 9f;
    private LightHandler _lightHandler;

    [Header("Setting/Scale")]
    public float DefaultScale = 1f;
    public float MinScale = 0.5f;
    public float MaxScale = 2.5f;
    private ScaleHandler _scaleHandler;

    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);

        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
        
        _scaleHandler.Init(MinScale, MaxScale, 1f);
        _scaleHandler.OnSetValue += SetScale;

        _lightHandler.Init(MinBright, MaxBright, 1f);
        _lightHandler.OnSetValue += SetLight;

        DisableControl();
        ResetFeature();
    }
    
    #region FeatureSetting
    private void Update()
    {
        if(_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput; 
        }
    }

    private void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void SetLight(float bright)
    {
        // Todo: Light 변경 효과
        light.transform.localScale = new Vector3(bright, bright, 1f);
    }
    
    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        foot.SetActive(true);
        _movement2D.MoveDir = Vector3.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        foot.SetActive(false);
        _movement2D.MoveDir = Vector3.zero;
    }
    
    public void ResetFeature()
    {
        _scaleHandler.SetValue(DefaultScale);
        _lightHandler.SetValue(DefaultLight);
    }
    #endregion FeatureSetting
}
