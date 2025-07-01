using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private Canvas _canvas;
    private GameBtnGroup _gameBtnGroup;
    private LanguageSetting _languageSetting;
    
    [SerializeField] private Button closeBtn;


    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _gameBtnGroup = GetComponentInChildren<GameBtnGroup>();
        _languageSetting = GetComponentInChildren<LanguageSetting>();
        
        closeBtn.onClick.AddListener(OpenSettingUI);
    }

    private void Start()
    {
        _canvas.enabled = false;
    }

    public void OpenSettingUI()
    {
        SwitchSettingUI();
        _canvas.enabled = !_canvas.enabled;
    }

    private void SwitchSettingUI()
    {
        // Setting UI 구분
        Scene currentScene = SceneManager.GetActiveScene();
        
        _gameBtnGroup.gameObject.SetActive(currentScene.name != "MenuScene");
        _languageSetting.gameObject.SetActive(currentScene.name == "MenuScene");
    }
}
