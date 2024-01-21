using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene {


    void Update() {

    }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        return true;
    }
}