using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo_ImageSlot : UI_SlotBase {

    #region Enums

    enum Texts {
        txtSlotImage,
    }

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindText(typeof(Texts));

        return true;
    }

    public void SetInfo(Player player) {
        //SetImage(sprite);
        //GetText((int)Texts.txtSlotImage).gameObject.SetActive(sprite != null);
    }

}