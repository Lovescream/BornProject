using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Menu : UI_Popup {

    #region Enums

    enum Objects {
        VolumeController_BGM,
        VolumeController_SFX,
    }
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
        BindObject(typeof(Objects));

        GetButton((int)Buttons.btnInfo).onClick.AddListener(OnBtnInfo);
        GetButton((int)Buttons.btnResume).onClick.AddListener(OnBtnResume);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);


        return true;
    }

    public void SetInfo() {
        GetObject((int)Objects.VolumeController_BGM).GetComponent<UI_VolumeController>().SetInfo(AudioType.BGM);
        GetObject((int)Objects.VolumeController_SFX).GetComponent<UI_VolumeController>().SetInfo(AudioType.SFX);
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