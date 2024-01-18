using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureData : Data {

    public float HpMax { get; set; }
    public float HpRegen { get; set; } // 1초당 생명력 회복 수치입니다.
    public float Damage { get; set; }
    public float Defense { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackSpeed { get; set; }
    public int Cost { get; set; } // 크리쳐가 장착할 수 있는 최대 아이템 Cost 수치입니다.
    public float Range { get; set; }
    public float Sight { get; set; }

}