using Define;

public class TutorialStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    protected override void Awake()
    {
        base.Awake();
        _goalTrigger = FindAnyObjectByType<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);
        
    }

    private void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(true);
        Invoke(nameof(CollectBlock), .05f);
    }

    private void CollectBlock()
    {
        FindAnyObjectByType<Inventory>().Collect(BlockType.PlayerControl);
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadScene();
    }
}
