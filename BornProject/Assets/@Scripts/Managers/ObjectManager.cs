using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectManager {
    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new();
    public HashSet<HitCollider> HitColliders { get; private set; } = new();

    public void Clear() {
        Player = null;
        Enemies.Clear();
        HitColliders.Clear();
    }

    public Player SpawnPlayer(string key, Vector2 position) {
        Player = Spawn<Player>(position);
        Player.SetInfo(Main.Data.Creatures[key]);
        return Player;
    }
    public Enemy SpawnEnemy(string key, Vector2 position) {
        if (string.IsNullOrEmpty(key)) return null;
        GameObject obj = Main.Resource.IsExist<GameObject>(key) ? Main.Resource.Instantiate($"{key}", pooling: true) : Main.Resource.Instantiate("Enemy", pooling: true);
        //GameObject obj = Main.Resource.IsExistPrefab(key) ? Main.Resource.Instantiate($"{key}", pooling: true) : Main.Resource.Instantiate("Enemy", pooling: true);
        obj.transform.position = position;
        Enemy enemy = obj.GetComponent<Enemy>();
        Enemies.Add(enemy);
        enemy.SetInfo(Main.Data.Creatures[key]);
        return enemy;
    }
    
    public void DespawnPlayer(Player player = null) {
        if (player == null) player = Player;
        if (player == null) return;
        Player = null;
        Despawn(player);
    }
    public void DespawnEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
        Despawn(enemy);
    }
    public HitCollider SpawnHitCollider(string key, string prefabKey, HitColliderInfo info, HitInfo hitInfo) {
        if (string.IsNullOrEmpty(prefabKey)) {
            Debug.LogError($"[ObjectManager] SpawnHitCollider({prefabKey}): Spawn Failed. the key is null or empty.");
            return null;
        }
        //GameObject obj = Main.Resource.Instantiate(Main.Resource.IsExistPrefab(prefabKey) ? prefabKey : "HitCollider_Base", pooling: true);
        GameObject obj = Main.Resource.Instantiate(Main.Resource.IsExist<GameObject>(prefabKey) ? prefabKey : "HitCollider_Base", pooling: true);
        obj.transform.position = Vector2.zero;

        HitCollider hitCollider = obj.GetComponent<HitCollider>();
        hitCollider.SetInfo(key, info, hitInfo);
        HitColliders.Add(hitCollider);
        return hitCollider;
    }
    public void DespawnHitCollider(HitCollider hitCollider) {
        HitColliders.Remove(hitCollider);
        Despawn(hitCollider);
    }

    public void ShowDamageText(Vector2 position, float damage) {
        GameObject obj = Main.Resource.Instantiate("DamageText", pooling: true);
        DamageText text = obj.GetOrAddComponent<DamageText>();
        text.SetInfo(position, damage);
    }
    public void ShowHpBar(Creature creature, float hp, float from = -1) {
        UI_HpBar bar = creature.GetComponentInChildren<UI_HpBar>(false);
        if (bar == null)
            bar = Main.Resource.Instantiate("UI_HpBar", pooling: true).GetOrAddComponent<UI_HpBar>();
        bar.SetInfo(creature, hp, from);
    }

    private T Spawn<T>(Vector2 position) where T : Entity {
        Type type = typeof(T);

        string prefabName = null;
        while (type != null) {
            prefabName = type.Name;
            //if (Main.Resource.LoadPrefab(prefabName) != null) break;
            if (Main.Resource.IsExist<GameObject>(prefabName)) break;
            type = type.BaseType;
        }
        if (string.IsNullOrEmpty(prefabName)) prefabName = "Thing";

        GameObject obj = Main.Resource.Instantiate($"{prefabName}", pooling: true);
        obj.transform.position = position;

        return obj.GetOrAddComponent<T>();
    }
    private T Spawn<T>(string key, Vector2 position) where T : Entity {
        if (string.IsNullOrEmpty(key)) {
            Debug.LogError($"[ObjectManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. the key is null or empty.");
            return null;
        }
        GameObject obj = Main.Resource.Instantiate(key, pooling: true);
        if (obj == null) {
            Debug.LogError($"[ObjectManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. Failed to load prefab.");
            return null;
        }
        obj.transform.position = position;

        return obj.GetComponent<T>();
    }

    private void Despawn<T>(T obj) where T : Entity {
        Main.Resource.Destroy(obj.gameObject);
    }
}
