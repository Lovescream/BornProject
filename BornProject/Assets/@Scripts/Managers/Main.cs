using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    #region Singleton

    private static Main _instance;

    public static Main Instance {
        get {
            if (_instance == null ) {
                GameObject obj = GameObject.Find("@Main");
                if (obj == null) {
                    obj = new("@Main");
                    obj.AddComponent<Main>();
                }
                DontDestroyOnLoad(obj);
                _instance = obj.GetComponent<Main>();
            }
            return _instance;
        }
    }

    #endregion

    public static bool IsInitialized => Instance != null;

    private PoolManager _pool = new();
    private DataManager _data = new();
    private ResourceManager _resource = new();
    private ObjectManager _object = new();
    private DungeonManager _dungeon = new();
    private UIManager _ui = new();
    private SkillManager _skill = new();
    private QuestManager _quest = new();

    public static PoolManager Pool => Instance?._pool;
    public static DataManager Data => Instance?._data;
    public static ResourceManager Resource => Instance?._resource;
    public static ObjectManager Object => Instance?._object;
    public static DungeonManager Dungeon => Instance?._dungeon;
    public static UIManager UI => Instance?._ui;
    public static SkillManager Skill => Instance?._skill;
    public static QuestManager Quest => Instance?._quest;
    public void ManualInitialize() {
        _pool = new();
        _data = new();
        _resource = new();
        _object = new();
        _dungeon = new();
        _ui = new();
        _skill = new();
    }

    #region Layers

    public static readonly int WallLayer = LayerMask.NameToLayer("DungeonWalls");
    public static readonly int CreatureLayer = LayerMask.NameToLayer("Creatures");
    public static readonly int HitColliderLayer = LayerMask.NameToLayer("HitColliders");

    #endregion
}