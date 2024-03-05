using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BossHpBar : UI_BarBase {

    #region Initialize / Set

    public void SetInfo(Enemy enemy) {
        ShowTitleText = true;
        SetTitle($"{enemy.Data.Key}");

        enemy.OnChangedHp += x => SetAmount(x, enemy.HpMax);
        SetAmount(enemy.Hp, enemy.HpMax);
    }

    #endregion

}