using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider_Acid : HitCollider {

    //#region Initialize / Set

    //public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
    //    base.SetInfo(key, info, hitInfo);

    //    this.RemainPenetration = 0;
    //    _animator.SetTrigger(AnimatorParameterHash_Initialize);

    //    _activated = true;
    //}

    //#endregion

    //protected override void Update() {
    //    base.Update();
    //}

    //#region Callbacks

    //protected override void OnHit(bool destroy = false) {
    //    if (!_activated) return;
    //    _collider.enabled = false;
    //    _rigidbody.simulated = false;

    //    _animator.SetTrigger(AnimatorParameterHash_Hit);
    //    _activated = false;
    //}

    //protected override void OnEndDuration() {
    //    if (!_activated) return;
    //    _animator.SetTrigger(AnimatorParameterHash_Disappear);
    //    _activated = false;
    //}

    //protected override void OnEndRange() {
    //    if (!_activated) return;
    //    _animator.SetTrigger(AnimatorParameterHash_Disappear);
    //    _activated = false;
    //}

    //protected override void OnExitAnimation() {
    //    base.OnExitAnimation();
    //}

    //#endregion
}