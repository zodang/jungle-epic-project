using UnityEngine;
using UnityEngine.UIElements;
using System.Collections; 

public class StoneStatue : MonoBehaviour, IFeatureResetable, IControllable
{
    //도전과제 변수
    private Coroutine _sortingCoroutine = null; // 정렬 확인 코루틴을 저장할 변수

    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;

    // IFeatureResetable
    private float _defaultLight = 0f;
    private float _defaultRotation = 250f;
    private float _defaultScale = 1f;
        
    // ILightAdjustable
    private float _minBright = 0.5f;
    private float _maxBright = 3f;
    
    // IRotatable
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    
    // IScalable
    private float _minScale = 0.8f;
    private float _maxScale = 2.5f;
    
    // IControllable
    private bool _enableMove;    
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;

    [SerializeField] private Transform model;
    [SerializeField] private GameObject foot;
    [SerializeField] private GameObject twinkleLv1;
    [SerializeField] private GameObject twinkleLv2;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movement2D = GetComponent<Movement2D>();
        
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.freezeRotation = true;

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);

        _rotateHandler.Init(_minAngle, _maxAngle, 1f);
        _rotateHandler.OnSetValue += SetRotate;

        _scaleHandler.Init(_minScale, _maxScale, 0f);
        _scaleHandler.OnSetValue += SetScale;

        _lightHandler.Init(_minBright, _maxBright, 1f);
        _lightHandler.OnSetValue += Twinkle;

        ResetFeature();
    }

    private void Start()
    {
        //foot.SetActive(false);
        twinkleLv1.SetActive(false);
        twinkleLv2.SetActive(false);
    }
    
    private void Update()
    {
        if(_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput; 
        }
    }

    #region IFeatureResetable
    public void ResetFeature()
    {
        _rotateHandler.SetValue(_defaultRotation);
        _scaleHandler.SetValue(_defaultScale);
        _lightHandler.SetValue(_defaultLight);
    }
    #endregion

    private void Twinkle(float brightness)
    {
        twinkleLv1.SetActive(brightness >= 1.5f);
        twinkleLv2.SetActive(brightness >= 2.5f);
    }
    
    void SetRotate(float angle)
    {
        model.localEulerAngles = new Vector3(0, 0, angle);

        // 목표 각도 "범위"에 들어왔는지 확인
        if (IsAngleInTargetRange(model.localEulerAngles.z))
        {
            // 도전과제가 아직 해금되지 않았고, 타이머 코루틴이 아직 실행 중이 아닐 때만
            if (!AchievementStatusManager._isSortingAchievementUnlocked && _sortingCoroutine == null)
            {
                _sortingCoroutine = StartCoroutine(CheckSortingStateAfterDelay());
            }
        }
        else // 목표 각도 "범위"에서 벗어났을 경우
        {
            // 만약 타이머 코루틴이 실행 중이었다면, 즉시 중단시킵니다.
            if (_sortingCoroutine != null)
            {
                StopCoroutine(_sortingCoroutine);
                _sortingCoroutine = null;
            }
        }
    }

    private IEnumerator CheckSortingStateAfterDelay()
    {
        // 1. 여기서 1초를 기다립니다.
        yield return new WaitForSeconds(3.0f);

        // 2. 1초 뒤, 각도가 여전히 목표 "범위" 안에 있는지 다시 한번 확인합니다.
        if (IsAngleInTargetRange(model.localEulerAngles.z))
        {
            // 3. 조건이 여전히 만족되면 도전과제를 해금합니다.
            AchievementStatusManager._isSortingAchievementUnlocked = true;
            SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_SORTING");
            Debug.Log("도전과제 '정렬'이 1초 유지 후 완료되었습니다.");
        }

        // 코루틴의 역할이 끝났으므로 변수를 다시 null로 초기화합니다.
        _sortingCoroutine = null;
    }

    private bool IsAngleInTargetRange(float angle)
    {
        return angle >= 355f || angle <= 5f;
    }

    void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    #region IControllable
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
    #endregion
}