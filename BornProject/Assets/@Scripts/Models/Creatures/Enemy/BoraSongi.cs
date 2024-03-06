using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZerolizeDungeon;

public class BoraSongi : Enemy {

    public Room Room { get; private set; }
    public bool CanEnterHit { get; private set; }

    private bool _basak;

    #region MonoBehaviours

    protected virtual void OnDisable() {
        if (!Main.IsInitialized) return;
        (Main.Scene.Current.SceneUI as UI_Scene_Game).SetBossHpBar(null);
        Main.Dungeon.StageClear();
    }

    #endregion

    #region Initialize / Set

    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);

        Room = CurrentRoom;
        CanEnterHit = false;
    }

    protected override void SetState(CreatureState defaultState = CreatureState.Idle) {
        base.SetState(CreatureState.Idle);

        State.AddOnEntered(CreatureState.Sleep, OnEnteredSleep);
        State.AddOnEntered(CreatureState.Idle, OnEnteredIdle);
        State.AddOnEntered(CreatureState.Chase, OnEnteredChase);
        State.AddOnStay(CreatureState.WakeUp, OnStayWakeUp);
        State.AddOnExited(CreatureState.Sleep, OnExitedSleep);

        State.Current = CreatureState.Sleep;


        // ======= Test용 Hp ========
        this.Status[StatType.HpMax].SetValue(200);
        this.Hp = HpMax;
        // ======= 나중에 수정 ========
    }

    #endregion

    #region State

    protected virtual void OnEnteredSleep() {
        _animator.SetBool(AnimatorParameterHash_Sleep, true);
    }
    protected virtual void OnEnteredIdle() {
        CanEnterHit = true;
    }
    protected virtual void OnEnteredChase() {
        SkillList.Set("BoraSongi_Base_PoisonSpore");
        SkillList.Set(SkillType.Range);
        CanEnterHit = true;
    }
    protected override void OnEnteredAttack() {
        base.OnEnteredAttack();
        SkillList.Set("BoraSongi_Normal_Mushboom");
        SkillList.Set(SkillType.Range);
        CanEnterHit = true;
    }
    protected override void OnStayChase() {
        // #1. Target이 죽거나 유효하지 않으면 Target 정보 초기화.
        if (!Target.IsValid() || Target.IsDead) {
            Target = null;
            return;
        }

        // #2. Target을 향해 이동.
        Vector2 delta = Target.transform.position - this.transform.position;
        Vector2 direction = delta.normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
        Indicator.IndicatorDirection = LookDirection;

        // #3. 빵구!
        Attack();

        // #4. Target이 일정 거리 안으로 들어오면 공격 상태로 전환.
        if (delta.sqrMagnitude <= 100) {
            State.Current = CreatureState.Attack;
            return;
        }
    }
    protected override void OnStayAttack() {
        // #1. Target이 죽거나 유효하지 않으면 Target 정보 초기화.
        if (!Target.IsValid() || Target.IsDead) {
            Target = null;
            return;
        }

        if (Attacker.CanAttack) {
            if (!_animator.GetBool(AnimatorParameterHash_AttackInit)) {
                CanEnterHit = false;
                Velocity = Vector2.zero;
                _animator.SetBool(AnimatorParameterHash_AttackInit, true);
            }
            Attack();
        }
        else if (!_basak) {
            CanEnterHit = true;
            _animator.SetBool(AnimatorParameterHash_AttackInit, false);
            Vector2 delta = Target.transform.position - this.transform.position;
            Vector2 direction = delta.normalized;
            Velocity = direction * Status[StatType.MoveSpeed].Value;
            LookDirection = direction;
            Indicator.IndicatorDirection = LookDirection;
            if (delta.sqrMagnitude >= 400) {
                State.Current = CreatureState.Chase;
                return;
            }
        }
    }

    protected virtual void OnStayWakeUp() {
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            State.Current = CreatureState.Idle;
    }

    protected virtual void OnExitedSleep() {
        _animator.SetBool(AnimatorParameterHash_Sleep, false);
        (Main.Scene.Current.SceneUI as UI_Scene_Game).SetBossHpBar(this);
    }

    public override void OnHit(IHitCollider attacker) {
        HitInfo hitInfo = attacker.HitInfo;
        float prevHp = Hp;
        Hp -= hitInfo.Damage;
        Main.Object.ShowHpBar(this, Hp, prevHp);
        Main.Object.ShowDamageText(this.transform.position, hitInfo.Damage);

        if (!CanEnterHit) return;
        CreatureState originState = State.Current == CreatureState.Hit ? State.NextState : State.Current; // 원래 상태 저장.
        State.Current = CreatureState.Hit;
        if (hitInfo.Knockback.time > 0) {
            KnockbackVelocity = (this.transform.position - attacker.CurrentPosition).normalized * hitInfo.Knockback.speed;
        }
        State.SetStateAfterTime(originState, hitInfo.Knockback.time); // 넉백 시간이 끝나면 원래 상태로 돌아간다.
    }

    #endregion

    public void WakeUp() {
        if (State.Current != CreatureState.Sleep) return;
        State.Current = CreatureState.WakeUp;
    }

    protected void BBQGoldenOliveChicken() {
        if (!_basak) _basak = true;
        else {
            _basak = false;
            _animator.SetBool(AnimatorParameterHash_AttackInit, false);
        }
    }


    protected override Creature FindTarget() {
        Player player = Main.Game.Player;
        if (player == null || !player.IsValid()) return null;
        if (player.CurrentRoom != this.Room) return null;
        return player;
    }

}