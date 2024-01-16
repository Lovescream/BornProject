using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<string, Sprite> _sprites = new();
    private Dictionary<string, GameObject> _prefabs = new();
    private Dictionary<string, TextAsset> _jsonData = new();
    private Dictionary<string, RuntimeAnimatorController> _animControllers = new();

    public void Initialize()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites"); // TODO: 경로 지정.
        foreach (Sprite sprite in sprites)
        {
            _sprites.Add(sprite.name, sprite);
        }

        GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject obj in objs)
        {
            _prefabs.Add(obj.name, obj);
        }

        TextAsset[] texts = Resources.LoadAll<TextAsset>("JsonData");
        foreach (TextAsset t in texts)
        {
            _jsonData.Add(t.name, t);
        }

        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>("Animations");
        foreach (RuntimeAnimatorController controller in controllers)
        {
            _animControllers.Add(controller.name, controller);
        }
    }

    // 리소스가 있는지 확인.
    public GameObject LoadPrefab(string key)
    {
        if (!_prefabs.TryGetValue(key, out GameObject prefab))
        {
            Debug.LogError($"[ResourceManager] LoadPrefab({key}): Failed to load prefab.");
            return null;
        }
        return prefab;
    }
    public Sprite LoadSprite(string key)
    {
        if (!_sprites.TryGetValue(key, out Sprite sprite))
        {
            Debug.LogError($"[ResourceManager] LoadSprite({key}): Failed to load sprite.");
            return null;
        }
        return sprite;
    }
    public TextAsset LoadJsonData(string key)
    {
        if (!_jsonData.TryGetValue(key, out TextAsset data))
        {
            Debug.LogError($"[ResourceManager] LoadJsonData({key}): Failed to load jsonData.");
            return null;
        }
        return data;
    }
    public RuntimeAnimatorController LoadAnimController(string key)
    {
        if (!_animControllers.TryGetValue(key, out RuntimeAnimatorController controller))
        {
            Debug.LogError($"[ResourceManager] LoadJsonData({key}): Failed to load animController.");
            return null;
        }
        return controller;
    }

    // 오브젝트가 풀 안에 있는지 없는지 확인 후 생성.
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = LoadPrefab(key);
        if (prefab == null)
        {
            Debug.LogError($"[ResourceManager] Instantiate({key}): Failed to load prefab.");
            return null;
        }

        if (pooling) return Main.Pool.Pop(prefab);

        GameObject obj = GameObject.Instantiate(prefab, parent);
        obj.name = prefab.name;
        return obj;
    }

    // 필요없는 오브젝트 파괴.
    public void Destroy(GameObject obj)
    {
        if (obj == null) return;

        if (Main.Pool.Push(obj)) return;

        Object.Destroy(obj);
    }
}