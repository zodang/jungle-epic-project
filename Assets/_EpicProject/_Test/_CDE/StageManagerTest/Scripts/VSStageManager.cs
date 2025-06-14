public class VSStageManager : StageBaseManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(true);
    }
}
