using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public float CurrentBgmVolume { get; private set; }
    public float CurrentSfxVolume { get; private set; }

    [SerializeField] private Slider BgmSlider;
    [SerializeField] private Slider SfxSlider;

    private void Awake()
    {
        //[MOD] KMS 기능수정 환경설정 오디오 초기 값 중간 값으로 설정
        // 1) 슬라이더 범위 세팅
        float optimalBgm = GetOptimalBgmVolume();
        float optimalSfx = GetOptimalSfxVolume();
        BgmSlider.minValue = 0f;
        BgmSlider.maxValue = optimalBgm * 2f;
        SfxSlider.minValue = 0f;
        SfxSlider.maxValue = optimalSfx * 2f;

        // 2) 리스너 등록 (이후 value 변경 시 Change... 호출)
        BgmSlider.onValueChanged.AddListener(OnBgmValueChanged);
        SfxSlider.onValueChanged.AddListener(OnSfxValueChanged);

        // 3) 중간값 계산 & 슬라이더에 할당
        float midBgm = (BgmSlider.minValue + BgmSlider.maxValue) / 2f;
        float midSfx = (SfxSlider.minValue + SfxSlider.maxValue) / 2f;
        BgmSlider.value = midBgm;
        SfxSlider.value = midSfx;

        // 4) 초기값을 실제 볼륨에도 즉시 적용
        OnBgmValueChanged(midBgm);
        OnSfxValueChanged(midSfx);
    }

    public void ChangeBgmVolume(float volume)
    {
        CurrentBgmVolume = volume;
        BgmSlider.value = volume;
        
        GameManager.Instance.AudioManager.SetBgmVolume(volume);
    }
    
    public void ChangeSfxVolume(float volume)
    {
        CurrentSfxVolume = volume;
        SfxSlider.value = volume;
        
        GameManager.Instance.AudioManager.SetSfxVolume(volume);
    }
    
    public float GetOptimalBgmVolume()
    {
        return GameManager.Instance.AudioManager.DefaultBgmVolume;
    }
    
    public float GetOptimalSfxVolume()
    {
        return GameManager.Instance.AudioManager.DefaultSfxVolume;
    }

    private void OnBgmValueChanged(float volume)
    {
        ChangeBgmVolume(volume);
    }
    
    private void OnSfxValueChanged(float volume)
    {
        ChangeSfxVolume(volume);
    }
}
