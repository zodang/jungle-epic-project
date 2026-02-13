using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    private bool _isInitialized = false;
    private bool _isAgreed = false;

    private string _lastStageId = "";
    private string _lastSectionIndex = "";
    private float _gameStartTime;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        _isInitialized = true;
        
        _isAgreed = GameManager.Instance.SaveManager.LoadPrivacyAgreementData();

        // 동의 시에만 정보 수집
        if (_isAgreed) OptIn();
        
        _gameStartTime = Time.realtimeSinceStartup;
    }

    public void OptIn()
    {
        _isAgreed = true;
        AnalyticsService.Instance.StartDataCollection();
    }
    
    public void OptOut()
    {
        _isAgreed = false;
        AnalyticsService.Instance.StopDataCollection();
    }

    private void OnApplicationQuit()
    {
        LogGameExit(_lastStageId, _lastSectionIndex, Time.realtimeSinceStartup - _gameStartTime);
    }

    private void LogGameExit(string stageId, string sectionId, float totalTime)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("game_exit")
        {
            { "stage_id", stageId },
            { "section_id", sectionId },
            { "total_time", totalTime }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        Debug.Log($"LOG SYSTEM: game_exit");
    }

    public void LogStageEnter(string stageId)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("stage_enter")
        {
            { "stage_id", stageId }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        _lastStageId = stageId;
        Debug.Log($"LOG SYSTEM: stage_enter");
    }

    public void LogStageExit(string stageId, string sectionId, string exitType, float elapsedTime)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("stage_exit")
        {
            { "stage_id", stageId }, 
            { "section_id", sectionId },
            { "exit_type", exitType }, 
            { "elapsed_time", elapsedTime }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        _lastStageId = stageId;
        Debug.Log($"LOG SYSTEM: stage_exit");
    }

    public void LogSectionEnter(string stageId, string sectionId, float elapsedTime)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("section_enter")
        {
            { "stage_id", stageId }, 
            { "section_id", sectionId },
            { "elapsed_time", elapsedTime }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        _lastSectionIndex = sectionId;
        Debug.Log($"LOG SYSTEM: section_enter");
    }

    public void LogGlitchUse(string stageId, string sectionId)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("glitch_use")
        {
            { "stage_id", stageId }, 
            { "section_id", sectionId }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        _lastSectionIndex = sectionId;
        Debug.Log($"LOG SYSTEM: glitch_use");
    }

    public void LogBlockControl(string stageId, string sectionId, string blockType, string blockValue, string targetObj)
    {
        if (!_isInitialized || !_isAgreed) return;

        CustomEvent customEvent = new CustomEvent("block_control")
        {
            { "stage_id", stageId },
            { "section_id", sectionId },
            { "block_type", blockType },
            { "block_value", blockValue },
            { "block_target", targetObj },
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

        Debug.Log($"LOG SYSTEM: block_control");
    }
}