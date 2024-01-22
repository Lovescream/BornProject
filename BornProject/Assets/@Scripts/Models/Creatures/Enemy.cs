using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature {

    #region Properties

    public Creature Target {
        get => _target;
        set {
            if (_target == value) return;
            _target = value;
            if (_target == null) State.Current = CreatureState.Idle;
            else State.Current = CreatureState.Chase;
        }
    }

    #endregion

    #region Fields

    private Creature _target;

    // Debugging.
    private Transform _doubleSight;
    private Transform _sight;
    private Transform _range;

    #endregion

    #region Initialize / Set

    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);

        _doubleSight = this.transform.Find("[Debug] EnemySightSight");
        _sight = this.transform.Find("[Debug] EnemySight");
        _range = this.transform.Find("[Debug] EnemyRange");

        _doubleSight.transform.localScale = new(4 * Sight, 4 * Sight, 1);
        _sight.transform.localScale = new(2 * Sight, 2 * Sight, 1);
        _range.transform.localScale = new(Range, Range, 1);
    }

    protected override void SetState() {
        base.SetState();
        State.AddOnStay(CreatureState.Idle, OnStayIdle);
        State.AddOnStay(CreatureState.Chase, OnStayChase);
    }

    #endregion

    #region State

    private void OnStayIdle() {
        Velocity = Vector2.zero;
        Target = FindTarget();
    }
    
    private void OnStayChase() {
        if (Target.IsValid() && Target.IsDead) {
            Target = null;
            return;
        }

        Vector2 delta = Target.transform.position - this.transform.position;
        if (delta.sqrMagnitude > 4 * Sight * Sight) {
            Target = null;
            return;
        }

        Vector2 direction = (Target.transform.position - this.transform.position).normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
    }

    #endregion

    // 이 Creature의 시야 내의 적을 찾습니다.
    private Creature FindTarget() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, Sight);
        foreach (Collider2D collider in hits) {
            Creature creature = collider.GetComponent<Creature>();
            if (creature == null || creature == this) continue;
            if (IsTarget(creature)) return creature;
        }
        return null;
    }
    
    // 해당 Creature가 이 Creature의 적인지 판별.
    // 플레이어가 아니더라도 적끼리 서로 싸울 수 있으니 일단 만들어 두었습니다.
    protected virtual bool IsTarget(Creature creature) {
        if (creature is Player) return true;
        return false;
    }
}