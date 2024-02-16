using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo : UI_Base {

    #region Enum

    enum Objects {
        PlayerImageSlot,
        SkillSlot_Main,
        SkillSlot_Sub,
        PlayerMemoryBar,
        PlayerHpBar,
    }
    enum Texts {
        txtSkillPoint,
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindObject(typeof(Objects));
        BindText(typeof(Texts));

        return true;
    }

    public void SetInfo(Player player) {
        Initialize();

        GetObject((int)Objects.PlayerImageSlot).GetComponent<UI_PlayerInfo_ImageSlot>().SetInfo(player);
        //GetObject((int)Objects.SkillSlot_Main).GetComponent<>().SetInfo(player); // TODO::
        //GetObject((int)Objects.SkillSlot_Sub).GetComponent<>().SetInfo(player); // TODO::
        GetObject((int)Objects.PlayerMemoryBar).GetComponent<UI_PlayerInfo_MemoryBar>().SetInfo(player);
        GetObject((int)Objects.PlayerHpBar).GetComponent<UI_PlayerInfo_HpBar>().SetInfo(player);
    }

    #endregion

}