using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZerolizeDungeon {

    public class Dungeon {

        #region Properties

        public Room StartRoom { get; private set; }
        public Room BossRoom { get; private set; }
        public Room TreasureRoom { get; private set; }
        
        public int FarthestDistance { get; private set; }

        public List<Room> Rooms => _rooms.Values.ToList();

        #endregion

        #region Fields

        private Dictionary<Vector2Int, Room> _rooms = new();
        private readonly HashSet<RoomGenerateData> _generateData;

        #endregion

        public Dungeon(HashSet<RoomGenerateData> result, Transform root) {
            _generateData = result;

            FarthestDistance = _generateData.Max(x => x.DistanceFromStart);

            foreach (RoomGenerateData data in _generateData) {
                Room newRoom = GenerateRoom(data.NeighbourInfo, data.DistanceFromStart == 0);
                newRoom.transform.SetParent(root);
                newRoom.SetInfo(this, data);
                _rooms[new(data.X, data.Y)] = newRoom;
                if (newRoom.Type == RoomType.Start) StartRoom = newRoom;
                else if (newRoom.Type == RoomType.Boss) BossRoom = newRoom;
                else if (newRoom.Type == RoomType.Treasure) TreasureRoom = newRoom;
            }

            foreach (Room room in _rooms.Values) {
                Room[] neighbours = new Room[4];
                for (int i = 0; i < (int)Direction.COUNT; i++) {
                    Vector2Int neighbourIndex = room.Index.GetDirectionIndex((Direction)i);
                    neighbours[i] = this[neighbourIndex];
                }
                room.SetNeighbours(neighbours);
            }

        }

        public Room this[Vector2Int index] {
            get {
                if (!_rooms.TryGetValue(index, out Room room)) return null;
                return room;
            }
        }

        private Room GenerateRoom(int direction, bool isStartRoom = false)
        {
            List<Room> rooms = Main.Resource.LoadRoom((RoomDirection)direction);

            Room room;
            if (isStartRoom) {
                room = rooms.Where(x => {
                    string[] s = x.Key.Split('_');
                    if (s.Length != 3) return false;
                    if (s[1] != "Basic") return false;
                    return true;
                }).FirstOrDefault();
            }
            else {
                var nonBasicRooms = rooms.Where(x => !x.Key.Contains("Basic")).ToList();
                room = nonBasicRooms.Count > 0 ? nonBasicRooms[Random.Range(0, nonBasicRooms.Count)] : null;
            }

            return room != null ? GameObject.Instantiate(room) : null;
        }
    }

}