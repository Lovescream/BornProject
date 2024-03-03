using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene_Game : UI_Scene {

    #region Enum

    enum Buttons {
        btnMenu,
        btnQuest,
        btnSkill,
       
    }
    enum Objects {
        PlayerInfo,
        BossHp,
    }

    #endregion

    #region Properties
    
    public UI_PlayerInfo PlayerInfo { get; protected set; }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        GetButton((int)Buttons.btnMenu).onClick.AddListener(OnBtnMenu);
        GetButton((int)Buttons.btnQuest).onClick.AddListener(OnBtnQuest);
        GetButton((int)Buttons.btnSkill).onClick.AddListener(OnBtnSkill);
        
        PlayerInfo = GetObject((int)Objects.PlayerInfo).GetComponent<UI_PlayerInfo>();
        PlayerInfo.SetInfo(Main.Object.Player);

        GetObject((int)Objects.BossHp).SetActive(false);

        return true;
    }

    #endregion

    //public void SetBossHpBar(Boss boss) {
    //    GameObject barObject = GetObject((int)Objects.BossHp);
    //    barObject.SetActive(boss != null);
    //    if (boss != null)
    //        barObject.GetComponent<UI_BossHpBar>().SetInfo(boss);
    //}

    #region OnButtons

    private void OnBtnMenu() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_Menu>().SetInfo();
    }

    
    private void OnBtnQuest() {
        Main.Audio.PlayOnButton();
        UI_Popup_Quest popup = Main.UI.GetLatestPopup<UI_Popup_Quest>();
        if (popup == null) Main.UI.OpenPopupUI<UI_Popup_Quest>().SetInfo();
        else Main.UI.ClosePopup(popup);
    }

    private void OnBtnSkill() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_Skill>().SetInfo();
    }

    #endregion
}