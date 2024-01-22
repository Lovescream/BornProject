using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격에 관한 정보를 담고 있습니다.
/// 공격의 주체인 IAttackable이 AttackInfo를 생성합니다.
/// 해당 정보를 바탕으로 IHit을 생성하여 공격합니다.
/// </summary>
public struct AttackInfo {

    public IAttackable Owner { get; set; }
    public float Damage { get; set; }
    public float Duration { get; set; }
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }
    public KnockbackInfo Knockback { get; set; }

    public readonly Vector2 Velocity => Direction * Speed;

}