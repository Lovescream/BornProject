using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemData : Data
{
    public ItemType Type { get; set; }    
    public string Description { get; set; }
    public float Cost { get; set; }
    public float Modifiers { get; set; }
}

public enum ItemType
{
    Weapon,
    Armor,
    COUNT,
}