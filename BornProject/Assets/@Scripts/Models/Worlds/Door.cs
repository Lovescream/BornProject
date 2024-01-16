using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DungeonGenerator {
    public class Door {

        #region Properties

        public Room Room { get; private set; }
        public Direction Direction { get; private set; }
        public DoorState State {
            get => _state;
            set {
                if (value == _state) return;
                _state = value;
                SetTypes(value);
            }
        }

        #endregion

        #region Fields

        private DoorState _state = DoorState.NONE;

        // Collections.
        private Tile[] _tiles;
        private Tile[] _floors;

        #endregion

        public Door(Room room, Direction direction, Tile[] tiles) {
            this.Room = room;
            this.Direction = direction;

            this._tiles = tiles;

            this._floors = direction switch {
                Direction.Top => new Tile[4] {
                    Room[_tiles[0].X - 1,_tiles[0].Y - 1],
                    Room[_tiles[0].X + 0,_tiles[0].Y - 1],
                    Room[_tiles[1].X + 0,_tiles[1].Y - 1],
                    Room[_tiles[1].X + 1,_tiles[1].Y - 1],
                },
                Direction.Left => new Tile[4] {
                    Room[_tiles[0].X + 1,_tiles[0].Y + 1],
                    Room[_tiles[0].X + 1,_tiles[0].Y + 0],
                    Room[_tiles[1].X + 1,_tiles[1].Y + 0],
                    Room[_tiles[1].X + 1,_tiles[1].Y - 1],
                },
                _ => new Tile[] { }
            };
            State = DoorState.Broken;
        }

        private void SetTypes(DoorState state) {
            if (state == DoorState.Blocked) {
                _tiles[0].Type = TileType.Wall;
                _tiles[1].Type = TileType.Wall;
                if (Direction == Direction.Top || Direction == Direction.Bottom) {
                    _tiles[0].WallType = WallType.Horizontal;
                    _tiles[1].WallType = WallType.Horizontal;
                    for (int i = 0; i < _floors.Length; i++) _floors[i].FloorType = FloorType.Top;
                }
                else {
                    _tiles[0].WallType = WallType.Vertical;
                    _tiles[1].WallType = WallType.Vertical;
                    for (int i = 0; i < _floors.Length; i++) _floors[i].FloorType = FloorType.Left;
                }
                return;
            }
            _tiles[0].Type = TileType.Door;
            _tiles[1].Type = TileType.Door;
            if (Direction == Direction.Top || Direction == Direction.Bottom) {
                _tiles[0].DoorType = state switch {
                    DoorState.Crack => DoorType.CrackHorizontalLeft,
                    DoorState.Breaking => DoorType.BreakingHorizontalLeft,
                    DoorState.Broken => DoorType.BrokenHorizontalLeft,
                    _ => DoorType.NONE
                };
                _tiles[1].DoorType = state switch {
                    DoorState.Crack => DoorType.CrackHorizontalRight,
                    DoorState.Breaking => DoorType.BreakingHorizontalRight,
                    DoorState.Broken => DoorType.BrokenHorizontalRight,
                    _ => DoorType.NONE
                };
                if (_floors.Length > 0) {
                    _floors[0].FloorType = state switch {
                        DoorState.Crack => FloorType.Top,
                        DoorState.Breaking => FloorType.Top,
                        DoorState.Broken => FloorType.BrokenTopLeftLeft,
                        _ => FloorType.NONE
                    };
                    _floors[1].FloorType = state switch {
                        DoorState.Crack => FloorType.CrackTopLeft,
                        DoorState.Breaking => FloorType.BreakingTopLeft,
                        DoorState.Broken => FloorType.BrokenTopLeft,
                        _ => FloorType.NONE
                    };
                    _floors[2].FloorType = state switch {
                        DoorState.Crack => FloorType.CrackTopRight,
                        DoorState.Breaking => FloorType.BreakingTopRight,
                        DoorState.Broken => FloorType.BrokenTopRight,
                        _ => FloorType.NONE
                    };
                    _floors[3].FloorType = state switch {
                        DoorState.Crack => FloorType.Top,
                        DoorState.Breaking => FloorType.Top,
                        DoorState.Broken => FloorType.BrokenTopRightRight,
                        _ => FloorType.NONE
                    };
                }
            }
            else {
                _tiles[0].DoorType = state switch {
                    DoorState.Crack => DoorType.CrackVerticalTop,
                    DoorState.Breaking => DoorType.BreakingVerticalTop,
                    DoorState.Broken => DoorType.BrokenVerticalTop,
                    _ => DoorType.NONE
                };
                _tiles[1].DoorType = state switch {
                    DoorState.Crack => DoorType.CrackVerticalBottom,
                    DoorState.Breaking => DoorType.BreakingVerticalBottom,
                    DoorState.Broken => DoorType.BrokenVerticalBottom,
                    _ => DoorType.NONE
                };
                if (_floors.Length > 0) {
                    _floors[0].FloorType = state switch {
                        DoorState.Crack => FloorType.Left,
                        DoorState.Breaking => FloorType.Left,
                        DoorState.Broken => FloorType.BrokenLeftTopTop,
                        _ => FloorType.NONE
                    };
                    _floors[1].FloorType = state switch {
                        DoorState.Crack => FloorType.CrackLeftTop,
                        DoorState.Breaking => FloorType.BreakingLeftTop,
                        DoorState.Broken => FloorType.BrokenLeftTop,
                        _ => FloorType.NONE
                    };
                    _floors[2].FloorType = state switch {
                        DoorState.Crack => FloorType.CrackLeftBottom,
                        DoorState.Breaking => FloorType.BreakingLeftBottom,
                        DoorState.Broken => FloorType.BrokenLeftBottom,
                        _ => FloorType.NONE
                    };
                    _floors[3].FloorType = state switch {
                        DoorState.Crack => FloorType.Left,
                        DoorState.Breaking => FloorType.Left,
                        DoorState.Broken => FloorType.BrokenLeftBottomBottom,
                        _ => FloorType.NONE
                    };
                }
            }
        }

    }

}