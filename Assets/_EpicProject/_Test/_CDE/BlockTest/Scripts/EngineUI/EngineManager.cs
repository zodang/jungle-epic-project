using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
    public event Action OnEngineSettingEnd;
    
    [SerializeField] private EngineController engineUIPrefab;

    private Dictionary<Clickable, EngineController> _engineDictionary = new();
    private BlockContainerBase _inventory;

    private void Start()
    {
        _inventory = StageBaseManager.Instance.PlayerManager.Inventory;
        
        // ESC 키로 모든 EngineUI 비활성화
        StageManager.Instance.InputManager.OnTabPressed += DeactivateAllEngine;
        
        // Clickable마다 UI 
        foreach (var clickable in FindObjectsByType<Clickable>(FindObjectsSortMode.None))
        {
            // 플레이어 제외
            if (clickable.GetComponent<PlayerManager>() != null)
            {
                clickable.InitBlockContainerBase(_inventory);
                continue;
            }
            
            EngineController engineController = Instantiate(engineUIPrefab, transform);
            _engineDictionary.Add(clickable, engineController);

            // Clickable대로 EngineUI 세팅 
            clickable.InitBlockContainerBase(engineController);
            clickable.InitEngineController(engineController);
        }
        
        OnEngineSettingEnd?.Invoke();
    }
    
    private void OnDestroy()
    {
        StageManager.Instance.InputManager.OnTabPressed -= DeactivateAllEngine;
    }

    public void DisableEngineDeactivate()
    {
        StageManager.Instance.InputManager.OnTabPressed -= DeactivateAllEngine;
    }

    public void ActivateEngineUI(Clickable clickable)
    {
        if (!_engineDictionary.TryGetValue(clickable, out EngineController engineController)) return;
        if (engineController.IsActivate) return;
        
        DeactivateAllEngineSilently();
        engineController.gameObject.SetActive(true);
        engineController.Activate();
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

        // 하나라도 꺼진다면 효과음 재생
        if (anyDeactivated)
        {
            GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        }
    }
    
    private void DeactivateAllEngineSilently()
    {
        foreach (var engineController in _engineDictionary.Values)
        {
            if (engineController.gameObject.activeSelf)
            {
                engineController.DeactivateSilently();
            }
        }
    }
}
