using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SFX
{
    OnClickButton,
    NewGameEnterButton,
    PlayerHit,
    Range_Laser_Fire,
    Range_Rapid_Fire,
    Range_ShotGun_Fire,
    Melee_Slash_Fire,
    Melee_Smash_Fire,
    Melee_Sting_Fire,
    EnemyBearDie,
    EnemyWolfDie,
    EnemyBeatleDie,
    EnemySnakeDie,
    

}
public class AudioController : MonoBehaviour
{

    #region Singleton

    public static AudioController Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<AudioController>();
            return instance;
        }
        set => instance = value;
    }
    private static AudioController instance;

    #endregion

    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private int sfxCursor;

    void Awake()
    {
        DontDestroyOnLoad(this.transform.gameObject);

        // Scene Initialize.
        SceneManager.sceneLoaded += InitializeOnSceneLoaded;
    }

    public void InitializeOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            bgmPlayer.clip = bgmClips[0];
            bgmPlayer.Play();
        }
        else if (scene.name == "MainScene")
        {
            bgmPlayer.clip = bgmClips[1];
            bgmPlayer.Play();
        }
        else if (scene.name == "GameScene")
        {
            bgmPlayer.clip = bgmClips[1];
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }


    public void SFXPlay(SFX type)
    {
        switch (type)
        {
            case SFX.OnClickButton:
                sfxPlayer[sfxCursor].clip = sfxClips[0];
                break;
            case SFX.NewGameEnterButton:
                sfxPlayer[sfxCursor].clip = sfxClips[1];
                break;
            case SFX.PlayerHit:
                sfxPlayer[sfxCursor].clip = sfxClips[2];
                break;
            case SFX.Range_Laser_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[3];
                break;
            case SFX.Range_Rapid_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[4];
                break;
            case SFX.Range_ShotGun_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[5];
                break;
            case SFX.Melee_Slash_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[6];
                break;
            case SFX.Melee_Smash_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[7];
                break;
            case SFX.Melee_Sting_Fire:
                sfxPlayer[sfxCursor].clip = sfxClips[8];
                break;            
            case SFX.EnemyBearDie:
                sfxPlayer[sfxCursor].clip = sfxClips[9];
                break;
            case SFX.EnemyWolfDie:
                sfxPlayer[sfxCursor].clip = sfxClips[10];
                break;
            case SFX.EnemyBeatleDie:
                sfxPlayer[sfxCursor].clip = sfxClips[11];
                break;
            case SFX.EnemySnakeDie:
                sfxPlayer[sfxCursor].clip = sfxClips[12];
                break;

        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor++;
        if (sfxCursor == sfxPlayer.Length) sfxCursor = 0;
    }
}
