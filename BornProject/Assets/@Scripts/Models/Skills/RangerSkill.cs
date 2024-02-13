using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerSkill {

    // 스킬은 보이든 안보이든 존재하며, 데이터를 갖는다.
    #region Properties
    public RangerSkillData Data {  get; private set; }

    public string Key => Data.Key;
    //public string Name => Data.Name; // 필요한가?.
    public RangerSkillType Type => Data.Type;
    public string Description => Data.Description;
    
    public float RadiusOffset => Data.RadiusOffset;
    public float RotationAngle => Data.RotationAngle;
    public int HitColliderCount => Data.HitColliderCount;
    public float HitColliderAngle => Data.HitColliderAngle;
    public float HitColliderSize => Data.HitColliderSize;
    public int Penetration => Data.Penetration;
    public float Speed => Data.Speed;
    public float DirectionX => Data.DirectionX;
    public float DirectionY => Data.DirectionY;
    public float Duration => Data.Duration;
    public float Range => Data.Range;
    public int MultipleAttack => Data.MultipleAttack;
    public float Damage => Data.Damage;
    public float CriticalChance => Data.CriticalChance;
    public float CriticalBonus => Data.CriticalBonus;
    public float AttackSpeed => Data.AttackSpeed;

    #endregion                             

}
