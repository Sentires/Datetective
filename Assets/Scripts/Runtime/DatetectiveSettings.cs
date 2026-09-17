using Eflatun.SceneReference;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TheDates.Runtime
{
    public class DatetectiveSettings : ScriptableObject
    {
        public SceneReference mainMenuScene;
        public SceneReference gameScene;
        
        private static DatetectiveSettings _instance;
        private static string assetPath => DatetectiveSettingsProvider.AssetPath;

        public static DatetectiveSettings instance {
            get {
                if (_instance != null ||
                    (_instance = Resources.Load<DatetectiveSettings>("DatetectiveSettings")) != null) 
                    return _instance;
                
                // create if missing (in Editor)
                _instance = CreateInstance<DatetectiveSettings>();
                AssetDatabase.CreateAsset(_instance, assetPath);
                AssetDatabase.SaveAssets();

                return _instance;
            }
        }
    }

    // Unity apparently provides a class for custom settings assets :D
    public class DatetectiveSettingsProvider : SettingsProvider {
        private DatetectiveSettings _settings;
        private const string SettingsPath = "Project/Settings/DatetectiveSettings";
        internal const string AssetPath = "Assets/Resources/DatetectiveSettings.asset";

        private DatetectiveSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            _settings = AssetDatabase.LoadAssetAtPath<DatetectiveSettings>(AssetPath);
            if (_settings != null) return;
            
            _settings = ScriptableObject.CreateInstance<DatetectiveSettings>();
            AssetDatabase.CreateAsset(_settings, AssetPath);
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
