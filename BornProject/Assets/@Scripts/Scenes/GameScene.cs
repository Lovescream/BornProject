using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {

    public UI_Scene_Game UI => SceneUI as UI_Scene_Game;

    public Player Player { get; private set; }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        // #1. Map 생성.
        Main.Dungeon.Generate();

        // #2. Player 생성.
        Player = Main.Object.SpawnPlayer("Player", Main.Dungeon.Current.StartRoom.CenterPosition);
        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);

        // #3. UI 생성.
        SceneUI = Main.UI.OpenSceneUI<UI_Scene_Game>();

        return true;
    }

}