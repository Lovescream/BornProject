using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : Entity, IHitCollider {

    public HitColliderInfo Info { get; protected set; }
    public HitInfo HitInfo { get; protected set; }

    #region Properties
    public Vector2 Velocity { get; set; }
    public int RemainPenetration { get; protected set; }

    public IAttackable Owner => HitInfo.Owner;
    public Vector3 CurrentPosition => this.transform.position;
    public float Duration => Info.Duration;
    public KnockbackInfo KnockbackInfo => HitInfo.Knockback;

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

    public virtual void SetInfo(HitColliderInfo info, HitInfo hitInfo) {
        this.Info = info;
        this.HitInfo = hitInfo;
        this.Velocity = info.Velocity;
        this.RemainPenetration = info.Penetration;

        if (Duration > 0) {
            if (_coDestroy != null) StopCoroutine(_coDestroy);
            _coDestroy = StartCoroutine(CoDestroy());
        }
    }

    #endregion

    #region Callbacks

    protected virtual void OnExitAnimation() {
        Debug.Log("A");
        Destroy();
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