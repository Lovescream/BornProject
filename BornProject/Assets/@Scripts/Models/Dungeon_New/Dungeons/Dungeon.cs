using DungeonGenerate;
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
                Room newRoom = GenerateRoom(data.NeighbourInfo);
                newRoom.transform.SetParent(root);
                newRoom.SetInfo(this, data);
                _rooms[new(data.X, data.Y)] = newRoom;
                if (newRoom.Type == RoomType.Start) StartRoom = newRoom;
                else if (newRoom.Type == RoomType.Boss) BossRoom = newRoom;
                else if (newRoom.Type == RoomType.Treasure) TreasureRoom = newRoom;
            }

        }

        private Room GenerateRoom(int direction) {
            List<Room> rooms = Main.Resource.LoadRoom((RoomDirection)direction);
            Room room = rooms[Random.Range(0, rooms.Count - 1)];
            return GameObject.Instantiate(room);
        }
    }

}