using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestModeUI : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Button stageBtnPrefab;
    private int _totalSceneCount;
    
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _totalSceneCount = SceneManager.sceneCountInBuildSettings;
    }

    private void Start()
    {
        for (int i = 0; i < _totalSceneCount - 2; i++)
        {
            int index = i;

            Button newBtn = Instantiate(stageBtnPrefab, contentTransform);
            newBtn.onClick.AddListener(() => LoadScene(index));
            newBtn.GetComponentInChildren<TMP_Text>().text = $"Stage {index}";
        }

        _canvas.enabled = false;
    }

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index + 2);
        _canvas.enabled = false;
    }

    public void ActivateStageCommandUI()
    {
        _canvas.enabled = !_canvas.enabled;
    }
}
