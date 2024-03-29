using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour {

    public UI_Scene SceneUI { get; protected set; }

    private bool _initialized;

    void Start() {
        Initialize();
    }

    protected virtual bool Initialize() {
        if (_initialized) return false;

        Main.Scene.Current = this;

        // 각종 초기화 함수.
        Main.Resource.Initialize();
        Main.Data.Initialize();
        Main.Audio.Initialize();

        if (FindObjectOfType<EventSystem>() == null) Main.Resource.Instantiate("@EventSystem").name = "@EventSystem";

        _initialized = true;
        return true;
    }

}