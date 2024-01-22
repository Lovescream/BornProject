using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerate {
    public class RoomGenerateData {

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2Int Index => new(X, Y);
        public int NeighbourInfo { get; private set; }
        public int NeighbourCount {
            get {
                int count = 0;
                for (int i = 0; i < 4; i++) {
                    if ((NeighbourInfo & (1 << i)) != 0) count++;
                }
                return count;
            }
        }
        public int DistanceFromStart { get; private set; }
        public RoomGenerateData(Vector2Int index, Vector2Int size, int neighbourInfo, int distanceFromStart) {
            X = index.x;
            Y = index.y;
            Width = size.x;
            Height = size.y;
            NeighbourInfo = neighbourInfo;
            DistanceFromStart = distanceFromStart;
        }
        public void UpdateIndex(Vector2Int origin) {
            X -= origin.x;
            Y -= origin.y;
        }
    }
}