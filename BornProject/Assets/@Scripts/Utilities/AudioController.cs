using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SFX
{
    SFX_OnButton,
    SFX_OnButon_NewGame,
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


    //public void SFXPlay(SFX type)
    //{
    //    switch (type)
    //    {
    //        case SFX.SFX_OnButton:
    //            sfxPlayer[sfxCursor].clip = sfxClips[0];
    //            break;
    //        case SFX.SFX_OnButon_NewGame:
    //            sfxPlayer[sfxCursor].clip = sfxClips[1];
    //            break;
    //        case SFX.PlayerHit:
    //            sfxPlayer[sfxCursor].clip = sfxClips[2];
    //            break;
    //        case SFX.Range_Laser_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[3];
    //            break;
    //        case SFX.Range_Rapid_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[4];
    //            break;
    //        case SFX.Range_ShotGun_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[5];
    //            break;
    //        case SFX.Melee_Slash_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[6];
    //            break;
    //        case SFX.Melee_Smash_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[7];
    //            break;
    //        case SFX.Melee_Sting_Fire:
    //            sfxPlayer[sfxCursor].clip = sfxClips[8];
    //            break;            
    //        case SFX.EnemyBearDie:
    //            sfxPlayer[sfxCursor].clip = sfxClips[9];
    //            break;
    //        case SFX.EnemyWolfDie:
    //            sfxPlayer[sfxCursor].clip = sfxClips[10];
    //            break;
    //        case SFX.EnemyBeatleDie:
    //            sfxPlayer[sfxCursor].clip = sfxClips[11];
    //            break;
    //        case SFX.EnemySnakeDie:
    //            sfxPlayer[sfxCursor].clip = sfxClips[12];
    //            break;

    //    }

    //    sfxPlayer[sfxCursor].Play();
    //    sfxCursor++;
    //    if (sfxCursor == sfxPlayer.Length) sfxCursor = 0;
    //}
}
