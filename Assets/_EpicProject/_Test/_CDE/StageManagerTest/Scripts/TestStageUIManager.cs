using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestStageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button startBtn;
    
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Button restartBtn;
    
    [SerializeField] private Button questionBtn;

    private void Awake()
    {
        startBtn.onClick.AddListener(OnClickStartBtn);
        restartBtn.onClick.AddListener(OnClickRestartBtn);
        questionBtn.onClick.AddListener(OnClickQuestionBtn);
    }

    private void Start()
    {
        startPanel.SetActive(true);
        endingPanel.SetActive(false);
    }

    private void OnClickStartBtn()
    {
        startPanel.SetActive(false);
    }

    private void OnClickRestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnClickQuestionBtn()
    {
        startPanel.SetActive(true);
    }
    
}
