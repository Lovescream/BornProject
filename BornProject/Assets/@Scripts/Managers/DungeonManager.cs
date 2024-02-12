using ZerolizeDungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class DungeonManager {

//    public DungeonGenerator Generator { get; private set; } = new(20, 20, 20, 20, 20);
//    public int DungeonWidth { get => Generator.DungeonWidth; set => Generator.DungeonWidth = value; }
//    public int DungeonHeight { get => Generator.DungeonHeight; set => Generator.DungeonHeight = value; }
//    public int RoomWidth { get => Generator.RoomWidth; set => Generator.RoomWidth = value; }
//    public int RoomHeight { get => Generator.RoomHeight; set => Generator.RoomHeight = value; }
//    public int Count { get => Generator.Count; set => Generator.Count = value; }

//    public Dungeon Current { get; private set; }

//    public Transform DungeonRoot {
//        get {
//            if (_dungeonRoot == null) {
//                GameObject obj = GameObject.Find("Dungeon");
//                if (obj == null) obj = new("Dungeon");
//                _dungeonRoot = obj.transform;
//            }
//            return _dungeonRoot;
//        }
//    }

//    private Transform _dungeonRoot;

//    public void Generate() {
//        if (Current != null) return;
//        Generator.Clear();
//        if (Generator.Generate())
//            Current = new Dungeon(Generator.Result, DungeonRoot);
//    }

//    public void Destroy() {
//        foreach (Room room in Current.Rooms) {
//            room.Object.transform.SetParent(null);
//            Main.Resource.Destroy(room.Object.gameObject);
//        }
//        Current = null;
//    }

//    public Room GetRoom(Vector2 position) {
//        return Current.Rooms.Where(room => room.IsInRoom(position)).FirstOrDefault();
//    }

//}

public class DungeonManager {

    public DungeonGenerator Generator { get; private set; } = new(20, 20, 30, 30, 20);
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
        if (Current != null) return;
        Generator.Clear();
        if (Generator.Generate())
            Current = new Dungeon(Generator.Result, DungeonRoot);
    }

    //public void Destroy() {
    //    foreach (Room room in Current.Rooms) {
    //        room.Object.transform.SetParent(null);
    //        Main.Resource.Destroy(room.Object.gameObject);
    //    }
    //    Current = null;
    //}

    public Room GetRoom(Vector2 position) {
        return Current.Rooms.Where(room => room.IsInRoom(position)).FirstOrDefault();
    }

}