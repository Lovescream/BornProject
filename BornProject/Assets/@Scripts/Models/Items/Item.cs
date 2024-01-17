using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Thing
{

    #region Properties

    public ItemData Data { get; protected set; }

    public string Key => Data.Key;
    public ItemType Type => Data.Type;
    public string Description => Data.Description;
    public float Cost => Data.Cost;
    public float Modifiers => Data.Modifiers;

    

    #endregion   

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        return true;
    }
    public virtual void SetInfo(ItemData data)
    {
        Initialize();

        this.Data = data;
        
        Debug.Log(Type+ "°ª");

    }
}