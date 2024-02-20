using UnityEngine;
/// <summary>
/// 공격할 수 있는 오브젝트임을 의미합니다. (공격의 주체)
/// Ex) Player, Enemy, Trap, Tower ...
/// </summary>
public interface IAttackable {

    public Transform Indicator { get; }
    public Vector2 LookDirection { get; }

    public void Attack();
    public HitColliderGenerationInfo GetHitColliderGenerationInfo();
    public HitColliderInfo GetHitColliderInfo();
    public HitInfo GetHitInfo();

}

