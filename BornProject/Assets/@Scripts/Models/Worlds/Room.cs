using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGenerator {

    public class Room {

        #region Properties

        // Index.
        public int X { get; private set; }
        public int Y { get; private set; }
        
        // Room Info.
        public RoomData Data { get; private set; }
        public RoomObject Object { get; set; }
        public int Width => Data.Width;
        public int Height => Data.Height;
        public RoomType Type { get; private set; }
        
        // Room State.
        public bool IsOpened {
            get => _isOpened;
            set {
                if (value == _isOpened) return;
                _isOpened = value;
                if (value) OnRoomOpened?.Invoke(this);
                else OnRoomClosed?.Invoke(this);
            }
        }
        public bool IsActivated { get; private set; }
        public bool IsClear { get; private set; }

        public List<Tile> Tiles => _tiles.Values.ToList();

        #endregion

        #region Fields

        // State.
        private bool _isOpened = true;

        // Collections.
        private Dictionary<Vector2Int, Tile> _tiles = new();
        private Dictionary<Direction, Door> _doors = new();

        // Callbacks.
        public event Action<Room> OnRoomOpened;
        public event Action<Room> OnRoomClosed;

        #endregion

        #region Constructor / Indexer

        public Tile this[int x, int y] => _tiles.TryGetValue(new(x, y), out Tile tile) ? tile : null;
        public Tile this[Vector2Int v] => _tiles.TryGetValue(v, out Tile tile) ? tile : null;

        public Room(RoomData data) {
            this.Data = data;

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Vector2Int index = new(x, y);
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                        _tiles[index] = new(this, index, TileType.Wall);
                    else
                        _tiles[index] = new(this, index, TileType.Floor);
                }
            }
            _tiles.Values.ToList().ForEach(x => x.Initialize());
            for (int i = 0; i < 4; i++) {
                if ((Data.DoorInfo & (1 << i)) == 0) continue;
                Direction direction = (Direction)i;
                _doors[direction] = GenerateDoor(direction);
            }

            OnRoomOpened += r => r.IsOpened = true;
            OnRoomClosed += r => r.IsOpened = false;
        }

        #endregion

        public void SetDoorState(DoorState state) {
            _doors.Values.ToList().ForEach(x => x.State = state);
        }

        private Door GenerateDoor(Direction direction) {
            Tile[] tiles = new Tile[2];
            switch (direction) {
                case Direction.Top:
                    tiles[0] = this[Width / 2 - 1, Height - 1];
                    tiles[1] = this[Width / 2, Height - 1];
                    break;
                case Direction.Right:
                    tiles[0] = this[Width - 1, Height / 2];
                    tiles[1] = this[Width - 1, Height / 2 - 1];
                    break;
                case Direction.Bottom:
                    tiles[0] = this[Width / 2 - 1, 0];
                    tiles[1] = this[Width / 2, 0];
                    break;
                case Direction.Left:
                    tiles[0] = this[0, Height / 2];
                    tiles[1] = this[0, Height / 2 - 1];
                    break;
            }
            return new Door(this, direction, tiles);
        }
    }


}