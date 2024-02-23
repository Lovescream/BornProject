using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillList {

    #region Properties

    public ISkillMan Owner { get; protected set; }

    public Skill Current {
        get => _current;
        set {
            if (value == _current) return;
            _current = value;
            OnChangedSkill?.Invoke(value);
        }
    }
    public SkillSet CurrentSkillSet {
        get => _currentSkillSet;
        set => _currentSkillSet = value;
    }

    #endregion

    #region Fields

    private Skill _current;
    private SkillSet _currentSkillSet;

    // Collections.
    private Dictionary<SkillType, SkillSet> _currentSkillSets;
    private List<SkillSet> _skillSets = new();

    // Events.
    public event Action<Skill> OnChangedSkill;

    #endregion

    #region Indexer

    public SkillSet this[SkillType type] => _currentSkillSets[type];

    #endregion

    #region Constructor

    public SkillList(ISkillMan owner, string defaultSkills = "") {
        // #1. 기본 데이터 설정 및 컬렉션 초기화.
        Owner = owner;
        _currentSkillSets = new();
        for (int i = 0; i < Enum.GetNames(typeof(SkillType)).Length; i++) {
            _currentSkillSets[(SkillType)i] = null;
        }

        // #2. Owner가 가진 SkillSet 생성.
        _skillSets = Owner.SkillSetList.Split('|').Select(x => new SkillSet(x)).ToList();

        // #3. 콜백 등록.
        OnChangedSkill += UpdateStatus;

        // #4. 기본으로 가지고 있는 스킬 설정.
        if (!string.IsNullOrEmpty(defaultSkills)) {
            foreach (string s in defaultSkills.Split('|')) {
                Set(Set(s).Type);
            }
        }
    }

    #endregion

    // 해당 스킬 타입의 트리와 스킬을 설정.
    public Skill Set(string key) {
        // #1. 해당 스킬 정보가 있는지 검사.
        if (!Main.Data.Skills.TryGetValue(key, out SkillData data)) {
            Debug.LogError($"Not found skill: {key}");
            return null;
        }

        // #2. 해당 스킬 타입의 트리와 스킬을 설정.
        Skill skill = _skillSets.Where(x => x.Type == data.Type).ToList().Where(x => x.BaseName.Equals(key.Split('_')[0])).FirstOrDefault().Set(key);
        if (skill != null)
            _currentSkillSets[skill.Type] = skill.SkillSet;

        //_skillSets.Where(x => x.Type == data.Type).ToList().ForEach(x => {
        //    Skill skill = x.Set(key);
        //    if (skill != null) {
        //        _currentSkillSets[skill.Type] = skill.SkillSet;
        //        if (skill.Data.Key.Equals(key)) thisSkill = skill;
        //    }
        //});
        return skill;
    }

    // 현재 스킬 트리를 변경.
    public void Set(SkillType type) {
        CurrentSkillSet = _currentSkillSets[type];
        if (CurrentSkillSet != null) Current = CurrentSkillSet.Current;
    }

    // 
    public List<Skill> GetBaseSkills(SkillType type) => _skillSets.Where(x => x.Type == type).Select(x => x.GetSubs(null)[0]).ToList();

    private void UpdateStatus(Skill skill) {
        if (skill != null) {
            Owner.Status[StatType.Damage].SetValue(skill.Data.Damage);
            Owner.Status[StatType.AttackSpeed].SetValue(skill.Data.AttackSpeed);
            Owner.Status[StatType.Range].SetValue(skill.Data.Range);
        }
        else {
            Owner.Status[StatType.Damage].SetValue(Owner.DefaultStatus.Damage.Value);
            Owner.Status[StatType.AttackSpeed].SetValue(Owner.DefaultStatus.AttackSpeed.Value);
            Owner.Status[StatType.Range].SetValue(Owner.DefaultStatus.Range.Value);
        }
    }

}