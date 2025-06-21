using Define;
using UnityEngine.SceneManagement;

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
        // BGM 실행
        FindAnyObjectByType<Inventory>().Collect(BlockType.PlayerControl);
    }

    private void OnGoalTriggered()
    {
        GameManager.Instance.FadeManager.LoadScene();
    }
}
