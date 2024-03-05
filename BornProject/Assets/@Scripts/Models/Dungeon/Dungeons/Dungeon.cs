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
                Room newRoom = GenerateRoom(data.NeighbourInfo, GetType(data));
                newRoom.transform.SetParent(root);
                newRoom.SetInfo(this, data, GetType(data));
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

        private Room GenerateRoom(int direction, RoomType type) {
            List<Room> rooms = Main.Resource.LoadRoom((RoomDirection)direction);

            Room room;
            if (type == RoomType.Start) {
                room = rooms.Where(x => {
                    string[] s = x.Key.Split('_');
                    if (s.Length != 3) return false;
                    if (s[1] != "Basic") return false;
                    return true;
                }).FirstOrDefault();
            }
            else if (type == RoomType.Boss) {
                room = rooms.Where(x => {
                    string[] s = x.Key.Split('_');
                    if (s.Length < 4) return false;
                    if (s[3] != "Boss") return false;
                    return true;
                }).FirstOrDefault();
            }
            else {
                rooms = rooms.Where(x => {
                    string[] s = x.Key.Split('_');
                    if (s.Length > 3 && s[3] == "Boss") return false;
                    if (s[1] == "Basic") return false;
                    return true;
                }).ToList();
                room = rooms.Count > 0 ? rooms[Random.Range(0, rooms.Count)] : null;
            }

            return room != null ? GameObject.Instantiate(room) : null;
        }

        private RoomType GetType(RoomGenerateData data) {
            if (StartRoom == null && data.DistanceFromStart == 0) return RoomType.Start;
            if (BossRoom == null && data.DistanceFromStart == FarthestDistance) return RoomType.Boss;
            return RoomType.Normal;
        }

        public Vector2Int GetRelativeIndex(Room room) {
            if (StartRoom == null) {
                Debug.LogError($"[Dungeon] GetReleativeIndex({room}): Not found start room.");
                return Vector2Int.zero;
            }
            return room.Index - StartRoom.Index;
        }
    }


}