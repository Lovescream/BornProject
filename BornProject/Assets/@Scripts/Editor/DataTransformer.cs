using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow {

#if UNITY_EDITOR

    [MenuItem("Tools/ParseExcel")]
    public static void ParseExcel() {
        ParseData<CreatureData>();
        ////ParseData<CharacterData>();
        //ParseData<ItemData>();
    }

    private static void ParseData<T>() where T : Data {
        // #1. 파싱 준비.
        Type type = typeof(T);
        List<T> list = new();

        // #2. 파일 읽기.
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{type.Name}.csv").Split("\n");

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
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{type.Name}.json", jsonString);
        AssetDatabase.Refresh();
    }

    private static object ConvertValue(Type type, string value) {
        // #1. 기본 값 자료인 경우 변환.
        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (converter != null && converter.CanConvertFrom(typeof(string))) return converter.ConvertFromString(value);

        // #2. 리스트 자료인 경우 변환.
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
            Type itemType = type.GetGenericArguments()[0];
            IList list = Activator.CreateInstance(type) as IList;
            TypeConverter itemConverter = TypeDescriptor.GetConverter(itemType);
            if (itemConverter != null && itemConverter.CanConvertFrom(typeof(string)))
                foreach (var item in value.Split('|')) list.Add(itemConverter.ConvertFromString(item));
            else
                foreach (var item in value.Split('|')) list.Add(Activator.CreateInstance(itemType, item));
            return list;
        }
        return Activator.CreateInstance(type);
    }


#endif

}