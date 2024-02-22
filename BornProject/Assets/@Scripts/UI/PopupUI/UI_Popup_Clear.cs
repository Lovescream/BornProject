public class UI_Popup_Clear : UI_Popup {

    #region Enums

    enum Buttons {
        btnNextStage,
        btnExit,
    }

    #endregion

    #region Properties

    public override bool IsPause => true;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.btnNextStage).onClick.AddListener(OnBtnNextStage);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);

        return true;
    }

    #endregion

    #region OnButtons

    private void OnBtnNextStage() {
        AudioController.Instance.SFXPlay(SFX.OnClickButton);
        Main.Dungeon.NextStage();
    }

    private void OnBtnExit() {
        AudioController.Instance.SFXPlay(SFX.OnClickButton);
        Main.UI.OpenPopupUI<UI_Popup_ExitGame>();
    }

    #endregion
}