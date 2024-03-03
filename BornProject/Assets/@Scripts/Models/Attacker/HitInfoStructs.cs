using UnityEngine;

public struct HitColliderGenerationInfo {
    public IAttackable Owner { get; set; }
    public string SkillKey { get; set; }
    public string HitColliderKey { get; set; }
    public float RadiusOffset { get; set; }
    public float RotationAngle { get; set; }
    public int Count { get; set; }
    public float SpreadAngle { get; set; }
    public float Size { get; set; }
    public float AttackTime { get; set; }
}
public struct HitColliderInfo {
    public int Penetration { get; set; }
    public float Speed { get; set; }
    public float DirectionX { get; set; }
    public float DirectionY { get; set; }
    public float Duration { get; set; }
    public float Range { get; set; }
    public readonly Vector2 Velocity => new Vector2(DirectionX, DirectionY) * Speed;
}
public struct HitInfo {
    public IAttackable Owner { get; set; }
    public float Damage { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalBonus { get; set; }
    public KnockbackInfo Knockback { get; set; }
}