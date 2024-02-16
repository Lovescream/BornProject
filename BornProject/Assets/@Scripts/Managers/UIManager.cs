using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD

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

=======

public class UIManager {

    #region Const

    private static readonly int InitialPopupOrder = 10;
    private static readonly Vector2 DefaultResolution = new(1920, 1080);

    #endregion

    #region Properties

    public SkillTreeMaker SkillTreeMaker { get; private set; }

    public Transform Root {
        get {
            if (_rootTransform == null) _rootTransform = new GameObject("@UI_Root").transform;
            return _rootTransform;
        }
    }

    #endregion

>>>>>>> Develop1.0
    #region Fields

    private int _popupOrder = InitialPopupOrder;

<<<<<<< HEAD
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
=======
    private Transform _rootTransform;

    // Collections.
    private List<UI_Popup> _popups = new();

    #endregion

    #region Generals

    public Canvas SetCanvas(GameObject obj) {
        // #1. Canvas 설정.
        Canvas canvas = obj.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        // #2. CanvasScaler 설정.
        CanvasScaler scaler = obj.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = DefaultResolution;

        return canvas;
    }

    public void Clear() {
        CloseAllPopup();
    }

    #endregion

    #region SceneUI
    
    public T OpenSceneUI<T>(string name = null) where T : UI_Scene {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject obj = Main.Resource.Instantiate($"{name}");
        obj.transform.SetParent(Root);
>>>>>>> Develop1.0

        return obj.GetOrAddComponent<T>();
    }

    #endregion

<<<<<<< HEAD
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
=======
    #region PopupUI

    public T OpenPopupUI<T>(string name = null) where T : UI_Popup {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject obj = Main.Resource.Instantiate($"{name}.prefab");
        obj.transform.SetParent(Root);
>>>>>>> Develop1.0
        T popup = obj.GetOrAddComponent<T>();
        _popups.Add(popup);

        return popup;
    }

<<<<<<< HEAD
    public void ClosePopup(UI_Popup popup)
    {
        if (_popups.Count == 0) return;

        bool isLatest = _popups[_popups.Count - 1] == popup;
=======
    public void ClosePopup(UI_Popup popup) {
        if (_popups.Count == 0) return;

        bool isLatest = _popups[^1] == popup;
>>>>>>> Develop1.0

        _popups.Remove(popup);
        Main.Resource.Destroy(popup.gameObject);

        if (isLatest) _popupOrder--;
        else ReorderAllPopups();
    }

<<<<<<< HEAD
    public void CloseAllPopup()
    {
        if (_popups.Count == 0) return;
        for (int i = _popups.Count - 1; i >= 0; i--)
        {
            Main.Resource.Destroy(_popups[i].gameObject);
        }
=======
    public void CloseAllPopup() {
        if (_popups.Count == 0) return;

        for (int i = _popups.Count - 1; i >= 0; i--) Main.Resource.Destroy(_popups[i].gameObject);
>>>>>>> Develop1.0
        _popups.Clear();
        _popupOrder = InitialPopupOrder;
    }

<<<<<<< HEAD
    #endregion
=======
    public int OrderUpPopup() => ++_popupOrder - 1;

    public void ReorderAllPopups() {
        _popupOrder = InitialPopupOrder;
        _popups.ForEach(x => x.SetOrder(_popupOrder++));
    }

    public void SetPopupToFront(UI_Popup popup) {
        if (!_popups.Remove(popup)) return;
        _popups.Add(popup);
        ReorderAllPopups();
    }

    #endregion

    #region SubItem

    public T CreateSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        T item = Main.Resource.Instantiate($"{name}", parent, pooling).GetOrAddComponent<T>();
        item.transform.SetParent(parent);

        return item;
    }

    #endregion

    #region Temp
>>>>>>> Develop1.0

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

    private T Spawn<T>(Vector2 position) where T : UI_Base {
        Type type = typeof(T);

        string prefabName = null;
        while (type != null) {
            prefabName = type.Name;
            if (Main.Resource.LoadPrefab(prefabName) != null) break;
            type = type.BaseType;
        }
        if (string.IsNullOrEmpty(prefabName)) prefabName = "Thing";

        GameObject obj = Main.Resource.Instantiate($"{prefabName}", pooling: true);
        obj.transform.position = position;

        return obj.GetOrAddComponent<T>();
    }
    private void Despawn<T>(T obj) where T : UI_Base {
        Main.Resource.Destroy(obj.gameObject); // Destroy가 아닌 비활성화로 해야하는거 아닌가?.
    }
<<<<<<< HEAD
=======

    #endregion
>>>>>>> Develop1.0
}