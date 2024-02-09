using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

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

    #region Matrix4x4

    //   1    0    0    x 
    //   0    1    0    y
    //   0    0    1    z
    //   0    0    0    1
    public static Matrix4x4 Matrix4x4Translate(Vector3 delta) {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m03 = delta.x;
        matrix.m13 = delta.y;
        matrix.m23 = delta.z;

        return matrix;
    }

    //   1    0    0    0 
    //   0   cos -sin   0 
    //   0   sin  cos   0 
    //   0    0    0    1 
    public static Matrix4x4 Matrix4x4RotateX(float deltaRad) {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m11 = Mathf.Cos(deltaRad);
        matrix.m22 = matrix.m11;
        matrix.m21 = Mathf.Sin(deltaRad);
        matrix.m12 = -matrix.m21;
        return matrix;
    }

    //  cos   0   sin   0 
    //   0    1    0    0 
    // -sin   0   cos   0 
    //   0    0    0    1 
    public static Matrix4x4 Matrix4x4RotateY(float deltaRad) {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = Mathf.Cos(deltaRad);
        matrix.m22 = matrix.m00;
        matrix.m02 = Mathf.Sin(deltaRad);
        matrix.m20 = -matrix.m02;
        return matrix;
    }

    //  cos -sin   0    0 
    //  sin  cos   0    0 
    //   0    0    1    0
    //   0    0    0    1 
    public static Matrix4x4 Matrix4x4RotateZ(float deltaRad) {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = Mathf.Cos(deltaRad);
        matrix.m11 = matrix.m00;
        matrix.m10 = Mathf.Sin(deltaRad);
        matrix.m01 = -matrix.m10;
        return matrix;
    }

    //  sx    0    0    0 
    //   0   sy    0    0 
    //   0    0   sz    0 
    //   0    0    0    1 
    public static Matrix4x4 Matrix4x4Scale(Vector3 delta) {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = delta.x;
        matrix.m11 = delta.y;
        matrix.m22 = delta.z;
        return matrix;
    }

    public static Vector2 TRS(Vector2 origin, Vector2 deltaPosition, float deltaZDeg, Vector3 deltaScale) {
        Matrix4x4 fxT = Matrix4x4Translate(deltaPosition);
        Matrix4x4 fxR = Matrix4x4RotateZ(deltaZDeg * Mathf.Deg2Rad);
        Matrix4x4 fxS = Matrix4x4Scale(deltaScale);
        //Matrix4x4 fx = fxT * fxR * fxS;
        Matrix4x4 fx = Matrix4x4.TRS(deltaPosition, Quaternion.Euler(0, 0, deltaZDeg), Vector3.one);
        return fx.MultiplyPoint(origin);
    }

    #endregion

}