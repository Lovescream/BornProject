using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {


    private static readonly int InitialPopupOrder = 10;


    #region Properties

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null) root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public SkillTreeMaker SkillTreeMaker { get; private set; }

    #endregion

    #region Fields

    private int _popupOrder = InitialPopupOrder;

    // Collections.
    private List<UI_Popup> _popups = new();


    public void SetCanvas(GameObject obj)
    {
        Canvas canvas = obj.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        CanvasScaler scaler = obj.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new(1920, 1080);
    }

    public void Clear()
    {
        CloseAllPopup();
        Time.timeScale = 1;
    }


    #region SceneUI

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject obj = Main.Resource.Instantiate($"{name}.prefab");
        obj.transform.SetParent(Root.transform);

        return obj.GetOrAddComponent<T>();
    }

    #endregion

    #region Popups

    public int OrderUpPopup()
    {
        _popupOrder++;
        return _popupOrder - 1;
    }

    public void ReorderAllPopups()
    {
        _popupOrder = InitialPopupOrder;
        for (int i = 0; i < _popups.Count; i++)
        {
            _popups[i].SetOrder(_popupOrder++);
        }
    }

    public void SetPopupToFront(UI_Popup popup)
    {
        if (!_popups.Remove(popup)) return;
        _popups.Add(popup);
        ReorderAllPopups();
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject obj = Main.Resource.Instantiate($"{name}.prefab");
        obj.transform.SetParent(Root.transform);
        T popup = obj.GetOrAddComponent<T>();
        _popups.Add(popup);

        return popup;
    }

    public void ClosePopup(UI_Popup popup)
    {
        if (_popups.Count == 0) return;

        bool isLatest = _popups[_popups.Count - 1] == popup;

        _popups.Remove(popup);
        Main.Resource.Destroy(popup.gameObject);

        if (isLatest) _popupOrder--;
        else ReorderAllPopups();
    }

    public void CloseAllPopup()
    {
        if (_popups.Count == 0) return;
        for (int i = _popups.Count - 1; i >= 0; i--)
        {
            Main.Resource.Destroy(_popups[i].gameObject);
        }
        _popups.Clear();
        _popupOrder = InitialPopupOrder;
    }

    #endregion

    public SkillTreeMaker SpawnSkillTreeMaker(string key, Vector2 position) // TODO.
    {
        SkillTreeMaker = Spawn<SkillTreeMaker>(position);

        return SkillTreeMaker;
    }


    public void DespawnSkillTreeMaker(SkillTreeMaker skillTreeMaker) // TODO.
    {
        if (skillTreeMaker == null) skillTreeMaker = SkillTreeMaker;
        if (skillTreeMaker == null) return;
        SkillTreeMaker = null;
        Despawn(skillTreeMaker);
    }

    private T Spawn<T>(Vector2 position) where T : UI_Base
    {
        Type type = typeof(T);

        string prefabName = null;
        while (type != null)
        {
            prefabName = type.Name;
            if (Main.Resource.LoadPrefab(prefabName) != null) break;
            type = type.BaseType;
        }
        if (string.IsNullOrEmpty(prefabName)) prefabName = "Thing";

        GameObject obj = Main.Resource.Instantiate($"{prefabName}", pooling: true);
        obj.transform.position = position;

        return obj.GetOrAddComponent<T>();
    }

    //private T Spawn<T>(string key, Vector2 position) where T : UI_Base
    //{
    //    if (string.IsNullOrEmpty(key))
    //    {
    //        Debug.LogError($"[UIManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. the key is null or empty.");
    //        return null;
    //    }
    //    GameObject obj = Main.Resource.Instantiate(key, pooling: true);
    //    if (obj == null)
    //    {
    //        Debug.LogError($"[UIManager] Spawn<{typeof(T).Name}>({key}, {position}): Spawn Failed. Failed to load prefab.");
    //        return null;
    //    }
    //    obj.transform.position = position;

    //    return obj.GetOrAddComponent<T>();
    //}

    private void Despawn<T>(T obj) where T : UI_Base
    {
        Main.Resource.Destroy(obj.gameObject); // Destroy가 아닌 비활성화로 해야하는거 아닌가?.
    }
}