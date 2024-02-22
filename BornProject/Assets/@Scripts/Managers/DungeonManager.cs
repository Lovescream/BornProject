using ZerolizeDungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager {

    public DungeonGenerator Generator { get; private set; } = new(20, 20, 30, 30, 5);
    public int DungeonWidth { get => Generator.DungeonWidth; set => Generator.DungeonWidth = value; }
    public int DungeonHeight { get => Generator.DungeonHeight; set => Generator.DungeonHeight = value; }
    public int RoomWidth { get => Generator.RoomWidth; set => Generator.RoomWidth = value; }
    public int RoomHeight { get => Generator.RoomHeight; set => Generator.RoomHeight = value; }
    public int Count { get => Generator.Count; set => Generator.Count = value; }

    public Dungeon Current { get; private set; }

    public Transform DungeonRoot {
        get {
            if (_dungeonRoot == null) {
                GameObject obj = GameObject.Find("Dungeon");
                if (obj == null) obj = new("Dungeon");
                _dungeonRoot = obj.transform;
            }
            return _dungeonRoot;
        }
    }

    private Transform _dungeonRoot;

    public void Generate() {
        Debug.Log("[DungeonManager] Dungeon Generate Start.");
        if (Current != null) Destroy();
        Generator.Clear();
        if (Generator.Generate())
            Current = new Dungeon(Generator.Result, DungeonRoot);
    }

    public void Destroy() {
        for (int i = DungeonRoot.childCount; i > 0; i--) {
            Main.Resource.Destroy(DungeonRoot.transform.GetChild(i).gameObject);
        }
        Current = null;
    }

    public Room GetRoom(Vector2 position) {
        return Current.Rooms.Where(room => room.IsInRoom(position)).FirstOrDefault();
    }

    public void CheckClear() {
        if (Current.Rooms.Where(x => x.ExistEnemy).Count() > 0) return;
        Main.Quest.ClearStageCount++;
        NextStage();
    }

    public void NextStage() {
        Main.UI.Clear();
        SceneManager.LoadScene("GameScene");
        Main.UI.OpenPopupUI<UI_Popup_Clear>();
    }
}