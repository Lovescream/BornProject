using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager {

    public Dictionary<string, CreatureData> Creatures = new();
    public Dictionary<string, SkillData> Skills = new();

    private bool _isInitialized;
    public void Initialize() {
        if (_isInitialized) return;
        _isInitialized = true;

        Creatures = LoadJson<CreatureData>();
        Skills = LoadJson<SkillData>();
    }

    private Dictionary<string, T> LoadJson<T>() where T : Data {
        return JsonConvert.DeserializeObject<List<T>>(Main.Resource.Get<TextAsset>($"{typeof(T).Name}").text).ToDictionary(x => x.Key);
        //return JsonConvert.DeserializeObject<List<T>>(Main.Resource.LoadJsonData($"{typeof(T).Name}").text).ToDictionary(data => data.Key);
    }
}