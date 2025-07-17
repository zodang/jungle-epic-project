using System;
using UnityEngine;

public class BrokenEmotionBlock : MonoBehaviour
{
    public Action OnLightCorrect;
    public Action OnScaleCorrect;
    public Action OnSpeedCorrect;

    public Action<EngineBlock> OnBlockChanged;
    private EngineBlock _currentBlock;

    [SerializeField] private SpriteRenderer block;
    [SerializeField] private SpriteRenderer heart;

    [SerializeField] private Color startColor;
    [SerializeField] private Color optimalColor;
    [SerializeField] private Color endColor;

    private Vector3 _originalHeartPos;
    private float _noiseIntensity = 0.3f;
    private readonly float _noiseSpeed = 10f;
    
    private LightHandler _lightHandler;
    private readonly float _minLight = 0f;
    private readonly float _maxLight = 1f;
    private readonly Vector2 _optimalLightRange = new Vector2(0.64f, 0.75f);
    private float _currentLight;
    
    private ScaleHandler _scaleHandler;
    private readonly float _minScale = 0.5f;
    private readonly float _maxScale = 1.5f;
    private readonly Vector2 _optimalScaleRange = new Vector2(0.9f, 1.1f);
    private float _currentScale = 0f;
    
    private SpeedHandler _speedHandler;
    private readonly int _defaultSpeed = 3;
    private readonly int _optimalSpeed = 0;
    private int _currentSpeed;

    private bool _isLightCorrect;
    private bool _isScaleCorrect;
    private bool _isSpeedCorrect;
    
    private float _lightTimer;
    private float _scaleTimer;
    private float _speedTimer;
    private readonly float _correctHoldTime = 1.5f;

    private void Awake()
    {
        // 기능 초기화
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);
        _lightHandler.Init(_minLight, _maxLight, _currentLight);
        _lightHandler.OnSetValue += ChangeLight;
        
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        _scaleHandler.Init(_minScale, _maxScale, _currentScale);
        _scaleHandler.OnSetValue += ChangeScale;
        
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _speedHandler.Init(_defaultSpeed);
        _speedHandler.OnSetValue += ChangeSpeed;

        _originalHeartPos = heart.transform.localPosition;
    }

    private void Start()
    {
        OnBlockChanged += WhenBlockChanged;
    }

    private void ChangeLight(float value)
    {
        if (_isLightCorrect) return;

        //  색상 변경
        _currentLight = value;
        if (value <= 0.7f)
        {
            float t = Mathf.InverseLerp(0f, 0.7f, value);
            heart.color = Color.Lerp(startColor, optimalColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(0.7f, 1f, value);
            heart.color = Color.Lerp(optimalColor, endColor, t);
        }
    }
    
    private void ChangeScale(float value)
    {
        if (_isScaleCorrect) return;
        
        // 크기 변경
        _currentScale = value;
        heart.transform.localScale = Vector2.one * value;
    }
    
    private void ChangeSpeed(int value)
    {
        if (_isSpeedCorrect) return;
        
        // 이동 변경
        _currentSpeed = value;
        _noiseIntensity = value * 0.1f;
    }

    private void Update()
    {
        float x = Mathf.PerlinNoise(Time.time * _noiseSpeed, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * _noiseSpeed) - 0.5f;

        heart.transform.localPosition = _originalHeartPos + new Vector3(x, y, 0) * _noiseIntensity;

        if (_currentBlock == null) return;
        
        // 정답 판정
        if (!_isLightCorrect)
        {
            var lightSlider = _currentBlock as SliderControlBase<ILightAdjustable>;
            if (lightSlider != null)
            {
                bool inRange = _currentLight >= _optimalLightRange.x && _currentLight <= _optimalLightRange.y;

                // 1) 조작 중에도 2초 이상 구간 유지 시 정답
                if (lightSlider.IsControlStarted)
                {
                    if (inRange)
                    {
                        _lightTimer += Time.deltaTime;
                        if (_lightTimer >= _correctHoldTime)
                        {
                            OnLightCorrect?.Invoke();
                            _isLightCorrect = true;
                        }
                    }
                    else
                    {
                        _lightTimer = 0f;
                    }
                }
                // 2) 조작이 끝난 시점에서 범위 안이면 즉시 정답
                else
                {
                    if (inRange)
                    {
                        OnLightCorrect?.Invoke();
                        _isLightCorrect = true;
                    }
                    _lightTimer = 0f; // (조작이 끝났으므로 누적시간 초기화)
                }
            }
        }
        
        if (!_isScaleCorrect)
        {
            var scaleSlider = _currentBlock as SliderControlBase<IScalable>;
            if (scaleSlider != null)
            {
                bool inRange = _currentScale >= _optimalScaleRange.x && _currentScale <= _optimalScaleRange.y;

                // 1) 조작 중에도 2초 이상 구간 유지 시 정답
                if (scaleSlider.IsControlStarted)
                {
                    if (inRange)
                    {
                        _scaleTimer += Time.deltaTime;
                        if (_scaleTimer >= _correctHoldTime)
                        {
                            OnScaleCorrect?.Invoke();
                            _isScaleCorrect = true;
                        }
                    }
                    else
                    {
                        _scaleTimer = 0f;
                    }
                }
                // 2) 조작이 끝난 시점에서 범위 안이면 즉시 정답
                else
                {
                    if (inRange)
                    {
                        OnScaleCorrect?.Invoke();
                        _isScaleCorrect = true;
                    }
                    _scaleTimer = 0f;
                }
            }
        }
        
        if (!_isSpeedCorrect)
        {
            var speedSlider = _currentBlock as SnapSliderControlBase<ISpeedChangeable>;
            if (speedSlider != null)
            {
                bool inRange = _currentSpeed == _optimalSpeed;
                Debug.Log($"@@DE ---> {speedSlider.IsControlStarted} / {_currentSpeed} / {_optimalSpeed}");
                // 1) 조작 중에도 2초 이상 구간 유지 시 정답
                if (speedSlider.IsControlStarted)
                {
                    if (inRange)
                    {
                        _speedTimer += Time.deltaTime;
                        if (_speedTimer >= _correctHoldTime)
                        {
                            OnSpeedCorrect?.Invoke();
                            _isSpeedCorrect = true;
                        }
                    }
                    else
                    {
                        _speedTimer = 0f;
                    }
                }
                // 2) 조작이 끝난 시점에서 범위 안이면 즉시 정답
                else
                {
                    if (inRange)
                    {
                        OnSpeedCorrect?.Invoke();
                        _isSpeedCorrect = true;
                    }
                    _speedTimer = 0f;
                }
            }
        }
    }
    
    private void WhenBlockChanged(EngineBlock engineBlock)
    {
        _currentBlock = engineBlock;
    }

    public void ChangeBlockRender()
    {
        block.sortingLayerName = "UI";
        heart.sortingLayerName = "UI";
    }
}
