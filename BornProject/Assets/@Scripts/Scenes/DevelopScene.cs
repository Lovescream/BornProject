using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopScene : BaseScene {

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (Entity entity in entities) {
            if (entity is Player player) {
                player.SetInfo(Main.Data.Creatures["Player"]);
            }
            else if (entity is Enemy enemy) {
                enemy.SetInfo(Main.Data.Creatures[$"{enemy.Key}"]);
            }
        }
        return true;
    }

}