using System.Collections.Generic;
using UnityEngine;

public class EngineManager : Singleton<EngineManager>
{
    [SerializeField] private EngineController engineUIPrefab;
    private Dictionary<Clickable, EngineController> _engineDictionary = new();

    private void Start()
    {
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
}
