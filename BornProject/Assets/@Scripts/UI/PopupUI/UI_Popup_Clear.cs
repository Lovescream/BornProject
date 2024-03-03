public class UI_Popup_Clear : UI_Popup {

    #region Enums

    enum Buttons {
        btnNextStage,
        btnExit,
    }

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
        Main.Audio.PlayOnButton();
        Main.Dungeon.NextStage();
    }

    private void OnBtnExit() {
        Main.Audio.PlayOnButton();
        Main.UI.OpenPopupUI<UI_Popup_ExitGame>();
    }

    #endregion
}