using System;
using UnityEngine;

public class CheckTutorialCave : MonoBehaviour
{
    bool _isInit = false;

    public Clickable Player;


    int _tutorialStep;

    private readonly string[] flagsName =
    {
        "None",
        "Tutorial_3_01"
    };

    void Start()
    {
        _tutorialStep = 1;
        _updateTimeDelta = 0f;
        Invoke("Init", 0.55f);
    }

    private void OnDisable()
    {
        if (_isInit)
        {
            //for(int i = 0; i < _checkActions.Count; i++)
            //{
            //    TutorialStageManager.Instance.EngineManager.OnActivateEngineUI -= CheckOpenPanel;
            //}
        }
    }

    private float _updateTimeDelta;

    private int _playerBlockCnt;

    // Update is called once per frame
    void Update()
    {
        if (!_isInit) return;

        _playerBlockCnt = Player.GetComponent<Inventory>().BlockList.Count;

        if(_tutorialStep == 1 && _playerBlockCnt > 1)
        {
            _updateTimeDelta += Time.deltaTime;
            if (_updateTimeDelta > 0.7f)
            {
                _updateTimeDelta = 0f;
                StartTutorial(_tutorialStep);
            }
        }
    }

    void Init()
    {
        _isInit = true;
    }

    public void StartTutorial(int ID)
    {
        if (_tutorialStep != ID || flagsName.Length <= ID) return;

        if (ID == flagsName.Length - 1) _tutorialStep = ID;

        StageBaseManager.Instance.FlagManager.SetFlag(flagsName[_tutorialStep]);

        Player.GetComponentInChildren<NPCInteraction>().InteractWithNPC();
        _tutorialStep++;
    }
    
    void StartTutorialInvoke()
    {
        StartTutorial(_tutorialStep);
    }

    public int TestTurorialID;
    [ContextMenu("TestTurorialID(TestTurorialID)")]
    void StartTutorial()
    {
        StartTutorial(TestTurorialID);
    }
}
