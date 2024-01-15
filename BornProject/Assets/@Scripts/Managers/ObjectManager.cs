using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager {

    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new();

    //public List<Creature> Creatures { get; private set; } = new();

    public Player SpawnPlayer(string key, Vector2 position) {
        Player = Spawn<Player>(key, position);
        Player.SetInfo(Main.Data.Creatures[key]);
        return Player;
    }
    public void DespawnPlayer(Player player) {
        Player = null;
        Despawn(player);
    }
    public Enemy SpawnEnemy(string key, Vector2 position) {
        Enemy enemy = Spawn<Enemy>(key, position);
        Enemies.Add(enemy);
        enemy.SetInfo(Main.Data.Creatures[key]);
        return enemy;
    }
    public void DespawnEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
        Despawn(enemy);
    }


    private T Spawn<T>(string key, Vector2 position) where T : Entity {
        Type type = typeof(T);

        string prefabName = null;

        while (type != null) {
            prefabName = type.Name;
            if (Main.Resource.IsExistPrefab(prefabName)) break;
            type = type.BaseType;
        }
        if (string.IsNullOrEmpty(prefabName)) prefabName = "Entity";


        GameObject obj = Main.Resource.Instantiate(prefabName, pooling: true);
        obj.transform.position = position;

        return obj.GetOrAddComponent<T>();

    }

    private void Despawn<T>(T obj) where T : Entity {
        Main.Resource.Destroy(obj.gameObject);
    }
}