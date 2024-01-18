using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public Player Player { get; private set; }

    //public Player SpawnPlayer(string key, Vector2 position) // TODO_¼±±³.
    //{
    //    Player = Spawn<Player>(key, position);
    //    Player.SetInfo(Main.Data.Players[key]);
    //    return Player;
    //}

    //private T Spawn<T>(string key, Vector2 position) where T : Thing
    //{
    //    Type type = typeof(T);

    //    string prefabName = null;
    //    while (type != null)
    //    {
    //        prefabName = type.Name;
    //        if (Main.Resource.IsExist($"{prefabName}.prefab")) break;
    //        type = type.BaseType;
    //    }
    //    if (string.IsNullOrEmpty(prefabName)) prefabName = "Thing.prefab";

    //    GameObject obj = Main.Resource.Instantiate($"{prefabName}.prefab", pooling: true);
    //    obj.transform.position = position;

    //    return obj.GetOrAddComponent<T>();
    //}
}
