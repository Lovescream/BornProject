using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [SerializeField]
    private bool _autoInitialize = true;
    [SerializeField]
    [ConditionalInspector("_autoInitialize", false)]
    private string _key = "";

    public string Key => _key;

    private bool _initialized;

    protected virtual void Awake() {
        if (_autoInitialize)
            Initialize();
    }

    public virtual bool Initialize() {
        if (_initialized) return false;

        _initialized = true;
        return true;
    }

}