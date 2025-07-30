using System;
using UnityEngine;

public class CaveFairy : MonoBehaviour, IFeatureResetable, IControllable
{
    // << 1. 이벤트를 추가합니다. >>
    // 어떤 CaveFairy든 상태가 바뀌면 이 이벤트가 발생합니다.
    public static event Action OnAnyFairyStateChanged;


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
        model.localScale = new Vector3(scale, scale, 1f);
        OnAnyFairyStateChanged?.Invoke(); // << 2. 상태 변경을 알립니다. >>
    }

    private void SetLight(float bright)
    {
        // Todo: Light 변경 효과
        light.transform.localScale = new Vector3(bright, bright, 1f);
        OnAnyFairyStateChanged?.Invoke(); // << 2. 상태 변경을 알립니다. >>
    }

    // << 3. 이 함수를 새로 추가합니다. >>
    /// <summary>
    /// 이 요정이 '작고 밝은' 상태인지 확인하는 함수
    /// </summary>
    public bool IsSmallAndBright()
    {
        // ScaleHandler와 LightHandler로부터 현재 값을 가져와서 비교합니다.
        // (GetCurrentValue()는 예시이며, 핸들러의 실제 현재값 가져오는 함수 이름으로 변경해야 할 수 있습니다.)
        bool isSmall = _scaleHandler.GetCurrentValue() <= MinScale;
        bool isBright = _lightHandler.GetCurrentValue() >= MaxBright;
        return isSmall && isBright;
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
    
    public void ResetFeature()
    {
        _scaleHandler.SetValue(DefaultScale);
        _lightHandler.SetValue(DefaultLight);
    }
    #endregion FeatureSetting

    public void SetDialogueFlag(string flag)
    {
        StageBaseManager.Instance.FlagManager.SetFlag(flag);
        if(!AchievementStatusManager._isFairyAchievementUnlocked)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_FAIRY_FRIEND");
            AchievementStatusManager._isFairyAchievementUnlocked = true; // 도전과제 해금 상태 업데이트
            Debug.Log("도전과제 '요정 친구'가 완료되었습니다.");
        }
        
    }

    public void ClearDialogueFlag(string flag)
    {
        //StageBaseManager.Instance.FlagManager.SetFlag(flag, false);
        StageBaseManager.Instance.FlagManager.ClearFlag(flag);
    }
}
