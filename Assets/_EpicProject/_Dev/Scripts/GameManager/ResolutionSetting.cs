using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    private static readonly List<Vector2Int> SupportedResolutions = new()
    {
        // 16:9, 16:10
        new Vector2Int(1280, 720),  // HD
        new Vector2Int(1280, 800),  // WXGA

        new Vector2Int(1600, 900),  // HD+
        new Vector2Int(1440, 900),  // WXGA+

        new Vector2Int(1920, 1080), // Full HD
        new Vector2Int(1920, 1200), // WUXGA

        new Vector2Int(2560, 1440), // QHD
        new Vector2Int(2560, 1600), // WQXGA

        new Vector2Int(3200, 1800), // QHD+
        new Vector2Int(3840, 2400),  // WQUXGA

        new Vector2Int(3840, 2160), // 4K UHD
    };
    
    public int CurrentIndex { get; private set; }
    public bool IsFullScreen { get; private set; } = true;
    
    private TMP_Dropdown _resolutionDropDown;
    private Toggle _fullScreenToggle;
    
    private List<Resolution> _resolutions = new List<Resolution>();
    private Resolution[] _allResolutions;
    
    private void Awake()
    {
        _resolutionDropDown = GetComponentInChildren<TMP_Dropdown>();
        _fullScreenToggle = GetComponentInChildren<Toggle>();
        _allResolutions = Screen.resolutions;
        
        InitDropdown();
    }

    private void Start()
    {
        // 해상도 설정
        Resolution resolution = _resolutions[CurrentIndex];
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(resolution.width, resolution.height, IsFullScreen);
        
        // 토글 설정
        _fullScreenToggle.SetIsOnWithoutNotify(true);
        
        _resolutionDropDown.onValueChanged.AddListener(OnResolutionValueChanged);
        _fullScreenToggle.onValueChanged.AddListener(OnFullScreenValueChanged);
    }

    public void ChangeResolution(int index)
    {
        // Index 변경
        CurrentIndex = index;
        _resolutionDropDown.value = CurrentIndex;
        
        // 해상도 적용
        Resolution resolution = _resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, IsFullScreen);
    }

    public void ChangeFullScreen(bool isFullScreen)
    {
        // Toggle 변경
        IsFullScreen = isFullScreen;
        _fullScreenToggle.isOn = isFullScreen;
        
        // 전체화면 적용
        Screen.fullScreen = isFullScreen;
    }

    public int GetOptimalResolutionIndex()
    {
        _resolutions.Clear();

        var current = Screen.currentResolution;
        int closestIndex = 0;
        int minDiff = int.MaxValue;
        int idx = 0;

        foreach (var resVec in SupportedResolutions)
        {
            // 실제 모니터에서 지원하는 해상도만 포함
            Resolution? match = null;
            foreach (var res in _allResolutions)
            {
                if (res.width == resVec.x && res.height == resVec.y)
                {
                    match = res;
                    break;
                }
            }

            if (!match.HasValue) continue;

            _resolutions.Add(match.Value);
            int diff = Mathf.Abs(match.Value.width - current.width) + Mathf.Abs(match.Value.height - current.height);
            if (diff < minDiff)
            {
                minDiff = diff;
                closestIndex = idx;
            }
            idx++;
        }
        return closestIndex;
    }
    private void InitDropdown()
    {
        List<string> optionList = new();
        int defaultIndex = GetOptimalResolutionIndex();
        
        _resolutionDropDown.ClearOptions();
        
        foreach (var res in _resolutions)
        {
            optionList.Add($"{res.width} x {res.height}");
        }

        CurrentIndex = defaultIndex;
        
        _resolutionDropDown.AddOptions(optionList);
        _resolutionDropDown.value = CurrentIndex;
        
        _resolutionDropDown.RefreshShownValue();
    }
    
    private void OnResolutionValueChanged(int index)
    {
        ChangeResolution(index);
    }

    private void OnFullScreenValueChanged(bool isFullScreen)
    {
        ChangeFullScreen(isFullScreen);
    }
}
