#define ReleaseSteam
using Steamworks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public SettingManager SettingManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    public FadeManager FadeManager { get; private set; }
    public GameUIManager UIManager { get; private set; }
#if STEAMWORKS_NET && ReleaseSteam
    private bool isSteamInitialized = false;
    private string steamUserName = "Unknown";
    private CSteamID steamUserID;
#endif

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

#if STEAMWORKS_NET && ReleaseSteam
        if (SteamAPI.Init())
        {
            isSteamInitialized = true;
            steamUserName = SteamFriends.GetPersonaName();
            steamUserID = SteamUser.GetSteamID();
            Debug.Log($"[SteamValidator] Steam Initialized: {isSteamInitialized}");
            Debug.Log($"[SteamValidator] Steam User: {steamUserName} | SteamID: {steamUserID}");
        }
        else
        {
            Debug.LogError("[SteamValidator] SteamAPI failed to initialize. Check if Steam is running and AppID is correct.");
        }
#endif
    }

#if STEAMWORKS_NET && ReleaseSteam
    private void Update()
    {
        if (isSteamInitialized)
        {
            SteamAPI.RunCallbacks();
        }
    }
#endif

#if STEAMWORKS_NET && ReleaseSteam
    void OnApplicationQuit()
    {
        if (isSteamInitialized)
        {
            SteamAPI.Shutdown();
            Debug.Log("[SteamValidator] SteamAPI shutdown complete.");
        }
    }
#endif
}