using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using GameFramework.System.Attributes;

namespace GameFramework.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(MonoScriptAttribute), false)]
    public class MonoScriptPropertyDrawer : PropertyDrawer
    {
        static Dictionary<string, MonoScript> scriptCache;
        static MonoScriptPropertyDrawer()
        {
            scriptCache = new Dictionary<string, MonoScript>();
            var scripts = Resources.FindObjectsOfTypeAll<MonoScript>();
            for (int i = 0; i < scripts.Length; i++)
            {
                var type = scripts[i].GetClass();
                if (type != null && !scriptCache.ContainsKey(type.FullName))
                {
                    scriptCache.Add(type.FullName, scripts[i]);
                }
            }
        }
        bool viewString = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                Rect r = EditorGUI.PrefixLabel(position, label);
                Rect labelRect = position;
                labelRect.xMax = r.xMin;
                position = r;
                viewString = GUI.Toggle(labelRect, viewString, "", "label");
                if (viewString)
                {
                    property.stringValue = EditorGUI.TextField(position, property.stringValue);
                    return;
                }
                MonoScript script = null;
                string typeName = property.stringValue;
                if (!string.IsNullOrEmpty(typeName))
                {
                    scriptCache.TryGetValue(typeName, out script);
                    if (script == null)
                        GUI.color = Color.red;
                }

                script = (MonoScript)EditorGUI.ObjectField(position, script, typeof(MonoScript), false);
                if (GUI.changed)
                {
                    if (script != null)
                    {
                        var Type = script.GetClass();
                        MonoScriptAttribute attr = (MonoScriptAttribute)attribute;
                        if (attr.Type != null && !attr.Type.IsAssignableFrom(Type))
                            Type = null;
                        if (Type != null)
                            property.stringValue = Type.FullName;
                        else
                            Debug.LogWarning("The script file " + script.name + " doesn't contain an assignable class");
                    }
                    else
                        property.stringValue = "";
                }
            }
            else
            {
                GUI.Label(position, "The MonoScript attribute can only be used on string variables");
            }
        }
    }
}