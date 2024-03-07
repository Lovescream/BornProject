using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopScene : BaseScene {

    public Player Player { get; protected set; }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) Player.Hp -= 9999;
    }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (Entity entity in entities) {
            if (entity is Player player) {
                Player = player;
                player.SetInfo(Main.Data.Creatures["Player"]);
                Main.Game.Player = player;
            }
            else if (entity is Enemy enemy) {
                enemy.SetInfo(Main.Data.Creatures[$"{enemy.ManualInitializingKey}"]);
            }
        }

        SceneUI = Main.UI.OpenSceneUI<UI_Scene_Develop>();

        return true;
    }

}