using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitCollider : Entity, IHitCollider {

    public HitColliderInfo Info { get; protected set; }
    public HitInfo HitInfo { get; protected set; }

    #region Properties
    public Vector2 Velocity { get; set; }
    public int RemainPenetration { get; protected set; }

    public IAttackable Owner => HitInfo.Owner;
    public Vector3 CurrentPosition => this.transform.position;
    public float Range => Info.Range < 0 ? Mathf.Infinity : Info.Range;
    public float Speed => Info.Speed;
    public float Duration => Info.Duration;
    public KnockbackInfo KnockbackInfo => HitInfo.Knockback;

    public bool Activated {     // True: 물리적 상호작용 O,  False: 애니메이션만
        get => _activated;
        set {
            _activated = value;
            _collider.enabled = value;
            _rigidbody.simulated = value;
        }
    }

    #endregion

    #region Fields

    protected static readonly int AnimatorParameterHash_Immediately = Animator.StringToHash("Immediately");
    protected static readonly int AnimatorParameterHash_Initialize = Animator.StringToHash("Initialize");
    protected static readonly int AnimatorParameterHash_Hit = Animator.StringToHash("Hit");
    protected static readonly int AnimatorParameterHash_Disappear = Animator.StringToHash("Disappear");

    private float _deltaPosition;
    private Vector3 _prevPosition;

    private bool _activated;

    // Components.
    protected Rigidbody2D _rigidbody;

    // Coroutines.
    private Coroutine _coDestroy;

    #endregion

    #region MonoBehaviours

    protected virtual void OnDisable() {
        StopAllCoroutines();
        _coDestroy = null;
    }

    protected virtual void Update() {
        if (!Activated && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.empty)
            Destroy();
    }

    protected virtual void FixedUpdate() {
        if (Speed < 0) {
            this.transform.localPosition = Vector2.zero;
            return;
        }
        _deltaPosition += (CurrentPosition - _prevPosition).magnitude;
        if (_deltaPosition >= Range) {
            OnEndRange();
        }
        _rigidbody.velocity = Velocity;
        _prevPosition = CurrentPosition;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (this is not HitCollider_Laser && collision.gameObject.layer == Main.WallLayer) {
            OnHit(destroy: true);
            return;
        }

        Creature creature = collision.GetComponent<Creature>();
        if (!creature.IsValid() || !this.IsValid()) return;
        if (creature is IAttackable attackable && attackable == this.Owner) return;
        if (!this.Owner.IsTarget(creature)) return;
        if (creature.State.Current == CreatureState.Dead) return;

        creature.OnHit(this);
        OnHit(destroy: false);
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rigidbody = this.GetComponent<Rigidbody2D>();

        this.gameObject.layer = Main.HitColliderLayer;

        return true;
    }

    public virtual void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
        Initialize();

        // #1. 기본 정보 설정.
        this.Info = info;
        this.HitInfo = hitInfo;

        // #2. Animator 설정.
        _animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{key}");
        _animator.ResetTrigger(AnimatorParameterHash_Initialize);
        _animator.SetTrigger(AnimatorParameterHash_Initialize);
        _animator.SetBool(AnimatorParameterHash_Immediately, _animator.GetCurrentAnimatorClipInfo(0)[0].clip.empty);

        Activated = true;

        Vector2 direction = new Vector2(info.DirectionX, info.DirectionY).normalized;
        if (direction.magnitude <= float.Epsilon)
            direction = hitInfo.Owner.LookDirection;

        this.Velocity = direction * info.Speed;
        this.RemainPenetration = info.Penetration;

        _deltaPosition = 0;
        _prevPosition = this.CurrentPosition;

        if (Duration > 0) {
            if (_coDestroy != null) StopCoroutine(_coDestroy);
            _coDestroy = StartCoroutine(CoDestroy());
        }
    }

    #endregion

    public void SetDirection(Vector2 direction) {
        Velocity = direction * Info.Speed;
    }

    #region Callbacks

    protected virtual void OnHit(bool destroy = false) {
        if (destroy) { Destroy(); return; }

        if (RemainPenetration >= 0) {
            if (--RemainPenetration <= 0) {
                if (!Activated) return;
                Activated = false;
                _animator.SetTrigger(AnimatorParameterHash_Hit);
            }
        }
    }

    protected virtual void OnEndDuration() {
        if (!Activated) return;
        Activated = false;
        _animator.SetTrigger(AnimatorParameterHash_Disappear);
    }

    protected virtual void OnEndRange() {
        if (!Activated) return;
        Activated = false;
        _animator.SetTrigger(AnimatorParameterHash_Disappear);
    }

    protected virtual void OnExitAnimation() {
        Destroy();
    }

    #endregion

    public void SetTransform(float radius, float angle, float scale, bool worldRotate = false) {
        Transform prevParent = this.transform.parent;
        this.transform.SetParent(Owner.Indicator.Point);
        this.transform.localPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
        
        if (worldRotate) {
            this.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else {
            if (this.Owner.Indicator.OriginDirection)
                this.transform.localRotation = Quaternion.Euler(0, 0, angle);
            else {
                this.transform.localRotation = Quaternion.Euler(0, 0, angle + 180);
            }
        }
        //this.transform.SetLocalPositionAndRotation(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius, Quaternion.Euler(0, 0, angle));
        _prevPosition = this.CurrentPosition;
        this.transform.SetParent(prevParent);
        this.transform.localScale = Vector3.one * scale;
    }

    private IEnumerator CoDestroy() {
        yield return new WaitForSeconds(Duration);
        _coDestroy = null;
        OnEndDuration();
    }

    private void Destroy() {
        Activated = false;
        _rigidbody.velocity = Vector2.zero;
        Velocity = Vector2.zero;
        if (this.IsValid()) Main.Object.DespawnHitCollider(this);
    }

}