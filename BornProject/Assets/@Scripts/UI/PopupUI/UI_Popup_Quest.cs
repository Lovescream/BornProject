using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Quest : UI_Popup {

    #region Enums

    enum Texts {
        Text,
    }

    #endregion

    #region Properties

    public override bool IsPause => false;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindText(typeof(Texts));

        return true;
    }

    public void SetInfo() {
        GetText((int)Texts.Text).text = $"던전 클리어 {Main.Quest.ClearStageCount} / {QuestManager.RequiredClearCount}";
        GetText((int)Texts.Text).text = $"보스 잡기 0/1"; // TODO
    }

    #endregion

}