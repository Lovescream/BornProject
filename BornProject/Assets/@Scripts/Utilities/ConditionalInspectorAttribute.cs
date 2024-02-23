using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ConditionalInspectorAttribute : PropertyAttribute {

    #region Properties

    public string ComparedPropertyName { get; private set; }
    public object ComparedValue { get; private set; }
    public DisableType DisablingType { get; private set; }

    #endregion

    #region Enums

    public enum DisableType { ReadOnly = 2, DontDraw = 3 }

    #endregion

    public ConditionalInspectorAttribute(string comparedPropertyName, object comparedValue, DisableType disablingType = DisableType.DontDraw) {
        this.ComparedPropertyName = comparedPropertyName;
        this.ComparedValue = comparedValue;
        this.DisablingType = disablingType;
    }
}