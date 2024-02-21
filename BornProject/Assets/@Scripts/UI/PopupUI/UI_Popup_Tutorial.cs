using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Tutorial : UI_Popup
{
    #region Enums

    enum Buttons {
        btnBackground
    }
    
    #endregion

    #region Properties

    public override bool IsPause => true;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.btnBackground).onClick.AddListener(OnBtnBackground);

        return true;
    }

    private void OnBtnBackground()
    {
        Main.UI.Clear();
    }

    #endregion
}