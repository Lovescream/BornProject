using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    #region Properties
    public SkillTreeMaker SkillTreeMaker {  get; private set; }

    #endregion


    public SkillTreeMaker SpawnSkillTreeMaker(string key, Vector2 position) // TODO.
    {
        SkillTreeMaker = Spawn<SkillTreeMaker>(position);

        return SkillTreeMaker;
    }


    public void DespawnSkillTreeMaker(SkillTreeMaker skillTreeMaker) // TODO.
    {
        if (skillTreeMaker == null) skillTreeMaker = SkillTreeMaker;
        if (skillTreeMaker == null) return;
        SkillTreeMaker = null;
        Despawn(skillTreeMaker);
    }

    private T Spawn<T>(Vector2 position) where T : UI_Base
    {
        Type type = typeof(T);

        string prefabName = null;
        while (type != null)
        {
            prefabName = type.Name;
            if (Main.Resource.LoadPrefab(prefabName) != null) break;
            type = type.BaseType;
        }
        if (string.IsNullOrEmpty(prefabName)) prefabName = "Thing";

        GameObject obj = Main.Resource.Instantiate($"{prefabName}", pooling: true);
        obj.transform.position = position;

        return obj.GetOrAddComponent<T>();
    }

    //private T Spawn<T>(string key, Vector2 position) where T : UI_Base
    //{
    //    if (string.IsNullOrEmpty(key))
    //    {
    //        Debug.LogError($"[UIManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. the key is null or empty.");
    //        return null;
    //    }
    //    GameObject obj = Main.Resource.Instantiate(key, pooling: true);
    //    if (obj == null)
    //    {
    //        Debug.LogError($"[UIManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. Failed to load prefab.");
    //        return null;
    //    }
    //    obj.transform.position = position;

    //    return obj.GetOrAddComponent<T>();
    //}

    private void Despawn<T>(T obj) where T : UI_Base
    {
        Main.Resource.Destroy(obj.gameObject); // Destroy가 아닌 비활성화로 해야하는거 아닌가?.
    }
}
