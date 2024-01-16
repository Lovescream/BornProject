using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {
    private GameObject _enemyPrefab;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject newObject = Main.Resource.Instantiate("Creature", pooling: true);
            Instantiate(newObject);
            Creature newCreature = newObject.GetComponent<Creature>();
            newCreature.SetInfo(Main.Data.Creatures["Player"]);
        }
    }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        return true;
    }

}