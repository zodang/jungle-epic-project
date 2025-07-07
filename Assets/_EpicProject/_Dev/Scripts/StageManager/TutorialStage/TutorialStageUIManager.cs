using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStageUIManager : MonoBehaviour
{
    [SerializeField] private Button questionBtn;
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button startBtn;

    private void Awake()
    {
        // questionBtn.onClick.AddListener(OnClickQuestionBtn);
        // startBtn.onClick.AddListener(OnClickStartBtn);
    }
    
    private void Start()
    {
        // startPanel.SetActive(true);
    }
    
    private void OnClickQuestionBtn()
    {
        startPanel.SetActive(true);
    }
    
    private void OnClickStartBtn()
    {
        startPanel.SetActive(false);
    }
}
