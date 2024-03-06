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

    public static bool IsInitialized => _instance != null;

    private PoolManager _pool = new();
    private DataManager _data = new();
    private ResourceManager _resource = new();
    private ObjectManager _object = new();
    private DungeonManager _dungeon = new();
    private UIManager _ui = new();
    private QuestManager _quest = new();
    private SceneManagerEx _scene = new();
    private GameManager _game = new();
    private AudioManager _audio = new();

    public static PoolManager Pool => Instance?._pool;
    public static DataManager Data => Instance?._data;
    public static ResourceManager Resource => Instance?._resource;
    public static ObjectManager Object => Instance?._object;
    public static DungeonManager Dungeon => Instance?._dungeon;
    public static UIManager UI => Instance?._ui;
    public static QuestManager Quest => Instance?._quest;
    public static SceneManagerEx Scene => Instance?._scene;
    public static GameManager Game => Instance?._game;
    public static AudioManager Audio => Instance?._audio;

    public static void Clear() {
        // Audio.Clear();
        UI.Clear();
        Pool.Clear();
        Object.Clear();
    }

    public static void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Application.OpenURL("https://lovescream.itch.io/zerolize-alpha");
#else
        Application.Quit();
#endif
    }

    public void ManualInitialize() {
        _pool = new();
        _data = new();
        _resource = new();
        _object = new();
        _dungeon = new();
        _ui = new();
        _quest = new();
        _scene = new();
        _game = new();
        _audio = new();
    }
}
public static class Layers {
    public static readonly int WallLayer = LayerMask.NameToLayer("DungeonWalls");
    public static readonly int CreatureLayer = LayerMask.NameToLayer("Creatures");
    public static readonly int HitColliderLayer = LayerMask.NameToLayer("HitColliders");
}