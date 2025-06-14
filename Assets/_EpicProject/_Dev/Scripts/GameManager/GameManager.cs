public class GameManager : Singleton<GameManager>
{
    public AudioManager AudioManager { get; private set; }
    public SettingManager SettingManager { get; private set; }
    public SaveManager SaveManager { get; private set; }

    public override void Awake()
    {
        base.Awake();

        AudioManager = GetComponentInChildren<AudioManager>();
        SettingManager = GetComponentInChildren<SettingManager>();
        SaveManager = GetComponentInChildren<SaveManager>();

        Init();
    }

    private void Init()
    {
        // 게임 초기 설정
        // TODO: 배경음 재생
    }
    
}