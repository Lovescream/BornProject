using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager {
    public bool IsInitialized { get; private set; } = false;

    private Dictionary<string, Sprite> _sprites = new();
    private Dictionary<string, GameObject> _prefabs = new();
    private Dictionary<string, TextAsset> _jsonData = new();
    private Dictionary<string, RuntimeAnimatorController> _animControllers = new();
    private Dictionary<string, ZerolizeDungeon.Room> _rooms = new();
    private Dictionary<string, AudioClip> _clips = new();

    private bool _isInitialized;
    public void Initialize() {
        if (_isInitialized) return;
        _isInitialized = true;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites"); // TODO: 경로 지정.
        foreach (Sprite sprite in sprites) {
            _sprites.Add(sprite.name, sprite);
        }

        GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject obj in objs) {
            _prefabs.Add(obj.name, obj);
        }

        TextAsset[] texts = Resources.LoadAll<TextAsset>("JsonData");
        foreach (TextAsset t in texts) {
            _jsonData.Add(t.name, t);
        }

        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>("Animations");
        foreach (RuntimeAnimatorController controller in controllers) {
            _animControllers.Add(controller.name, controller);
        }

        ZerolizeDungeon.Room[] rooms = Resources.LoadAll<ZerolizeDungeon.Room>("Rooms");
        foreach (ZerolizeDungeon.Room room in rooms) {
            _rooms.Add(room.Key, room);
        }

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (AudioClip clip in clips) _clips.Add(clip.name, clip);

        IsInitialized = true;
    }

    // 리소스가 있는지 확인.
    public bool IsExistPrefab(string key) => _prefabs.ContainsKey(key);
    public GameObject LoadPrefab(string key) {
        if (!_prefabs.TryGetValue(key, out GameObject prefab)) {
            Debug.LogError($"[ResourceManager] LoadPrefab({key}): Failed to load prefab.");
            return null;
        }
        return prefab;
    }
    public Sprite LoadSprite(string key) {
        if (!_sprites.TryGetValue(key, out Sprite sprite)) {
            Debug.LogError($"[ResourceManager] LoadSprite({key}): Failed to load sprite.");
            return null;
        }
        return sprite;
    }
    public TextAsset LoadJsonData(string key) {
        if (!_jsonData.TryGetValue(key, out TextAsset data)) {
            Debug.LogError($"[ResourceManager] LoadJsonData({key}): Failed to load jsonData.");
            return null;
        }
        return data;
    }
    public RuntimeAnimatorController LoadAnimController(string key) {
        if (!_animControllers.TryGetValue(key, out RuntimeAnimatorController controller)) {
            Debug.LogError($"[ResourceManager] LoadAnimController({key}): Failed to load animController.");
            return null;
        }
        return controller;
    }
    public AudioClip LoadAudioClip(string key) {
        if (!_clips.TryGetValue(key, out AudioClip clip)) {
            Debug.LogError($"[ResourceManager] LoadAudioClip({key}): Failed to load AudioClip.");
            return null;
        }
        return clip;
    }

    #region Load Rooms

    public List<ZerolizeDungeon.Room> LoadRoom(ZerolizeDungeon.RoomDirection direction) {
        if ((int)direction == -1) direction = (ZerolizeDungeon.RoomDirection)15;
        return _rooms.Values.Where(r => r.Direction == direction).ToList();
    }

    #endregion

    // 오브젝트가 풀 안에 있는지 없는지 확인 후 생성.
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false) {
        GameObject prefab = LoadPrefab(key);
        if (prefab == null) {
            Debug.LogError($"[ResourceManager] Instantiate({key}): Failed to load prefab.");
            return null;
        }
        if (pooling) return Main.Pool.Pop(prefab);
        GameObject obj = GameObject.Instantiate(prefab, parent);
        obj.name = prefab.name;
        return obj;
    }

    // 필요없는 오브젝트 파괴.
    public void Destroy(GameObject obj) {
        if (obj == null) return;

        if (Main.Pool.Push(obj)) return;

        Object.Destroy(obj);
    }
}