using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameBtnGroup : MonoBehaviour
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button quitBtn;
    
    private void Awake()
    {
        restartBtn.onClick.AddListener(OnClickRestartBtn);
        menuBtn.onClick.AddListener(OnClickMenuBtn);
        quitBtn.onClick.AddListener(OnClickQuitBtn);
    }

    private void OnClickRestartBtn()
    {
        GameManager.Instance.FadeManager.LoadCurrentScene();
        GameManager.Instance.SettingManager.OpenSetting();
    }

    private void OnClickMenuBtn()
    {
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            "게임 종료",
            "메뉴로 돌아가시겠습니까?",
            onOk: () =>
            {
                GameManager.Instance.FadeManager.LoadScene(1);
                GameManager.Instance.SettingManager.OpenSetting();
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            "돌아가기",
            "취소"
        );
    }

    private void OnClickQuitBtn()
    {
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            "게임 종료",
            "게임을 종료하시겠습니까?",
            onOk: () =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            "종료하기",
            "취소"
        );
    }
}
