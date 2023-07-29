using UnityEngine;
using UnityEditor;

using GameFramework.System.Attributes;
using GameFramework.System;

using System;

namespace GameFramework.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(GameplayTagAttribute))]
    public class GameplayTagDrawer : PropertyDrawer
    {
        GameplayTagList TagList = Resources.Load<GameplayTagList>(ProjectStatics.ProjectTagsAssetPath);
        string[] StringList = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (TagList == null)
            {
                TagList = Resources.Load<GameplayTagList>(ProjectStatics.ProjectTagsAssetPath);
                if (TagList == null) return;
            }

            if (StringList != TagList.GetTags()) StringList = TagList.GetTags();

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