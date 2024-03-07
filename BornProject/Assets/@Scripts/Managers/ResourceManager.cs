using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZerolizeDungeon;
using Object = UnityEngine.Object;
public class ResourceManager {
    private Dictionary<Type, Dictionary<string, Object>> _resources = new();

    private bool _isInitialized;
    public void Initialize() {
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
}