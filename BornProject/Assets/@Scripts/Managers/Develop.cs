using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Develop : MonoBehaviour {

    #region Singleton

    private static Develop _instance;
    private static bool _initialized;

    public static Develop Instance {
        get {
            if (!_initialized) {
                _initialized = true;

                GameObject obj = GameObject.Find("@Develop");
                if (obj == null) {
                    obj = new() { name = "@Develop" };
                    obj.AddComponent<Develop>();
                }
                _instance = obj.GetComponent<Develop>();
                Resource.Initialize();
                Data.Initialize();
            }
            return _instance;
        }
    }

    #endregion


    private PoolManager _pool = new();
    private DataManager _data = new();
    private ResourceManager _resource = new();
    private ObjectManager _object = new();
    private DungeonManager _dungeon = new();

    public static PoolManager Pool => Instance?._pool;
    public static DataManager Data => Instance?._data;
    public static ResourceManager Resource => Instance?._resource;
    public static ObjectManager Object => Instance?._object;
    public static DungeonManager Dungeon => Instance?._dungeon;

}