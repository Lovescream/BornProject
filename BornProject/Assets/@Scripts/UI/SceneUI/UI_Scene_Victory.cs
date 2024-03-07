using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_Scene_Victory : UI_Scene {

    #region Enums 

    enum Images {

        imageVictoryStage1
    }

    enum Buttons {

        btnExit

    }

    enum Texts {
        textVictory,

    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {

        if (!base.Initialize()) return false;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);

        return true;
    }

    #endregion

    #region OnButtons

    private void OnBtnExit() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_ExitGame>();
    }
    #endregion
}
