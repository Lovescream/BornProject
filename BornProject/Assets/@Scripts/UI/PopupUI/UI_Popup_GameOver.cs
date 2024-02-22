using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_Popup_GameOver : UI_Popup {

    #region Enums
    enum Buttons {
        btnRetry,
        btnExit,
    }

    enum Texts {
        textMessage
    }

    #endregion

    #region Properties

    public UI_PlayerInfo PlayerInfo {get;protected set;}

    #endregion

    #region Initialize / Set

public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.btnRetry).onClick.AddListener(OnBtnRetry);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);


        return true;
    }


    private void OnBtnRetry() {

        //AudioController.Instance.SFXPlay(SFX.Button);
        //
        Main.UI.Clear();
        Main.Quest.ClearStageCount = 0;
        SceneManager.LoadScene("GameScene");
    }

    private void OnBtnExit() {

        //AudioController.Instance.SFXPlay(SFX.Button);
        
        Main.UI.Clear();
        Main.Quest.ClearStageCount = 0;
        SceneManager.LoadScene("TitleScene");
    }

    #endregion

}