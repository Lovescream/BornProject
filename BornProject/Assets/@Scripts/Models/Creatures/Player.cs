using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : Creature, ISkillMan, IAttackable {

    #region Properties

    public Entity Entity => this;
    public Attacker Attacker { get; protected set; }
    public AttackIndicator Indicator { get; protected set; }
    public SkillList SkillList { get; protected set; }
    public SkillStatus DefaultStatus { get; protected set; }

    public string SkillSetList => Data.Skills;

    public Vector2 MousePosition { get; protected set; }
    //public override Vector2 LookDirection {
    //    get => _lookDirection;
    //    set => _lookDirection = value;
    //}

    #endregion

    #region Fields

    private bool _isAttacking;

    #endregion

    #region MonoBehaviours

    protected override void FixedUpdate() {
        base.FixedUpdate();

        Attacker.OnUpdate();
    }

    protected override void Update() {
        base.Update();

        if (_isAttacking) Attack();
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null) camera.SetTarget(this.transform);

        this.GetComponent<PlayerInput>().actions.FindAction("AttackMain").started += a => {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                SkillList.Set(SkillType.Range);
                _isAttacking = true;
            }
        };
        this.GetComponent<PlayerInput>().actions.FindAction("AttackMain").canceled += a => _isAttacking = false;
        this.GetComponent<PlayerInput>().actions.FindAction("AttackSub").started += a => {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                SkillList.Set(SkillType.Melee);
                _isAttacking = true;
            }
        };
        this.GetComponent<PlayerInput>().actions.FindAction("AttackSub").canceled += a => _isAttacking = false;

        return true;
    }
    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);

        Indicator = this.gameObject.FindChild<AttackIndicator>();
        Indicator.SetInfo(this);

        SkillList = new(this);
        for (int i = 0; i < Enum.GetNames(typeof(SkillType)).Length; i++) {
            string skillKey = Main.Game.Current[(SkillType)i];
            if (string.IsNullOrEmpty(skillKey)) continue;
            SkillList.Set(skillKey);
            if (i == 0) SkillList.Set((SkillType)i);
        }
    }
    protected override void SetStatus(bool isFullHp = true) {
        base.SetStatus(isFullHp);
        DefaultStatus = new() {
            Damage = Status[StatType.Damage],
            AttackSpeed = Status[StatType.AttackSpeed],
            Range = Status[StatType.Range],
        };
    }
    protected override void SetState() {
        base.SetState();

        this.Attacker = new(this);
        this.Attacker.OnStartAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, true);
            Main.Audio.Play(SkillList.Current);
        };
        this.Attacker.OnEndAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, false);
        };

        this.State.AddOnEntered(CreatureState.Dead, OnEnteredDead);
    }

    #endregion

    private void OnEnteredDead() {
        Main.UI.OpenPopupUI<UI_Popup_GameOver>();
    }


    #region Input

    protected void OnMove(InputValue value) {
        Velocity = value.Get<Vector2>().normalized * Status[StatType.MoveSpeed].Value;
    }
    protected void OnLook(InputValue value) {
        if (this.IsDead) return;

        MousePosition = Camera.main.ScreenToWorldPoint(value.Get<Vector2>());
        LookDirection = (MousePosition - (Vector2)this.transform.position).normalized;
        Indicator.IndicatorDirection = LookDirection;
    }
    protected void OnAttackMain() {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        ////SkillList.ChangeBasicSkill(0);
        //SkillList.SetRangeSkill();
        //Attack();
    }
    protected void OnAttackSub() {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        ////SkillList.ChangeBasicSkill(1);
        //SkillList.SetMeleeSkill();
        //Attack();
    }
    protected void OnButtonTab() {
        Main.Audio.PlayOnButton();
        Main.Dungeon.ToggleMap();
    }
    protected void OnButtonQ() {
        Main.Audio.PlayOnButton();
        UI_Popup_Quest popup = Main.UI.GetLatestPopup<UI_Popup_Quest>();
        if (popup == null) Main.UI.OpenPopupUI<UI_Popup_Quest>().SetInfo();
        else Main.UI.ClosePopup(popup);
    }
    protected void OnButtonK() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_Skill>().SetInfo();
    }

    // 대쉬 임시로 넣어놨습니다.
    private bool isDashing = false;

    protected void OnDash()
    {
        if (this.IsDead) return;

        if (!isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        CreatureState OriginState = State.Current;
        State.Current = CreatureState.Dash;

        // 대쉬에 사용할 힘의 크기를 설정합니다.
        float dashPower = 10f; // 대쉬에 사용할 힘의 크기 (원하는 값으로 설정)

        // 대쉬 방향을 설정합니다.
        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // 만약 움직이지 않은 상태에서 스페이스바를 누르면, 플레이어의 시선 방향으로 대쉬합니다.
        if (dashDirection == Vector2.zero)
        {
            dashDirection = LookDirection;
        }

        // 대쉬할 때의 속도를 설정합니다.
        Vector2 dashVelocity = dashDirection * dashPower;

        // 현재 속도에 대쉬 속도를 더해줍니다.
        Velocity += dashVelocity;

        // 대쉬가 끝날 때까지 대기합니다.
        yield return new WaitForSeconds(0.5f); // 대쉬 지속시간 (원하는 값으로 설정)

        isDashing = false;
        State.Current = OriginState;
    }

    #endregion

    public override void OnHit(IHitCollider attacker) {
        base.OnHit(attacker);
        Main.Audio.Play(this, CreatureState.Hit);
    }
    public void Attack() {
        if (this.IsDead) return;
        if (SkillList.Current == null) return;
        Attacker.Attack(GetHitColliderGenerationInfo(), GetHitColliderInfo(), GetHitInfo());
    }



    public HitColliderGenerationInfo GetHitColliderGenerationInfo() {
        Skill skill = SkillList.Current;
        return new() {
            Owner = this,
            SkillKey = skill.Data.Key,
            HitColliderKey = skill.Data.HitColliderKey,
            RadiusOffset = skill.Data.RadiusOffset,
            RotationAngle = skill.Data.RotationAngle,
            Count = skill.Data.HitColliderCount,
            SpreadAngle = skill.Data.HitColliderAngle,
            Size = skill.Data.HitColliderSize,
            AttackTime = skill.Data.AttackTime,
        };
    }
    public HitColliderInfo GetHitColliderInfo() {
        Skill skill = SkillList.Current;
        return new() {
            Penetration = skill.Data.Penetration,
            Speed = skill.Data.Speed,
            DirectionX = skill.Data.DirectionX,
            DirectionY = skill.Data.DirectionY,
            Duration = skill.Data.Duration,
            Range = skill.Data.Range,
        };
    }
    public HitInfo GetHitInfo() {
        Skill skill = SkillList.Current;
        return new() {
            Owner = this,
            Damage = this.Damage,
            CriticalChance = skill.Data.CriticalChance,
            CriticalBonus = skill.Data.CriticalBonus,
            Knockback = new() {
                time = 0.1f,
                speed = 10f,
            }
        };
    }

    public bool IsTarget(Creature creature) => true;
}