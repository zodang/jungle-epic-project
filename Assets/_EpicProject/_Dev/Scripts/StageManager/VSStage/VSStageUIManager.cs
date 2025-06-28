using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VSStageUIManager : MonoBehaviour
{
    [SerializeField] private Button questionBtn;
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button startBtn;
    
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Button restartBtn;

    private void Awake()
    {
        questionBtn.onClick.AddListener(OnClickQuestionBtn);
        startBtn.onClick.AddListener(OnClickStartBtn);
        restartBtn.onClick.AddListener(OnClickRestartBtn);
    }

    private void Start()
    {
        endingPanel.SetActive(false);
    }
    
    private void OnClickQuestionBtn()
    {
        startPanel.SetActive(true);
    }

    private void OnClickStartBtn()
    {
        startPanel.SetActive(false);
    }

    private void OnClickRestartBtn()
    {
        GameManager.Instance.AudioManager.PlayBgm(false);
        GameManager.Instance.FadeManager.LoadScene(1);
    }
}
