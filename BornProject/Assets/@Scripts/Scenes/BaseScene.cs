using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour {

    private bool _initialized;

    void Start() {
        Initialize();
    }

    protected virtual bool Initialize() {
        if (_initialized) return false;
        // 각종 초기화 함수.
        Main.Resource.Initialize();
        Main.Data.Initialize();
        // DataManager 초기화
        // GameManager 초기화

        _initialized = true;
        return true;
    }

}