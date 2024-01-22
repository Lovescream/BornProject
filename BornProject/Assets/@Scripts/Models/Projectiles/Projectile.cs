using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity, IHit {

    #region Properties

    public IAttackable Owner => AttackInfo.Owner;
    public AttackInfo AttackInfo { get; protected set; }

    public Vector3 CurrentPosition => this.transform.position;
    public Vector2 Velocity { get; set; } // 발사체 속도.
    

    public float Duration => AttackInfo.Duration;
    public float Damage => AttackInfo.Damage;
    public KnockbackInfo KnockbackInfo => AttackInfo.Knockback;

    #endregion

    #region Fields

    // Components.
    protected Rigidbody2D _rigidbody; // 충돌처리를 위한 Rigidbody.

    // Coroutines.
    private Coroutine _coDestroy; // 코루틴이 null인지 활성화 되어있는지 확인하기 위한 필드.

    #endregion

    // Rigidbody가 붙은 발사체에 대한 속도.
    protected virtual void FixedUpdate() {
        _rigidbody.velocity = Velocity;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        Creature creature = collision.GetComponent<Creature>();
        if (!creature.IsValid() || !this.IsValid()) return;
        if (creature is IAttackable a && a == this.Owner) return; // Creature가 발사체의 주인이라면.
        if (creature.State.Current == CreatureState.Dead) return; // Creature의 현재 상태가 죽었다면.

        creature.OnHit(this);
        //creature.OnHit(Owner, Damage, KnockbackInfo);
        // creature.OnHit(Owner, Damage, new() { time = 0.1f, speed = 10f, direction = (creature.transform.position - this.transform.position).normalized }); // Creature 공격.

        _rigidbody.velocity = Velocity = Vector2.zero;
        if (this.IsValid()) Main.Object.DespawnProjectile(this);
    }

    protected void OnDisable() {
        StopAllCoroutines(); // 모든 코루틴 중단.
        _coDestroy = null;
    }

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rigidbody = this.GetComponent<Rigidbody2D>();

        return true;
    }

    public virtual Projectile SetInfo(AttackInfo attackInfo) // Creature에 대한 발사체 정보.
    {
        this.AttackInfo = attackInfo;
        Velocity = attackInfo.Velocity;

        if (_coDestroy != null) StopCoroutine(this._coDestroy); // 코루틴 중복 방지.
        _coDestroy = StartCoroutine(CoDestroy());

        return this;
    }

    private IEnumerator CoDestroy() {
        yield return new WaitForSeconds(Duration); // 발사체 지속시간만큼 대기.
        _coDestroy = null;
        Main.Object.DespawnProjectile(this);
    }
}