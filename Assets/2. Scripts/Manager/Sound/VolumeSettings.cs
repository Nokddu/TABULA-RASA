using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 볼륨조절 스크립트 
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    // === 임시 ===
    [SerializeField] private Button _applyBtn;
    [SerializeField] private Button _saveBtn;
    [SerializeField] private Button _closeBtn;

    private SaveManager _saveManager;
    private float _bgm;
    private float _sfx;

    private void Awake()
    {
        _bgmSlider.onValueChanged.AddListener(Set_BgmVolume);
        _sfxSlider.onValueChanged.AddListener(Set_SfxVolume);

        _applyBtn.onClick.AddListener(Set_Volume);
        _saveBtn.onClick.AddListener(Save_Volume);
        _closeBtn.onClick.AddListener(Return_Volume);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        _saveManager = SaveManager.Instance;

        _bgmSlider.value = _saveManager.SystemData.BgmVolume;
        _sfxSlider.value = _saveManager.SystemData.SfxVolume;
    }

    private void Set_Volume()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        _bgm = _bgmSlider.value;
        _sfx = _sfxSlider.value;

        _saveManager.SystemData.BgmVolume = _bgmSlider.value;
        _saveManager.SystemData.SfxVolume = _sfxSlider.value;

        Set_BgmVolume(_bgm); 
        Set_SfxVolume(_sfx); 
    }

    private void Save_Volume()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        _saveManager.SystemData.BgmVolume = _bgmSlider.value;
        _saveManager.SystemData.SfxVolume = _sfxSlider.value;

        _bgm = _bgmSlider.value;
        _sfx = _sfxSlider.value;

        _saveManager.Save_System(_saveManager.SystemData);
    }

    private void Return_Volume()
    {
        _bgmSlider.value = _bgm;
        _sfxSlider.value = _sfx;

        Set_BgmVolume(_bgm);
        Set_SfxVolume(_sfx);
    }

    private void Set_BgmVolume(float value)
    {
        _mixer.SetFloat(AudioVolume.BGM, Mathf.Log10(value) * 20);
    }

    private void Set_SfxVolume(float value)
    {
        _mixer.SetFloat(AudioVolume.SFX, Mathf.Log10(value) * 20);
    }
}
