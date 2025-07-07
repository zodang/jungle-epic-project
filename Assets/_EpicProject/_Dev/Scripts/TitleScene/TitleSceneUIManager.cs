using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUIManager : MonoBehaviour
{
    private TitleSceneManager _titleSceneManager;
    
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueGameBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitGameBtn;

    private Image _continueImg;
    private TMP_Text _continueText;

    private void Awake()
    {
        _titleSceneManager = FindAnyObjectByType<TitleSceneManager>();
        
        _continueImg = continueGameBtn.GetComponent<Image>();
        _continueText = continueGameBtn.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        newGameBtn.onClick.AddListener(OnClickNewGameBtn);
        continueGameBtn.onClick.AddListener(OnClickContinueGameBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        exitGameBtn.onClick.AddListener(OnClickExitGameBtn);
    }
    
    public void SetContinueBtn(bool isNew)
    {
        if (isNew)
        {
            continueGameBtn.enabled = false;
            _continueImg.color = new Color(_continueImg.color.r, _continueImg.color.g, _continueImg.color.b, 0.5f);
            _continueText.color = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            continueGameBtn.enabled = true;
            _continueImg.color = new Color(_continueImg.color.r, _continueImg.color.g, _continueImg.color.b, 1.0f);
            _continueText.color = new Color(0, 0, 0, 1.0f);
        }
    }

    private void OnClickNewGameBtn()
    {
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            "새로 시작",
            "진행 중인 게임 데이터가 삭제됩니다.\n계속 진행하시겠습니까?",
            onOk: () =>
            {
                _titleSceneManager.StartNewGame();
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            "계속하기",
            "취소"
        );
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
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            "게임 종료",
            "게임을 종료하시겠습니까?",
            onOk: () =>
            {
                _titleSceneManager.ExitGame();
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            "종료하기",
            "취소"
        );
    }
}
