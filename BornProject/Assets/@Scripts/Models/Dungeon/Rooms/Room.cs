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
    public enum RoomExploreType {
        Current,
        Explored,
        NotExplored,
        Hide,
    }

    public class Room : MonoBehaviour {

        #region Inspector

        [SerializeField]
        private RoomDirection _direction;
        [SerializeField]
        private Vector2Int _size;

        #endregion

        #region Properties

        #region Room Info

        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int Index => new(X, Y);
        public string Key => this.gameObject.name;
        public RoomType Type { get; private set; }

        #endregion

        #region Transform Info

        public Vector2Int Size => _size;
        public int Width => Size.x;
        public int Height => Size.y;
        public Vector2 OriginPosition => new(X * Width, Y * Height);
        public Vector2 CenterPosition => OriginPosition + new Vector2(Width / 2, Height / 2);
        public Vector2 MaxPosition => OriginPosition + new Vector2(Width, Height);
        public RoomFullCollider FullCollider { get; protected set; }
        public RoomInsideCollider InsideCollider { get; protected set; }
        #endregion

        #region Neighbours Info

        public Room[] Neighbours => _neighbours.Values.ToArray();
        public RoomDirection Direction => (int)_direction == -1 ? (RoomDirection)15 : _direction;
        public Room Top => _neighbours[ZerolizeDungeon.Direction.Top];
        public Room Right => _neighbours[ZerolizeDungeon.Direction.Right];
        public Room Bottom => _neighbours[ZerolizeDungeon.Direction.Bottom];
        public Room Left => _neighbours[ZerolizeDungeon.Direction.Left];

        #endregion

        #region State Info

        public bool IsActivated {
            get => _isActivated;
            set {
                _isActivated = value;
                if (value == false) return;
                SpawnEnemy();
            }
        }
        public bool IsOpened {
            get => _isOpened;
            set {
                if (value == _isOpened) return;
                _isOpened = value;
                if (value) OnRoomOpened?.Invoke(this);
                else OnRoomClosed?.Invoke(this);
            }
        }
        public bool IsExplored {
            get => _isExplored;
            set {
                _isExplored = value;
                CheckClear();
            }
        }
        public bool IsClear {
            get => _isClear;
            set {
                _isClear = value;
                IsOpened = value;
            }
        }
        public RoomExploreType ExploreType {
            get => _exploreType;
            set {
                _exploreType = value;
                OnChangeExploreType?.Invoke(value);
            }
        }
        public bool ExistEnemy => _enemies.Count > 0;

        #endregion

        #endregion

        #region Fields

        // State.
        private bool _isActivated = false;
        private bool _isOpened = true;
        private bool _isClear = false;
        private bool _isExplored = false;
        private RoomExploreType _exploreType;

        // Collections.
        private Dictionary<Direction, Debris> _debris = new();
        private Dictionary<Direction, Room> _neighbours = new();
        private List<Enemy> _enemies = new();

        // Components.
        private Transform _objects;

        // Callbacks.
        public event Action<Room> OnRoomOpened;
        public event Action<Room> OnRoomClosed;
        public event Action<RoomExploreType> OnChangeExploreType;

        private bool _isInitialized;

        #endregion

        #region Initialize / Set

        public virtual bool Initialize() {
            if (_isInitialized) return false;
            _isInitialized = true;

            Tilemap[] tilemaps = this.transform.Find("Walls").GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.gameObject.layer = Layers.WallLayer;
                tilemap.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
            }

            _objects = this.transform.Find("Objects");

            return true;
        }

        public virtual void SetInfo(Dungeon dungeon, RoomGenerateData data, RoomType type) {
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
            //SetType(dungeon, data);
            Type = type;
            ExploreType = RoomExploreType.Hide;

            // #4. 통로 설정.
            for (int i = 0; i < 4; i++) {
                if (((int)Direction & (1 << i)) == 0) continue;
                Debris debris = Main.Resource.Instantiate("Debris", this.transform, true).GetComponent<Debris>();
                debris.SetInfo(this, (Direction)i);
                _debris[(Direction)i] = debris;
            }
            OnRoomOpened += r => OpenDoor();
            OnRoomClosed += r => CloseDoor();
            IsOpened = false;
            IsOpened = true;

            // #5. 진입 콜라이더 생성.
            FullCollider = new GameObject().AddComponent<RoomFullCollider>();
            FullCollider.SetInfo(this);
            InsideCollider = new GameObject().AddComponent<RoomInsideCollider>();
            InsideCollider.SetInfo(this);

            // #6. 콜백 등록.
            FullCollider.OnEnteredRoom += room => {
                if (!room.IsActivated) room.IsActivated = true;
                IsExplored = true;
                room.ExploreType = RoomExploreType.Current;
                foreach (Room neighbour in room.Neighbours) {
                    if (neighbour == null) continue;
                    if (neighbour.ExploreType == RoomExploreType.Hide) neighbour.ExploreType = RoomExploreType.NotExplored;
                    if (!neighbour.IsActivated) neighbour.IsActivated = true;
                    foreach (Room neighbourneighbour in neighbour.Neighbours) {
                        if (neighbourneighbour == null) continue;
                        if (!neighbourneighbour.IsActivated) neighbourneighbour.IsActivated = true;
                    }
                }
            };
            FullCollider.OnExitedRoom += room => {
                room.ExploreType = RoomExploreType.Explored;
            };
            InsideCollider.OnEnteredRoom += room => {
                if (!room.IsActivated) room.IsActivated = true;
                if (room.ExistEnemy) room.IsOpened = false;
            };
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
            Room[] rooms = _neighbours.Values.Where(x => x != null).ToArray();
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

        #region Spawn Objects

        private void SpawnEnemy() {
            if (_objects == null) return;
            EnemySpawner[] spawners = _objects.GetComponentsInChildren<EnemySpawner>();
            for (int i = spawners.Length - 1; i >= 0; i--) {
                Enemy enemy = Main.Object.SpawnEnemy(spawners[i].Key, spawners[i].Position);
                _enemies.Add(enemy);
                enemy.OnDead += () => {
                    _enemies.Remove(enemy);
                    CheckClear();
                };
                if (enemy is BoraSongi qjtjtqhRdma)
                    InsideCollider.OnEnteredRoom += room => qjtjtqhRdma.WakeUp();
                Destroy(spawners[i].gameObject);
            }
        }

        #endregion

        #region Type

        //private void SetType(Dungeon dungeon, RoomGenerateData data) {
        //    if (dungeon.StartRoom == null && data.DistanceFromStart == 0) {
        //        Type = RoomType.Start;
        //        return;
        //    }
        //    if (dungeon.BossRoom == null && data.DistanceFromStart == dungeon.FarthestDistance) {
        //        Type = RoomType.Boss;
        //        return;
        //    }
        //    if (dungeon.TreasureRoom == null && data.NeighbourCount == 1) {
        //        Type = RoomType.Treasure;
        //        return;
        //    }
        //    Type = RoomType.Normal;
        //}

        #endregion

        private void CheckClear() {
            if (IsClear) return;
            if (_enemies.Count <= 0) IsClear = true;
            Main.Dungeon.CheckClear();
        }
    }
}