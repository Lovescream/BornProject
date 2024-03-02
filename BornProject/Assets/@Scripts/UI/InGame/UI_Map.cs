using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZerolizeDungeon;

public class UI_Map : MonoBehaviour {

    public readonly float RoomSize = 50;
    public readonly float Margin = 10;

    #region Fields

    // Collections.
    private List<UI_Map_Room> _mapSlots = new();

    // Components.
    private Transform _parent;

    private bool _isInitialized;

    #endregion

    private void OnEnable() {
        Initialize();
    }

    private void Initialize() {
        if (_isInitialized) return;

        _parent = this.gameObject.FindChild("Parent").transform;

        _isInitialized = true;
    }

    public void SetInfo() {
        List<Room> rooms = Main.Dungeon.Current.Rooms;
        foreach (Room room in rooms) {
            UI_Map_Room roomMap = Main.UI.CreateSubItem<UI_Map_Room>(_parent, pooling: false);
            roomMap.SetInfo(room);
            _mapSlots.Add(roomMap);
            room.FullCollider.OnEnteredRoom += Refresh;
        }
    }

    private void Refresh(Room room) {
        _parent.GetComponent<RectTransform>().anchoredPosition = -_mapSlots.Find(x => x.Room.Equals(room)).Position;
    }

}