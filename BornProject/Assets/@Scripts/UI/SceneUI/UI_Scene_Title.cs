using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Scene_Title : UI_Scene {

   

    #region Enums 

    enum Images{

        ImageTitle
    }

    enum Buttons{

        btnNewGame,
        btnContinue,
        btnController,
        btnExit

    }

    enum Texts{
        textTitle,
        textNewGame,
        textContinue,
        textController,
        textExit
    }

    #endregion

   /* public void Start() {

        Init();
    }

   /* public override bool Init() {
        if (!base.Init()) return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.btnNewGame).gameObject.BindEvent(OnButtonStart);

        return true;
    }

    private void OnButtonStart(PointerEventData data) {
        Main.UI.CloseAllPopupUI();
        Main.Scene.LoadScene("TestScene");
    }
   */
}