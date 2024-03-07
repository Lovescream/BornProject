using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo_SkillSlot : UI_SlotBase {

    #region Enums



    #endregion

    #region Initialize / Set

    public void SetInfo(Skill skill) {
        //Sprite sprite = Main.Resource.LoadSprite($"Icon_{skill.Data.Key}");
        Sprite sprite = Main.Resource.Get<Sprite>($"Icon_{skill.Data.Key}");
        SetImage(sprite);
    }

    #endregion

}