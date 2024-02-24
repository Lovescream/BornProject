using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beatle : Enemy {

    protected static readonly int AnimatorParameterHash_Fly = Animator.StringToHash("Fly");

    #region Initialize / Set

    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);
    }
    protected override void SetStatus(bool isFullHp = true) {
        base.SetStatus(isFullHp);
    }
    protected override void SetState() {
        base.SetState();
        State.AddOnEntered(CreatureState.Chase, OnEnteredChase);
        State.AddOnEntered(CreatureState.Attack, OnEnteredAttack);
        State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
        State.AddOnExited(CreatureState.Chase, OnExitedChase);
        State.AddOnExited(CreatureState.Attack, OnExitedAttack);
    }

    #endregion

    #region State

    private void OnEnteredChase() {
        _animator.SetBool(AnimatorParameterHash_Fly, true);
    }
    private void OnEnteredAttack() {
        Velocity = Vector2.zero;
    }
    private void OnEnteredDead() {
        _animator.SetBool(AnimatorParameterHash_Fly, false);
    }
    private void OnExitedChase() {
        if (Target == null) _animator.SetBool(AnimatorParameterHash_Fly, false);
    }
    private void OnExitedAttack() {
        if (Target == null) _animator.SetBool(AnimatorParameterHash_Fly, false);
    }

    #endregion

}