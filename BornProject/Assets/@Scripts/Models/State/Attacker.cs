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

    #endregion

    #region Fields

    private float _currentCooldown;

    #endregion

    #region Constructor

    public Attacker(Creature owner) { 
        Owner = owner;
    }

    #endregion

    public void OnUpdate() {
        CurrentCooldown -= Time.deltaTime;
    }

    public void Attack(AttackInfo attackInfo) {
        if (!CanAttack) return;

        Main.Object.SpawnProjectile("", Owner.transform.position).SetInfo(attackInfo);

        CurrentCooldown = Cooldown;
    }
}