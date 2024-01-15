using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {

    //void Update() {
    //    if (Input.GetKeyDown(KeyCode.B)) {
    //        Main.Object.SpawnEnemy("Slime", new(1, 2));
    //    }
    //    if (Input.GetKeyDown(KeyCode.C)) {
    //        Main.Object.SpawnEnemy("Zombie", new(-1, -2));
    //    }
    //    if (Input.GetKeyDown(KeyCode.D)) {
    //        Main.Object.SpawnEnemy("Skeleton", new(3, 3));
    //    }

    //    if (Input.GetKeyDown(KeyCode.F)) {
    //        foreach (Enemy enemy in FindObjectsOfType<Creature>()) {
    //            Main.Object.DespawnEnemy(enemy);
    //        }
    //    }
    //}

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        Player player = Main.Object.SpawnPlayer("Character_01", new(1, 1));

        Enemy enemy = Main.Object.SpawnEnemy("Character_02", new(2, 2));
        
        //GameObject player = Main.Resource.Instantiate("Player", pooling: true);
        //player.GetComponent<SpriteRenderer>().sprite = Main.Resource.LoadSprite("SilverAxe");

        return true;
    }

}