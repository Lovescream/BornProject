using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Thing
{
    
    #region Properties

    public CreatureData Data { get; private set; }

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
            else _hp = value;
        }
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

    // Components.
    protected SpriteRenderer _spriter;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    // Callbacks.
    public event Action<float> OnChangedHp;

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
        this.Hp = Data.HpMax;
        Debug.Log(Hp + "현재 체력");

    }
   

    #endregion
}
