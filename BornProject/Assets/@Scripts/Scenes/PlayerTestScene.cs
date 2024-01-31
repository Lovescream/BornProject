using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestScene : BaseScene {

    public Player Player { get; private set; }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        Player = Main.Object.SpawnPlayer("Player", Vector2.zero);

        return true;
    }

}