using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGenerate {
    public class Dungeon {

        public Room StartRoom { get; private set; }
        public Room BossRoom { get; private set; }
        public Room TreasureRoom { get; private set; }

        private Dictionary<Vector2Int, Room> _rooms = new();

        private HashSet<RoomGenerateData> _generateData;

        public Dungeon(HashSet<RoomGenerateData> result) {
            _generateData = result;
            foreach (RoomGenerateData data in _generateData) {
                Vector2Int index = new(data.X, data.Y);
                Room room = new(GetRoomData(data), index);
                _rooms[index] = room;
                if (room.Type == RoomType.Start) StartRoom = room;
                else if (room.Type == RoomType.Boss) BossRoom = room;
                else if (room.Type == RoomType.Treasure) TreasureRoom = room;
            }
            //foreach (Vector2Int index in result) {
            //    Room newRoom = new(GetRoomData(), index);
            //    _rooms[index] = newRoom;
            //}

            foreach (Room room in _rooms.Values) {
                Room[] neighbours = new Room[4];
                for (int i=0;i<(int)Direction.COUNT;i++) {
                    Vector2Int neighbourIndex = room.Index.GetDirectionIndex((Direction)i);
                    neighbours[i] = this[neighbourIndex];
                }
                room.SetNeighbours(neighbours);
                room.GenerateDoors();
            }
            foreach (Room room in _rooms.Values) {
                room.ConnectDoor();
            }
            foreach (Room room in _rooms.Values) {
                RoomObject roomObject = Main.Resource.Instantiate("Room").GetComponent<RoomObject>();
                roomObject.SetInfo(room);
            }
        }

        public Room this[Vector2Int index] {
            get {
                if (!_rooms.TryGetValue(index, out Room room)) return null;
                return room;
            }
        }
        public Room this[int x, int y] => this[new(x, y)];

        private RoomData GetRoomData(RoomGenerateData generateData) {
            return new() {
                Key = "Room00",
                TilemapKey = "Room00",
                Type = GetRoomType(generateData),
                Width = 20,
                Height = 20,
            };
        }
        private RoomType GetRoomType(RoomGenerateData generateData) {
            if (StartRoom == null) {
                if (generateData.DistanceFromStart == 0) return RoomType.Start;
            }

            if (BossRoom == null) {
                int farthestDistance = _generateData.Max(x => x.DistanceFromStart);
                HashSet<RoomGenerateData> farthests = _generateData.Where(x => x.DistanceFromStart == farthestDistance).ToHashSet();
                if (farthests.Contains(generateData)) return RoomType.Boss;
            }

            if (TreasureRoom == null) {
                if (generateData.NeighbourCount == 1) return RoomType.Treasure;
            }

            return RoomType.Normal;
        }
    }
}