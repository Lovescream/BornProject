using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager
{

    public Dictionary<string, CreatureData> Creatures = new();
    public Dictionary<string, ItemData> Items = new();

    public void Initialize()
    {
        Creatures = LoadJson<CreatureData>();
        Items = LoadJson<ItemData>();

    }

    private Dictionary<string, T> LoadJson<T>() where T : Data
    {
        //TextAsset textAsset = Main.Resource.LoadJsonData(typeof(T).Name);
        TextAsset textAsset = Resources.Load<TextAsset>("JsonData/CreatureData");

        Dictionary<string, T> dic = JsonConvert.DeserializeObject<List<T>>(textAsset.text).ToDictionary(data => data.Key);
        return dic;
    }
}
