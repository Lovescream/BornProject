using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow {

#if UNITY_EDITOR

    public static string csvDataPath = "@Resources/Data/Excel";
    public static string jsonDataPath = "Resources/JsonData";

    [MenuItem("Tools/ParseExcel")]
    public static void ParseExcel() {
        ParseData<CreatureData>();
        ////ParseData<CharacterData>();
        ParseData<ItemData>();
    }

    private static void ParseData<T>() where T : Data {
        // #1. 파싱 준비.
        Type type = typeof(T);
        List<T> list = new();

        // #2. 파일 읽기.
        string csvPath = $"{Application.dataPath}/{csvDataPath}/{type.Name}.csv";
        if (!File.Exists(csvPath)) {
            Debug.LogError($"[DataTransformer] ParseData<{typeof(T)}>(): The path was not found. ({csvPath})");
        }
        string[] lines = File.ReadAllText(csvPath).Split("\n");
        Debug.Log($"[DataTransformer] ParseData<{typeof(T)}>(): Read the file. ({csvPath})");

        // #3. 프로퍼티 이름 캐싱.
        if (lines.Length <= 0) return;
        string[] propertyNames = lines[0].Replace("\r", "").Split(',');

        // #4. 데이터 파싱.
        for (int y = 1; y < lines.Length; y++) {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            T data = Activator.CreateInstance<T>();

            for (int i = 0; i < row.Length; i++) {
                PropertyInfo property = type.GetProperty(propertyNames[i]);
                if (property == null) {
                    Debug.LogError($"[DataTransformer] ParseData<{type.Name}>(): Data parsing failed. Property '{propertyNames[i]}' not found.");
                    return;
                }
                property.SetValue(data, ConvertValue(property.PropertyType, row[i]));
            }

            list.Add(data);
        }

        // #5. Json으로 저장.
        string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented);
        string jsonPath = $"{Application.dataPath}/{jsonDataPath}/{type.Name}.json";
        File.WriteAllText(jsonPath, jsonString);
        Debug.Log($"[DataTransformer] ParseData<{typeof(T)}>(): Save the file. ({jsonPath})");
        AssetDatabase.Refresh();
    }

    private static object ConvertValue(Type type, string value) {
        // #1. 기본 값 자료인 경우 변환.
        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (converter != null && converter.CanConvertFrom(typeof(string))) return string.IsNullOrEmpty(value) ? default : converter.ConvertFromString(value);

        // #2. 리스트 자료인 경우 변환.
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
            Type itemType = type.GetGenericArguments()[0];
            IList list = Activator.CreateInstance(type) as IList;
            if (string.IsNullOrEmpty(value)) return list;
            TypeConverter itemConverter = TypeDescriptor.GetConverter(itemType);
            if (itemConverter != null && itemConverter.CanConvertFrom(typeof(string)))
                foreach (var item in value.Split('|'))
                    if (string.IsNullOrEmpty(item)) continue;
                    else list.Add(itemConverter.ConvertFromString(item));
            else
                foreach (var item in value.Split('|'))
                    if (string.IsNullOrEmpty(item)) continue;
                    else list.Add(Activator.CreateInstance(itemType, item));
            return list;
        }
        return Activator.CreateInstance(type, value);
    }


#endif

}