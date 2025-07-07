using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSetting : MonoBehaviour
{
    public int CurrentIndex { get; private set; }

    private TMP_Dropdown _languageDropdown;
    private readonly List<string> _languageOptions = new List<string> {"English", "한국어", "中文" };
    
    private void Awake()
    {
        _languageDropdown = GetComponentInChildren<TMP_Dropdown>();
        _languageDropdown.onValueChanged.AddListener(LanguageValueChanged);
        InitDropdown();
    }

    private void LanguageValueChanged(int index)
    {
        ChangeLanguage(index);
    }

    private void InitDropdown()
    {
        _languageDropdown.ClearOptions();
        _languageDropdown.AddOptions(_languageOptions);
        
        ChangeLanguage(CurrentIndex);
    }

    public void ChangeLanguage(int index)
    {
        CurrentIndex = index;
        
        _languageDropdown.value = CurrentIndex;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
