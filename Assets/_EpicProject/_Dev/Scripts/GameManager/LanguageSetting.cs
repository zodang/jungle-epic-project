using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSetting : MonoBehaviour
{
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
        
        // Todo: 저장된 언어 설정 불러오기
        int savedIndex = 1;
        
        _languageDropdown.value = savedIndex;
        ChangeLanguage(savedIndex);
    }

    private void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        // Todo: 언어 설정 저장
    }
}
