using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckTutorial : MonoBehaviour
{
    bool _isInit = false;

    public Clickable Player;
    public Clickable Rock;

    int _tutorialStep;

    private readonly string[] flagsName =
    {
        "None",
        "Tutorial_1_01",
        "Tutorial_1_02",
        "Tutorial_1_03",
        "Tutorial_1_04",
        "Tutorial_1_05",
        "Tutorial_2_01",
        "Tutorial_2_02",
        "Tutorial_2_03",
        "Tutorial_2_04"
    };
    //[SerializeField] private bool _isOpenPlayerUI;
    //[SerializeField] private bool _isOpenRockUI;

    //private List<Action<Clickable>> _checkActions = new List<Action<Clickable>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tutorialStep = 1;
        _updateTimeDelta = 0f;
        Invoke("Init", 0.3f);
    }

    private void OnDisable()
    {
        if(_isInit)
        {
            //for(int i = 0; i < _checkActions.Count; i++)
            //{
            //    TutorialStageManager.Instance.EngineManager.OnActivateEngineUI -= CheckOpenPanel;
            //}
        }
    }

    private float _updateTimeDelta;

    private bool _isPlayerUIOpen;
    private bool _isRockUIOpen;
    private bool _isPlayerMoveEnable;
    private bool _isRockMoveEnable;
    private bool _isGlitchVision;

    // Update is called once per frame
    void Update()
    {
        TestDebug();
        if (!_isInit) return;

        _isPlayerUIOpen = TutorialStageManager.Instance.EngineManager.GetActivateEngineUI(Player);
        _isRockUIOpen = TutorialStageManager.Instance.EngineManager.GetActivateEngineUI(Rock);
        _isPlayerMoveEnable = Player.GetComponent<PlayerManager>().Feature._enableMove;
        _isRockMoveEnable = Rock.GetComponent<Rock>().EnableMove;
        _isGlitchVision = FindAnyObjectByType<GlitchVision>().IsGlitchVisionActive;

        switch (_tutorialStep)
        {
            case 2:
                if(StageManager.Instance.InputManager.MoveInput != Vector2.zero)
                {
                    _updateTimeDelta += Time.deltaTime;
                    if(_updateTimeDelta >= 1f)
                    {
                        _updateTimeDelta = 0f;
                        StartTutorial(_tutorialStep);
                    }
                }
                break;
            case 3:
                if(_isGlitchVision)
                {
                    _updateTimeDelta += Time.deltaTime;
                    if(_updateTimeDelta >= 0.5f)
                    {
                        _updateTimeDelta = 0f;
                        StartTutorial(_tutorialStep);
                    }
                }
                break;
            case 4:
                if (_isPlayerUIOpen)
                {
                    _updateTimeDelta += Time.deltaTime;
                    if (_updateTimeDelta >= 0.8f)
                    {
                        _updateTimeDelta = 0f;
                        StartTutorial(_tutorialStep);
                    }
                }
                break;
            case 5:
                if (_isPlayerMoveEnable)
                {
                    if(StageManager.Instance.InputManager.MoveInput != Vector2.zero)
                    {
                        _updateTimeDelta += Time.deltaTime;
                        if(_updateTimeDelta > 1f)
                        {
                            _updateTimeDelta = 0f;
                            StartTutorial(_tutorialStep);
                        }
                    }
                }
                break;
            case 7:
                if(_isGlitchVision)
                {
                    _updateTimeDelta += Time.deltaTime;
                    if(_updateTimeDelta >= 0.5f)
                    {
                        _updateTimeDelta = 0f;
                        StartTutorial(_tutorialStep);
                    }
                }
                break;
            case 8:
                if (_isRockMoveEnable)
                {
                    StartTutorial(_tutorialStep);
                }
                break;
        }
    }
    
    void Init()
    {
        Debug.Log("Call Init");
        //var activatePlayerUIAction = CreateCheckAction(Player, () => _isOpenPlayerUI = true);
        //_checkActions.Add(activatePlayerUIAction);
        //var activateRockUIAction = CreateCheckAction(Rock, () => _isOpenRockUI = true);
        //_checkActions.Add(activateRockUIAction);


        //for (int i = 0; i < _checkActions.Count; i++)
        //{
        //    TutorialStageManager.Instance.EngineManager.OnActivateEngineUI += _checkActions[i];
        //}
        _isInit = true;
    }

    void CheckTutorial1()
    {
        
    }

    public void StartTutorial(int ID)
    {
        if (_tutorialStep != ID || flagsName.Length <= ID) return;

        StageManager.Instance.FlagManager.SetFlag(flagsName[_tutorialStep]);


        Player.GetComponentInChildren<NPCInteraction>().InteractWithNPC();
        _tutorialStep++;
    }

    void StartTutorial1()
    {
        Debug.Log("call StartTutorial1");
    }

    public void StartTutorial2()
    {
        Debug.Log("call StartTutorial2");
    }

    void CheckOpenPanel(Clickable target)
    {
    }

    [Header("Test")]
    public bool TestIsPlayerUIOpen;
    public bool TestIsRockUIOpen;
    public bool TestIsPlayerMoveEnable;
    public bool TestIsRockMoveEnable;
    public bool TestIsGlitchVision;
    void TestDebug()
    {
        TestIsPlayerUIOpen = TutorialStageManager.Instance.EngineManager.GetActivateEngineUI(Player);
        TestIsRockUIOpen = TutorialStageManager.Instance.EngineManager.GetActivateEngineUI(Rock);
        TestIsPlayerMoveEnable = Player.GetComponent<PlayerManager>().Feature._enableMove;
        TestIsRockMoveEnable = Rock.GetComponent<Rock>().EnableMove;
        TestIsGlitchVision = FindAnyObjectByType<GlitchVision>().IsGlitchVisionActive;
    }

    public Action<Clickable> CreateCheckAction(Clickable target, Action onMatch)
    {
        return (Clickable input) =>
        {
            if (IsSameClickable(input, target))
            {
                onMatch?.Invoke();
            }
        };
    }

    public bool IsSameClickable(Clickable a, Clickable b)
    {
        return a == b;
    }

    public int TestTurorialID;
    [ContextMenu("testaa")]
    void asdads()
    {
        StartTutorial(TestTurorialID);
    }
}
