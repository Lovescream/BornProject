using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ZerolizeDungeonDirection = ZerolizeDungeon.Direction;
using DijkstraDirection = Dijkstra.Direction;
public static class Extension {

    #region Generals

    public static GameObject FindChild(this GameObject obj, string name) => Utilities.FindChild(obj, name);
    public static T FindChild<T>(this GameObject obj, string name) where T : UnityEngine.Object => Utilities.FindChild<T>(obj, name);
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => Utilities.GetOrAddComponent<T>(obj);
    public static void DestroyChilds(this GameObject obj) {
        Transform[] children = new Transform[obj.transform.childCount];
        for (int i = 0; i < obj.transform.childCount; i++)
            children[i] = obj.transform.GetChild(i);
        foreach (Transform child in children) {
            if (child.gameObject.IsValid())
                Main.Resource.Destroy(child.gameObject);
        }
    }

    #endregion

    #region UI

    public static Canvas SetCanvas(this UI_Base ui) => Main.UI.SetCanvas(ui.gameObject);
    public static void SetPopupToFront(this UI_Popup popup) => Main.UI.SetPopupToFront(popup);

    #endregion

    #region Dungeons

    public static Vector2Int GetDirectionIndex(this Vector2Int index, ZerolizeDungeonDirection direction) {
        return direction switch {
            ZerolizeDungeonDirection.Top => new(index.x + 0, index.y + 1),
            ZerolizeDungeonDirection.Right => new(index.x + 1, index.y + 0),
            ZerolizeDungeonDirection.Bottom => new(index.x + 0, index.y - 1),
            ZerolizeDungeonDirection.Left => new(index.x - 1, index.y + 0),
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

    #endregion

    #region Vector2, Vector2Int

    public static bool IsInRange(this Vector2Int index, Vector2Int min, Vector2Int max) {
        if (index.x < min.x || max.x < index.x) return false;
        if (index.y < min.y || max.y < index.y) return false;
        return true;
    }
    public static bool IsInRange(this Vector2 v, Vector2 min, Vector2 max) {
        if (v.x < min.x || max.x < v.x) return false;
        if (v.y < min.y || max.y < v.y) return false;
        return true;
    }
    public static int Distance(this Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion

    #region Check Valid

    public static bool IsValid(this GameObject obj) {
        return obj != null && obj.activeSelf;
    }
    public static bool IsValid(this Entity entity) {
        return entity != null && entity.isActiveAndEnabled;
    }

    #endregion

    //public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.UIEvent type = Define.UIEvent.Click) {
    //    UI_Base.BindEvent(go, action, type);
    //}

    #region Matrix4x4

    public static Matrix4x4 Translate(this Matrix4x4 _, Vector3 delta) => Utilities.Matrix4x4Translate(delta);
    public static Matrix4x4 RotateX(this Matrix4x4 _, float deltaRad) => Utilities.Matrix4x4RotateX(deltaRad);
    public static Matrix4x4 RotateY(this Matrix4x4 _, float deltaRad) => Utilities.Matrix4x4RotateY(deltaRad);
    public static Matrix4x4 RotateZ(this Matrix4x4 _, float deltaRad) => Utilities.Matrix4x4RotateZ(deltaRad);
    public static Matrix4x4 Scale(this Matrix4x4 _, Vector3 delta) => Utilities.Matrix4x4Scale(delta);
    public static void TRS(this Transform transform, Vector2 deltaPosition, float deltaRotation, Vector3 deltaScale) {
        transform.position = transform.position + (Vector3)Utilities.TRS(Vector2.zero, deltaPosition, deltaRotation, deltaScale);
    }
    public static void RT(this Transform transform, Vector2 deltaPosition, float deltaRotationDeg) {
        Vector3 position = transform.position;
        position += (Vector3)(Utilities.Matrix4x4RotateZ(deltaRotationDeg * Mathf.Deg2Rad) * Utilities.Matrix4x4Translate(deltaPosition).MultiplyPoint(Vector3.zero));
        transform.position = position;
    }
    public static void SRT(this Transform transform, Vector2 deltaPosition, float deltaRotationDeg, float deltaScale) {
        Vector3 position = transform.position;
        position += (Vector3)(Utilities.Matrix4x4Scale(Vector3.one * deltaScale) * Utilities.Matrix4x4RotateZ(deltaRotationDeg * Mathf.Deg2Rad) * Utilities.Matrix4x4Translate(deltaPosition).MultiplyPoint3x4(Vector3.zero));
        transform.position = position;
    }
    public static void RotateZ(this Transform transform, Vector2 origin, float rotationDeg) {
        Vector3 offset = transform.position - (Vector3)origin;
        transform.position = transform.position + Utilities.Matrix4x4RotateZ(rotationDeg * Mathf.Deg2Rad).MultiplyPoint(offset);
    }
    #endregion
}