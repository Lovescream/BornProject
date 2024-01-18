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
    //Cost,
    //Range,
    //Sight,
    COUNT // StatType 갯수 파악을 위한 상수.
}

public enum StatModifierType
{
    Add,
    Multiple,
    Override,
}


public class Status
{
    private Dictionary<StatType, Stat> _stats;

    public Stat this[StatType type]
    {
        get => _stats[type];
    }

    // 스탯 타입을 확인하고 딕셔너리에 추가.
    public Status()
    {
        _stats = new();
        for (int i = 0; i < (int)StatType.COUNT; i++)
        {
            _stats.Add((StatType)i, new Stat((StatType)i));
        }
    }

    // 크리처의 기존 데이터를 받아와서 스탯에 대입.
    public Status(CreatureData data)
    {
        _stats = new()
        {
            [StatType.HpMax] = new(StatType.HpMax, data.HpMax),
            [StatType.HpRegen] = new(StatType.HpRegen, data.HpRegen),
            [StatType.Damage] = new(StatType.Damage, data.Damage),
            [StatType.Defense] = new(StatType.Defense, data.Defense),
            [StatType.MoveSpeed] = new(StatType.MoveSpeed, data.MoveSpeed),
            [StatType.AttackSpeed] = new(StatType.AttackSpeed, data.AttackSpeed),
        };
    }

    // 변화될 스탯을 리스트에서 찾아 스탯에 변화를 주는 함수를 추가 / 제거.
    public void AddModifiers(List<StatModifier> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            this[modifiers[i].Stat].AddModifier(modifiers[i]);
        }
    }
    public void RemoveModifiers(List<StatModifier> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            this[modifiers[i].Stat].RemoveModifier(modifiers[i]);
        }
    }
}