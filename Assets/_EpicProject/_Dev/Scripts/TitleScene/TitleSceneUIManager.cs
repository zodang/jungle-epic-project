using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUIManager : MonoBehaviour
{
    private TitleSceneManager _titleSceneManager;
    
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueGameBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitGameBtn;

    private void Awake()
    {
        _titleSceneManager = FindAnyObjectByType<TitleSceneManager>();
    }

    private void Start()
    {
        newGameBtn.onClick.AddListener(OnClickNewGameBtn);
        continueGameBtn.onClick.AddListener(OnClickContinueGameBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        exitGameBtn.onClick.AddListener(OnClickExitGameBtn);
    }

    public void SetContinueBtn(bool isEnable)
    {
        // continueGameBtn.enabled = isEnable;
    }

    private void OnClickNewGameBtn()
    {
        _titleSceneManager.StartNewGame();
    }

    private void OnClickContinueGameBtn()
    {
        _titleSceneManager.ContinueGame();
    }
    
    private void OnClickSettingBtn()
    {
        GameManager.Instance.SettingManager.OpenSetting();
    }
    
    private void OnClickExitGameBtn()
    {
        _titleSceneManager.ExitGame();
    }
}
