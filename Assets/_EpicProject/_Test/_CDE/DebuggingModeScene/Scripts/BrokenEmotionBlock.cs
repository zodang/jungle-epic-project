using System;
using UnityEngine;

public class BrokenEmotionBlock : MonoBehaviour
{
    public Action OnLightCorrect;
    public Action OnScaleCorrect;
    public Action OnSpeedCorrect;

    [SerializeField] private SpriteRenderer block;
    [SerializeField] private SpriteRenderer heart;

    [SerializeField] private Color grayColor;
    [SerializeField] private Color redColor;

    private Vector3 _originalHeartPos;
    private float _noiseIntensity = 0.3f;
    private readonly float _noiseSpeed = 10f;
    
    private LightHandler _lightHandler;
    private readonly float _minLight = 0f;
    private readonly float _maxLight = 1f;
    private float _currentLight;
    
    private ScaleHandler _scaleHandler;
    private readonly float _minScale = 0.5f;
    private readonly float _maxScale = 1f;
    private float _currentScale = 0f;
    
    private SpeedHandler _speedHandler;
    private readonly int _defaultSpeed = 3;
    private int _currentSpeed;

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

    private void ChangeLight(float value)
    {
        // 색상 변경
        heart.color = Color.Lerp(grayColor, redColor, value);
        
        // 정답 판정
        if (value >= _maxLight) OnLightCorrect?.Invoke();
    }
    
    private void ChangeScale(float value)
    {
        // 크기 변경
        heart.transform.localScale = Vector2.one * value;
        
        // 정답 판정
        if (value >= _maxScale) OnScaleCorrect?.Invoke();
    }
    
    private void ChangeSpeed(int value)
    {
        // 이동 변경
        _noiseIntensity = value * 0.1f;
        
        // 정답 판정
        if (value <= 0) OnSpeedCorrect?.Invoke();
    }

    private void Update()
    {
        float x = Mathf.PerlinNoise(Time.time * _noiseSpeed, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * _noiseSpeed) - 0.5f;

        heart.transform.localPosition = _originalHeartPos + new Vector3(x, y, 0) * _noiseIntensity;
    }
}
