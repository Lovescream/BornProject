using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene {

    public Player Player { get; private set; }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        // #1. Map 생성.
        Main.Dungeon.Generate();

        // #2. Player 생성.
        Player = Main.Object.SpawnPlayer("Player", Main.Dungeon.Current.StartRoom.CenterPosition);
        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);

        // #3. Player 옆 방에 Enemy 생성.
        Main.Object.SpawnEnemy("Bear", Main.Dungeon.Current.StartRoom.GetRandomNeighbour().CenterPosition);

        return true;
    }

}