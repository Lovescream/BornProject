using Dijkstra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        SkillList = new(this) {
            BasicRange = Main.Data.RangerSkills["BasicBeam"],
            BasicMelee = Main.Data.MeleeSkills["PowerSlash1"],
        };
    }
    protected override void SetState() {
        base.SetState();

        this.Attacker = new(this);
        this.Attacker.OnStartAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, true);
        };
        this.Attacker.OnEndAttack += () => {
            _animator.SetBool(AnimatorParameterHash_Attack, false);
        };
    }

    #endregion

    #region Input

    protected void OnMove(InputValue value) {
        Velocity = value.Get<Vector2>().normalized * Status[StatType.MoveSpeed].Value;
    }
    protected void OnLook(InputValue value) {
        Vector2 v = Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position;
        LookDirection = v.normalized;
    }
    protected void OnAttackMain() {
        _isRangeAttack = true;
        Attack();
    }
    protected void OnAttackSub() {
        _isRangeAttack = false;
        Attack();
    }

    #endregion

    public void Attack() {
        if (_isRangeAttack){
            Attacker.Attack(GetRangerHitColliderGenerationInfo(), GetRangerHitColliderInfo(), GetRangerHitInfo());
            Debug.Log("데미지 :"+ Damage + " 원거리 수치입니다.");
        }
        else if(!_isRangeAttack) {
            Attacker.Attack(GetMeleeHitColliderGenerationInfo(), GetMeleeHitColliderInfo(), GetMeleeHitInfo());
            Debug.Log("데미지 :" + Damage + " 근접 수치입니다.");
        }        
    }

    public HitColliderGenerationInfo GetRangerHitColliderGenerationInfo() {        
        RangerSkillData basicSkill = SkillList.BasicRange;      
        return new()
        {
            Owner = this,
            HitColliderKey = basicSkill.HitColliderKey,
            RadiusOffset = basicSkill.RadiusOffset,
            RotationAngle = basicSkill.RotationAngle < 0 ? LookAngle : basicSkill.RotationAngle,
            Count = basicSkill.HitColliderCount,
            SpreadAngle = basicSkill.HitColliderAngle,
            Size = basicSkill.HitColliderSize,
        };
    }
    public HitColliderGenerationInfo GetMeleeHitColliderGenerationInfo() {
        MeleeSkillData basicSkill = SkillList.BasicMelee;
        return new()
        {
            Owner = this,
            HitColliderKey = basicSkill.HitColliderKey,
            RadiusOffset = basicSkill.RadiusOffset,
            RotationAngle = basicSkill.RotationAngle < 0 ? LookAngle : basicSkill.RotationAngle,
            Count = basicSkill.HitColliderCount,
            SpreadAngle = basicSkill.HitColliderAngle,
            Size = basicSkill.HitColliderSize,
        };
    }
    public HitColliderInfo GetRangerHitColliderInfo() {
        RangerSkillData basicSkill = SkillList.BasicRange;
        return new() {
            Penetration = basicSkill.Penetration,
            Speed = basicSkill.Speed,
            DirectionX = basicSkill.DirectionX,
            DirectionY = basicSkill.DirectionY,
            Duration = basicSkill.Duration,
            Range = basicSkill.Range,
        };
    }
    public HitColliderInfo GetMeleeHitColliderInfo() {
        MeleeSkillData basicSkill = SkillList.BasicMelee;
        return new()
        {
            Penetration = basicSkill.Penetration,
            Speed = basicSkill.Speed,
            DirectionX = basicSkill.DirectionX,
            DirectionY = basicSkill.DirectionY,
            Duration = basicSkill.Duration,
            Range = basicSkill.Range,
        };
    }
    public HitInfo GetRangerHitInfo() {
        RangerSkillData basicSkill = SkillList.BasicRange;
        return new() {
            Owner = this,
            Damage = this.Damage,
            CriticalChance = basicSkill.CriticalChance,
            CriticalBonus = basicSkill.CriticalBonus,
            Knockback = new() {
                time = 0.1f,
                speed = 10f,
            }
        };
    }
    public HitInfo GetMeleeHitInfo(){
        MeleeSkillData basicSkill = SkillList.BasicMelee;
        return new()
        {
            Owner = this,
            Damage = this.Damage,            
            CriticalChance = basicSkill.CriticalChance,
            CriticalBonus = basicSkill.CriticalBonus,
            Knockback = new()
            {
                time = 0.1f,
                speed = 10f,
            }
        };
    }
}
    //public AttackInfo GetAttackInfo() {
    //    Skill_Basic basicSkill = _isRangeAttack_Temp ? SkillList.BasicRange : SkillList.BasicMelee;

    //    HitColliderGenerationInfo generationInfo = new() {
    //        Owner = this,
    //        HitColliderKey = basicSkill.Name,
    //        Offset = basicSkill.Offset,
    //        RotationAngle = basicSkill.RotationAngle < 0 ? LookAngle : basicSkill.RotationAngle,
    //        Count = basicSkill.HitColliderCount,
    //        SpreadAngle = basicSkill.HitColliderAngle,
    //        Size = basicSkill.HitColliderSize,
    //    };
    //    HitColliderInfo hitColliderInfo = new() {
    //        Penetration = basicSkill.Penetration,
    //        Speed = basicSkill.Speed,
    //        Direction = basicSkill.Direction.magnitude <= float.Epsilon ? LookDirection : basicSkill.Direction,
    //        Duration = basicSkill.Duration,
    //        Range = basicSkill.Range,
    //    };
    //    HitInfo hitInfo = new() {
    //        Owner = this,
    //        Damage = this.Damage,
    //        CriticalChance = basicSkill.CriticalChance,
    //        CriticalBonus = basicSkill.CriticalBonus,
    //        Knockback = new() {
    //            time = 0.1f,
    //            speed = 10f,
    //        }
    //    };











    //    return new() {
    //        Owner = this,
    //        HitColliderKey = basicSkill.Name,
    //        Offset = basicSkill.Offset,
    //        RotationAngle = basicSkill.RotationAngle < 0 ? this.LookAngle : basicSkill.RotationAngle,
    //        Damage = this.Damage,
    //        CriticalChance = basicSkill.CriticalChance,
    //        CriticalBonus = basicSkill.CriticalBonus,
    //        Penetrate = basicSkill.Penetration,
    //        Speed = basicSkill.Speed,
    //        Direction = basicSkill.Direction.magnitude <= float.Epsilon ? LookDirection : basicSkill.Direction,
    //        Duration = basicSkill.Duration,
    //        Knockback = new() {
    //            time = 0.1f,
    //            speed = 10,
    //        }
    //    };
    //    //return new() {
    //    //    Owner = this,
    //    //    HitColliderKey = _isRangeAttack_Temp ? "BasicProjectile" : "BasicMelee",
    //    //    Damage = this.Damage,
    //    //    CriticalChance = 0,
    //    //    CriticalBonus = 1.5f,
    //    //    Penetrate = 1,
    //    //    Speed = 10,
    //    //    Direction = LookDirection,
    //    //    Duration = 5,
    //    //    Knockback = new() {
    //    //        time = 0.1f,
    //    //        speed = 10,
    //    //    }
    //    //};
    //}
//}