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
    }

    void Update()
    {
        
    }
}
