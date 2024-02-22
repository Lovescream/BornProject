using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier
{
    public StatType Stat { get; set; }
    public StatModifierType Type { get; set; }
    public float Value { get; set; }

    // 기본 생성자.
    public StatModifier() { }

    // Modifier 값을 받아온다.
    public StatModifier(StatType stat, StatModifierType type, float value)
    {
        Stat = stat;
        Type = type;
        Value = value;
    }

    // json 파일의 Modifier를 나눠서 Enum으로 변환한다. 
    public StatModifier(string s)
    {
        string[] strings = s.Split('_');
        Stat = (StatType)Enum.Parse(typeof(StatType), strings[0]);
        Type = (StatModifierType)Enum.Parse(typeof(StatModifierType), strings[1]);
        Value = float.Parse(strings[2]);
    }

    // 받은 값들을 깊은복사한다.
    public StatModifier Copy()
    {
        return new(Stat, Type, Value);
    }
}
