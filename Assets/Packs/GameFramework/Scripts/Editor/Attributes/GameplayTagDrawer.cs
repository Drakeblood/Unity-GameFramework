using System;

using UnityEngine;
using UnityEditor;

using GameFramework.System.Attributes;
using GameFramework.System;

namespace GameFramework.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(GameplayTagAttribute))]
    public class GameplayTagDrawer : PropertyDrawer
    {
        string[] StringList = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (StringList != GameplayTagsManager.GetTagsNames()) StringList = GameplayTagsManager.GetTagsNames();

            if (StringList != null && StringList.Length != 0)
            {
                SerializedProperty SerializedProperty = property.FindPropertyRelative("TagName");

                int SelectedIndex = Mathf.Max(Array.IndexOf(StringList, SerializedProperty.stringValue), 0);
                SelectedIndex = EditorGUI.Popup(position, property.name, SelectedIndex, StringList);
                SerializedProperty.stringValue = StringList[SelectedIndex];
            }
            else EditorGUI.PropertyField(position, property, label);
        }
    }
}