using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_ExitGame : UI_Popup {

    #region Enums

    enum Buttons {
        btnConfirm,
        btnCancel,
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.btnConfirm).onClick.AddListener(OnBtnConfirm);
        GetButton((int)Buttons.btnCancel).onClick.AddListener(OnBtnCancel);

        return true;
    }

    #endregion

    #region OnButtons

    private void OnBtnConfirm() {
        Application.Quit();
    }
    private void OnBtnCancel() {
        Close();
    }

    #endregion

}