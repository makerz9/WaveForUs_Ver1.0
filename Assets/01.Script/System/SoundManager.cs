using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable] //1. 이게 뭐하는 코드지?

public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance; // 싱글톤 선언

    #region singleton

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        LoadVolume();

    }

    #endregion

    public AudioSource[] audioSourceEffects;
    public AudioSource bgmSource;
    public AudioClip mainBGMClip;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;



    // Start is called before the first frame update
    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length]; // 왜 이렇게 해준걸까?
    }

    public void PlaySE(string _name, float pitch = 1.0f, float volume = 1.0f)
    {
        // 효과음 찾기
        Sound sound = null;
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].pitch = pitch;
                        //audioSourceEffects[j].volume = volume; //  볼륨 설정
                        audioSourceEffects[j].volume = volume * sfxVolume;
                        audioSourceEffects[j].Play();
                        Debug.Log($"효과음 재생: {_name} (Pitch: {pitch}, Volume: {volume})");
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용 중입니다.");
                return;
            }
        }
        Debug.LogWarning($"{_name} 사운드가 등록되지 않았습니다.");
    }


    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(audioSourceEffects[i].isPlaying && playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                playSoundName[i] = null;
                Debug.Log($" ?????? ????: {_name}");
                return;
            }
        }
        Debug.Log($" ???? ???? ???? ?????? {_name}?? ???? ?? ????????.");

    }





    #region BGM ????
    //  BGM ???? (???? ???? ?????? ???????? ????)
    public void PlayBGM(string bgmName, float pitch = 1.0f, float volume = 0.05f)
    {
        // 이미 재생 중이면 볼륨만 업데이트
        //if (bgmSource.isPlaying && bgmSource.clip != null && bgmSource.clip.name == bgmName)
        if (bgmSource.isPlaying) //재시작해도 브금 유지되는버전
        {
            bgmSource.volume = volume * bgmVolume;
            Debug.Log($"BGM 이미 재생 중 (볼륨: {volume * bgmVolume})");
            return;
        }

        // 새로운 BGM 재생
        bgmSource.clip = mainBGMClip;
        bgmSource.loop = true;
        bgmSource.pitch = pitch;
        bgmSource.volume = volume * bgmVolume;
        bgmSource.Play();
        Debug.Log($"BGM 재생: {bgmName} (Pitch: {pitch}, Volume: {volume * bgmVolume})");
    }

    //  BGM ???? (???? ?????? ???? ?? ????)
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            Debug.Log(" BGM ????");
        }
    }

    //  ???? BGM?? ???? ?????? ???????? ???? ????
    public bool IsBGMPlaying()
    {
        return bgmSource.isPlaying;
    }
    #endregion


    #region ?????? ????/??????
    public void EnableEffects()
    {
        foreach (var effectSource in audioSourceEffects)
        {
            effectSource.enabled = true;
        }
        Debug.Log(" ?????? ??????");
    }

    //  ?????? ????????
    public void DisableEffects()
    {
        foreach (var effectSource in audioSourceEffects)
        {
            if (effectSource.isPlaying)
            {
                effectSource.Stop(); // ???? ???? ???????? ????
            }
            effectSource.enabled = false; // AudioSource ????????
        }
        Debug.Log(" ?????? ????????");
    }
    #endregion


    #region SoundVolume

    void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);

        bgmSource.volume = 0.05f * bgmVolume; // ← 기본 볼륨 0.05 적용

        Debug.Log($"불러온 볼륨 - BGM: {bgmVolume}, SFX: {sfxVolume}");
    }

    //BGM 볼륨조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = 0.08f * volume; // ← 수정!

        PlayerPrefs.SetFloat("BGM_Volume", volume);
        PlayerPrefs.Save();

        Debug.Log($"BGM 볼륨: {volume}");
    }

    // 효과음 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        float oldVolume = sfxVolume;
        sfxVolume = volume;

        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(audioSourceEffects[i].isPlaying)
            {
                if(oldVolume > 0)
                {
                    float ratio = volume / oldVolume;
                    audioSourceEffects[i].volume *= ratio; // ← *= 사용!
                }
                else
                {
                    audioSourceEffects[i].volume = volume;
                }

            }
        }


        PlayerPrefs.SetFloat("SFX_Volume", volume);
        PlayerPrefs.Save();

        Debug.Log($"SFX 볼륨: {volume}");
    }


    #endregion



}
