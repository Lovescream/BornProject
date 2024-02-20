using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager {

    public Dictionary<string, SkillData> Data => Main.Data.Skills;
    public List<SkillData> Skills => Data.Values.ToList();
    public SkillData BaseRange => _rangeSkills.Count == 0 ? null : _rangeSkills[0];
    public SkillData BaseMelee => _meleeSkills.Count == 0 ? null : _meleeSkills[0];
    public SkillData RangeSkill => _rangeSkills.Count == 0 ? null : _rangeSkills[^1];
    public SkillData MeleeSkill => _meleeSkills.Count == 0 ? null : _meleeSkills[^1];

    private List<SkillData> _rangeSkills = new();
    private List<SkillData> _meleeSkills = new();

    public event Action<SkillData> OnGetSkill;

    public void Initialize() {
        OnGetSkill += s => GameObject.FindObjectOfType<Player>().SkillList.SetSkill(s.Type == SkillType.Range ? RangeSkill : MeleeSkill);
    }

    public void GetSkill(SkillData skill) {
        List<SkillData> list = skill.Type == SkillType.Range ? _rangeSkills : _meleeSkills;
        string baseName = GetSkillBaseName(skill);
        string advancedName = GetSkillAdvancedName(skill);
        SkillData baseSkill = GetBaseSkill(baseName);

        if (list.Count == 0) {
            list.Add(baseSkill);
            if (skill.Level == SkillLevel.Normal) list.Add(skill);
            else if (skill.Level == SkillLevel.Rare) {
                list.Add(GetNormalSkill(baseName, advancedName));
                list.Add(skill);
            }
            OnGetSkill?.Invoke(skill);
        }
        else {
            if (GetSkillBaseName(list[0]) != baseName) {
                list.Clear();
                list.Add(baseSkill);
            }
            if (skill.Level == SkillLevel.Base) {
                OnGetSkill?.Invoke(skill);
                return;
            }
            if (list.Count > 1) {
                if (GetSkillAdvancedName(list[1]) == advancedName) {
                    OnGetSkill?.Invoke(skill);
                    return;
                }
                for (int i = list.Count - 1; i >= 1; i--) list.RemoveAt(i);
            }
            if (skill.Level == SkillLevel.Rare) list.Add(GetNormalSkill(baseName, advancedName));
            list.Add(skill);
        }

        
    }

    public string GetSkillBaseName(SkillData skill) {
        return skill.Key.Split('_')[0];
    }
    public string GetSkillAdvancedName(SkillData skill) {
        return skill.Key.Split('_')[2];
    }

    public SkillData GetBaseSkill(string skillName) {
        return Skills.Where(x => x.Key.Split('_')[0].Equals(skillName)).Where(x => x.Level == SkillLevel.Base).FirstOrDefault();
    }
    public SkillData GetNormalSkill(string skillName, string advancedName) {
        return Skills.Where(x => x.Key.Split('_')[0].Equals(skillName)).Where(x => x.Key.Split('_')[2].Equals(advancedName)).Where(x => x.Level == SkillLevel.Normal).FirstOrDefault();
    }
}