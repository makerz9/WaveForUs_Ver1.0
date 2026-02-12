using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Volume Text")]
    [SerializeField] private TMP_Text bgmVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);
        //1 뭐지? 지금슬라이더 값을 PlayerPref에 보내주는건가?

        // 초기 텍스트 표시
        UpdateBGMText(bgmSlider.value);
        UpdateSFXText(sfxSlider.value);
        //2 이건 뭐지??

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        //3 이건 뭐지??
    }

    public void OnBGMVolumeChanged(float volume)
    {
        SoundManager.instance.SetBGMVolume(volume); // 지금 값을 volume에 주는건가??
        UpdateBGMText(volume);
        //4 이건 뭐지??
    }

    public void OnSFXVolumeChanged(float volume)
    {
        SoundManager.instance.SetSFXVolume(volume);  // 지금 값을 volume에 주는건가??
        UpdateSFXText(volume);
        //5 이건 뭐지??
    }

    void UpdateBGMText(float volume)
    {
        if (bgmVolumeText != null)
            bgmVolumeText.text = $"{Mathf.RoundToInt(volume * 100)}%";
    }

    void UpdateSFXText(float volume)
    {
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"{Mathf.RoundToInt(volume * 100)}%";
    }
}