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
    }

    private void OnGoalTriggered()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
