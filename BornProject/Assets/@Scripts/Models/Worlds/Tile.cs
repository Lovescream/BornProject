using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator {
    public class Tile {

        #region Properties

        // Index.
        public int X { get; private set; }
        public int Y { get; private set; }

        // Tile Info.
        public Room Room { get; private set; }
        public TileType Type {
            get => _type;
            set {
                if (value == _type) return;
                _type = value;
                switch (_type) {
                    case TileType.Floor:
                        _wallType = WallType.NONE;
                        _doorType = DoorType.NONE;
                        break;
                    case TileType.Wall:
                        _floorType = FloorType.NONE;
                        _doorType = DoorType.NONE;
                        break;
                    case TileType.Door:
                        _floorType = FloorType.NONE;
                        _wallType = WallType.NONE;
                        break;
                    default:
                        _floorType = FloorType.NONE;
                        _wallType = WallType.NONE;
                        _doorType = DoorType.NONE;
                        break;
                }
                OnTileChanged?.Invoke(this);
            }
        }
        public FloorType FloorType {
            get => _floorType;
            set {
                if (value == _floorType) return;
                _floorType = value;
                OnTileChanged?.Invoke(this);
            }
        }
        public WallType WallType {
            get => _wallType;
            set {
                if (value == _wallType) return;
                _wallType = value;
                OnTileChanged?.Invoke(this);
            }
        }
        public DoorType DoorType {
            get => _doorType;
            set {
                if (value == _doorType) return;
                _doorType = value;
                OnTileChanged?.Invoke(this);
            }
        }
        public string TileName => $"{Room.Data.Key}_{Type}_{Type switch { TileType.Floor => FloorType, TileType.Wall => WallType, TileType.Door => DoorType, _ => "" }}";

        // NeighbourInfo.
        public Tile Top => Room[X, Y + 1];
        public Tile Right => Room[X + 1, Y];
        public Tile Bottom => Room[X, Y - 1];
        public Tile Left => Room[X - 1, Y];

        #endregion

        #region Fields

        private TileType _type = TileType.NONE;
        private FloorType _floorType = FloorType.NONE;
        private WallType _wallType = WallType.NONE;
        private DoorType _doorType = DoorType.NONE;

        public event Action<Tile> OnTileChanged;

        #endregion

        #region Constructor

        public Tile(Room room, Vector2Int index, TileType type = TileType.Floor) {
            Room = room;
            X = index.x;
            Y = index.y;
            Type = type;
        }

        public void Initialize() {
            if (Type == TileType.Floor) {
                if (Top != null && Top.Type == TileType.Wall) {
                    if (Left != null && Left.Type == TileType.Wall)
                        FloorType = FloorType.TopLeft;
                    else if (Left != null)
                        FloorType = FloorType.Top;
                }
                else if (Left != null && Left.Type == TileType.Wall) {
                    FloorType = FloorType.Left;
                }
                else FloorType = FloorType.Default;
            }
            else if (Type == TileType.Wall) {
                if (Top != null && Top.Type == TileType.Wall) {
                    if (Right != null && Right.Type == TileType.Wall)
                        WallType = WallType.CornerTopRight;
                    else if (Bottom != null && Bottom.Type == TileType.Wall)
                        WallType = WallType.Vertical;
                    else if (Left != null && Left.Type == TileType.Wall)
                        WallType = WallType.CornerTopLeft;
                }
                else if (Right != null && Right.Type == TileType.Wall) {
                    if (Bottom != null && Bottom.Type == TileType.Wall)
                        WallType = WallType.CornerRightBottom;
                    else if (Left != null && Left.Type == TileType.Wall)
                        WallType = WallType.Horizontal;
                }
                else if (Bottom != null && Bottom.Type == TileType.Wall) {
                    if (Left != null && Left.Type == TileType.Wall)
                        WallType = WallType.CornerBottomLeft;
                }
            }
        }

        #endregion
    }
}