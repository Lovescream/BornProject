using DungeonGenerator;
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

        RoomData roomData = new() {
            Key = "Room00",
            Width = 20,
            Height = 20,
            DoorInfo = 0b_1111,
        };
        testRoom = new(roomData);
        RoomObject roomObject = Main.Resource.Instantiate("Room").GetComponent<RoomObject>();
        roomObject.SetInfo(testRoom);

        return true;
    }
}