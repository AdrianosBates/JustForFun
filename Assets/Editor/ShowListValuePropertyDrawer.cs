using Codice.Client.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

//[CustomPropertyDrawer(typeof(ShowListValueAttribute))]
public class ShowListValuePropertyDrawer : PropertyDrawer {
    // Draw the property inside the given rect
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    //    //// First get the attribute since it contains the range for the slider
    //    //RangeAttribute range = attribute as RangeAttribute;

    //    //// Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
    //    //if (property.propertyType == SerializedPropertyType.Float)
    //    //    EditorGUI.Slider(position, property, range.min, range.max, label);
    //    //else if (property.propertyType == SerializedPropertyType.Integer)
    //    //    EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
    //    //else
    //    //    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");

    //    EditorGUI.BeginProperty(position, label, property);
    //    var field = property.objectReferenceValue as ElementalStatusEffect;

    //    position.width *= 0.75f;
    //    EditorGUI.PropertyField(position, property, new GUIContent(""));
        
    //    position.width /= 0.75f;
    //    position.x = position.xMax * 0.8f;

    //    var value = System.Math.Round(field.value, 4).ToString();
    //    var content = new GUIContent(value);
    //    EditorGUI.LabelField(position, content);

    //    //for (int i = 0; i < list.Count; i++) {
    //    //    var value = System.Math.Round(list[i].value, 4).ToString();
    //    //    var content = new GUIContent(value);


    //    //    //EditorGUI.PropertyField(position, property, new GUIContent(""));
    //    //    EditorGUI.LabelField(position, content);
    //    //}


    //    EditorGUI.EndProperty();
    //}
}
