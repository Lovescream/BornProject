using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Basic {

    #region Properties

    // 기본 정보
    public string Name { get; set; }
    public string Description { get; set; }

    // 공격 정보
    public float Damage { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalBonus { get; set; }
    public int Penetration { get; set; }
    public float AttackSpeed { get; set; }
    public int HitColliderCount { get; set; }

    // 운동 정보
    public float HitColliderAngle { get; set; }
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }

    // 시간 정보
    public float Duration { get; set; }

    // 거리 정보
    public float Range { get; set; }

    // 크기 정보
    public float HitColliderSize { get; set; }

    #endregion

}