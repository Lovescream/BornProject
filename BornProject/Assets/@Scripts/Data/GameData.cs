using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {

    public bool FirstPlay { get; set; }

    private Dictionary<SkillType, string> _skills = new();

    public string this[SkillType type] {
        get {
            if (!_skills.TryGetValue(type, out string skillKey)) return "";
            return skillKey;
        }
        set {
            _skills[type] = value;
        }
    }

    public GameData() {
        FirstPlay = true;
    }

}