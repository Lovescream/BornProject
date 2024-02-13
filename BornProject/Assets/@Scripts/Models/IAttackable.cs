/// <summary>
/// 공격할 수 있는 오브젝트임을 의미합니다. (공격의 주체)
/// Ex) Player, Enemy, Trap, Tower ...
/// </summary>
public interface IAttackable {

    public void Attack();
    public HitColliderGenerationInfo GetRangerHitColliderGenerationInfo();
    public HitColliderGenerationInfo GetMeleeHitColliderGenerationInfo();
    public HitColliderInfo GetRangerHitColliderInfo();
    public HitColliderInfo GetMeleeHitColliderInfo();
    public HitInfo GetRangerHitInfo();
    public HitInfo GetMeleeHitInfo();

}

