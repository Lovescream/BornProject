using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_Scene_Title : UI_Scene {

   

    #region Enums 

    enum Images{

        imageTitle
    }

    enum Buttons{

        btnNewGame,
        btnContinue,
        btnController,
        btnExit

    }

    enum Texts{
        textTitle,
        textxit
    }

    #endregion

    void Start()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        
        GetButton((int)Buttons.btnNewGame).onClick.AddListener(OnBtnNewGame);

        return true;
    }

    private void OnBtnNewGame() {

        Main.UI.CloseAllPopup();
        //Main.Scene.LoadScene("TestScene");
    }


}