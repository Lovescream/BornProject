using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : Creature, ISkillMan, IAttackable {

    #region Properties

    public Attacker Attacker { get; protected set; }
    public SkillList SkillList { get; protected set; }
    public SkillStatus DefaultStatus { get; protected set; }

    public string SkillSetList => Data.Skills;

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
            SkillFireAudioSource();
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
        if (!this.IsDead) // 안죽었다면
        {
            Vector2 v = Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position;
            LookDirection = v.normalized;
        }
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

    #endregion

    public override void OnHit(IHitCollider attacker) {
        base.OnHit(attacker);
        AudioController.Instance.SFXPlay(SFX.PlayerHit);
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
            HitColliderKey = skill.Data.HitColliderKey,
            RadiusOffset = skill.Data.RadiusOffset,
            RotationAngle = skill.Data.RotationAngle < 0 ? LookAngle : skill.Data.RotationAngle,
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
    private void SkillFireAudioSource() {
        AudioController.Instance.SFXPlay(SkillList.Current.Data.Name switch {
            "BasicLaserBeam" => SFX.Range_Laser_Fire,
            "BasicRapidFire" => SFX.Range_Rapid_Fire,
            "BasicShotGun" => SFX.Range_ShotGun_Fire,
            "BasicSlash" => SFX.Melee_Slash_Fire,
            "BasicSting" => SFX.Melee_Sting_Fire,
            "BasicSmash" => SFX.Melee_Smash_Fire,
            _ => SFX.Range_Rapid_Fire,
        });
    }

    public bool IsTarget(Creature creature) => true;
}