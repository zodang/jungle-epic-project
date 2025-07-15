using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    private bool _isInitialized = false;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }

    public void LogStageEnter(string stageId)
    {
        if (!_isInitialized) return;

        CustomEvent customEvent = new CustomEvent("stage_enter")
        {
            { "stage_id", stageId }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
        AnalyticsService.Instance.Flush();

        Debug.Log($"LOG SYSTEM: stage_enter");
    }

    public void LogStageExit(string stageId, string exitType, float elapsedTime)
    {
        if (!_isInitialized) return;

        CustomEvent customEvent = new CustomEvent("stage_exit")
        {
            { "stage_id", stageId }, 
            { "exit_type", exitType }, 
            { "elapsed_time", elapsedTime }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
        AnalyticsService.Instance.Flush();

        Debug.Log($"LOG SYSTEM: stage_exit");
    }

    public void LogSectionEnter(string stageId, string sectionId, float elapsedTime)
    {
        if (!_isInitialized) return;

        CustomEvent customEvent = new CustomEvent("section_enter")
        {
            { "stage_id", stageId }, 
            { "section_id", sectionId },
            { "elapsed_time", elapsedTime }
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
        AnalyticsService.Instance.Flush();

        Debug.Log($"LOG SYSTEM: section_enter");
    }
    
    public void LogBlockControl(string stageId, string sectionId, string targetName, string blockType)
    {
        if (!_isInitialized) return;

        CustomEvent customEvent = new CustomEvent("block_control")
        {
            { "stage_id", stageId }, 
            { "section_id", sectionId },
            { "target", targetName },
            { "block", blockType}
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
        AnalyticsService.Instance.Flush();

        Debug.Log($"LOG SYSTEM: block_control");
    }
}