using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScene))]
public class TestSceneEditor : Editor {

#if UNITY_EDITOR

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        //DrawDefaultInspector();

        TestScene scene = (TestScene)target;
        if (GUILayout.Button("Dungeon Generate")) {
            scene.GenerateDungeon();
        }
        if (GUILayout.Button("Dungeon Regenerate")) {
            scene.RegenerateDungeon();
        }
    }

#endif

}