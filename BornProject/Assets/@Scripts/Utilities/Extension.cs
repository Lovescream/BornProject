using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DungeonDirection = DungeonGenerate.Direction;
using DijkstraDirection = Dijkstra.Direction;
public static class Extension {
    public static GameObject FindChild(this GameObject obj, string name) => Utilities.FindChild(obj, name);
    public static T FindChild<T>(this GameObject obj, string name) where T : UnityEngine.Object => Utilities.FindChild<T>(obj, name);
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => Utilities.GetOrAddComponent<T>(obj);
    public static Vector2Int GetDirectionIndex(this Vector2Int index, DungeonDirection direction) {
        return direction switch {
            DungeonDirection.Top => new(index.x + 0, index.y + 1),
            DungeonDirection.Right => new(index.x + 1, index.y + 0),
            DungeonDirection.Bottom => new(index.x + 0, index.y - 1),
            DungeonDirection.Left => new(index.x - 1, index.y + 0),
            _ => index
        };
    }
    public static Vector2Int GetDirectionIndex(this Vector2Int index, DijkstraDirection direction) {
        return direction switch {
            DijkstraDirection.Top => new(index.x + 0, index.y + 1),
            DijkstraDirection.Right => new(index.x + 1, index.y + 0),
            DijkstraDirection.Bottom => new(index.x + 0, index.y - 1),
            DijkstraDirection.Left => new(index.x - 1, index.y + 0),
            _ => index
        };
    }
    public static bool IsInRange(this Vector2Int index, Vector2Int min, Vector2Int max) {
        if (index.x < min.x || max.x < index.x) return false;
        if (index.y < min.y || max.y < index.y) return false;
        return true;
    }
    public static int Distance(this Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    //public static void SetCanvas(this UI_Base ui) => Main.UI.SetCanvas(ui.gameObject);
    //public static void SetPopupToFront(this UI_Popup popup) => Main.UI.SetPopupToFront(popup);
    //public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.UIEvent type = Define.UIEvent.Click) {
    //    UI_Base.BindEvent(go, action, type);
    //}

    public static bool IsValid(this GameObject obj) {
        return obj != null && obj.activeSelf;
    }

    //public static bool IsValid(this Thing thing) {
    //    return thing != null && thing.isActiveAndEnabled;
    //}

    public static void DestroyChilds(this GameObject obj) {
        Transform[] children = new Transform[obj.transform.childCount];
        for (int i = 0; i < obj.transform.childCount; i++)
            children[i] = obj.transform.GetChild(i);
        foreach (Transform child in children) {
            if (child.gameObject.IsValid())
                Main.Resource.Destroy(child.gameObject);
        }
    }
}