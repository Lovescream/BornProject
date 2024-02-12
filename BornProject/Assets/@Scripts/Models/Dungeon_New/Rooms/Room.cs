using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZerolizeDungeon {
    [Flags]
    public enum RoomDirection {
        None = 0,
        Up = 1,         // 1 << 0
        Right = 2,      // 1 << 1
        Down = 4,       // 1 << 2
        Left = 8,       // 1 << 3
    }

    public class Room : MonoBehaviour {

        #region Properties

        public string Key => this.gameObject.name;
        public RoomDirection Direction => _direction;

        #endregion

        #region Inspector

        [SerializeField]
        private RoomDirection _direction;

        #endregion
    }
}