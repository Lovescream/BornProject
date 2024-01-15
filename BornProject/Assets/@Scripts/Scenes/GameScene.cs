using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {
    private GameObject _enemyPrefab;

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        return true;
    }

}