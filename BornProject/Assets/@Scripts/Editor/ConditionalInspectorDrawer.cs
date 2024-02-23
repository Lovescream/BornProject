using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalInspectorAttribute))]
public class ConditionalInspectorDrawer : PropertyDrawer {

    #region Fields

    // Reference to the attribute on the property.
    private ConditionalInspectorAttribute _conditionalInspector;

    // Field that is being compared.
    private SerializedProperty _comparedField;

    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!Show(property) && _conditionalInspector.DisablingType == ConditionalInspectorAttribute.DisableType.DontDraw) return 0;

        // The height of the property should be defaulted to the default height.
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // If the condition is met, simply draw the field.
        if (Show(property)) EditorGUI.PropertyField(position, property);
        // Check if the disabling type is read only. If it is, draw it disabled.
        else if (_conditionalInspector.DisablingType == ConditionalInspectorAttribute.DisableType.ReadOnly) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }


    private bool Show(SerializedProperty property) {
        _conditionalInspector = attribute as ConditionalInspectorAttribute;

        // Replace propertyName to the value from the parameter.
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, _conditionalInspector.ComparedPropertyName) : _conditionalInspector.ComparedPropertyName;
        _comparedField = property.serializedObject.FindProperty(path);
        if (_comparedField == null) {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        // Get the value and compare based on types.
        switch (_comparedField.type) {
            case "bool": return _comparedField.boolValue.Equals(_conditionalInspector.ComparedValue);
            case "Enum": return _comparedField.enumValueIndex.Equals((int)_conditionalInspector.ComparedValue);
            default: Debug.LogError($"Error: {_comparedField.type} is not supported of " + path); return true;
        }
    }
}

