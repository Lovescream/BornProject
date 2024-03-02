using System;
using UnityEngine;
using ZerolizeDungeon;

public class RoomFullCollider : MonoBehaviour {

    public Room Room { get; protected set; }

    public event Action<Room> OnEnteredRoom;
    public event Action<Room> OnExitedRoom;

    private BoxCollider2D _collider;

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() == null) return;
        OnEnteredRoom?.Invoke(Room);
    }

    protected void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<Player>() == null) return;
        OnExitedRoom?.Invoke(Room);
    }

    public void SetInfo(Room room) {
        this.Room = room;
        this.name = "FullCollider";
        this.transform.SetParent(Room.transform);
        this.transform.localPosition = new(15, 15);
        _collider = this.gameObject.AddComponent<BoxCollider2D>();
        _collider.isTrigger = true;
        _collider.size = new(30, 30);
    }

}