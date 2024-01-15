using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    #region Singleton

    private static Main _instance;
    private static bool _initialized;

    public static Main Instance {
        get {
            if (!_initialized) {
                _initialized = true;

                GameObject obj = GameObject.Find("@Main");
                if (obj == null) {
                    obj = new() { name = "@Main" };
                    obj.AddComponent<Main>();
                    DontDestroyOnLoad(obj);
                }
                _instance = obj.GetComponent<Main>();
            }

            return _instance;
        }
    }

    #endregion

    private PoolManager _pool = new();
    private ResourceManager _resource = new();

    public static PoolManager Pool => Instance?._pool;
    public static ResourceManager Resource => Instance?._resource;
}