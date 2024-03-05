using ZerolizeDungeon;
using System;
using UnityEngine;
using UnityEditor.PackageManager;

public class Creature : Entity {

    #region Properties

    // Data.
    public CreatureData Data { get; private set; }
    public Status Status { get; protected set; }
    public State<CreatureState> State { get; protected set; }

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
            float origin = _hp;
            if (value <= 0) {
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
    public Vector2 KnockbackVelocity { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public virtual Vector2 LookDirection {
        get => _lookDirection;
        set {
            _lookDirection = value;
            Flip = value.x < 0;
        }
    }
    public float LookAngle => Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;

    public bool IsDead => State.Current == CreatureState.Dead;
    public Room CurrentRoom => Main.Dungeon.GetRoom(this.transform.position);
    #endregion

    #region Fields

    protected static readonly int AnimatorParameterHash_Speed = Animator.StringToHash("Speed");
    protected static readonly int AnimatorParameterHash_Hit = Animator.StringToHash("Hit");
    protected static readonly int AnimatorParameterHash_Attack = Animator.StringToHash("Attack");
    protected static readonly int AnimatorParameterHash_Dead = Animator.StringToHash("Dead");
    protected static readonly int AnimatorParameterHash_Dash = Animator.StringToHash("Dash");

    // State, Status.
    private float _hp;
    private bool _invincibility = false;
    protected Vector2 _lookDirection;

    // Components.
    protected Rigidbody2D _rigidbody;

    // Callbacks.
    public event Action<float> OnChangedHp;
    public event Action<float> OnChangedExistPower;

    #endregion

    #region MonoBehaviours

    protected virtual void Update() { }

    protected virtual void FixedUpdate() {
        State.OnStay();
        if (KnockbackVelocity.magnitude <= float.Epsilon)
            _rigidbody.velocity = Velocity;
        else
            _rigidbody.velocity = KnockbackVelocity;
        _animator.SetFloat(AnimatorParameterHash_Speed, Velocity.magnitude);
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rigidbody = this.GetComponent<Rigidbody2D>();

        this.gameObject.layer = Layers.CreatureLayer;

        return true;
    }
    public virtual void SetInfo(CreatureData data) {
        Initialize();

        this.Data = data;

        _animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{Data.Key}");
        _animator.SetBool(AnimatorParameterHash_Dead, false);

        _collider.enabled = true;
        _rigidbody.simulated = true;

        SetStatus(isFullHp: true);
        SetState();
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
        State.AddOnEntered(CreatureState.Dash, OnEnteredDash);
        State.AddOnExited(CreatureState.Dash, OnExitedDash);

    }
    #endregion

    #region State

    private void OnEnteredHit() {
        Debug.Log("ddddd");
        _animator.SetBool(AnimatorParameterHash_Hit, true);
    }
    private void OnEnteredDead() {
        _collider.enabled = false;
        _rigidbody.simulated = false;
        _animator.SetBool(AnimatorParameterHash_Hit, false);
        _animator.SetBool(AnimatorParameterHash_Attack, false);
        _animator.SetBool(AnimatorParameterHash_Dead, true);
    }
    private void OnEnteredDash() {
        Debug.Log("나와");
        _animator.SetBool(AnimatorParameterHash_Dash, true);
    }
    private void OnExitedHit() {
        _animator.SetBool(AnimatorParameterHash_Hit, false);
        KnockbackVelocity = Vector2.zero;
    }
    private void OnExitedDash() {
        _animator.SetBool(AnimatorParameterHash_Dash, false);
    }

    public virtual void OnHit(IHitCollider attacker) {
        HitInfo hitInfo = attacker.HitInfo;
        float prevHp = Hp;
        Hp -= hitInfo.Damage;
        Main.Object.ShowHpBar(this, Hp, prevHp);
        Main.Object.ShowDamageText(this.transform.position, hitInfo.Damage);

        CreatureState originState = State.Current == CreatureState.Hit ? State.NextState :State.Current; // 원래 상태 저장.
        State.Current = CreatureState.Hit;
        if (hitInfo.Knockback.time > 0) {
            KnockbackVelocity = (this.transform.position - attacker.CurrentPosition).normalized * hitInfo.Knockback.speed;
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
    Dash,
}

public struct KnockbackInfo {
    public float time;
    public float speed;
}