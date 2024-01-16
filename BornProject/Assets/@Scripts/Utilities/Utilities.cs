using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utilities {
    public static T FindChild<T>(GameObject obj, string name = null) where T : Object {
        if (obj == null) return null;
        T[] components = obj.GetComponentsInChildren<T>(true);
        if (string.IsNullOrEmpty(name)) return components[0];
        else return components.Where(x => x.name == name).FirstOrDefault();
    }
    public static T FindChildDirect<T>(GameObject obj, string name = null) where T : Object {
        if (obj == null) return null;
        for (int i = 0; i < obj.transform.childCount; i++) {
            Transform t = obj.transform.GetChild(i);
            if (string.IsNullOrEmpty(name) || t.name == name) {
                if (t.TryGetComponent<T>(out T component)) return component;
            }
        }
        return null;
    }
    public static GameObject FindChild(GameObject obj, string name = null) {
        Transform transform = FindChild<Transform>(obj, name);
        if (transform == null) return null;
        return transform.gameObject;
    }
    public static GameObject FindChildDirect(GameObject obj, string name = null) {
        Transform transform = FindChildDirect<Transform>(obj, name);
        if (transform == null) return null;
        return transform.gameObject;
    }
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component {
        if (!obj.TryGetComponent<T>(out T component))
            component = obj.AddComponent<T>();
        return component;
    }
}