using Define;
using UnityEngine;
public class TutorialStageManager : StageBaseManager
{
    private TriggerArea _goalTrigger;
    protected override void Awake()
    {
        base.Awake();
        // [MOD: SMG 25-06-23] 객체 다중 검색 문제로 태그 검색 방식으로 변경
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        //_goalTrigger = FindAnyObjectByType<TriggerArea>();
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
