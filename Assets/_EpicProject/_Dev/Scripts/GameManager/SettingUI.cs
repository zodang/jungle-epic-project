using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Action OnCloseBtnClicked;
    private Canvas _canvas;
    private GameBtnGroup _gameBtnGroup;
    private LanguageSetting _languageSetting;

    [SerializeField] private Button closeBtn;


    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _gameBtnGroup = GetComponentInChildren<GameBtnGroup>();
        _languageSetting = GetComponentInChildren<LanguageSetting>();

        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void Start()
    {
        OpenSettingUI(false);
    }

    private void OnClickCloseBtn()
    {
        OnCloseBtnClicked?.Invoke();
    }

    public void OpenSettingUI(bool isOpen)
    {
        SwitchSettingUI(); 
        _canvas.enabled = isOpen;
    }

    private void SwitchSettingUI()
    {
        // Setting UI 구분
        Scene currentScene = SceneManager.GetActiveScene();

        _gameBtnGroup.gameObject.SetActive(currentScene.name != "MenuScene");
        _languageSetting.gameObject.SetActive(currentScene.name == "MenuScene");
    }
}