using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {
    private GameObject _enemyPrefab;

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        GameObject enemy = Main.Pool.Pop(Main.Resource.EnemyPrefab);
        enemy.GetComponent<SpriteRenderer>().sprite = Main.Resource.Enemy["DiamondAxe"];

        return true;
    }

}