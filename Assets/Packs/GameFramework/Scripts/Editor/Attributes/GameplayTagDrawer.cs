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
        string[] stringList = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (stringList != GameplayTagsManager.TagsNames) stringList = GameplayTagsManager.TagsNames;

            if (stringList != null && stringList.Length != 0)
            {
                SerializedProperty serializedProperty = property.FindPropertyRelative("TagName");

                int selectedIndex = Mathf.Max(Array.IndexOf(stringList, serializedProperty.stringValue), 0);
                selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList);
                serializedProperty.stringValue = stringList[selectedIndex];
            }
            else EditorGUI.PropertyField(position, property, label);
        }
    }
}