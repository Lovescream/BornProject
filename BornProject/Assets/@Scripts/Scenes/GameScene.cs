using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {

    public Player Player { get; private set; }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        //Main.Object.SpawnEnemy("Enemy1", new(4, 0)); //애니메이션 추가 후 사용 가능.

        return true;
    }

}