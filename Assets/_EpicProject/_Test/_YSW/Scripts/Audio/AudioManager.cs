using UnityEngine;
using Define;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("#BGM")]
    //public AudioClip bgmClip;
    public float DefaultBgmVolume { get; private set; } = 0.2f;
    public float DefaultSfxVolume { get; private set; } = 0.5f;

    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    // BGM 타입과 오디오 클립을 연결하기 위한 클래스
    [System.Serializable]
    public class BgmSound
    {
        public BgmType type;
        public AudioClip clip;
    }

    [SerializeField]
    private List<BgmSound> bgmClips; // Inspector에서 설정할 BGM 리스트
    private Dictionary<BgmType, AudioClip> _bgmClipDict; // 실제 게임에서 사용할 BGM 데이터
    private BgmType _currentBgmType = BgmType.None;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex = 0;

    // Unity Inspector에서 SfxType별 쿨타임을 설정할 수 있도록 도와주는 클래스입니다.
    // AudioManager 클래스 밖에 있어도 되고, 안에 있어도 괜찮습니다.
    [System.Serializable]
    public class SfxCooldown
    {
        public SfxType type;
        public float cooldown;
    }

    // --- [새로 추가된 부분 1] 쿨다운 관련 변수들 ---
    [Header("#SFX Cooldown Settings")]
    [Tooltip("여기에 쿨타임을 적용할 효과음과 시간을 설정하세요.")]
    [SerializeField] private List<SfxCooldown> sfxCooldownSettings; // Inspector에서 설정할 쿨타임 리스트

    private Dictionary<SfxType, float> _sfxCooldowns; // 실제 게임에서 사용할 쿨타임 데이터
    private Dictionary<SfxType, float> _sfxLastPlayTimes; // 각 효과음의 마지막 재생 시간을 저장
    // --- [새로 추가된 부분 1 끝] ---



    private void Awake()
    {
        Init();
    }

    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        //bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // --- [추가] BGM 데이터 초기화 ---
        // Inspector에서 설정한 값을 Dictionary로 옮겨서 사용하기 쉽게 만듭니다.
        _bgmClipDict = new Dictionary<BgmType, AudioClip>();
        foreach (BgmSound bgm in bgmClips)
        {
            _bgmClipDict.Add(bgm.type, bgm.clip);
        }


        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        
        for (int index=0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; // 리스너 이펙트 무시．
            sfxPlayers[index].volume = sfxVolume;
        }

        // --- [새로 추가된 부분 2] 쿨다운 데이터 초기화 ---
        // Inspector에서 설정한 값을 Dictionary로 옮겨서 사용하기 쉽게 만듭니다.
        _sfxCooldowns = new Dictionary<SfxType, float>();
        _sfxLastPlayTimes = new Dictionary<SfxType, float>();
        foreach (SfxCooldown setting in sfxCooldownSettings)
        {
            _sfxCooldowns[setting.type] = setting.cooldown;
        }
        // --- [새로 추가된 부분 2 끝] ---
    }

    // --- [수정] BGM 재생 함수 ---
    public void PlayBgm(BgmType bgmType)
    {
        // 요청한 BGM 타입이 Dictionary에 있는지 확인합니다.
        if (!_bgmClipDict.ContainsKey(bgmType))
        {
            Debug.LogError($"BgmType '{bgmType}' not found in dictionary.");
            return;
        }

        AudioClip clipToPlay = _bgmClipDict[bgmType];
        _currentBgmType = bgmType;
        
        bgmPlayer.clip = clipToPlay;
        bgmPlayer.Play();
    }
    
    public void ContinueBgm(BgmType bgmType)
    {
        // 요청한 BGM 타입이 Dictionary에 있는지 확인합니다.
        if (!_bgmClipDict.ContainsKey(bgmType))
        {
            Debug.LogError($"BgmType '{bgmType}' not found in dictionary.");
            return;
        }

        AudioClip clipToPlay = _bgmClipDict[bgmType];
        
        // 재생 중인 BGM 있다면 그대로 사용
        if (bgmType == _currentBgmType) return;
        
        //  재생 중인 BGM 없다면 새로 재생
        bgmPlayer.clip = clipToPlay;
        bgmPlayer.Play();
    }

    // --- [추가] BGM 정지 함수 ---
    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(SfxType sfx)
    {
        // --- [수정된 부분] 쿨다운 체크 로직 ---
        // 이 효과음에 쿨타임이 설정되어 있는지 확인합니다.
        if (_sfxCooldowns.ContainsKey(sfx))
        {
            float cooldown = _sfxCooldowns[sfx];
            float lastPlayTime = 0f;

            // 이 효과음이 재생된 적이 있는지 확인하고, 있다면 마지막 재생 시간을 가져옵니다.
            _sfxLastPlayTimes.TryGetValue(sfx, out lastPlayTime);

            // 쿨타임이 아직 지나지 않았다면, 소리를 재생하지 않고 함수를 바로 종료합니다.
            if (Time.time < lastPlayTime + cooldown)
            {
                return;
            }

            // 쿨타임이 지났으므로, 마지막 재생 시간을 현재 시간으로 기록합니다.
            _sfxLastPlayTimes[sfx] = Time.time;
        }
        // --- [수정된 부분 끝] ---

        // 아래는 기존의 효과음 재생 로직입니다. 쿨타임 체크를 통과해야만 실행됩니다.
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            // 효과음 2개 이상있는 것들 랜덤 재생 여기에 구현

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    
    public void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp(value, 0f, DefaultBgmVolume * 2);
        if (bgmPlayer != null)
            bgmPlayer.volume = bgmVolume;
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp(value, 0f, DefaultSfxVolume * 2);
        if (sfxPlayers != null)
        {
            foreach (var player in sfxPlayers)
            {
                if (player != null)
                    player.volume = sfxVolume;
            }
        }
    }

    #region Fade Audio

    private Coroutine _bgmFadeCoroutine;
    
    public void FadeOutAudio(float duration)
    {
        if (_bgmFadeCoroutine != null) StopCoroutine(_bgmFadeCoroutine);
        _bgmFadeCoroutine = StartCoroutine(FadeAudioCo(duration, 0, 0));
    }

    public void FadeInAudio(float duration)
    {
        if (_bgmFadeCoroutine != null) StopCoroutine(_bgmFadeCoroutine);
        _bgmFadeCoroutine = StartCoroutine(FadeAudioCo(duration, bgmVolume, sfxVolume));
    }

    private IEnumerator FadeAudioCo(float duration, float targetBgm, float targetSfx)
    {
        float startBgmVol = bgmPlayer.volume;
        float targetBgmVol = targetBgm;

        float startSfxVol = sfxPlayers[0].volume;
        float targetSfxVol = targetSfx;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            bgmPlayer.volume = Mathf.Lerp(startBgmVol, targetBgmVol, t);
            foreach (var sfxPlayer in sfxPlayers)
            {
                sfxPlayer.volume = Mathf.Lerp(startSfxVol, targetSfxVol, t);
            }

            yield return null;
        }
        
        bgmPlayer.volume = targetBgmVol;
        foreach (var sfxPlayer in sfxPlayers)
        {
            sfxPlayer.volume = targetSfxVol;
        }
    }
    #endregion
}
