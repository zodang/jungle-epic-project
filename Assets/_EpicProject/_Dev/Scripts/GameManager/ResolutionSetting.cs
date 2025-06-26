using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{
    private TMP_Dropdown _resolutionDropDown;
    private Toggle _fullScreenToggle;
    
    private List<Resolution> _resolutions = new List<Resolution>();
    private int _currentResolutionIndex;
    private bool _isFullScreen = true;
    
    private void Awake()
    {
        _resolutionDropDown = GetComponentInChildren<TMP_Dropdown>();
        _fullScreenToggle = GetComponentInChildren<Toggle>();
        
        InitDropdown();
    }

    private void Start()
    {
        // 해상도 설정
        Resolution resolution = _resolutions[_currentResolutionIndex];
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(resolution.width, resolution.height, _isFullScreen);
        
        // 토글 설정
        _fullScreenToggle.SetIsOnWithoutNotify(true);
        
        _resolutionDropDown.onValueChanged.AddListener(OnResolutionValueChanged);
        _fullScreenToggle.onValueChanged.AddListener(OnFullScreenValueChanged);
    }
    
    private void OnResolutionValueChanged(int index)
    {
        Resolution resolution = _resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, _isFullScreen);
    }

    private void OnFullScreenValueChanged(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    private void InitDropdown()
    {
        _resolutions.Clear();
        _resolutionDropDown.options.Clear();

        List<string> optionList = new();
        HashSet<string> addedResolutions = new(); // 중복 방지용
        Resolution[] allResolutions = Screen.resolutions;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            Resolution res = allResolutions[i];
            float aspect = (float)res.width / res.height;

            // 16:9 혹은 16:10 비율
            bool is16by9 = Mathf.Approximately(aspect, 16f / 9f);
            bool is16by10 = Mathf.Approximately(aspect, 16f / 10f);

            if (!is16by9 && !is16by10) continue;

            string key = $"{res.width} x {res.height}";

            // 중복 해상도 방지
            if (addedResolutions.Contains(key)) continue;

            addedResolutions.Add(key);
            _resolutions.Add(res);
            
            string label = $"{res.width} x {res.height} {res.refreshRateRatio}Hz";

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                // 현재 해상도 설정
                _currentResolutionIndex = _resolutions.Count - 1;
                label = $"{res.width} x {res.height} {res.refreshRateRatio}Hz *";
            }
            
            optionList.Add(label);
        }

        _resolutionDropDown.AddOptions(optionList);
        _resolutionDropDown.value = _currentResolutionIndex;
        _resolutionDropDown.RefreshShownValue();
    }
}
