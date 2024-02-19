using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList {

    #region Properties

    public Creature Owner { get; protected set; }

    public SkillData CurrentBasicSkill {
        get => _currentBasicSkill;
        set {
            if (value == _currentBasicSkill) return;
            _currentBasicSkill = value;
            Debug.Log("현재 스킬의 이름" + CurrentBasicSkill.Name);
            Debug.Log("현재 SkillData의 List 수" + _basicSkills.Count);
            OnChangedBasicSkill?.Invoke(value);
        }
    }

    #endregion

    #region Fields

    private int _currentBasicSkillIndex;
    private SkillData _currentBasicSkill;

    // Collections.
    private List<SkillData> _basicSkills;

    // Events.
    public event Action<SkillData> OnChangedBasicSkill;

    #endregion

    #region Constructor

    public SkillList(Creature owner) {
        Owner = owner;
        OnChangedBasicSkill += UpdateStatus;

        _basicSkills = new();
    }

    #endregion

    #region Add/Remove Basic Skill

    public void AddBasicSkill(SkillData skillData) {
        _basicSkills.Add(skillData);
        if (_basicSkills.Count != 1) return;
        _currentBasicSkillIndex = 0;
        CurrentBasicSkill = _basicSkills[0];
    }
    public void RemoveBasicSkill(SkillData skillData) {
        if (!_basicSkills.Contains(skillData)) return;
        int index = _basicSkills.IndexOf(skillData);
        if (_currentBasicSkillIndex >= index) _currentBasicSkillIndex--;
        _basicSkills.Remove(skillData);
        CurrentBasicSkill = _basicSkills[_currentBasicSkillIndex];
    }

    #endregion

    #region Change Basic Skill

    public void NextBasicSkill() {
        if (_basicSkills.Count <= 0) return;
        if (++_currentBasicSkillIndex > _basicSkills.Count) _currentBasicSkillIndex = 0;

        CurrentBasicSkill = _basicSkills[_currentBasicSkillIndex];
    }
    public void PrevBasicSkill() {
        if (_basicSkills.Count <= 0) return;
        if (--_currentBasicSkillIndex < 0) _currentBasicSkillIndex = _basicSkills.Count - 1;

        CurrentBasicSkill = _basicSkills[_currentBasicSkillIndex];
    }
    public void ChangeBasicSkill(int index) {
        if (index >= _basicSkills.Count) return;
        _currentBasicSkillIndex = index;

        CurrentBasicSkill = _basicSkills[_currentBasicSkillIndex];
    }

    #endregion

    private void UpdateStatus(SkillData skillData) {
        if (skillData != null) {
            Owner.Status[StatType.Damage].SetValue(skillData.Damage);
            Owner.Status[StatType.AttackSpeed].SetValue(skillData.AttackSpeed);
            Owner.Status[StatType.Range].SetValue(skillData.Range);
        }
        else {
            Owner.Status[StatType.Damage].SetValue(Owner.Data.Damage);
            Owner.Status[StatType.AttackSpeed].SetValue(Owner.Data.AttackSpeed);
            Owner.Status[StatType.Range].SetValue(Owner.Data.Range);
        }
    }
}