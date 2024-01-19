using Dijkstra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGenerate {
    public class DungeonGenerator {

        #region Properties

        // Generate Info.
        public int Width { get; set; }
        public int Height { get; set; }
        public int Count { get; set; }
        public Vector2Int Min => Vector2Int.zero;
        public Vector2Int Max => new(Width - 1, Height - 1);

        // Generate Result.
        public Vector2Int Start { get; private set; }
        //public HashSet<Vector2Int> Indexes => _roomIndexes;
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

        public DungeonGenerator(int width, int height, int count) {
            Width = width;
            Height = height;
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

        public void Generate() {
            Vector2Int index = new(Width / 2, Height / 2);
            Start = index;
            AddRoomIndex(index);

            for (int i = 1; i < Count; i++) {
                UpdatePotentials(index);
                index = SelectRandomIndex();
                AddRoomIndex(index);
            }

            Graph graph = new(_roomIndexes, Start);
            Dictionary<Vector2Int, int> costs = graph.GetCost();

            foreach (Vector2Int roomIndex in _roomIndexes) {
                int neighbourInfo = 0;
                for (int i = 0; i < (int)Direction.COUNT; i++) {
                    if (_roomIndexes.Contains(roomIndex.GetDirectionIndex((Direction)i)))
                        neighbourInfo |= (1 << i);
                }
                //_datas.Add(new(roomIndex, neighbourInfo, roomIndex.Distance(Start)));
                _datas.Add(new(roomIndex, neighbourInfo, costs[roomIndex]));
            }
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

        private Vector2Int SelectRandomIndex() {
            return _openIndexes.ElementAt(Random.Range(0, _openIndexes.Count));
        }
    }
}