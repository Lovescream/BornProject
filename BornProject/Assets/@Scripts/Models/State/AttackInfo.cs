using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격에 관한 정보를 담고 있습니다.
/// 공격의 주체인 IAttackable이 AttackInfo를 생성합니다.
/// 해당 정보를 바탕으로 IHit을 생성하여 공격합니다.
/// </summary>
public struct AttackInfo {

    // 기본 정보
    public IAttackable Owner { get; set; }
    public string HitColliderKey { get; set; }

    // 공격 정보
    public float Damage { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalBonus { get; set; }
    public float Penetrate { get; set; }

    // 운동 정보
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }

    // 시간 정보
    public float Duration { get; set; }

    // 부가 효과 정보
    public KnockbackInfo Knockback { get; set; }

    // 속성
    public readonly Vector2 Velocity => Direction * Speed;

}