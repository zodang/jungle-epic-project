using Define;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PuzzleFSM : MonoBehaviour
{
    public Action OnBlockCorrect; 

    [Header("Block System")]
    private BrokenEmotionBlock _target;
    private DebugSlot _debugSlot;
    private EngineController _engineController;
    private RectTransform _engineRectTransform;

    [Header("UI")]
    private SimpleDialogueUI _simpleDialogueUI;
    private ConsoleBox _consoleBox;

    [Header("Step")]
    private int _currentStep = -1;
    private EngineBlock _currentBlock;
    private FSM<FSMState> _fsm;
    private string _currentId;
    private BlockType[] _answerBlockTypes = new[] { BlockType.Light, BlockType.Scale, BlockType.Speed };

    private void Start()
    {
        _simpleDialogueUI = FindAnyObjectByType<SimpleDialogueUI>();
        _target = FindAnyObjectByType<BrokenEmotionBlock>();
        
        _fsm = new FSM<FSMState>();
    }

    public void StartPuzzle()
    {
        _engineController = FindAnyObjectByType<EngineController>();
        _engineRectTransform = _engineController.GetComponent<RectTransform>();
        
        _consoleBox = FindAnyObjectByType<ConsoleBox>();
        _debugSlot = FindAnyObjectByType<DebugSlot>();
        
        _debugSlot.OnBlockSet += CheckAnswer;

        ChangeStep();
    }
    
    private void CheckAnswer(EngineBlock block)
    {
        _currentBlock = block;
        
        if (block.Type != _answerBlockTypes[_currentStep])
        {
            // 오답 블록 시
            StopAllCoroutines();
            StartCoroutine(HandleWrongBlock(block));
            return;
        }
        
        // 정답 블록 시
        OnBlockCorrect?.Invoke();
        _target.OnBlockChanged?.Invoke(_currentBlock);
        _currentBlock.SetInteraction(false, false, false);
    }

    public void ChangeToNextStep()
    {
        StopAllCoroutines();
        StartCoroutine(HandleAnswer(_currentBlock));
    }

    private void ChangeStep()
    {
        _currentStep++;
        
        switch (_currentStep)
        {
            case 0:
                // 로그 시스템
                StageBaseManager.Instance.ChangeStageSection("53_1_puzzle1");
                // fsm 변경
                _fsm.ChangeState(new LightStepState(this, _target));
                break;
            
            case 1:
                // 로그 시스템
                StageBaseManager.Instance.ChangeStageSection("53_2_puzzle2");
                // fsm 변경
                _fsm.ChangeState(new ScaleStepState(this, _target));
                break;
            
            case 2:
                // 로그 시스템
                StageBaseManager.Instance.ChangeStageSection("53_3_puzzle3");
                // fsm 변경
                _fsm.ChangeState(new SpeedStepState(this, _target));
                break;
            
            case 3:
                // 로그 시스템
                string stageId = StageBaseManager.Instance.StageId;
                string sectionId = StageBaseManager.Instance.SectionId;
                float elapsedTime = Time.realtimeSinceStartup - StageBaseManager.Instance.StageStartTime;
                
                StageBaseManager.Instance.ChangeStageSection("53_4_exit_debug");
                GameManager.Instance.LogManager.LogStageExit(stageId, sectionId,"clear", elapsedTime);
                
                // 씬 변경
                GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
                GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
                break;
        }
    }

    public void ChangeDialogueText(string id)
    {
        _currentId = id;
        _simpleDialogueUI.ChangeSpeechBubbleUI(id);
    }

    public void ChangeConsoleText(string id)
    {
        _consoleBox.ChangeConsoleText(id);
    }

    private IEnumerator HandleWrongBlock(EngineBlock block)
    {
        // 오답 대사로 변경
        _simpleDialogueUI.ChangeSpeechBubbleUI("step_retry_player");
        _engineRectTransform.DOShakePosition(1f, new Vector2(10f, 10f));
        
        _currentBlock.SetInteraction(true, false, false);
        block.SetVisualState(SlotType.ToolBoxSlot);
        yield return new WaitForSeconds(1f);
        
        // 기존 Slot으로 이동
        _engineController.DropToToolBoxSlot(block);
        _debugSlot.SetSlotBlock(null);
        _currentBlock.SetInteraction(true, true, true);
        yield return new WaitForSeconds(3.0f);
        
        // 유도 대사로 변경
        _simpleDialogueUI.ChangeSpeechBubbleUI(_currentId);
    }

    private IEnumerator HandleAnswer(EngineBlock block)
    {
        yield return new WaitForSeconds(3f);
        
        _engineController.DropToToolBoxSlot(block);
        _debugSlot.SetSlotBlock(null);
        _currentBlock.SetInteraction(false, true, true);
        _currentBlock = null;
        
        ChangeStep();
    }
}
