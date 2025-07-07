using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    public int CurrentIndex { get; private set; }
    public bool IsFullScreen { get; private set; } = true;
    
    private TMP_Dropdown _resolutionDropDown;
    private Toggle _fullScreenToggle;
    
    private List<Resolution> _resolutions = new List<Resolution>();
    
    private void Awake()
    {
        _resolutionDropDown = GetComponentInChildren<TMP_Dropdown>();
        _fullScreenToggle = GetComponentInChildren<Toggle>();
        
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
        HashSet<string> added = new();
        Resolution[] allRes = Screen.resolutions;

        var current = Screen.currentResolution;
        int closestIndex = 0;
        int minDiff = int.MaxValue;
        int idx = 0;

        for (int i = 0; i < allRes.Length; i++)
        {
            Resolution res = allRes[i];
            float aspect = (float)res.width / res.height;
            bool is16by9 = Mathf.Approximately(aspect, 16f / 9f);
            bool is16by10 = Mathf.Approximately(aspect, 16f / 10f);
            if (!is16by9 && !is16by10) continue;

            string key = $"{res.width}x{res.height}";
            if (added.Contains(key)) continue;
            added.Add(key);

            _resolutions.Add(res);

            int diff = Mathf.Abs(res.width - current.width) + Mathf.Abs(res.height - current.height);
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
