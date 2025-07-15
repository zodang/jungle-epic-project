using Define;
using UnityEngine;

public class OpeningManager : StageBaseManager
{
    private void Awake()
    {
        stageFilePath = "StageInfos/Opening";
        base.Awake();
    }

    void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(BgmType.Menu);

        FindAnyObjectByType<VisualNovelSystem>().OnFinish += LoadNextSccene;
    }

    void LoadNextSccene()
    {
        GameManager.Instance.FadeManager.LoadNextScene();
    }
}
