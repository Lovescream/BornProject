using ZerolizeDungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Creature : Entity {

    #region Properties

    // Data.
    public CreatureData Data { get; private set; }
    public Status Status { get; protected set; }
    public State<CreatureState> State { get; protected set; }
    public Transform Indicator { get; protected set; }

    // Status.
    public float HpMax => Status[StatType.HpMax].Value;
    public float Damage => Status[StatType.Damage].Value;
    public float AttackSpeed => Status[StatType.AttackSpeed].Value;
    public float Sight => Status[StatType.Sight].Value;
    public float Range => Status[StatType.Range].Value;

    public float Hp {
        get => _hp;
        set {
            if (_hp == value) return;
            if (this.GetComponent<Player>() != null)
            {
                Debug.Log($"Player의 Hp를 {value}로 설정합니다.");
            }
            if (value <= 0)
            {
                if (State.Current != CreatureState.Dead)
                    Debug.Log($"[Creature: {this.Data.Key}] 쥬금");
                    State.Current = CreatureState.Dead;
                _hp = 0;
            }
            else if (value >= HpMax) {
                _hp = HpMax;
            }
            else _hp = value;
            OnChangedHp?.Invoke(_hp);
        }
    }
    public bool Invincibility {
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
    protected static readonly int AnimatorParameterHash_Attack = Animator.StringToHash("Attack");
    protected static readonly int AnimatorParameterHash_Dead = Animator.StringToHash("Dead");

    // State, Status.
    private float _hp;
    private float _existPower;
    private bool _invincibility = false;

    // Components.
    protected Transform _indicatorAxisAxis;
    protected Transform _indicatorAxis;
    protected SpriteRenderer _spriter;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    // Callbacks.
    public event Action<float> OnChangedHp;
    public event Action<float> OnChangedExistPower;

    #endregion

    #region MonoBehaviours

    protected virtual void Update() {
        if (_indicatorAxisAxis != null) {
            _indicatorAxisAxis.localScale = new(LookDirection.x >= 0 ? 1 : -1, 1, 1);
            _indicatorAxis.localScale = new(LookDirection.x >= 0 ? 1 : -1, 1, 1);
        }
        if (_indicatorAxis != null) {
            _indicatorAxis.localRotation = Quaternion.Euler(0, 0, (LookDirection.x >= 0 ? 1 : -1) * LookAngle);
        }
    }

    protected virtual void FixedUpdate() {
        State.OnStay();
        _spriter.flipX = LookDirection.x < 0;
        _rigidbody.velocity = Velocity;
        _animator.SetFloat(AnimatorParameterHash_Speed, Velocity.magnitude);
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<Collider2D>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();

        this.gameObject.layer = Main.CreatureLayer;

        return true;
    }
    public virtual void SetInfo(CreatureData data) {
        Initialize();

        this.Data = data;

        _animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{Data.Key}");
        _animator.SetBool(AnimatorParameterHash_Dead, false);

        _collider.enabled = true;
        if (_collider is BoxCollider2D boxCollider) {
            Sprite sprite = _spriter.sprite;
            if (sprite != null) {
                float x = sprite.textureRect.width / sprite.pixelsPerUnit;
                float y = sprite.textureRect.height / sprite.pixelsPerUnit;
                boxCollider.size = new(x, y);
            }
        }
        _rigidbody.simulated = true;

        SetStatus(isFullHp: true);
        SetState();

        _indicatorAxisAxis = this.transform.Find("Axis");
        if (_indicatorAxisAxis != null) {
            _indicatorAxis = _indicatorAxisAxis.Find("Indicator");
            if (_indicatorAxis != null) {
                Indicator = _indicatorAxis.Find("WeaponIndicator");
            }
        }
    }
    protected virtual void SetStatus(bool isFullHp = true) {
        this.Status = new(Data);
        if (isFullHp) {
            Hp = HpMax;
        }

    }

    protected virtual void SetState() {
        State = new() {
            Current = CreatureState.Idle
        };
        State.AddOnEntered(CreatureState.Hit, OnEnteredHit);
        State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
        State.AddOnExited(CreatureState.Hit, OnExitedHit);
    }
    #endregion

    #region State

    private void OnEnteredHit() {
        _animator.SetBool(AnimatorParameterHash_Hit, true);
    }
    private void OnExitedHit() {
        _animator.SetBool(AnimatorParameterHash_Hit, false);
    }

    private void OnEnteredDead() {
        _collider.enabled = false;
        _rigidbody.simulated = false;
        _animator.SetBool(AnimatorParameterHash_Dead, true);
    }

    public virtual void OnHit(IHitCollider attacker) {
        HitInfo hitInfo = attacker.HitInfo;
        //if (this.GetComponent<Enemy>() != null) Debug.Log($"곰이 {hitInfo.Damage} 피해를 입엇다");
        //Debug.Log($"{hitInfo.Damage}의 피해를 입었따. 죽어라 - !");
        Hp -= hitInfo.Damage;

        CreatureState originState = State.Current == CreatureState.Hit ? State.NextState :State.Current; // 원래 상태 저장.
        State.Current = CreatureState.Hit;
        if (hitInfo.Knockback.time > 0) {
            Velocity = (this.transform.position - attacker.CurrentPosition).normalized * hitInfo.Knockback.speed;
        }
        State.SetStateAfterTime(originState, hitInfo.Knockback.time); // 넉백 시간이 끝나면 원래 상태로 돌아간다.
    }

    #endregion



}

public enum CreatureState
{
    Idle,
    Chase,
    Hit,
    Attack,
    Dead,
}

public struct KnockbackInfo {
    public float time;
    public float speed;
}