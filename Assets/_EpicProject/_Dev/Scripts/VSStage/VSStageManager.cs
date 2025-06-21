public class VSStageManager : StageBaseManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(true);
        ObjectPropertyController foundAxe = FindAnyObjectByType<ObjectPropertyController>();// FindObjectOfType<ObjectPropertyController>();
        if (foundAxe != null)
        {
            foundAxe.Submerge();
        }
    }
}
