using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public SettingManager SettingManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    public FadeManager FadeManager { get; private set; }
    public GameUIManager UIManager { get; private set; }
    public LogManager LogManager { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        AudioManager = GetComponentInChildren<AudioManager>();
        SettingManager = GetComponentInChildren<SettingManager>();
        SaveManager = GetComponentInChildren<SaveManager>();
        FadeManager = GetComponentInChildren<FadeManager>();
        UIManager = GetComponentInChildren<GameUIManager>();
        LogManager = GetComponentInChildren<LogManager>();
    }
}