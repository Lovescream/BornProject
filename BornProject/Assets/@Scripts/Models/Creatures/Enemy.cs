using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature {

    //public new EnemyData Data => base.Data as EnemyData; // 기존.
    public new EnemyData Data; // 수정 : 확인 필요.
    public float Exp => Data.Exp;

    protected override void SetStateEvent()
    {
        base.SetStateEvent();
        State.AddOnStay(CreatureState.Idle, () =>
        {
            Velocity = Vector2.zero;
        });
    }
    protected override void SetStatus(bool isFullHp = true)
    {
        this.Status = new(Data);
        if (isFullHp)
        {
            Hp = Status[StatType.HpMax].Value;
        }

        //OnChangedHp -= ShowHpBar; // Hp바 만들때 사용예정.
        //OnChangedHp += ShowHpBar;
    }
}