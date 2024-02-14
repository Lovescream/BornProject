using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerInfoWindow : EditorWindow {
#if UNITY_EDITOR

    #region Properties

    #endregion

    #region Fields

    #endregion

    [MenuItem("Tools/Open Player Info Window")]
    public static void ShowWindow() {
        PlayerInfoWindow window = GetWindow<PlayerInfoWindow>();
        window.titleContent = new("Player Info");
        window.position = new Rect(1000, 200, 500, 800);
        window.maxSize = new Vector2(500, 800);
        window.Show();
    }
    private void OnGUI() {

    }

#endif
}