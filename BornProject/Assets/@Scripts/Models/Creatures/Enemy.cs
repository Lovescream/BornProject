using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature, IAttackable {

    #region Properties

    public Attacker Attacker { get; protected set; }

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

    protected static readonly int AnimatorParameterHash_Chase = Animator.StringToHash("Chase");

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
        State.AddOnEntered(CreatureState.Chase, OnEnteredChase);
        State.AddOnStay(CreatureState.Idle, OnStayIdle);
        State.AddOnStay(CreatureState.Chase, OnStayChase);
        State.AddOnStay(CreatureState.Attack, OnStayAttack);

        this.Attacker = new(this);
        this.Attacker.OnStartAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, true);
        };
        this.Attacker.OnEndAttack += () => {
            Debug.Log("ABC");
            _animator.SetBool(AnimatorParameterHash_Attack, false);
        };
    }

    #endregion

    #region State
    private void OnEnteredChase()
    {
        _animator.SetTrigger(AnimatorParameterHash_Chase);
    }

    private void OnStayIdle() {
        Velocity = Vector2.zero;
        Target = FindTarget();
    }
    
    private void OnStayChase() {
        if (Target.IsValid() || Target.IsDead) {
            Target = null;
            return;
        }

        Vector2 delta = Target.transform.position - this.transform.position;
        if (delta.sqrMagnitude > 4 * Sight * Sight) {
            Target = null;
            return;
        }

        // 공격범위에 들어왔을 때 Attack으로 현재상태 전환.
        if (delta.sqrMagnitude < Range * Range)
        {
            State.Current = CreatureState.Attack;
            return;
        }

        Vector2 direction = (Target.transform.position - this.transform.position).normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
    }

    private void OnStayAttack()
    {
        Vector2 delta = Target.transform.position - this.transform.position;
        if (Range * Range < delta.sqrMagnitude && delta.sqrMagnitude <= 4 * Sight * Sight) // 공격범위를 벗어났고 시야범위에 있다면
        {
            State.Current = CreatureState.Chase;
            Debug.Log("공격범위 벗어남");
            return;
        }
        else if (delta.sqrMagnitude > 4 * Sight * Sight)
        {
            State.Current = CreatureState.Idle;
        }
        else
        {
            Attack();
            Debug.Log("퍽");
        }

        // 공격할 때 플레이어의 위치를 실시간으로 체크.
        Vector2 direction = (Target.transform.position - this.transform.position).normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
    }

    #endregion

    #region Attack
    public void Attack()
    {
        Attacker.Attack(GetHitColliderGenerationInfo(), GetHitColliderInfo(), GetHitInfo());
    }

    public HitColliderGenerationInfo GetHitColliderGenerationInfo()
    {
        return new()
        {
            Owner = this,
            HitColliderKey = "Slash",
            RadiusOffset = 0.5f,
            RotationAngle = -1f,
            Count = 1,
            SpreadAngle = 0,
            Size = 1,
        };
    }

    public HitColliderInfo GetHitColliderInfo()
    {
        return new()
        {
            Penetration = 1,
            Speed = 0,
            DirectionX = 0,
            DirectionY = 0,
            Duration = 0,
            Range = 0,
        };
    }

    public HitInfo GetHitInfo()
    {
        return new()
        {
            Owner = this,
            Damage = this.Damage,
            CriticalChance = 0,
            CriticalBonus = 0,
            Knockback = new()
            {
                time = 0.1f,
                speed = 10f,
            }
        };
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