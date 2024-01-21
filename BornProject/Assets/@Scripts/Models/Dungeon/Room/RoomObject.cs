using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerate {

    public class RoomObject : MonoBehaviour {

        #region Properties
        
        public Room Room { get; private set; }
        public RoomType Type => Room.Type;
        public int X => Room.X;
        public int Y => Room.Y;
        public bool IsOpened {
            get => Room.IsOpened;
            set => Room.IsOpened = value;
        }
        public bool IsActivated => Room.IsActivated;
        public bool IsCleared => Room.IsClear;

        #endregion

        #region Fields

        // Collections.
        private Dictionary<string, UnityEngine.Tilemaps.Tile> _tiles = new();

        // Components.
        private Grid _grid;
        private TileMap _floorTilemap;
        private TileMap _wallTilemap;
        private TileMap _doorTilemap;

        // Coroutines.
        private Coroutine _coOpen;
        private Coroutine _coClose;

        private bool _initialized;

        #endregion

        #region MonoBehaviours

        void Awake() {
            Initialize();
        }

        #endregion

        #region Initialize / Set

        public virtual bool Initialize() {
            if (_initialized) return false;

            _grid = new GameObject("Grid").GetOrAddComponent<Grid>();
            _grid.transform.SetParent(this.transform);
            _grid.transform.localPosition = Vector2.zero;

            _floorTilemap = new("Floors", _grid.transform, _tiles, 0, false);
            _wallTilemap = new("Walls", _grid.transform, _tiles, 1, true);
            _doorTilemap = new("Doors", _grid.transform, _tiles, 2, true);

            _initialized = true;
            return true;
        }

        public void SetInfo(Room room) {
            Initialize();

            this.Room = room;
            room.Object = this;
            _tiles = Main.Resource.LoadTileset(Room.Data.TilemapKey);

            _floorTilemap.Clear();
            _floorTilemap.SetTileset(_tiles);
            _wallTilemap.Clear();
            _wallTilemap.SetTileset(_tiles);
            _doorTilemap.Clear();
            _doorTilemap.SetTileset(_tiles);

            foreach (Tile tile in Room.Tiles) {
                tile.OnTileChanged += DrawTile;
                DrawTile(tile);
            }

            this.transform.position = room.OriginPosition;

            // TODO::
            if (Room.Type == RoomType.Start) {
                _floorTilemap.Map.color = new Color(0.25f, 1f, 0.25f, 1);
            }
            else if (Room.Type == RoomType.Boss) {
                _floorTilemap.Map.color = new Color(1f, 0.25f, 0.25f, 1);
            }
            else if (Room.Type == RoomType.Treasure) {
                _floorTilemap.Map.color = new Color(1f, 1f, 0.25f, 1);
            }
        }

        #endregion

        public void Open() {
            if (Room.IsOpened) return;
            if (_coOpen != null) StopCoroutine(_coOpen);
            _coOpen = StartCoroutine(CoOpen());
        }
        public void Close() {
            if (!Room.IsOpened) return;
            if (_coClose != null) StopCoroutine(_coClose);
            _coClose = StartCoroutine(CoClose());
        }

        private void DrawTile(Tile tile) {
            _floorTilemap.SetTile(tile.X, tile.Y, tile.Type == TileType.Floor ? tile.TileName : "");
            _wallTilemap.SetTile(tile.X, tile.Y, tile.Type == TileType.Wall ? tile.TileName : "");
            _doorTilemap.SetTile(tile.X, tile.Y, tile.Type == TileType.Door ? tile.TileName : "");
        }

        private IEnumerator CoOpen() {
            Room.SetDoorState(DoorState.Crack);
            yield return new WaitForSeconds(0.1f);
            Room.SetDoorState(DoorState.Breaking);
            yield return new WaitForSeconds(0.1f);
            Room.SetDoorState(DoorState.Broken);

            IsOpened = true;

            _coOpen = null;
        }
        private IEnumerator CoClose() {
            Room.SetDoorState(DoorState.Breaking);
            yield return new WaitForSeconds(0.1f);
            Room.SetDoorState(DoorState.Crack);
            yield return new WaitForSeconds(0.1f);
            Room.SetDoorState(DoorState.Blocked);

            IsOpened = false;

            _coClose = null;
        }

    }

}