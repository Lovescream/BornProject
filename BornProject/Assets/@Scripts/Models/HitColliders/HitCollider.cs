using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : Entity, IHitCollider {

    #region Properties

    public AttackInfo AttackInfo { get; protected set; }
    public Vector2 Velocity { get; set; }
    public float RemainPenetration { get; protected set; }

    public IAttackable Owner => AttackInfo.Owner;
    public Vector3 CurrentPosition => this.transform.position;
    public float Damage => AttackInfo.Damage;
    public float Penetrate => AttackInfo.Penetrate;
    public float Duration => AttackInfo.Duration;
    public KnockbackInfo KnockbackInfo => AttackInfo.Knockback;

    #endregion

    #region Fields

    // Components.
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;

    // Coroutines.
    private Coroutine _coDestroy;

    #endregion

    #region MonoBehaviours

    protected virtual void OnDisable() {
        StopAllCoroutines();
        _coDestroy = null;
    }

    protected virtual void FixedUpdate() {
        _rigidbody.velocity = Velocity;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        Creature creature = collision.GetComponent<Creature>();
        if (!creature.IsValid() || !this.IsValid()) return;
        if (creature is IAttackable attackable && attackable == this.Owner) return;
        if (creature.State.Current == CreatureState.Dead) return;

        creature.OnHit(this);

        HandlePenetrate();
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _collider = this.GetComponent<Collider2D>();
        _rigidbody = this.GetComponent<Rigidbody2D>();

        return true;
    }

    public virtual void SetInfo(AttackInfo attackInfo) {
        this.AttackInfo = attackInfo;
        this.Velocity = attackInfo.Velocity;
        this.RemainPenetration = Penetrate;

        if (_coDestroy != null) StopCoroutine(_coDestroy);
        _coDestroy = StartCoroutine(CoDestroy());
    }

    #endregion

    private void HandlePenetrate() {
        if (RemainPenetration < 0) return;

        if (--RemainPenetration == 0) Destroy();
    }

    private IEnumerator CoDestroy() {
        yield return new WaitForSeconds(Duration);
        _coDestroy = null;
        Destroy();
    }

    private void Destroy() {
        _rigidbody.velocity = Vector2.zero;
        Velocity = Vector2.zero;
        if (this.IsValid()) Main.Object.DespawnHitCollider(this);
    }

}