using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager {

    public DungeonGenerator Generator { get; private set; } = new(20, 20, 20);
    public int Width { get => Generator.Width; set => Generator.Width = value; }
    public int Height { get => Generator.Height; set => Generator.Height = value; }
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
        if (Current != null) return;
        Generator.Clear();
        if (Generator.Generate())
            Current = new Dungeon(Generator.Result, DungeonRoot);
    }

    public void Destroy() {
        foreach (Room room in Current.Rooms) {
            room.Object.transform.SetParent(null);
            Main.Resource.Destroy(room.Object.gameObject);
        }
        Current = null;
    }

    public Room GetRoom(Vector2 position) {
        return Current.Rooms.Where(room => room.IsInRoom(position)).FirstOrDefault();
    }
}