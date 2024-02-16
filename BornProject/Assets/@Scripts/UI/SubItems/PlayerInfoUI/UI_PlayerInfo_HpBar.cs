using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo_HpBar : UI_BarBase {

    #region Initialize / Set

    public void SetInfo(Player player) {
        ShowValueText = true;
        ShowValueTextFull = true;

        player.OnChangedHp += x => SetAmount(x, player.HpMax);
        SetAmount(player.Hp, player.HpMax);
    }

    #endregion

}