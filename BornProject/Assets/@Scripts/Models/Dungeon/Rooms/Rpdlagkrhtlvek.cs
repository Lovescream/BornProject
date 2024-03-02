using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZerolizeDungeon;

public class Rpdlagkrhtlvek : MonoBehaviour {

    public Room Room { get; protected set; }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() == null) return;
        Room.cbOnEnteredRoom?.Invoke(Room);
    }
    protected void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<Player>() == null) return;
        Room.cbOnExitedRoom?.Invoke(Room);
    }

    private bool _isInitialized;

    public bool Initialize() {
        if (_isInitialized) return false;

        return true;
    }

    public void SetInfo(Room room) {
        this.Room = room;
        this.transform.localPosition = new(15, 15);
    }
}