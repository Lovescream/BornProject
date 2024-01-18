using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Thing
{

    #region Properties

    public CreatureData Data { get; private set; }

    //public State<CreatureState> State { get; private set; }
    public Status Status { get; protected set; }
    //public CreatureInventory Inventory { get; private set; }

    public float Hp
    {
        get => _hp;
        set
        {
            if (_hp == value) return;
            if (value <= 0)
            {
                _hp = 0;
                //State.Current = CreatureState.Dead;
            }
            else if (value >= Status[StatType.HpMax].Value)
            {
                _hp = Status[StatType.HpMax].Value;
            }
            else _hp = value;
            OnChangedHp?.Invoke(_hp);
        }
    }
    //public float ExistPower
    //{
    //    get => _existPower;
    //    set
    //    {
    //        if (_existPower == value) return;
    //        if (value <= 0)
    //        {
    //            _existPower = 0;
    //            State.Current = CreatureState.Dead;
    //        }
    //        else if (value >= Status[StatType.ExistPowerMax].Value)
    //        {
    //            _existPower = Status[StatType.ExistPowerMax].Value;
    //        }
    //        else _existPower = value;
    //        OnChangedExistPower?.Invoke(_existPower);
    //    }
    //}
    public bool Invincibility
    {
        get { return _invincibility; }
        set { _invincibility = value; }
    }
    public Vector2 Velocity { get; protected set; }
    public Vector2 LookDirection { get; protected set; }
    public float LookAngle => Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;

    //public UI_HpBar HpBar { get; set; }

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
        //State.OnStay();
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

        //_animator.runtimeAnimatorController = Main.Resource.Load<RuntimeAnimatorController>($"{Data.Key}.animController");
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

        //SetStateEvent();
        SetStatus(isFullHp: true);
        //SetInventory();
    }
    protected virtual void SetStatus(bool isFullHp = true)
    {
        this.Status = new(Data);
        if (isFullHp)
        {
            Hp = Status[StatType.HpMax].Value;
            //ExistPower = Status[StatType.ExistPowerMax].Value;
        }

        //OnChangedHp -= ShowHpBar;
        //OnChangedHp += ShowHpBar;
    }
    //protected virtual void SetStateEvent()
    //{
    //    State = new();
    //    State.AddOnEntered(CreatureState.Hit, () => _animator.SetTrigger(AnimatorParameterHash_Hit));
    //    State.AddOnEntered(CreatureState.Dead, () => {
    //        _collider.enabled = false;
    //        _rigidbody.simulated = false;
    //        _animator.SetBool(AnimatorParameterHash_Dead, true);

    //        DropOBJ dropSkill = GetComponent<DropOBJ>();
    //        if (dropSkill != null)
    //        {
    //            dropSkill.MobDrop();
    //        }
    //    });
    //}
    //protected virtual void SetInventory()
    //{
    //    Inventory = new(this, 20);
    //}

    #endregion

    #region State

    //public virtual void OnHit(Creature attacker, float damage = 0, KnockbackInfo knockbackInfo = default)
    //{
    //    if (Invincibility == false)
    //    {
    //        Hp -= damage;

    //        if (knockbackInfo.time > 0)
    //        {
    //            State.Current = CreatureState.Hit;
    //            Velocity = knockbackInfo.KnockbackVelocity;
    //            State.SetStateAfterTime(CreatureState.Idle, knockbackInfo.time);
    //        }
    //    }
    //}

    #endregion

    //protected void ShowHpBar(float hp)
    //{
    //    if (HpBar != null)
    //    {
    //        HpBar.ResetInfo();
    //        return;
    //    }
    //    HpBar = Main.UI.ShowHpBar(this);
    //}
}

public enum CreatureState
{
    Idle,
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