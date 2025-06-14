public class VSStageManager : StageBaseManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        AudioManager.Instance.PlayBgm(true);
    }
}
