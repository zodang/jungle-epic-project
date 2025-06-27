using Define;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
    [SerializeField] private EngineController engineUIPrefab;

    private Dictionary<Clickable, EngineController> _engineDictionary = new();
    private EngineUIManager _engineUIManager;
    private bool _isTabHomeGroupActive = true;

    private void Awake()
    {
        _engineUIManager = GetComponent<EngineUIManager>();
    }

    private void Start()
    { 
        // ESC 키로 모든 EngineUI 비활성화
        StageManager.Instance.InputManager.OnEscPressed += DeactivateAllEngine;
        StageManager.Instance.InputManager.OnTabPressed += ToggleTabHome;
        
        // Clickable마다 UI 
        foreach (var clickable in FindObjectsByType<Clickable>(FindObjectsSortMode.None))
        {
            EngineController engineController = Instantiate(engineUIPrefab, transform);
            _engineDictionary.Add(clickable, engineController);

            // Clickable대로 EngineUI 세팅 
            engineController.InitEngineController(clickable);
            
            // Clickable의 기본 블록 세팅
            clickable.InitClickable(engineController);
        }
    }
    
    private void OnDestroy()
    {
        StageManager.Instance.InputManager.OnEscPressed -= DeactivateAllEngine;
        StageManager.Instance.InputManager.OnTabPressed -= ToggleTabHome;
    }

    public void ActivateEngineUI(Clickable clickable)
    {
        if (!_engineDictionary.TryGetValue(clickable, out EngineController engineController)) return;

        // 타겟 Engine이 정렬 상태
        if (_isTabHomeGroupActive)
        {
            engineController.gameObject.SetActive(true);
            engineController.Activate();
            return;
        }

        // 타겟 Engine이 비정렬 상태
        if (engineController.IsInHome)
        {
            ToggleTabHome();
        }
        else
        {
            engineController.gameObject.SetActive(true);
            engineController.Activate();
        }
    }

    public bool GetActivateEngineUI(Clickable clickable)
    {
        // 튜토리얼 상태 체크를 위해 추가
        if (_engineDictionary.TryGetValue(clickable, out EngineController engineController))
        {
            return engineController.IsActivate;            
        }
        return false;
    }

    private void DeactivateAllEngine()
    {
        bool anyDeactivated = false;
        
        foreach (var engineController in _engineDictionary.Values)
        {
            if (engineController.gameObject.activeSelf)
            {
                // HomeGroup 비활성화 상태에서 정렬되어 있는 엔진은 넘어감 
                if (!_isTabHomeGroupActive && engineController.IsInHome) continue;
                
                engineController.DeactivateSilently();
                anyDeactivated = true;
            }
        }

        // 하나라도 꺼진다면 효과음 재생
        if (anyDeactivated)
        {
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        }
    }

    private void ToggleTabHome()
    {
        _isTabHomeGroupActive = !_isTabHomeGroupActive;
        _engineUIManager.ActivateTabHomeGroup(_isTabHomeGroupActive);
    }
}
