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
        
    }

    #endregion

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        
        GetButton((int)Buttons.btnNewGame).onClick.AddListener(OnBtnNewGame);
        GetButton((int)Buttons.btnContinue).onClick.AddListener(OnBtnContinue);
        GetButton((int)Buttons.btnController).onClick.AddListener(OnBtnController);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);

        return true;
    }

    private void OnBtnNewGame() {

        Main.UI.CloseAllPopup();
        SceneManager.LoadScene("GameScene");
    }

    private void OnBtnContinue() {

    }

    private void OnBtnController() {

    }

    private void OnBtnExit() {
        Debug.Log("게임종료");
    }
}