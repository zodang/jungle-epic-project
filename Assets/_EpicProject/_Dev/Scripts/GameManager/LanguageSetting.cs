using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSetting : MonoBehaviour
{
    public int CurrentIndex { get; private set; }

    private TMP_Dropdown _languageDropdown;
    private readonly List<string> _languageOptions = new List<string> {"English", "한국어", "中文" };
    
    private bool _suppressDropdownEvent = false;
    
    private void Awake()
    {
        _languageDropdown = GetComponentInChildren<TMP_Dropdown>();
        _languageDropdown.onValueChanged.AddListener(LanguageValueChanged);

        CurrentIndex = GetOptimalLanguage();
        InitDropdown();
    }

    public int GetOptimalLanguage()
    {
        int optimalLanguage = 0;
        
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                optimalLanguage = 1;
                break;
            
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.Chinese:
                optimalLanguage = 2;
                break;
            
            case SystemLanguage.English:
            default:
                optimalLanguage = 0;
                break;
        }

        return optimalLanguage;
    }

    private void InitDropdown()
    {
        _languageDropdown.ClearOptions();
        _languageDropdown.AddOptions(_languageOptions);
        
        ChangeLanguage(CurrentIndex);
    }

    private void LanguageValueChanged(int index)
    {
        if (_suppressDropdownEvent) return;
        ChangeLanguage(index);
    }

    public void ChangeLanguage(int index)
    {
        CurrentIndex = index;
        
        _languageDropdown.value = CurrentIndex;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
