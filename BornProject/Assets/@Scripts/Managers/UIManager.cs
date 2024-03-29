using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {

    #region Const

    private static readonly int InitialPopupOrder = 10;
    private static readonly Vector2 DefaultResolution = new(1920, 1080);

    #endregion

    #region Properties

    public Transform Root {
        get {
            if (_rootTransform == null) _rootTransform = new GameObject("@UI_Root").transform;
            return _rootTransform;
        }
    }

    #endregion

    #region Fields

    private int _popupOrder = InitialPopupOrder;
    private int _toastOrder = 500;

    private Transform _rootTransform;

    // Collections.
    private List<UI_Popup> _popups = new();
    private List<UI_Toast> _toasts = new();

    #endregion

    #region Generals

    public Canvas SetCanvas(GameObject obj, bool isToast = false) {
        // #1. Canvas 설정.
        Canvas canvas = obj.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        // #2. CanvasScaler 설정.
        CanvasScaler scaler = obj.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = DefaultResolution;

        if (isToast) {
            _toastOrder++;
            canvas.sortingOrder = _toastOrder;
        }

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

        return obj.GetOrAddComponent<T>();
    }

    #endregion

    #region PopupUI

    public T OpenPopupUI<T>(string name = null) where T : UI_Popup {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        for (int i = 0; i < _popups.Count; i++) {
            if (_popups[i] is not T openedPopup) continue;
            openedPopup.SetPopupToFront();
            return openedPopup;
        }

        GameObject obj = Main.Resource.Instantiate($"{name}");
        obj.transform.SetParent(Root);
        T popup = obj.GetOrAddComponent<T>();
        _popups.Add(popup);

        if (popup.IsPause) Time.timeScale = 0f;

        return popup;
    }

    public T GetLatestPopup<T>() where T : UI_Popup {
        for (int i = _popups.Count - 1; i>=0; i--) {
            if (_popups[i] is T popup) return popup;
        }
        return null;
    }

    public void ClosePopupUI<T>() where T : UI_Popup {
        if (_popups.Count == 0) return;

        foreach (var popup in _popups.Where(x => x is T)) {
            ClosePopup(popup);
        }
    }

    public void ClosePopup(UI_Popup popup) {
        if (_popups.Count == 0) return;

        bool isLatest = _popups[^1] == popup;

        _popups.Remove(popup);
        Main.Resource.Destroy(popup.gameObject);

        if (isLatest) _popupOrder--;
        else ReorderAllPopups();

        for (int i = 0; i < _popups.Count; i++) {
            if (_popups[i].IsPause) {
                Time.timeScale = 0;
                return;
            }
        }
        Time.timeScale = 1;
    }

    public void CloseAllPopup() {
        if (_popups.Count == 0) return;

        for (int i = _popups.Count - 1; i >= 0; i--) {
            if (_popups[i] == null) continue;
            Main.Resource.Destroy(_popups[i].gameObject);
        }
        _popups.Clear();
        _popupOrder = InitialPopupOrder;

        Time.timeScale = 1;
    }

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

    #region ToastUI

    public UI_Toast ShowToast(string message) {
        UI_Toast toast = Main.Resource.Instantiate($"UI_Toast", pooling: true).GetComponent<UI_Toast>();
        toast.SetInfo(message);
        _toasts.Add(toast);
        toast.transform.SetParent(Root.transform);
        return toast;
    }
    public void CloseToast(UI_Toast toast) {
        if (_toasts.Count == 0) return;
        _toasts.Remove(toast);
        Main.Resource.Destroy(toast.gameObject);
        _toastOrder--;
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

}