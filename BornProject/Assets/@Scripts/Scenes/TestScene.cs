using DungeonGenerate;
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

        return true;
    }
}