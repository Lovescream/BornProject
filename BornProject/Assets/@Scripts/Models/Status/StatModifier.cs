using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    HpMax,
    HpRegen,
    Damage,
    Defense,
    MoveSpeed,
    AttackSpeed,
    Cost,
    Range,
    Sight,
}

public enum StatModifierType
{
    Add,
    Multiple,
    Override,
}

public class StatModifier
{
    public StatType Stat { get; set; }
    public StatModifierType Type { get; set; }
    public float Value { get; set; }

    // �⺻ ������.
    public StatModifier() { }

    // Modifier ���� �޾ƿ´�.
    public StatModifier(StatType stat, StatModifierType type, float value)
    {
        Stat = stat;
        Type = type;
        Value = value;
    }

    // json ������ Modifier�� �߶� Enum���� ��ȯ�Ѵ�. 
    public StatModifier(string s)
    {
        string[] strings = s.Split('_');
        Stat = (StatType)Enum.Parse(typeof(StatType), strings[0]);
        Type = (StatModifierType)Enum.Parse(typeof(StatModifierType), strings[1]);
        Value = float.Parse(strings[2]);
    }

    // ���� ������ ���������Ѵ�.
    public StatModifier Copy()
    {
        return new(Stat, Type, Value);
    }
}
