using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        closeBtn.onClick.AddListener(OpenSettingUI);
    }

    private void Start()
    {
        _canvas.enabled = false;
    }

    public void OpenSettingUI()
    {
        _canvas.enabled = !_canvas.enabled;
    }
}
