using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    private bool _initialized;

    protected virtual void Awake() {
        Initialize();
    }

    public virtual bool Initialize() {
        if (_initialized) return false;

        _initialized = true;
        return true;
    }

}