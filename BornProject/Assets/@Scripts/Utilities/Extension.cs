using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extension {
    public static GameObject FindChild(this GameObject obj, string name) => Utilities.FindChild(obj, name);
    public static T FindChild<T>(this GameObject obj, string name) where T : UnityEngine.Object => Utilities.FindChild<T>(obj, name);
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => Utilities.GetOrAddComponent<T>(obj);
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