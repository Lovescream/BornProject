using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using ZerolizeDungeon;
using Object = UnityEngine.Object;
public class ResourceManager {
    //private Dictionary<string, Sprite> _sprites = new();
    //private Dictionary<string, GameObject> _prefabs = new();
    //private Dictionary<string, TextAsset> _jsonData = new();
    //private Dictionary<string, RuntimeAnimatorController> _animControllers = new();
    //private Dictionary<string, ZerolizeDungeon.Room> _rooms = new();
    //private Dictionary<string, AudioClip> _clips = new();
    private Dictionary<Type, Dictionary<string, Object>> _resources = new();

    private bool _isInitialized;
    public void Initialize() {
        //if (_isInitialized) return;
        //_isInitialized = true;
        //Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites"); // TODO: 경로 지정.
        //foreach (Sprite sprite in sprites) {
        //    _sprites.Add(sprite.name, sprite);
        //}

        //GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs");
        //foreach (GameObject obj in objs) {
        //    _prefabs.Add(obj.name, obj);
        //}

        //TextAsset[] texts = Resources.LoadAll<TextAsset>("JsonData");
        //foreach (TextAsset t in texts) {
        //    _jsonData.Add(t.name, t);
        //}

        //RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>("Animations");
        //foreach (RuntimeAnimatorController controller in controllers) {
        //    _animControllers.Add(controller.name, controller);
        //}

        //Room[] rooms = Resources.LoadAll<Room>("Rooms");
        //foreach (Room room in rooms) {
        //    _rooms.Add(room.Key, room);
        //}

        //AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        //foreach (AudioClip clip in clips) _clips.Add(clip.name, clip);

        // ======

        if (_isInitialized) return;
        _isInitialized = true;
        LoadResource<Sprite>("Sprites");
        LoadResource<GameObject>("Prefabs");
        LoadResource<TextAsset>("JsonData");
        LoadResource<RuntimeAnimatorController>("Animations");
        LoadResource<Room>("Rooms", x => x.Key);
        LoadResource<AudioClip>("Sounds");

    }
    private void LoadResource<T>(string path, Func<T, string> keyFinder = null) where T : Object {
        keyFinder ??= x => x.name;
        _resources[typeof(T)] = Resources.LoadAll<T>(path).ToDictionary(keyFinder, x => (Object)x);
        //Dictionary<string, Object> dictionary;
        //if (!_resources.TryGetValue(typeof(T), out dictionary)) {
        //    dictionary = new();
        //    _resources[typeof(T)] = dictionary;
        //}
        //T[] objs = Resources.LoadAll<T>(path);
        //foreach (T obj in objs)
        //    dictionary.Add(keyFinder != null ? keyFinder(obj) : obj.name, obj);
    }
    public bool IsExist<T>(string key) where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) return false;
        return dictionary.ContainsKey(key);
    }
    public T Get<T>(string key) where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) {
            Debug.LogError($"[ResourceManager] Get<{typeof(T)}>({key}): Failed to load resource. The Resource of type {typeof(T)} does not exist.");
            return null;
        }
        if (!dictionary.TryGetValue(key, out Object resource)) {
            Debug.LogError($"[ResourceManager] Get<{typeof(T)}>({key}): Failed to load resource. The Resource with the key {key} does not exist.");
            return null;
        }
        return resource as T;
    }
    public List<T> GetAll<T>() where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) {
            Debug.LogError($"[ResourceManager] GetAll<{typeof(T)}>(): Failed to load resource. The Resource of type {typeof(T)} does not exist.");
            return null;
        }
        return dictionary.Values.Select(x => x as T).ToList();
    }
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false) {
        GameObject prefab = Get<GameObject>(key);
        if (prefab == null) {
            Debug.LogError($"[ResourceManager] Instantiate({key}): Failed to load prefab.");
            return null;
        }
        if (pooling) return Main.Pool.Pop(prefab);
        GameObject obj = GameObject.Instantiate(prefab, parent);
        obj.name = prefab.name;
        return obj;
    }
    public void Destroy(GameObject obj) {
        if (obj == null) return;
        if (Main.Pool.Push(obj)) return;
        Object.Destroy(obj);
    }



    //// 오브젝트가 풀 안에 있는지 없는지 확인 후 생성.
    //public GameObject Instantiate(string key, Transform parent = null, bool pooling = false) {
    //    GameObject prefab = LoadPrefab(key);
    //    if (prefab == null) {
    //        Debug.LogError($"[ResourceManager] Instantiate({key}): Failed to load prefab.");
    //        return null;
    //    }
    //    if (pooling) return Main.Pool.Pop(prefab);
    //    GameObject obj = GameObject.Instantiate(prefab, parent);
    //    obj.name = prefab.name;
    //    return obj;
    //}

    // 필요없는 오브젝트 파괴.

    //// 리소스가 있는지 확인.
    //public bool IsExistPrefab(string key) => _prefabs.ContainsKey(key);
    //public bool IsExistAudioClip(string key) => _clips.ContainsKey(key);
    //public GameObject LoadPrefab(string key) {
    //    if (!_prefabs.TryGetValue(key, out GameObject prefab)) {
    //        Debug.LogError($"[ResourceManager] LoadPrefab({key}): Failed to load prefab.");
    //        return null;
    //    }
    //    return prefab;
    //}
    //public Sprite LoadSprite(string key) {
    //    if (!_sprites.TryGetValue(key, out Sprite sprite)) {
    //        Debug.LogError($"[ResourceManager] LoadSprite({key}): Failed to load sprite.");
    //        return null;
    //    }
    //    return sprite;
    //}
    //public TextAsset LoadJsonData(string key) {
    //    if (!_jsonData.TryGetValue(key, out TextAsset data)) {
    //        Debug.LogError($"[ResourceManager] LoadJsonData({key}): Failed to load jsonData.");
    //        return null;
    //    }
    //    return data;
    //}
    //public RuntimeAnimatorController LoadAnimController(string key) {
    //    if (!_animControllers.TryGetValue(key, out RuntimeAnimatorController controller)) {
    //        Debug.LogError($"[ResourceManager] LoadAnimController({key}): Failed to load animController.");
    //        return null;
    //    }
    //    return controller;
    //}
    //public AudioClip LoadAudioClip(string key) {
    //    if (!_clips.TryGetValue(key, out AudioClip clip)) {
    //        Debug.LogError($"[ResourceManager] LoadAudioClip({key}): Failed to load AudioClip.");
    //        return null;
    //    }
    //    return clip;
    //}
    //#region Load Rooms

    //public List<Room> LoadRoom(RoomDirection direction) {
    //    if ((int)direction == -1) direction = (RoomDirection)15;
    //    return _rooms.Values.Where(r => r.Direction == direction).ToList();
    //}

    //#endregion
}