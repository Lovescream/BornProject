using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature, ISkillMan, IAttackable {

    #region Properties

    public Attacker Attacker { get; protected set; }
    public SkillList SkillList { get; protected set; }
    public SkillStatus DefaultStatus { get; protected set; }

    public string SkillSetList => Data.Skills;

    public Creature Target {
        get => _target;
        set {
            if (_target == value) return;
            _target = value;
            if (State.Current == CreatureState.Hit) {
                if (_target == null) State.SetStateAfterTime(CreatureState.Idle);
                else State.SetStateAfterTime(CreatureState.Chase);
            }
            if (_target == null) State.Current = CreatureState.Idle;
            else State.Current = CreatureState.Chase;
        }
    }

    public float DetectingRange => 2 * Sight;

    #endregion

    #region Fields

    private Creature _target;
    private Creature _lastAttacker;

    // Debugging.
    private Transform _doubleSight;
    private Transform _sight;
    private Transform _range;

    // Coroutines.
    private Coroutine _coDead;

    // Callbacks.
    public event Action OnDead;

    #endregion

    #region MonoBehaviours

    protected override void FixedUpdate() {
        base.FixedUpdate();

        Attacker.OnUpdate();
    }

    #endregion

    #region Initialize / Set

    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);

        _doubleSight = this.transform.Find("[Debug] EnemySightSight");
        _sight = this.transform.Find("[Debug] EnemySight");
        _range = this.transform.Find("[Debug] EnemyRange");

        _doubleSight.transform.localScale = 2 * DetectingRange * Vector3.one;
        _sight.transform.localScale = 2 * Sight * Vector3.one;
        _range.transform.localScale = 2 * Range * Vector3.one;

        SkillList = new(this, Data.DefaultSkills);
    }
    protected override void SetStatus(bool isFullHp = true) {
        base.SetStatus(isFullHp);
        DefaultStatus = new() {
            Damage = Status[StatType.Damage],
            AttackSpeed = Status[StatType.AttackSpeed],
            Range = Status[StatType.Range],
        };
    }
    protected override void SetState() {
        base.SetState();
        State.AddOnEntered(CreatureState.Attack, OnEnteredAttack);
        State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
        State.AddOnStay(CreatureState.Idle, OnStayIdle);
        State.AddOnStay(CreatureState.Chase, OnStayChase);
        State.AddOnStay(CreatureState.Attack, OnStayAttack);

        this.Attacker = new(this);
        this.Attacker.OnStartAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, true);
        };
        this.Attacker.OnEndAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, false);
        };

        OnDead = null;
    }

    #endregion

    #region State

    private void OnEnteredAttack() {
        Velocity = Vector2.zero;
    }
    private void OnEnteredDead() {
        OnDead?.Invoke();
        if (_coDead != null) StopCoroutine(_coDead);
        _coDead = StartCoroutine(CoDead());
    }

    private void OnStayIdle() {
        Velocity = Vector2.zero;
        Target = FindTarget();
    }

    private void OnStayChase() {
        // #1. Target이 죽거나 유효하지 않으면 Target 정보 초기화.
        if (!Target.IsValid() || Target.IsDead) {
            Target = null;
            return;
        }

        // #2. Target과의 거리 구하기. (제곱근을 구하는 연산은 비용이 크므로, 제곱 형태로 비교.)
        float sqrDistance = (Target.transform.position - this.transform.position).sqrMagnitude;

        // #3. Target이 선빵친 애가 아니고 감지할 수 있는 거리 밖으로 벗어나면 Target 정보 초기화. (적 놓침 판정)
        if (sqrDistance > DetectingRange * DetectingRange && Target != _lastAttacker) {
            Target = null;
            return;
        }

        // #4. Target이 사정거리 내에 진입했다면, 공격 상태에 진입.
        if (sqrDistance <= Range * Range) {
            State.Current = CreatureState.Attack;
            return;
        }

        // #5. Target이 사정거리 밖에 있다면 대상을 향해 이동.
        Vector2 direction = (Target.transform.position - this.transform.position).normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
    }

    private void OnStayAttack() {
        // #1. Target이 죽거나 유효하지 않으면 Target 정보 초기화.
        if (!Target.IsValid() || Target.IsDead) {
            Target = null;
            return;
        }

        // #2. Target과의 거리 구하기. (제곱근을 구하는 연산은 비용이 크므로, 제곱 형태로 비교.)
        float sqrDistance = (Target.transform.position - this.transform.position).sqrMagnitude;

        // #3. Target이 사정거리 밖으로 벗어났다면 추적 상태에 진입.
        if (sqrDistance > Range * Range) {
            State.Current = CreatureState.Chase;
            return;
        }

        // #4. 공격!
        Attack();

        // #5. Target이 공격거리 밖에 있다면 대상을 향해 이동.
        Vector2 direction = (Target.transform.position - this.transform.position).normalized;
        Velocity = direction * Status[StatType.MoveSpeed].Value;
        LookDirection = direction;
    }

    public override void OnHit(IHitCollider attacker) {
        if (attacker.HitInfo.Owner is Creature attackerCreature && attackerCreature.IsValid() && !attackerCreature.IsDead && IsTarget(attackerCreature))
            Target = attackerCreature;
            _lastAttacker = Target;
        base.OnHit(attacker);
    }

    #endregion

    #region Attack
    public void Attack() {
        Attacker.Attack(GetHitColliderGenerationInfo(), GetHitColliderInfo(), GetHitInfo());
    }

    public HitColliderGenerationInfo GetHitColliderGenerationInfo() {
        Skill skill = SkillList.Current;
        return new()
        {
            Owner = this,
            HitColliderKey = skill.Data.HitColliderKey,
            RadiusOffset = skill.Data.RadiusOffset,
            RotationAngle = skill.Data.RotationAngle < 0 ? LookAngle : skill.Data.RotationAngle,
            Count = skill.Data.HitColliderCount,
            SpreadAngle = skill.Data.HitColliderAngle,
            Size = skill.Data.HitColliderSize,
            AttackTime = skill.Data.AttackTime,
        };
    }

    public HitColliderInfo GetHitColliderInfo() {
        Skill skill = SkillList.Current;
        return new()
        {
            Penetration = skill.Data.Penetration,
            Speed = skill.Data.Speed,
            DirectionX = skill.Data.DirectionX,
            DirectionY = skill.Data.DirectionY,
            Duration = skill.Data.Duration,
            Range = skill.Data.Range,
        };
    }

    public HitInfo GetHitInfo() {
        Skill skill = SkillList.Current;
        return new()
        {
            Owner = this,
            Damage = this.Damage,
            CriticalChance = skill.Data.CriticalChance,
            CriticalBonus = skill.Data.CriticalBonus,
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
    public virtual bool IsTarget(Creature creature) {
        if (creature is Player) return true;
        return false;
    }

    private IEnumerator CoDead() {
        yield return new WaitForSeconds(2f);
        Main.Object.DespawnEnemy(this);
        _coDead = null;
        StopAllCoroutines();
        yield break;
    }

}