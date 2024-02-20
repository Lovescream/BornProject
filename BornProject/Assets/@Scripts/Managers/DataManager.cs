using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager {
    public bool IsInitialized { get; private set; } = false;

    public Dictionary<string, CreatureData> Creatures = new();
    public Dictionary<string, SkillData> Skills = new();
    public Dictionary<string, EnemySkillData> EnemySkills = new();
    public Dictionary<string, ItemData> Items = new();

    public void Initialize()
    {
        Creatures = LoadJson<CreatureData>();
        Skills = LoadJson<SkillData>();
        EnemySkills = LoadJson<EnemySkillData>();

        IsInitialized = true;
    }

    private Dictionary<string, T> LoadJson<T>() where T : Data
    {
        return JsonConvert.DeserializeObject<List<T>>(Main.Resource.LoadJsonData($"{typeof(T).Name}").text).ToDictionary(data => data.Key);
    }
}

