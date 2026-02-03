using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TitleSceneUIManager : MonoBehaviour
{
    private TitleSceneManager _titleSceneManager;
    
    [SerializeField] private Button privacyPolicyBtn;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueGameBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitGameBtn;

    private bool _isNewGame;
    private Image _continueImg;
    private TMP_Text _continueText;
    
    private RawImage _continueRawImage;
    
    private void Awake()
    {
        _titleSceneManager = FindAnyObjectByType<TitleSceneManager>();
        
        _continueImg = continueGameBtn.GetComponent<Image>();
        _continueText = continueGameBtn.GetComponentInChildren<TMP_Text>();
        _continueRawImage = continueGameBtn.GetComponentInChildren<RawImage>();
    }

    private void Start()
    {
        privacyPolicyBtn.onClick.AddListener(OnClickPrivacyPolicyBtn);
        newGameBtn.onClick.AddListener(OnClickNewGameBtn);
        continueGameBtn.onClick.AddListener(OnClickContinueGameBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        exitGameBtn.onClick.AddListener(OnClickExitGameBtn);
    }

    private void OnClickPrivacyPolicyBtn()
    {
        string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Title");
        string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Message");
        string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Confirm");
        string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_More");

        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            title,
            message,
            onOk: () =>
            {
                GameManager.Instance.SaveManager.SavePrivacyData(true);
                GameManager.Instance.UIManager.ActivateGameUIManager(true);
            },
            onCancel: () =>
            {
                GameManager.Instance.SaveManager.SavePrivacyData(true);
                GameManager.Instance.UIManager.ActivateGameUIManager(true);
            },
            okLabel,
            cancelLabel
        );
    }
    
    public void SetContinueBtn(bool isNew)
    {
        _isNewGame = isNew;
        
        if (isNew)
        {
            continueGameBtn.enabled = false;
            _continueImg.color = new Color(_continueImg.color.r, _continueImg.color.g, _continueImg.color.b, 0f);
            _continueText.color = new Color(0, 0, 0, 0.5f);
            _continueRawImage.color = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            continueGameBtn.enabled = true;
            _continueImg.color = new Color(_continueImg.color.r, _continueImg.color.g, _continueImg.color.b, 0f);
            _continueText.color = new Color(0, 0, 0, 1.0f);
            _continueRawImage.color = new Color(0, 0, 0, 1f);
        }
    }

    private void OnClickNewGameBtn()
    {
        if (_isNewGame)
        {
            _titleSceneManager.StartNewGame();
        }
        else
        {
            string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "New_Title");
            string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "New_Message");
            string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "New_Confirm");
            string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "New_Cancel");
            
            GameManager.Instance.UIManager.PopupUI.ShowPopup
            (
                title,
                message,
                onOk: () => { _titleSceneManager.StartNewGame(); },
                onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
                okLabel,
                cancelLabel
            );
        }
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
        string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Title");
        string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Message");
        string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Confirm");
        string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Cancel");
        
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            title,
            message,
            onOk: () => {_titleSceneManager.ExitGame(); },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            okLabel,
            cancelLabel
        );
    }
}
