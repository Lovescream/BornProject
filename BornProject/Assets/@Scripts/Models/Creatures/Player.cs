using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Player : Creature, IAttackable {

    #region Properties

    public Attacker Attacker { get; protected set; }
    public SkillList SkillList { get; protected set; }

    #endregion

    #region Fields

    protected static readonly int AnimatorParameterHash_Attack = Animator.StringToHash("Attack");

    private bool _isRangeAttack = false;

    #endregion

    #region MonoBehaviours

    protected override void FixedUpdate() {
        base.FixedUpdate();

        Attacker.OnUpdate(); 
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null) camera.SetTarget(this.transform);

        return true;
    }
    public override void SetInfo(CreatureData data) {
        base.SetInfo(data);

        SkillList = new(this);
        if (Main.Skill.RangeSkill != null) SkillList.SetSkill(Main.Skill.RangeSkill);
        if (Main.Skill.MeleeSkill != null) SkillList.SetSkill(Main.Skill.MeleeSkill);
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
        Main.Skill.Clear();
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
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //SkillList.ChangeBasicSkill(0);
        SkillList.SetRangeSkill();
        Attack();
    }
    protected void OnAttackSub() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //SkillList.ChangeBasicSkill(1);
        SkillList.SetMeleeSkill();
        Attack();
    }

    #endregion

    public override void OnHit(IHitCollider attacker)
    {        
        base.OnHit(attacker);
        AudioController.Instance.SFXPlay(SFX.PlayerHit);
    }
    public void Attack() {
        if (this.IsDead) return;
        if (SkillList.CurrentBasicSkill == null) return;
        Attacker.Attack(GetHitColliderGenerationInfo(), GetHitColliderInfo(), GetHitInfo());
    }

    

    public HitColliderGenerationInfo GetHitColliderGenerationInfo() {        
        SkillData skillData = SkillList.CurrentBasicSkill;      
        return new()
        {
            Owner = this,
            HitColliderKey = skillData.HitColliderKey,
            RadiusOffset = skillData.RadiusOffset,
            RotationAngle = skillData.RotationAngle < 0 ? LookAngle : skillData.RotationAngle,
            Count = skillData.HitColliderCount,
            SpreadAngle = skillData.HitColliderAngle,
            Size = skillData.HitColliderSize,
            AttackTime = skillData.AttackTime,
        };
    }
    public HitColliderInfo GetHitColliderInfo() {
        SkillData skillData = SkillList.CurrentBasicSkill;
        return new() {
            Penetration = skillData.Penetration,
            Speed = skillData.Speed,
            DirectionX = skillData.DirectionX,
            DirectionY = skillData.DirectionY,
            Duration = skillData.Duration,
            Range = skillData.Range,
        };
    }
    public HitInfo GetHitInfo() {
        SkillData skillData = SkillList.CurrentBasicSkill;
        return new() {
            Owner = this,
            Damage = this.Damage,
            CriticalChance = skillData.CriticalChance,
            CriticalBonus = skillData.CriticalBonus,
            Knockback = new() {
                time = 0.1f,
                speed = 10f,
            }
        };
    }
    private void SkillFireAudioSource() {
        AudioController.Instance.SFXPlay(SkillList.CurrentBasicSkill.Name switch {
            "BasicLaserBeam" => SFX.Range_Laser_Fire,
            "BasicRapidFire" => SFX.Range_Rapid_Fire,
            "BasicShotGun" => SFX.Range_ShotGun_Fire,
            "BasicSlash" => SFX.Melee_Slash_Fire,
            "BasicSting" => SFX.Melee_Sting_Fire,
            "BasicSmash" => SFX.Melee_Smash_Fire,
            _ => SFX.Range_Rapid_Fire,
        });
    }
}