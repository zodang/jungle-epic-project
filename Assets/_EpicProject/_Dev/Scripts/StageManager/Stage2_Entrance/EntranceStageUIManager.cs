using UnityEngine;
using UnityEngine.UI;

public class EntranceStageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Button restartBtn;
    
    private void Awake()
    {
        restartBtn.onClick.AddListener(OnClickRestartBtn);
    }
    private void Start()
    {
        endingPanel.SetActive(false);
    }
    
    private void OnClickRestartBtn()
    {
        GameManager.Instance.AudioManager.PlayBgm(false);
        GameManager.Instance.FadeManager.LoadScene(1);
    }
}
