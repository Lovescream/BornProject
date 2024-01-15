using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureData : Data {

    // ������Ƽ set �κ��� '����ȭ ����'�� ���ؼ� ���������ڸ� ���� �ʱ�� �Ͽ���.
    public float HpMax { get; set; }
    public float HpRegen { get; set; } // 1�ʴ� ����� ȸ�� ��ġ�Դϴ�.
    public float Damage { get; set; }
    public float Defense { get; set; }
    public float MoveSpeed {  get; set; }
    public float AttackSpeed { get; set;}
    public int Cost { get; set; } // ũ���İ� ������ �� �ִ� �ִ� ������ Cost ��ġ�Դϴ�.
    public float Range {  get; set; }
    public float Sight { get; set; }


}