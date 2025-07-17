using Define;
using UnityEngine;

public class EndingManager : StageBaseManager
{
    private void Awake()
    {
        stageFilePath = "StageInfos/Ending";
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Ending);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadMenuSccene;
    }

    void LoadMenuSccene()
    {
        GameManager.Instance.FadeManager.LoadScene(1);
    }
}
