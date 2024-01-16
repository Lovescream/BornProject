using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing : MonoBehaviour
{

    private bool initialized;

    void Awake()
    {
        Initialize();
    }
    public virtual bool Initialize()
    {
        if (initialized) return false;
        initialized = true;

        return true;
    }

}