using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {

    public UI_Scene_Game UI => SceneUI as UI_Scene_Game;

    public bool IsPlaying { get; protected set; }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.Z)) Player.Hp -= 9999;
    }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        // #1. Map 생성.
        Main.Dungeon.Generate();

        // #2. Player 생성.
        Player player = Main.Object.SpawnPlayer("Player", Main.Dungeon.Current.StartRoom.CenterPosition);
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        Main.Game.Player = player;

        // #3. UI 생성.
        SceneUI = Main.UI.OpenSceneUI<UI_Scene_Game>();

        // #4. Skill 체크.
        if (player.SkillList[SkillType.Range] == null || player.SkillList[SkillType.Melee] == null)
            Main.UI.OpenPopupUI<UI_Popup_Skill>();

        // #5.
        if (Main.Game.Current.FirstPlay) {
            Main.UI.OpenPopupUI<UI_Popup_Tutorial>();
            Main.Game.Current.FirstPlay = false;
        }

        IsPlaying = true;

        return true;
    }

}