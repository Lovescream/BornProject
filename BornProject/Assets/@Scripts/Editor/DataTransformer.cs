using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow {

#if UNITY_EDITOR

    public static string csvDataPath = "@Resources/Data/Excel";
    public static string jsonDataPath = "Resources/JsonData";
    public static string spriteDataPath = "@Resources/Data/Base64/Sprites";
    public static string base64Path = "Resources";

    [MenuItem("Tools/ParseExcel")]
    public static void ParseExcel() {
        ParseData<CreatureData>();
        ParseData<SkillData>();
    }
    [MenuItem("Tools/ParseSpriteToBase64")]
    public static void ParseSpriteToBase64() {
        DirectoryInfo directoryInfo = new DirectoryInfo($"{Application.dataPath}/{spriteDataPath}/");
        StringBuilder stringBuilder = new();
        foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
            if (!fileInfo.Extension.Equals(".png")) continue;
            byte[] bytes = File.ReadAllBytes(fileInfo.FullName);
            stringBuilder.AppendLine(fileInfo.Name);
            stringBuilder.AppendLine(Convert.ToBase64String(bytes));
        }
        File.WriteAllText($"{Application.dataPath}/{base64Path}/RuleIcon.minecraftvirusohmygod", stringBuilder.ToString());
    }

    #region Parse Data

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

        // #2. Vector형 자료인 경우 변환. "X:0 Y:0"
        if (type == typeof(Vector2)) {
            string[] s = value.Replace("X", "").Replace("Y", "").Replace(":", "").Split(' ');
            return new Vector2(float.Parse(s[0]), float.Parse(s[1]));
        }
        else if (type == typeof(Vector3)) {
            string[] s = value.Replace("X", "").Replace("Y", "").Replace(":", "").Split(' ');
            return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
        }

        // #3. 리스트 자료인 경우 변환.
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

    #endregion

    #region Base64

    public static string SpriteToBase64(string fileName) {
        // Read Base64 file.
        FileInfo fileInfo = new($"{Application.dataPath}/{spriteDataPath}/{fileName}");
        string base64String = "";
        try {
            using StreamReader reader = new(fileInfo.OpenRead(), Encoding.UTF8);
            base64String = reader.ReadToEnd();
        }
        catch (Exception e) {
            Debug.LogError($"[Utilities] SpriteToBase64({fileName}): Failed to convert. {e}");
        }
        return base64String;
    }
    public static Sprite SpriteFromBase64(string base64String) {
        // Base64 -> bytes.
        byte[] bytes = Convert.FromBase64String(base64String.Split(',')[1]);

        // bytes -> Texture2D.
        Texture2D texture2D = new(1, 1);
        texture2D.LoadImage(bytes);

        // Texture2D -> Sprite.
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
    }

    #endregion

#endif

}