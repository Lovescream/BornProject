using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGenerate {

    public class Room {

        #region Properties

        // Index.
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int Index => new(X, Y);
        
        // Room Info.
        public RoomData Data { get; private set; }
        public RoomObject Object { get; set; }
        public int Width => Data.Width;
        public int Height => Data.Height;
        public Vector2 OriginPosition => new(X * Width, Y * Height);
        public Vector2 CenterPosition => OriginPosition + new Vector2(Width / 2, Height / 2);
        public Vector2 MaxPosition => OriginPosition + new Vector2(Width, Height);
        public RoomType Type => Data.Type;
        
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

        // Neighbours Info.
        public Room Top => _neighbours[Direction.Top];
        public Room Right => _neighbours[Direction.Right];
        public Room Bottom => _neighbours[Direction.Bottom];
        public Room Left => _neighbours[Direction.Left];

        public List<Tile> Tiles => _tiles.Values.ToList();

        #endregion

        #region Fields

        // State.
        private bool _isOpened = true;

        // Collections.
        private Dictionary<Vector2Int, Tile> _tiles = new();
        private Dictionary<Direction, Door> _doors = new();
        private Dictionary<Direction, Room> _neighbours = new();

        // Callbacks.
        public event Action<Room> OnRoomOpened;
        public event Action<Room> OnRoomClosed;

        #endregion

        #region Constructor / Indexer / Initialize

        public Tile this[int x, int y] => _tiles.TryGetValue(new(x, y), out Tile tile) ? tile : null;
        public Tile this[Vector2Int v] => _tiles.TryGetValue(v, out Tile tile) ? tile : null;

        public Room(RoomData data, Vector2Int index) {
            // #1. Data 설정.
            this.Data = data;
            this.X = index.x;
            this.Y = index.y;

            // #2. 컬렉션 초기화.
            for (int i = 0; i < (int)Direction.COUNT; i++) {
                _doors[(Direction)i] = null;
                _neighbours[(Direction)i] = null;
            }

            // #3. Tile 정보 설정.
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Vector2Int tileIndex = new(x, y);
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                        _tiles[tileIndex] = new(this, tileIndex, TileType.Wall);
                    else
                        _tiles[tileIndex] = new(this, tileIndex, TileType.Floor);
                }
            }
            _tiles.Values.ToList().ForEach(x => x.Initialize());

            // #4. 콜백 등록.
            OnRoomOpened += r => r.IsOpened = true;
            OnRoomClosed += r => r.IsOpened = false;
        }

        public override string ToString() => $"{Type}[{X}, {Y}]";
        #endregion

        public bool IsInRoom(Vector2 position) => position.IsInRange(OriginPosition, MaxPosition);

        #region Neighbours

        public void SetNeighbours(Room[] rooms) {
            for (int i = 0; i < rooms.Length; i++) {
                _neighbours[(Direction)i] = rooms[i];
            }
        }

        #endregion

        #region Door

        public Door GetDoor(Direction direction) {
            if (!_doors.TryGetValue(direction, out Door door)) return null;
            return door;
        }
        public void GenerateDoors() {
            foreach (KeyValuePair<Direction, Room> pair in _neighbours) {
                if (pair.Value == null) continue;
                GenerateDoor(pair.Key);
            }
        }
        public void ConnectDoor() {
            foreach (KeyValuePair<Direction, Room> pair in _neighbours) {
                Room neighbourRoom = pair.Value;
                if (neighbourRoom == null) continue;
                Direction direction = pair.Key;
                int i = (int)pair.Key + 2;
                if (i > 3) i -= 3;
                Direction opposite = (Direction)i;

                _doors[direction].ConnectedDoor = neighbourRoom.GetDoor(opposite);
            }
        }

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
            Door door = new Door(this, direction, tiles);
            _doors[direction] = door;
            return door;
        }

        #endregion
    }


}