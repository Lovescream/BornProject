using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo_SkillSlot : UI_SlotBase {

    #region Enums



    #endregion

    #region Initialize / Set

    public void SetInfo(SkillData skill) {
        Sprite sprite = Main.Resource.LoadSprite($"Icon_{skill.Key}");
        SetImage(sprite);
    }

    #endregion

}