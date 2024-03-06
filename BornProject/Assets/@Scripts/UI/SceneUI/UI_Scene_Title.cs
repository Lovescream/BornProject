public class UI_Scene_Title : UI_Scene {

    #region Enums 

    enum Images {

        imageTitle
    }

    enum Buttons {

        btnNewGame,
        btnContinue,
        btnExit

    }

    enum Texts {
        textTitle,

    }

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));


        GetButton((int)Buttons.btnNewGame).onClick.AddListener(OnBtnNewGame);
        GetButton((int)Buttons.btnContinue).onClick.AddListener(OnBtnContinue);
        GetButton((int)Buttons.btnExit).onClick.AddListener(OnBtnExit);

        return true;
    }

    private void OnBtnNewGame() {
        Main.Audio.PlaySFX("SFX_OnButton_NewGame");
        Main.Scene.LoadScene("GameScene");
    }

    private void OnBtnContinue() {


    }

    private void OnBtnExit() {
        Main.ExitGame();
    }
}