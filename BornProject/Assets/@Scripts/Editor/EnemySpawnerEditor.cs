using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(EnemySpawner), true)]
[CanEditMultipleObjects]
public class EnemySpawnerEditor : Editor {

    #region Properties

    public EnemySpawner Spawner => target as EnemySpawner;

    #endregion

    #region Editor

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set Enemy")) {
            SpriteRenderer spriter = Spawner.gameObject.GetOrAddComponent<SpriteRenderer>();
            spriter.sprite = GetSprite();
            spriter.sortingLayerName = "Object";
            spriter.sortingOrder = 1049;
        }
    }

    #endregion

    private Sprite GetSprite() {
        RuntimeAnimatorController animController = Resources.LoadAll<RuntimeAnimatorController>("Animations").Where(x => x.name == Spawner.Key).FirstOrDefault();
        if (animController == null) {
            Debug.LogError($"[EnemySpawner] No RuntimeAnimatorController exists for the Key({Spawner.Key}).");
            return null;
        }

        AnimationClip clip = animController.animationClips[0];
        if (clip == null) {
            Debug.LogError($"[EnemySpawner] No AnimationClip exists for the Key({Spawner.Key})");
            return null;
        }

        return AnimationUtility.GetObjectReferenceCurve(clip, AnimationUtility.GetObjectReferenceCurveBindings(clip)[0])[0].value as Sprite;
    }

}

#endif