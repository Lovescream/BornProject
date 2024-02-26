using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider_Acid : HitCollider {

    #region Fields

    protected static readonly int AnimatorParameterHash_Initialize = Animator.StringToHash("Initialize");
    protected static readonly int AnimatorParameterHash_Hit = Animator.StringToHash("Hit");
    protected static readonly int AnimatorParameterHash_Disappear = Animator.StringToHash("Disappear");

    private bool _activated = false;

    #endregion

    #region Initialize / Set

    public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
        base.SetInfo(key, info, hitInfo);

        this.RemainPenetration = 0;
        _animator.SetTrigger(AnimatorParameterHash_Initialize);

        _activated = true;
    }

    #endregion

    #region Callbacks

    protected override void OnHit(bool destroy = false) {
        if (!_activated) return;
        _collider.enabled = false;
        _rigidbody.simulated = false;

        _animator.SetTrigger(AnimatorParameterHash_Hit);
        _activated = false;
    }

    protected override void OnEndDuration() {
        if (!_activated) return;
        _animator.SetTrigger(AnimatorParameterHash_Disappear);
        _activated = false;
    }

    protected override void OnEndRange() {
        if (!_activated) return;
        _animator.SetTrigger(AnimatorParameterHash_Disappear);
        _activated = false;
    }

    protected override void OnExitAnimation() {
        base.OnExitAnimation();
    }

    #endregion
}