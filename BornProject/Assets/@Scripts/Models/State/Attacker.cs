using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void Attack(HitColliderGenerationInfo generationInfo, HitColliderInfo hitColliderInfo, HitInfo hitInfo) {
        if (!CanAttack) return;

        OnStartAttack?.Invoke();
        CurrentAttackTime += generationInfo.AttackTime;

        if (generationInfo.Count == 1) {
            HitCollider hitCollider = GenerateHitCollider(generationInfo.SkillKey, generationInfo.HitColliderKey, hitColliderInfo, hitInfo);
            hitCollider.SetTransform(generationInfo.RadiusOffset, generationInfo.RotationAngle, generationInfo.Size);
        }
        else if (generationInfo.Count > 1) {
            float minAngle = generationInfo.RotationAngle - generationInfo.SpreadAngle * 0.5f;
            float maxAngle = generationInfo.RotationAngle + generationInfo.SpreadAngle * 0.5f;
            float deltaAngle = (maxAngle - minAngle) / (generationInfo.Count - 1);
            for (int i = 0; i < generationInfo.Count; i++) {
                float angle = minAngle + deltaAngle * i;
                HitCollider hitCollider = GenerateHitCollider(generationInfo.SkillKey, generationInfo.HitColliderKey, hitColliderInfo, hitInfo);

                hitCollider.SetTransform(generationInfo.RadiusOffset, angle, generationInfo.Size);
                hitCollider.SetDirection((hitCollider.transform.position - hitInfo.Owner.Indicator.position).normalized);
            }
        }

        CurrentCooldown = Cooldown;
    }

    private HitCollider GenerateHitCollider(string key, string prefabKey, HitColliderInfo info, HitInfo hitInfo) {
        return Main.Object.SpawnHitCollider(key, prefabKey, info, hitInfo);
    }
}