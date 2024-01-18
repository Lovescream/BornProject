using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager {

    public DungeonGenerator Generator { get; protected set; } = new(20, 20, 20);
    public int Width { get => Generator.Width; set => Generator.Width = value; }
    public int Height { get => Generator.Height; set => Generator.Height = value; }
    public int Count { get => Generator.Count; set => Generator.Count = value; }

    public Dungeon Current { get; protected set; }

    public void Generate() {
        Generator.Generate();

        Current = new Dungeon(Generator.Result);
    }
}