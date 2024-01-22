using DungeonGenerate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity
{    
    #region Properties

    // Data.
    public CreatureData Data { get; private set; }
    public Status Status { get; protected set; }
    public State<CreatureState> State { get; protected set; }

    // Status.
    public float HpMax => Status[StatType.HpMax].Value;
    public float Sight => Status[StatType.Sight].Value;
    public float Range => Status[StatType.Range].Value;

    public float Hp
    {
        get => _hp;
        set
        {
            if (_hp == value) return;
            if (value <= 0)
            {
                _hp = 0;
            }
            else if (value >= HpMax)
            {
                _hp = HpMax;
            }
            else _hp = value;
            OnChangedHp?.Invoke(_hp);
        }
    }
    public bool Invincibility
    {
        get { return _invincibility; }
        set { _invincibility = value; }
    }
    public Vector2 Velocity { get; protected set; }
    public Vector2 LookDirection { get; protected set; }
    public float LookAngle => Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;

    public bool IsDead => State.Current == CreatureState.Dead;
    public Room CurrentRoom => Main.Dungeon.GetRoom(this.transform.position);
    #endregion

    #region Fields

    protected static readonly int AnimatorParameterHash_Speed = Animator.StringToHash("Speed");
    protected static readonly int AnimatorParameterHash_Hit = Animator.StringToHash("Hit");
    protected static readonly int AnimatorParameterHash_Dead = Animator.StringToHash("Dead");

    // State, Status.
    private float _hp;
    private float _existPower;
    private bool _invincibility = false;

    // Components.
    protected SpriteRenderer _spriter;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    // Callbacks.
    public event Action<float> OnChangedHp;
    public event Action<float> OnChangedExistPower;

    #endregion

    #region MonoBehaviours

    protected virtual void FixedUpdate()
    {
        State.OnStay();
        _spriter.flipX = LookDirection.x < 0;
        _rigidbody.velocity = Velocity;
        _animator.SetFloat(AnimatorParameterHash_Speed, Velocity.magnitude);
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<Collider2D>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();

        return true;
    }
    public virtual void SetInfo(CreatureData data)
    {
        Initialize();

        this.Data = data;

        _animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{Data.Key}");
        _animator.SetBool(AnimatorParameterHash_Dead, false);

        _collider.enabled = true;
        if (_collider is BoxCollider2D boxCollider)
        {
            Sprite sprite = _spriter.sprite;
            if (sprite != null)
            {
                float x = sprite.textureRect.width / sprite.pixelsPerUnit;
                float y = sprite.textureRect.height / sprite.pixelsPerUnit;
                boxCollider.size = new(x, y);
            }
        }
        _rigidbody.simulated = true;

        SetStatus(isFullHp: true);
        SetState();
    }
    protected virtual void SetStatus(bool isFullHp = true)
    {
        this.Status = new(Data);
        if (isFullHp)
        {
            Hp = HpMax;
        }

    }

    protected virtual void SetState() {
        State = new() {
            Current = CreatureState.Idle
        };
        State.AddOnEntered(CreatureState.Hit, OnEnteredHit);
        State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
    }
    #endregion

    #region State

    private void OnEnteredHit() {
        _animator.SetTrigger(AnimatorParameterHash_Hit);
    }
    private void OnEnteredDead() {
        _collider.enabled = false;
        _rigidbody.simulated = false;
        _animator.SetBool(AnimatorParameterHash_Dead, true);
    }

    public virtual void OnHit(Creature attacker, float damage, KnockbackInfo knockbackInfo = default) {
        Hp -= damage;
        
        if (knockbackInfo.time > 0) {
            State.Current = CreatureState.Hit;
            Velocity = knockbackInfo.KnockbackVelocity;
            State.SetStateAfterTime(CreatureState.Idle, knockbackInfo.time);
        }
    }

    #endregion



}

public enum CreatureState
{
    Idle,
    Chase,
    Hit,
    Dead,
}

public struct KnockbackInfo
{
    public Vector2 KnockbackVelocity => direction.normalized * speed;

    public float time;
    public float speed;
    public Vector2 direction;
}