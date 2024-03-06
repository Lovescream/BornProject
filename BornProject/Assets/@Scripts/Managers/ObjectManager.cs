using System.Collections.Generic;
using UnityEngine;

public class ObjectManager {
    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new();
    public HashSet<HitCollider> HitColliders { get; private set; } = new();

    #region General

    public void Clear() {
        Player = null;
        Enemies.Clear();
        HitColliders.Clear();
    }
    private GameObject Spawn(string prefabKey, string defaultKey, bool pooling) {
        if (string.IsNullOrEmpty(prefabKey)) {
            if (string.IsNullOrEmpty(defaultKey)) {
                Debug.LogError($"[ObjectManager] Spawn({prefabKey}, {defaultKey}): Spawn Failed. The key is null or empty.");
                return null;
            }
            return Main.Resource.Instantiate(defaultKey, pooling: pooling);
        }
        if (Main.Resource.IsExist<GameObject>(prefabKey))
            return Main.Resource.Instantiate(prefabKey, pooling: pooling);
        if (string.IsNullOrEmpty(defaultKey)) {
            Debug.LogError($"[ObjectManager] Spawn({prefabKey}, {defaultKey}): Spawn Failed. The key is null or empty.");
            return null;
        }
        if (Main.Resource.IsExist<GameObject>(defaultKey))
            return Main.Resource.Instantiate(defaultKey, pooling: pooling);

        Debug.LogError($"[ObjectManager] Spawn({prefabKey}, {defaultKey}): Spawn Failed. Not found prefab with key.");
        return null;
    }
    private void Despawn<T>(T obj) where T : Entity {
        Main.Resource.Destroy(obj.gameObject);
    }

    #endregion

    #region Spawn Entity

    public Player SpawnPlayer(string key, Vector2 position) {
        Player = Main.Resource.Instantiate("Player", pooling: false).GetComponent<Player>();
        Player.transform.position = position;
        Player.SetInfo(Main.Data.Creatures[key]);
        return Player;
    }
    public Enemy SpawnEnemy(string key, Vector2 position) {
        GameObject obj = Spawn(key, "Enemy", true);
        if (obj == null) {
            Debug.LogError($"[ObjectManager] SpawnEnemy({key}, {position}): Spawn Failed.");
            return null;
        }
        obj.transform.position = position;

        Enemy enemy = obj.GetComponent<Enemy>();
        Enemies.Add(enemy);
        enemy.SetInfo(Main.Data.Creatures[key]);
        return enemy;
    }
    public HitCollider SpawnHitCollider(string key, string prefabKey, HitColliderInfo info, HitInfo hitInfo) {
        GameObject obj = Spawn(prefabKey, "HitCollider_Base", true);
        if (obj == null) {
            Debug.LogError($"[ObjectManager] SpawnHitCollider({key}, {prefabKey}): Spawn Failed.");
            return null;
        }
        obj.transform.position = Vector2.zero;

        HitCollider hitCollider = obj.GetComponent<HitCollider>();
        hitCollider.SetInfo(key, info, hitInfo);
        HitColliders.Add(hitCollider);
        return hitCollider;
    }

    #endregion

    #region Despawn Entity

    public void DespawnPlayer() {
        Despawn(Player);
        Player = null;
    }
    public void DespawnEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
        Despawn(enemy);
    }
    public void DespawnHitCollider(HitCollider hitCollider) {
        HitColliders.Remove(hitCollider);
        Despawn(hitCollider);
    }

    #endregion

    #region Spawn InGameUI

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

    #endregion

}
