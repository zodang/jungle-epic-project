using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private static int activePauseSources = 0;

    public void RequestPauseGame()
    {
        activePauseSources++;
        UpdateTimeScale();
    }

    public void RequestResumeGame()
    {
        activePauseSources--;
        if (activePauseSources < 0) activePauseSources = 0;
        
        UpdateTimeScale();
    }
    
    public void ResumeGame()
    {
        activePauseSources = 0;
        Time.timeScale = 1f;
    }

    private static void UpdateTimeScale()
    {
        if (activePauseSources > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
