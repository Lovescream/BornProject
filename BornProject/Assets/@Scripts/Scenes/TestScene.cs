using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene {

    private Room testRoom;

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) testRoom.Object.Open();
        if (Input.GetKeyDown(KeyCode.S)) testRoom.Object.Close();
    }

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        //Main.Dungeon.Generate();

        return true;
    }

    #region Develop

    public void Test() => Debug.Log("Test");
    public void GenerateDungeon() {
        Main.Resource.Initialize();
        Main.Data.Initialize();
        Main.Dungeon.Generate();
    }
    public void RegenerateDungeon() {
        Main.Dungeon.Destroy();
        Main.Dungeon.Generate();
    }

    #endregion
}