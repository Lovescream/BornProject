using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Quest : UI_Popup {

    #region Enums

    enum Objects {
        QuestInfo,
    }

    #endregion

    #region Properties

    public override bool IsPause => true;

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

      
        return true;
    }

    #endregion
}
