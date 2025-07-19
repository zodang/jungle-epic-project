using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public float CurrentBgmVolume { get; private set; }
    public float CurrentSfxVolume { get; private set; }

    [SerializeField] private Slider BgmSlider;
    [SerializeField] private Slider SfxSlider;

    private void Start()
    {
        BgmSlider.minValue = 0f;
        BgmSlider.maxValue = GetOptimalBgmVolume() * 2f;
        // BgmSlider.value = GetOptimalBgmVolume();
        BgmSlider.onValueChanged.AddListener(OnBgmValueChanged);
        
        SfxSlider.minValue = 0f;
        SfxSlider.maxValue = GetOptimalSfxVolume() * 2f;
        // SfxSlider.value = GetOptimalSfxVolume();
        SfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
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
