using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillData : Data {

    // 기본 정보
    public string Name { get; set; }
    public string Description { get; set; }
    public Stage Stage { get; set; }

    // HitCollider 생성 정보.
    public string HitColliderKey => Key;
    public float RadiusOffset { get; set; }
    public float RotationAngle { get; set; }
    public int HitColliderCount { get; set; }
    public float HitColliderAngle { get; set; }
    public float HitColliderSize { get; set; }
    public float AttackTime { get; set; }

    // 투사체 정보
    public int Penetration { get; set; }
    public float Speed { get; set; }
    public float DirectionX { get; set; }
    public float DirectionY { get; set; }
    public float Duration { get; set; }
    public float Range { get; set; }
    public int ExtraShotCount { get; set; }
    public int BerserkAttackCount { get; set; }
    public int BerserkExtraShotCount { get; set; }
    // 능력치 정보
    public float Damage { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalBonus { get; set; }
    public float AttackSpeed { get; set; }

}

public enum Stage {
    Stage1,
    Stage2,
    Stage3,
}

public enum Phase
{
    Phase1,
    Phase2,
    Phase3,
}