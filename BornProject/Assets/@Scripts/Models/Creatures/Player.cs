using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Creature, IAttackable {
    #region Properties

    public Attacker Attacker { get; protected set; }

    #endregion

    #region Fields

    protected static readonly int AnimatorParameterHash_Attack = Animator.StringToHash("Attack");

    private bool _isRangeAttack_Temp = false;

    #endregion

    #region MonoBehaviours

    protected override void FixedUpdate() {
        base.FixedUpdate();

        Attacker.OnUpdate();
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null) camera.SetTarget(this.transform);

        return true;
    }
    protected override void SetState() {
        base.SetState();

        this.Attacker = new(this);
        this.Attacker.OnStartAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, true);
        };
        this.Attacker.OnEndAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, false);
        };
    }

    #endregion

    #region Input

    protected void OnMove(InputValue value) {
        Velocity = value.Get<Vector2>().normalized * Status[StatType.MoveSpeed].Value;
    }
    protected void OnLook(InputValue value) {
        Vector2 v = Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position;
        LookDirection = v.normalized;
    }
    protected void OnFire() {
        Attack();
    }

    #endregion

    public void Attack() {
        _isRangeAttack_Temp = true;
        Attacker.Attack(GetAttackInfo());
    }

    public AttackInfo GetAttackInfo() {
        return new() {
            Owner = this,
            HitColliderKey = _isRangeAttack_Temp ? "BasicProjectile" : "BasicMelee",
            Damage = this.Damage,
            CriticalChance = 0,
            CriticalBonus = 1.5f,
            Penetrate = 1,
            Speed = 10,
            Direction = LookDirection,
            Duration = 5,
            Knockback = new() {
                time = 0.1f,
                speed = 10,
            }
        };
    }
}