using Define;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
    [SerializeField] private EngineController engineUIPrefab;
    private Dictionary<Clickable, EngineController> _engineDictionary = new();
    private EngineUIManager _engineUIManager;
    private bool _isTabHomeGroupActive = true;

    private void Start()
    {
        _engineUIManager = GetComponent<EngineUIManager>();
        
        // ESC 키로 모든 EngineUI 비활성화
        StageManager.Instance.InputManager.OnEscPressed += DeactivateAllEngine;
        StageManager.Instance.InputManager.OnTabPressed += ToggleTabHome;
        
        // Clickable마다 UI 추가
        Clickable[] clickables = FindObjectsByType<Clickable>(FindObjectsSortMode.None);

        foreach (var clickable in clickables)
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

        if (_isTabHomeGroupActive)
        {
            engineController.gameObject.SetActive(true);
            engineController.Activate();
            return;
        }

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
                engineController.DeactivateSilently();
                anyDeactivated = true;
            }
        }

        if (anyDeactivated)
        {
            // 하나라도 꺼진다면 효과음 재생
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        }
    }

    private void ToggleTabHome()
    {
        _isTabHomeGroupActive = !_isTabHomeGroupActive;
        _engineUIManager.ActivateTabHomeGroup(_isTabHomeGroupActive);
    }
}
