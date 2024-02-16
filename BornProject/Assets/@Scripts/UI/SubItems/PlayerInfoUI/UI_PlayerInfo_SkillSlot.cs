using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo_SkillSlot : UI_SlotBase {

    #region Enums



    #endregion

    #region Initialize / Set

    public void SetInfo(SkillBase skill) {
        SetImage(skill.Icon);
    }

    #endregion

}