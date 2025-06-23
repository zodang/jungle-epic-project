using Define;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
    [SerializeField] private EngineController engineUIPrefab;
    private Dictionary<Clickable, EngineController> _engineDictionary = new();
    //public event Action<Clickable> OnActivateEngineUI;
    

    private void Start()
    {
        // ESC 키로 모든 EngineUI 비활성화
        StageManager.Instance.InputManager.OnOffEngine += DeactivateAllEngine;
        
        // Clickable마다 UI 추가
        Clickable[] clickables = FindObjectsByType<Clickable>(FindObjectsSortMode.None);

        foreach (var clickable in clickables)
        {
            EngineController engineUI = Instantiate(engineUIPrefab, transform);
            _engineDictionary.Add(clickable, engineUI);

            // Clickable의 기본 블록 세팅
            clickable.InitDefaultBlock();
            
            // Clickable대로 EngineUI 세팅 
            engineUI.InitEngineController(clickable);
        }
    }

    public void ActivateEngineUI(Clickable clickable)
    {
        if (_engineDictionary.TryGetValue(clickable, out EngineController engineController))
        {
            //OnActivateEngineUI?.Invoke(clickable);
            engineController.gameObject.SetActive(true);
            engineController.Activate();
        }
    }

    public void NotifyBlockChanged(Clickable clickable)
    {
        if (_engineDictionary.TryGetValue(clickable, out EngineController engineController))
        {
            engineController.RefreshSlot(clickable);
        }
    }

    // [Mod: SMG 25-06-23] 튜토리얼 상태 체크를 위해 추가
    public bool GetActivateEngineUI(Clickable clickable)
    {
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
    
    private void OnDestroy()
    {
        StageManager.Instance.InputManager.OnOffEngine -= DeactivateAllEngine;
    }
}
