using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDevelopScene : BaseScene {
    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        Main.Dungeon.Generate();

        return true;
    }
}