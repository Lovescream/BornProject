using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList {

    #region Properties

    public Creature Owner { get; protected set; }

    public Skill_Basic BasicRange {
        get => _basicRange;
        set {
            if (value == _basicRange) return;
            _basicRange = value;
            OnChangedBasicRange?.Invoke(value);
        }
    }
    public Skill_Basic BasicMelee {
        get => _basicMelee;
        set {
            if (value == _basicMelee) return;
            _basicMelee = value;
            OnChangedBasicMelee?.Invoke(value);
        }
    }

    #endregion

    #region Fields

    private Skill_Basic _basicRange;
    private Skill_Basic _basicMelee;

    // Events.
    public event Action<Skill_Basic> OnChangedBasicRange;
    public event Action<Skill_Basic> OnChangedBasicMelee;

    #endregion

    #region Constructor

    public SkillList(Creature owner) {
        Owner = owner;
        OnChangedBasicRange += OnChangedBasicSkill;
        OnChangedBasicMelee += OnChangedBasicSkill;
    }

    #endregion

    private void OnChangedBasicSkill(Skill_Basic basicSkill) {
        Owner.Status[StatType.Damage].SetValue(basicSkill.Damage);
        Owner.Status[StatType.AttackSpeed].SetValue(basicSkill.AttackSpeed);
        Owner.Status[StatType.Range].SetValue(basicSkill.Range);
    }
}