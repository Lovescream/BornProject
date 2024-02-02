using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker {

    #region Properties

    public Creature Owner { get; private set; }

    public bool CanAttack => CurrentCooldown <= 0;
    public float Cooldown => 1 / Owner.AttackSpeed;
    public float CurrentCooldown {
        get => _currentCooldown;
        set {
            if (value <= 0) {
                _currentCooldown = 0;
            }
            else {
                _currentCooldown = value;
            }
        }
    }
    public float AttackTime => 0.1f;
    public float CurrentAttackTime {
        get => _currentAttackTimer;
        set {
            if (value <= 0) {
                if (_currentAttackTimer <= 0) return;
                _currentAttackTimer = 0;
                OnEndAttack?.Invoke();
            }
            else {
                _currentAttackTimer = value;
            }
        }
    }

    #endregion

    #region Fields

    private float _currentCooldown;
    private float _currentAttackTimer;

    public event Action OnStartAttack;
    public event Action OnEndAttack;

    #endregion

    #region Constructor

    public Attacker(Creature owner) { 
        Owner = owner;
    }

    #endregion

    public void OnUpdate() {
        CurrentCooldown -= Time.deltaTime;
        CurrentAttackTime -= Time.deltaTime;
    }

    public void Attack(AttackInfo attackInfo) {
        if (!CanAttack) return;

        OnStartAttack?.Invoke();
        CurrentAttackTime += AttackTime;

        HitCollider hitCollider = Main.Object.SpawnHitCollider(attackInfo.HitColliderKey, Owner.transform.position, attackInfo);
        hitCollider.transform.SetParent(Owner.transform);
        hitCollider.transform.localRotation = Quaternion.Euler(0, 0, attackInfo.RotationAngle);
        hitCollider.transform.localPosition = attackInfo.Offset;

        CurrentCooldown = Cooldown;
    }
}