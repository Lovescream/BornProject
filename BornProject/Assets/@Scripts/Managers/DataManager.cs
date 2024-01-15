using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class DataManager {

    public Dictionary<string, CreatureData> Creatures = new();

    public void Initialize() {
        Creatures = LoadJson<CreatureData>();
    }

    private Dictionary<string, T> LoadJson<T>() where T : Data {
        TextAsset textAsset = Main.Resource.LoadJsonData(typeof(T).Name);

        Dictionary<string, T> dic = JsonConvert.DeserializeObject<List<T>>(textAsset.text).ToDictionary(data => data.Key);

        return dic;
    }


}