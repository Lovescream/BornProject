using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : Data {
    public string TilemapKey { get; set; }
    public RoomType Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}