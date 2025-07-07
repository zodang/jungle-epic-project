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
        GameManager.Instance.FadeManager.LoadScene(1);
        GameManager.Instance.SettingManager.OpenSetting();
    }

    private void OnClickQuitBtn()
    {
        GameManager.Instance.FadeManager.LoadScene(1);
        GameManager.Instance.SettingManager.OpenSetting();
    }
    
}
