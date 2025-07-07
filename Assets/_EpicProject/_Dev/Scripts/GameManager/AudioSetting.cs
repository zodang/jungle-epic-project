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
        BgmSlider.value = GetOptimalBgmVolume();
        BgmSlider.onValueChanged.AddListener(OnBgmValueChanged);
        
        SfxSlider.minValue = 0f;
        SfxSlider.maxValue = GetOptimalSfxVolume() * 2f;
        SfxSlider.value = GetOptimalSfxVolume();
        SfxSlider.onValueChanged.AddListener(OnSfxValueChanged);

    }

    private void OnBgmValueChanged(float volume)
    {
        CurrentBgmVolume = volume;
        GameManager.Instance.AudioManager.SetBgmVolume(volume);
    }
    
    private void OnSfxValueChanged(float volume)
    {
        CurrentSfxVolume = volume;
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
}
