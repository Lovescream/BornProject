using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZerolizeDungeon;

public class UI_Map_Room : UI_Base {

    public Room Room { get; protected set; }
    public Vector2 Position => _rect.anchoredPosition;

    private string _spriteKey;

    private UI_Map _map;
    private RectTransform _rect;
    private Image _image;

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rect = this.GetComponent<RectTransform>();
        _map = FindObjectOfType<UI_Map>();
        _image = this.GetComponent<Image>();

        return true;
    }

    public void SetInfo(Room room, bool show = true) {
        Initialize();

        this.Room = room;

        Vector2 index = Main.Dungeon.Current.GetRelativeIndex(room);
        _rect.anchoredPosition = new((_map.RoomSize + _map.Margin) * index.x, (_map.RoomSize + _map.Margin) * index.y);
        _rect.sizeDelta = Vector2.one * _map.RoomSize;

        _spriteKey = room.Type switch {
            RoomType.Treasure => "UI_Map_Room_Treasure",
            RoomType.Boss => "UI_Map_Room_Boss",
            _ => "UI_Map_Room_Normal"
        };

        room.OnChangeExploreType += Refresh;
        Refresh(room.ExploreType);

        this.gameObject.SetActive(show);
    }

    private void Refresh(RoomExploreType type) {
        if (!this.gameObject.IsValid()) return;
        if (type == RoomExploreType.Hide) {
            _image.enabled = false;
            return;
        }
        _image.enabled = true;
        _image.sprite = Main.Resource.LoadSprite($"{_spriteKey}_{(int)type:00}");
    }
}