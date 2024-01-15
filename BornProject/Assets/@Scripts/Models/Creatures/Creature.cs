using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity {

    #region Properties

    public CreatureData Data { get; protected set; }

    public float Hp {
        get => _hp;
        set {
            if (_hp == value) return;
            if (value <= 0) {
                _hp = 0;
                // TODO:: 죽었을 때 처리.
                Main.Resource.Destroy(this.gameObject);
            }
            else if (value >= hpMax) {
                _hp = hpMax;
            }
            else _hp = value;
            OnChangedHp?.Invoke(_hp);
        }
    }

    public Vector2 Velocity { get; protected set; }
    public Vector2 LookDirection { get; protected set; }

    #endregion

    #region Inspector (TEMP)

    public float hpMax = 0;
    public float damage = 0;
    public float defense = 0;

    #endregion

    #region Fields

    // Status / State.
    private float _hp;

    // Components.
    private SpriteRenderer _spriter;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    // Callbacks.
    public event Action<float> OnChangedHp;

    #endregion

    #region MonoBehaviours

    protected virtual void FixedUpdate() {
        _rigidbody.velocity = Velocity;
        _animator.SetFloat("Speed", Velocity.magnitude);
        _spriter.flipX = LookDirection.x < 0;
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        _animator = this.GetComponent<Animator>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _collider = this.GetComponent<Collider2D>();

        OnChangedHp += OnChangeHp;

        return true;
    }

    public void SetInfo(CreatureData data) {
        Initialize();

        Data = data;

        _animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{Data.Key}");
        _animator.SetBool("Dead", false);

        hpMax = data.HpMax;
        damage = data.Damage;
        defense = data.Defense;

        Hp = hpMax;
    }

    #endregion

    #region Callbacks

    private void OnChangeHp(float hp) {
        Debug.Log($"현재 Hp는: {hp}");
    }

    #endregion
}