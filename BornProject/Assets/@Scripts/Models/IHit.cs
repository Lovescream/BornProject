using UnityEngine;

/// <summary>
/// 공격의 매개체임을 의미합니다.
/// Projectile, Bullet, MeleeAttackEffect ...
/// </summary>
public interface IHit {

    public AttackInfo AttackInfo { get; }
    public Vector3 CurrentPosition { get; }

}