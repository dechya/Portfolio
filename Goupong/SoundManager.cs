using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("���� ����")]
    [SerializeField] public Sound[] sfx = null;
    // [SerializeField] Sound[] ransfx = null;
    [SerializeField] AudioSource[] sfxPlayer = null;
    // [SerializeField] GameObject sfxPlayerParent = null;

    [Header("�������")]
    [SerializeField] public Sound bgm = null;
    // [SerializeField] public GameObject ranking;
    [SerializeField] AudioSource bgmPlayer = null;

    public int click = 0; 

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        SoundManager.Instance.PlayBGM("�������");
    }

    // private void FixedUpdate()
    // {
    //     if (ranking.activeSelf == true)
    //     {
    //         bgmPlayer.Stop();
    //     }
    // }

    public void PlayBGM(string bgmName)
    {
        if (bgmName == bgm.name)
        {
            bgmPlayer.clip = bgm.clip;
            bgmPlayer.Play();
        }
    }

    public void SoundCtrl()
    {
        SoundManager.Instance.PlaySFX("�޴�Ŭ��");
        click++;

        if (click == 1)
        {
            for(int i = 0; i < sfxPlayer.Length; i++)
            {
                sfxPlayer[i].volume = 0.0625f;
                bgmPlayer.volume = 0.0625f;
            }                
        }
        if (click == 2)
        {
            for (int i = 0; i < sfxPlayer.Length; i++)
            {
                sfxPlayer[i].volume = 0f;
                bgmPlayer.volume = 0f;
            }                
        }
        if (click > 2)
        {
            for (int i = 0; i < sfxPlayer.Length; i++)
            {
                sfxPlayer[i].volume = 0.125f;
                bgmPlayer.volume = 0.125f;
            }                
            click = 0;
        }
    }

    public void PlaySFX(string p_sfxName)
    {
        // Debug.Log("ȿ���� ���");
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i].name == p_sfxName/*|| ransfx[i].name == p_sfxName*/)
            {
                /*if(p_sfxName == "�����")
                {
                    int rand = Random.Range(0, ransfx.Length);
                    for (int x = 0; x < sfxPlayer.Length; x++)
                    {
                        if (!sfxPlayer[x].isPlaying)
                        {
                            sfxPlayer[x].clip = ransfx[rand].clip;
                            sfxPlayer[x].Play();
                            return;
                        }
                    }
                    Debug.Log("��� ����� �÷��̾ ������Դϴ�.");
                    return;
                }*/
                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        sfxPlayer[x].clip = sfx[i].clip;
                        sfxPlayer[x].Play();
                        return;
                    }
                }
                Debug.Log("��� ����� �÷��̾ ������Դϴ�.");
                return;
            }
        }
        Debug.Log(p_sfxName + "�̸��� ȿ������ �����ϴ�..");
    }
}
