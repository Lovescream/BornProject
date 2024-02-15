using DungeonGenerate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace ZerolizeDungeon {
    [Flags]
    public enum RoomDirection {
        None = 0,
        Top = 1,         // 1 << 0
        Right = 2,      // 1 << 1
        Bottom = 4,       // 1 << 2
        Left = 8,       // 1 << 3
        Everything = Top | Right | Bottom | Left,
    }
    public enum RoomType {
        Normal,
        Start,
        Treasure,
        Shop,
        Boss,
    }

    public class Room : MonoBehaviour {

        #region Inspector

        [SerializeField]
        private RoomDirection _direction;
        [SerializeField]
        private Vector2Int _size;

        #endregion

        #region Properties

        // Room Info.
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int Index => new(X, Y);
        public string Key => this.gameObject.name;
        public RoomDirection Direction => (int)_direction == -1 ? (RoomDirection)15 : _direction;
        public Vector2Int Size => _size;
        public int Width => Size.x;
        public int Height => Size.y;
        public Vector2 OriginPosition => new(X * Width, Y * Height);
        public Vector2 CenterPosition => OriginPosition + new Vector2(Width / 2, Height / 2);
        public Vector2 MaxPosition => OriginPosition + new Vector2(Width, Height);
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

        // Neighbours Info.
        public Room Top => _neighbours[ZerolizeDungeon.Direction.Top];
        public Room Right => _neighbours[ZerolizeDungeon.Direction.Right];
        public Room Bottom => _neighbours[ZerolizeDungeon.Direction.Bottom];
        public Room Left => _neighbours[ZerolizeDungeon.Direction.Left];

        #endregion

        #region Fields

        // State.
        private bool _isOpened = true;

        // Collections.
        private Dictionary<Direction, Debris> _debris = new();
        private Dictionary<Direction, Room> _neighbours = new();

        // Callbacks.
        public event Action<Room> OnRoomOpened;
        public event Action<Room> OnRoomClosed;

        private bool _isInitialized;

        #endregion

        #region Initialize / Set

        public virtual bool Initialize() {
            if (_isInitialized) return false;
            _isInitialized = true;

            Tilemap[] tilemaps = this.transform.Find("Walls").GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
                tilemap.gameObject.layer = Main.WallLayer;

            return true;
        }

        public virtual void SetInfo(Dungeon dungeon, RoomGenerateData data) {
            Initialize();

            // #1. Room 설정.
            this.X = data.X;
            this.Y = data.Y;
            this.transform.name = $"Room[{X}, {Y}]";
            this.transform.position = OriginPosition;

            // #2. 컬렉션 초기화.
            for (int i = 0; i < (int)ZerolizeDungeon.Direction.COUNT; i++) {
                _debris[(Direction)i] = null;
                _neighbours[(Direction)i] = null;
            }

            // #3. 타입 설정.
            SetType(dungeon, data);

            // #4. 통로 설정.
            for (int i = 0; i < 4; i++) {
                if (((int)Direction & (1 << i)) == 0) continue;
                Debris debris = Main.Resource.Instantiate("Debris", this.transform, true).GetComponent<Debris>();
                debris.transform.SetParent(this.transform);
                debris.transform.localPosition = (Direction)i switch {
                    ZerolizeDungeon.Direction.Top => new(15, 28),
                    ZerolizeDungeon.Direction.Right => new(28, 15),
                    ZerolizeDungeon.Direction.Bottom => new(15, 2),
                    ZerolizeDungeon.Direction.Left => new(2, 15),
                    _ => new(0, 0)
                };
                _debris[(Direction)i] = debris;
            }
            OnRoomOpened += r => OpenDoor();
            OnRoomClosed += r => CloseDoor();
            IsOpened = false;
            IsOpened = true;
        }

        #endregion

        #region Neighbours

        public void SetNeighbours(Room[] rooms) {
            for (int i = 0; i < rooms.Length; i++)
                _neighbours[(Direction)i] = rooms[i];
        }

        #endregion

        public bool IsInRoom(Vector2 position) => position.IsInRange(OriginPosition, MaxPosition);

        public Room GetRandomNeighbour() {
            Room[] rooms = _neighbours.Values.ToArray();
            return rooms[Random.Range(0, rooms.Length - 1)];
        }

        #region Door

        public Debris GetDoor(Direction direction) {
            if (!_debris.TryGetValue(direction, out Debris debris)) return null;
            return debris;
        }
        public void OpenDoor(Direction direction) {
            if (_debris[direction] == null) return;
            _debris[direction].gameObject.SetActive(false);
        }
        public void CloseDoor(Direction direction) {
            if (_debris[direction] == null) return;
            _debris[direction].gameObject.SetActive(true);
        }
        public void OpenDoor() {
            for (int i = 0; i < 4; i++) OpenDoor((Direction)i);
        }
        public void CloseDoor() {
            for (int i = 0; i < 4; i++) CloseDoor((Direction)i);
        }

        #endregion

        #region Type

        private void SetType(Dungeon dungeon, RoomGenerateData data) {
            if (dungeon.StartRoom == null && data.DistanceFromStart == 0) {
                Type = RoomType.Start;
                return;
            }
            if (dungeon.BossRoom == null && data.DistanceFromStart == dungeon.FarthestDistance) {
                Type = RoomType.Boss;
                return;
            }
            if (dungeon.TreasureRoom == null && data.NeighbourCount == 1) {
                Type = RoomType.Treasure;
                return;
            }
            Type = RoomType.Normal;
        }

        #endregion
    }
}