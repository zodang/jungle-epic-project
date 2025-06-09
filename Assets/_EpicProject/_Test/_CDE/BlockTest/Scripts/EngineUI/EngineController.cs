using Define;
using UnityEngine;
using System.Collections;

public class EngineController : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }

    private EngineUIController _engineUIController;
    private EngineBlockController _engineBlockInspector;
    private Animator _engineAnimator;

    //0.25초 후에 UI 끄기 위해
    [SerializeField] private float _durationTime = 0.25f;

    private void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        _engineBlockInspector = GetComponent<EngineBlockController>();
        _engineAnimator = GetComponent<Animator>(); 
    }

    private void Start()
    {
        // Button 기능 연결
        _engineUIController.OnClickCloseBtn += CloseInspector;
        _engineUIController.OnResetBtnClicked += ResetFeature;
        
        gameObject.AddComponent<DraggableUI>();
        gameObject.SetActive(false);
    }

    public void OpenInspector(Clickable target)
    {
        // 다른 오브젝트일 때만 작동
        if (CurrentTarget == target) return;
        
        AudioManager.instance.playSfx(SfxType.Open);
        
        // Block 세팅
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
        
        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile(), target);
        _engineUIController.SetUIPosition(target);
        
        gameObject.SetActive(true);
        
        // On 애니메이션 실행 
        _engineAnimator.Play("On Ani");
    }

    private void CloseInspector()
    {
        AudioManager.instance.playSfx(SfxType.Close);

        CurrentTarget = null;
        _engineUIController.ClearProfile();

        // Off 애니메이션 실행 
        _engineAnimator.Play("Off Ani");
        StartCoroutine(CloseAfterAnimation());
    }
    
    public void RefreshSlot(Clickable target)
    {
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
    }

    private void ResetFeature()
    {
        if (CurrentTarget == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = CurrentTarget.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }
    
    private IEnumerator CloseAfterAnimation()
    {
        yield return new WaitForSeconds(_durationTime);

        CurrentTarget = null;
        _engineUIController.ClearProfile();

        gameObject.SetActive(false);
    }
}
