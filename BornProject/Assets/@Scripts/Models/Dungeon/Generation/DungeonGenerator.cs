using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZerolizeDungeon {
    public enum Direction {
        Top,
        Right,
        Bottom,
        Left,
        COUNT,
    }

    public class DungeonGenerator {

        #region Properties

        public int DungeonWidth { get; set; }
        public int DungeonHeight { get; set; }
        public int RoomWidth { get; set; }
        public int RoomHeight { get; set; }
        public int Count { get; set; }
        public Vector2Int Min => Vector2Int.zero;
        public Vector2Int Max => new(DungeonWidth - 1, DungeonHeight - 1);

        // Generate Result.
        public Vector2Int Start { get; private set; }
        public HashSet<RoomGenerateData> Result => _datas;

        #endregion

        #region Fields

        // Collections.
        private HashSet<Vector2Int> _openIndexes = new();
        private HashSet<Vector2Int> _closedIndexes = new();
        private HashSet<Vector2Int> _roomIndexes = new();
        private HashSet<RoomGenerateData> _datas = new();

        #endregion

        #region Constructor

        public DungeonGenerator(int dungeonWidth, int dungeonHeight, int roomWidth, int roomHeight, int count) {
            DungeonWidth = dungeonWidth;
            DungeonHeight = dungeonHeight;
            RoomWidth = roomWidth;
            RoomHeight = roomHeight;
            Count = count;
        }

        #endregion

        public void Clear() {
            _openIndexes.Clear();
            _closedIndexes.Clear();
            _roomIndexes.Clear();
            _datas.Clear();
            Start = Vector2Int.zero;
        }

        public bool Generate() {
            Debug.Log("Generate");
            Vector2Int index = new(DungeonWidth / 2, DungeonHeight / 2);
            Start = index;
            AddRoomIndex(index);

            for (int i=1;i<Count;i++) {
                UpdatePotentials(index);

                Vector2Int? _index = SelectRandomIndex();
                if (_index == null) {
                    Debug.LogError($"[DungeonGenerator] Generate(): Dungeon generation failed. The size of dungeon is too small for the count of rooms.");
                    Clear();
                    return false;
                }
                index = (Vector2Int)_index;
                AddRoomIndex(index);
            }

            Dijkstra.Graph graph = new(_roomIndexes, Start);
            Dictionary<Vector2Int, int> costs = graph.GetCost();

            foreach (Vector2Int roomIndex in _roomIndexes) {
                int neighbourInfo = 0;
                for (int i = 0; i < (int)Direction.COUNT; i++)
                    if (_roomIndexes.Contains(roomIndex.GetDirectionIndex((Direction)i)))
                        neighbourInfo |= (1 << i);
                _datas.Add(new(roomIndex, new(RoomWidth, RoomHeight), neighbourInfo, costs[roomIndex]));
            }

            int originX = _datas.OrderBy(info => info.X).FirstOrDefault().X;
            int originY = _datas.OrderBy(info => info.Y).FirstOrDefault().Y;
            foreach (RoomGenerateData data in _datas) {
                data.UpdateIndex(new(originX, originY));
            }
            return true;
        }

        private void AddRoomIndex(Vector2Int index) {
            if (!_roomIndexes.Contains(index)) _roomIndexes.Add(index);
            if (_openIndexes.Contains(index)) _openIndexes.Remove(index);
            if (!_closedIndexes.Contains(index)) _closedIndexes.Add(index);
        }

        private void UpdatePotentials(Vector2Int index) {
            foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
                UpdateIndex(index.GetDirectionIndex(direction));
            }
        }

        private void UpdateIndex(Vector2Int index) {
            // #1. 범위 밖 Index는 고려하지 않는다.
            if (!index.IsInRange(Min, Max)) return;

            // #2. 이미 닫힌 Index는 고려하지 않는다.
            if (_closedIndexes.Contains(index)) return;

            // #3. 이미 고려한 Index는 닫는다.
            if (_openIndexes.Contains(index)) {
                _openIndexes.Remove(index);
                _closedIndexes.Add(index);
                return;
            }

            // #4. 방이 확정된 Index는 닫는다.
            if (_roomIndexes.Contains(index)) {
                _closedIndexes.Add(index);
                return;
            }

            // #5. 방이 될 수 있는 목록에 넣는다.
            _openIndexes.Add(index);
        }

        private Vector2Int? SelectRandomIndex() {
            if (_openIndexes.Count <= 0) {
                Debug.LogError($"[DungeonGenerator] SelectRandomIndex(): _openIndexes.Count = {_openIndexes}");
                return null;
            }
            return _openIndexes.ElementAt(Random.Range(0, _openIndexes.Count));
        }
    }

    
}