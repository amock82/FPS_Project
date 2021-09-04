using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float sfxVolum = 1;
    public float gunVolum = 1;
    public float bgmVolum = 1;

    public bool isSfxMute = false;

    public AudioClip mainBgm;
    public AudioClip fireSnd;
    public AudioClip step_A;
    public AudioClip step_B;
    public AudioClip step_C;
    public AudioClip zombieAkt;
    public AudioClip bloodStabHit;
    public AudioClip handAtk;
    public AudioClip bloodDeath;
    public AudioClip itemUse;
    public AudioClip levelUp;

    AudioSource bgmSource;

    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }


    public void PlaySfx(Vector3 pos, AudioClip sfx, float delayed, float volm)
    {
        if (isSfxMute) return;

        StartCoroutine(PlaySfxDelayed(pos, sfx, delayed, volm));
    }

    IEnumerator PlaySfxDelayed(Vector3 pos, AudioClip sfx, float delayed, float volm)
    {
        yield return new WaitForSeconds(delayed);

        GameObject sfxObj = new GameObject("sfx");

        AudioSource _aud = sfxObj.AddComponent<AudioSource>();

        _aud.transform.position = pos;
        _aud.clip = sfx;
        _aud.minDistance = 5.0f;
        _aud.maxDistance = 10.0f;
        _aud.volume = volm;
        _aud.Play();

        Destroy(sfxObj, sfx.length);
    }

    // 배경음 재생 함수
    public void PlayBgm(AudioClip bgm, float delayed, bool loop)
    {
        StartCoroutine(PlayBGM_Delayed(bgm, delayed, loop));
    }

    IEnumerator PlayBGM_Delayed(AudioClip bgm, float delayed, bool loop)
    {
        yield return new WaitForSeconds(delayed);

        GameObject bgmObj = new GameObject("BGM");

        if (!bgmSource) bgmSource = bgmObj.AddComponent<AudioSource>();

        bgmSource.clip = bgm;
        bgmSource.volume = bgmVolum;
        bgmSource.loop = loop;
        bgmSource.Play();


    }
}
