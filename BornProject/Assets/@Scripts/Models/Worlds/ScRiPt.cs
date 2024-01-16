using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator {
    public enum TileType {
        NONE,
        Floor,
        Wall,
        Door,
    }
    public enum FloorType {
        NONE,
        Default,
        Top,
        Left,
        TopLeft,
        CrackTopLeft,
        CrackTopRight,
        CrackLeftTop,
        CrackLeftBottom,
        BreakingTopLeft,
        BreakingTopRight,
        BreakingLeftTop,
        BreakingLeftBottom,
        BrokenTopLeftLeft,
        BrokenTopLeft,
        BrokenTopRight,
        BrokenTopRightRight,
        BrokenLeftTopTop,
        BrokenLeftTop,
        BrokenLeftBottom,
        BrokenLeftBottomBottom,
    }
    public enum WallType {
        NONE,
        Horizontal,
        Vertical,
        CornerRightBottom,
        CornerBottomLeft,
        CornerTopLeft,
        CornerTopRight,
    }
    public enum DoorType {
        NONE,
        CrackHorizontalLeft,
        CrackHorizontalRight,
        CrackVerticalTop,
        CrackVerticalBottom,
        BreakingHorizontalLeft,
        BreakingHorizontalRight,
        BreakingVerticalTop,
        BreakingVerticalBottom,
        BrokenHorizontalLeft,
        BrokenHorizontalRight,
        BrokenVerticalTop,
        BrokenVerticalBottom,
    }
    public enum DoorState {
        NONE,
        Blocked,
        Crack,
        Breaking,
        Broken,
    }

    public enum Direction {
        Top,
        Right,
        Bottom,
        Left,
    }

    public class RoomData : Data {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte DoorInfo { get; set; } // 0000(0) ~ 1111(15)
    }

    public struct Pair<T1, T2> {

        public T1 Value1 { get; }
        public T2 Value2 { get; }

        public Pair(T1 value1, T2 value2) {
            Value1 = value1;
            Value2 = value2;
        }

        public override string ToString() {
            return base.ToString();
        }
    }

    public enum RoomType {
        Start,
        Normal,
        Treasure,
        Shop,
        Boss,
    }
}















public class ScRiPt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
