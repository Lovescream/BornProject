using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beatle : Enemy {

    #region Initialize / Set

    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);
    }
    protected override void SetStatus(bool isFullHp = true) {
        base.SetStatus(isFullHp);
    }
    protected override void SetState(CreatureState defaultState = CreatureState.Idle) {
        base.SetState();
        State.AddOnEntered(CreatureState.Chase, OnEnteredChase);
        State.AddOnEntered(CreatureState.Attack, OnEnteredAttack);
        State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
        State.AddOnExited(CreatureState.Chase, OnExitedChase);
        State.AddOnExited(CreatureState.Attack, OnExitedAttack);
    }

    #endregion

    #region State

    protected virtual void OnEnteredChase() {
        _animator.SetBool(AnimatorParameterHash_Fly, true);
    }
    protected override void OnEnteredDead() {
        _animator.SetBool(AnimatorParameterHash_Fly, false);
        base.OnEnteredDead();
    }
    protected virtual void OnExitedChase() {
        if (Target == null) _animator.SetBool(AnimatorParameterHash_Fly, false);
    }
    protected virtual void OnExitedAttack() {
        if (Target == null) _animator.SetBool(AnimatorParameterHash_Fly, false);
    }

    #endregion

}