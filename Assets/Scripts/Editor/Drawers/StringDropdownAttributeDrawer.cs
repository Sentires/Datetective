using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TheDates.General
{
    [CustomPropertyDrawer(typeof(StringDropdownAttribute))]
    public class StringDropdownAttributeDrawer : PropertyDrawer
    {
        private string[] _options;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String && attribute is StringDropdownAttribute attr)
            {
                if (attr.IsDynamic) {
                    var targetObj = property.serializedObject.targetObject;
                    _options = GetOptions(targetObj, attr.SourceName);
                }
                else _options = attr.Options;
            
                if (_options == null) {
                    EditorGUI.LabelField(position, label.text, "List source missing or invalid.");
                    return;
                }
            
                // Get the current value and index
                int index = System.Array.IndexOf(_options, property.stringValue);

                if (index == -1) {
                    property.stringValue = string.Empty;
                    index = 0; // Default the visual dropdown to the first item (or add an "Empty" entry)
                }
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label.text, index, _options);
                
                if (EditorGUI.EndChangeCheck()) {
                    property.stringValue = _options[index];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
        
        private string[] GetOptions(object target, string name) {
            var type = target.GetType();
        
            // 1. Try Method
            var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null) return (string[])method.Invoke(target, null);

            // 2. Try Property (includes Auto-Properties)
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null) return (string[])prop.GetValue(target);

            // 3. Try Field
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null) return (string[])field.GetValue(target);

            return null;
        }
    }
}
