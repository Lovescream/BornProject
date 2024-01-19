using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerate {
    public class RoomGenerateData {

        public int X { get; private set; }
        public int Y { get; private set; }
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
        public RoomGenerateData(Vector2Int index, int neighbourInfo, int distanceFromStart) {
            X = index.x;
            Y = index.y;
            NeighbourInfo = neighbourInfo;
            DistanceFromStart = distanceFromStart;
        }
    }
}