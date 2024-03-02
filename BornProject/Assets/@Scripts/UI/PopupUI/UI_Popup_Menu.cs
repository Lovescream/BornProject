using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Menu : UI_Popup {

    #region Enums

    enum Buttons {
        btnInfo,
        btnResume,
        btnExit,
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.btnInfo).onClick.AddListener(OnBtnInfo);
        GetButton((int)Buttons.btnResume).onClick.AddListener(OnBtnResume);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);

        return true;
    }

    #endregion

    #region OnButtons

    private void OnBtnInfo() {
        Main.Audio.PlayOnButton();
        // 정보 창 띄우기.
    }
    private void OnBtnResume() {
        Main.Audio.PlayOnButton();
        Close();
    }
    private void OnBtnExit() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_ExitGame>();
    }

    #endregion

}