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
    private bool isQuestPopupOpen = false;

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
        AudioController.Instance.SFXPlay(SFX.OnClickButton);
        Main.UI.OpenPopupUI<UI_Popup_Menu>();
    }

    
    private void OnBtnQuest() {
        AudioController.Instance.SFXPlay(SFX.OnClickButton);
        if (!isQuestPopupOpen)
        {
            Main.UI.OpenPopupUI<UI_Popup_Quest>().SetInfo(); // 퀘스트 팝업을 열어줘
            isQuestPopupOpen = true; // 퀘스트 팝업이 열렸다고 표시해줘
        }
        else
        {
            Main.UI.Clear(); // 이미 열려있는 퀘스트 팝업을 닫아줘
            isQuestPopupOpen = false; // 퀘스트 팝업이 닫혔다고 표시해줘
        }
    }


    private void OnBtnSkill() {
        AudioController.Instance.SFXPlay(SFX.OnClickButton);
        Main.UI.OpenPopupUI<UI_Popup_Skill>().SetInfo();
    }

    

    #endregion
}