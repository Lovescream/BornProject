namespace DungeonGenerate {
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
        COUNT,
    }
}