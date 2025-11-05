using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount]; // 배경음악/효과음 오디오 재생 컴포넌트 

    public AudioClip BgmClip; // 배경음악 클립
    public AudioClip SfxClip; // 효과음 클립 
    public AudioClip AmbienceClip; // 환경음 클립 
    
    public Dictionary<BGM, AudioClip> BgmContainer = new();
    public Dictionary<SFX, AudioClip> SfxContainer = new();
    public Dictionary<Ambience, AudioClip> AmbienceContainer = new();

    private AudioClip[] _clips;

    private Dictionary<string, GameObject> _activeAmbience = new();
    [SerializeField] private AudioMixer _mixer;

    protected override void Awake()
    {
        base.Awake();

        // 초기화 (오디오 소스, 볼륨, 루프 유무)
        _audioSources = GetComponents<AudioSource>();

        // Bgm / Sfx 컨테이너 초기화 
        Set_BgmContainer();
        Set_SfxContainer();
        Set_AmbienceContainer();

        Set_Bgm(BGM.Title);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        LoadVolume();
    }

    // 이전 볼륨 로딩 메서드 (씬이 전환되면 호출)
    public void LoadVolume()
    {
        float BgmVolume = SaveManager.Instance.SystemData.BgmVolume; 
        float SfxVolume = SaveManager.Instance.SystemData.SfxVolume; 

        _mixer.SetFloat(AudioVolume.BGM, Mathf.Log10(BgmVolume) * 20);
        _mixer.SetFloat(AudioVolume.SFX, Mathf.Log10(SfxVolume) * 20);
    }

    private void Set_BgmContainer()
    {
        // 배경음악 가져오기 
        _clips = ResourceManager.Instance.Create_BGM<AudioClip>();

        foreach (AudioClip clip in _clips)
        {
            // Music 열거형과 일치하는 이름이면 
            if (Enum.TryParse(clip.name, out BGM musicName))
            {
                // 컨테이너에 클립 넣기 
                BgmContainer[musicName] = clip;
            }
        }

        _clips = null;
    }

    private void Set_SfxContainer()
    {
        // 효과음 가져오기
        _clips = ResourceManager.Instance.Create_SFX<AudioClip>();

        foreach (AudioClip clip in _clips)
        {
            if (Enum.TryParse(clip.name, out SFX musicName))
            {
                SfxContainer[musicName] = clip;
            }
        }

        _clips = null;
    }

    private void Set_AmbienceContainer()
    {
        // 효과음 가져오기
        _clips = ResourceManager.Instance.Create_AMBIENCE<AudioClip>();

        foreach (AudioClip clip in _clips)
        {
            if (Enum.TryParse(clip.name, out Ambience musicName))
            {
                AmbienceContainer[musicName] = clip;
            }
        }

        _clips = null;
    }

    public void Set_Bgm(BGM bgm)
    {
        BgmClip = BgmContainer[bgm];

        Play_Bgm();
    }

    // 이전 배경음악 정지하고 새 배경음악 재생
    public void Play_Bgm()
    {
        if (_audioSources[(int)Sound.Bgm].clip == BgmClip) return;

        _audioSources[(int)Sound.Bgm].Stop();
        _audioSources[(int)Sound.Bgm].clip = BgmClip;
        _audioSources[(int)Sound.Bgm].Play();
    }

    // 효과음 재생 
    public void Play_Sfx(SFX sfx)
    {
        Stop_Loop_Sfx();
        _audioSources[(int)Sound.Sfx].clip = SfxContainer[sfx];
        _audioSources[(int)Sound.Sfx].PlayOneShot(SfxContainer[sfx]);
    }

    public void Play_Loop_Sfx(SFX sfx)
    {
        _audioSources[(int)Sound.SfxLoop].clip = SfxContainer[sfx];
        _audioSources[(int)Sound.SfxLoop].loop = true;
        _audioSources[(int)Sound.SfxLoop].Play();
    }

    public void Stop_Loop_Sfx()
    {
        _audioSources[(int)Sound.SfxLoop].loop = false;
        _audioSources[(int)Sound.SfxLoop].Stop();
    }

    public void Trace_Ambience(string tag, GameObject ambienceObject, Ambience sfx)
    {
        AudioSource audioSource = ambienceObject.GetComponent<AudioSource>();

        audioSource.clip = Play_Loop_Ambience(sfx);
        audioSource.loop = true;
        audioSource.Play();

        _activeAmbience[tag] = ambienceObject;
    }

    public AudioClip Play_Loop_Ambience(Ambience sfx)
    {
        return AmbienceContainer[sfx];
    }

    public GameObject Remove_Ambience(string tag)
    {
        if (_activeAmbience.TryGetValue(tag, out GameObject activeObject))
        {
            _activeAmbience.Remove(tag);

            if (activeObject.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.Stop();
            }
            return activeObject;
        }
        return null;
    }

    public void All_Stop_Ambience()
    {
        _activeAmbience.Clear();
    }

    public void SilenceMixer(float volume)
    {
        _mixer.SetFloat(AudioVolume.BGM, volume);
        _mixer.SetFloat(AudioVolume.SFX, volume);
    }
}
