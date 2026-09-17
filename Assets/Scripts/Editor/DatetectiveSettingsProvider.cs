using TheDates.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TheDates.Editor
{
    public class DatetectiveSettingsProvider : SettingsProvider {
        private DatetectiveSettings _settings;
        private const string SettingsPath = "Project/Settings/DatetectiveSettings";
		private static string assetPath => DatetectiveSettings.AssetPath;

        private DatetectiveSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            _settings = AssetDatabase.LoadAssetAtPath<DatetectiveSettings>(assetPath);
            if (_settings != null) return;
            
            _settings = ScriptableObject.CreateInstance<DatetectiveSettings>();
            AssetDatabase.CreateAsset(_settings, assetPath);
        }

        public override void OnGUI(string searchContext) {
            if (_settings == null) return;
            
            var serializedObject = new SerializedObject(_settings);
            EditorGUI.BeginChangeCheck();
            
            // We need to declare all settings we want to add
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DatetectiveSettings.mainMenuScene)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DatetectiveSettings.gameScene)));

            if (!EditorGUI.EndChangeCheck()) return;
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_settings); // Doesnt save if you dont do this
        }

        [SettingsProvider]
        public static SettingsProvider CreateDatetectiveSettingsProvider() {
            return new DatetectiveSettingsProvider(SettingsPath); // Already defaults as SettingsScope.Project
        }
    }
}