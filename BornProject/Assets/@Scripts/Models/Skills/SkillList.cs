using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList {

    #region Properties

    public Creature Owner { get; protected set; }

    public RangerSkillData BasicRange {
        get => _basicRange;
        set {
            if (value == _basicRange) return;
            _basicRange = value;
            OnChangedBasicRange?.Invoke(value);
        }
    }
    public MeleeSkillData BasicMelee {
        get => _basicMelee;
        set {
            if (value == _basicMelee) return;
            _basicMelee = value;
            OnChangedBasicMelee?.Invoke(value);
        }
    }

    #endregion

    #region Fields

    private RangerSkillData _basicRange;
    private MeleeSkillData _basicMelee;

    // Events.
    public event Action<RangerSkillData> OnChangedBasicRange;
    public event Action<MeleeSkillData> OnChangedBasicMelee;

    #endregion

    #region Constructor

    public SkillList(Creature owner) {
        Owner = owner;
        OnChangedBasicRange += OnChangedRangerBasicSkill;
        OnChangedBasicMelee += OnChangedMeleeBasicSkill;
    }

    #endregion

    private void OnChangedRangerBasicSkill(RangerSkillData basicSkill) {
        Owner.Status[StatType.Damage].SetValue(basicSkill.Damage);
        Owner.Status[StatType.AttackSpeed].SetValue(basicSkill.AttackSpeed);
        Owner.Status[StatType.Range].SetValue(basicSkill.Range);
    }

    private void OnChangedMeleeBasicSkill(MeleeSkillData basicSkill)
    {
        Owner.Status[StatType.Damage].SetValue(basicSkill.Damage);
        Owner.Status[StatType.AttackSpeed].SetValue(basicSkill.AttackSpeed);
        Owner.Status[StatType.Range].SetValue(basicSkill.Range);
    }
}