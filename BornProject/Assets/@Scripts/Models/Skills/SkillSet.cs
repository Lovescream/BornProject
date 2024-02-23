using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillSet {

    #region Properties

    public string BaseName { get; protected set; }                  // 스킬셋의 가장 첫 번째 스킬.
    public SkillType Type { get; protected set; }                   // 스킬셋의 스킬 타입.
    public Skill Current { get; protected set; }                    // 현재 선택된 스킬.

    #endregion

    #region Fields

    private Dictionary<SkillLevel, Skill> _bases = new();           // 현재 선택된 스킬의 스킬트리.
    private Dictionary<SkillLevel, List<Skill>> _skills = new();    // 스킬셋의 모든 스킬들.

    #endregion

    #region Indexer

    public Skill this[SkillLevel level] => _bases[level];

    #endregion

    #region Constructor

    public SkillSet(string baseName) {
        Debug.Log($"[SkillList: {baseName}] Create new SkillSet.");
        // #1. 데이터 설정 및 컬렉션 초기화.
        this.BaseName = baseName;
        int count = Enum.GetNames(typeof(SkillLevel)).Length;
        for (int i = 0; i < count; i++) {
            _bases[(SkillLevel)i] = null;
            _skills[(SkillLevel)i] = new();
        }

        // #2. 스킬셋 생성: BaseName으로 시작하는 모든 스킬들
        List<SkillData> skills = Main.Data.Skills.Values.Where(x => x.Key.StartsWith(BaseName)).OrderBy(x => x.Level).ToList();
        if (skills.Count == 0) return;
        Type = skills[0].Type;
        skills.ForEach(x => {
            // #2-1. SkillData를 통해 Skill을 생성하여, 레벨을 구분해 리스트에 넣는다.
            Skill newSkill = new(x);
            _skills[x.Level].Add(newSkill);
            newSkill.SkillSet = this;
            Debug.Log($"[SkillList: {baseName}] Add new skill({newSkill.Data.Key}) to SkillSet.");

            // #2-2. 해당 Skill의 상위 스킬을 설정.
            int levelIndex = (int)newSkill.Level;
            if (levelIndex > 0) {
                var list = _skills[(SkillLevel)(levelIndex - 1)].Where(x => x.BaseKey == newSkill.BaseKey);
                if (list.Count() > 1) list = list.Where(x => x.AdvancedKey == newSkill.AdvancedKey);
                Skill parentSkill = list.FirstOrDefault();
                if (parentSkill == null) Debug.LogError($"Not found parent skill: {newSkill.Data.Key}");
                else newSkill.Parent = parentSkill;

                if (parentSkill != null) Debug.Log($"[SkillList: {baseName}] Set Skill({parentSkill.Data.Key}) to the Parent of This Skill({newSkill.Data.Key}).");
            }
        });
    }

    #endregion

    // 이 SkillSet에 속한 스킬 중 해당 key를 가진 스킬을 선택.
    public Skill Set(string key) {
        // #1. 해당 key를 가진 스킬이 이 SkillSet에 속한 스킬인지 검사.
        if (!Main.Data.Skills.TryGetValue(key, out SkillData data)) {
            Debug.LogError($"Not found skill: {key}");
            Current = null;
            return Current;
        }
        if (!_skills.TryGetValue(data.Level, out List<Skill> skills)) {
            Debug.LogError($"Not found skill: {key}");
            Current = null;
            return Current;
        }

        // #2. 현재 선택된 스킬 설정.
        Current = skills.Where(x => x.Data.Key.Equals(key)).FirstOrDefault();
        if (Current == null) return null;

        // #3. 현재 선택된 스킬의 스킬트리 설정.
        Skill skill = Current;
        for (int i = 0; i <= (int)Current.Level; i++) {
            _bases[(SkillLevel)i] = skill;
            skill = Current.Parent;
        }

        return Current;
    }

    // 해당 Skill의 하위 스킬 리스트 리턴.
    public List<Skill> GetSubs(Skill skill = null) {
        if (skill == null) return new() { _skills[SkillLevel.Base][0] };
        List<Skill> list = new();
        foreach (List<Skill> skills in _skills.Values) {
            skills.ForEach(x => {
                if (x.Parent != null && x.Parent.Equals(skill)) list.Add(x);
            });
        }
        return list;
    }
}
