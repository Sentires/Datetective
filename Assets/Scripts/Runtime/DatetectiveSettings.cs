using Eflatun.SceneReference;
using UnityEditor;
using UnityEngine;

namespace TheDates.Runtime
{
    public class DatetectiveSettings : ScriptableObject
    {
        public SceneReference mainMenuScene;
        public SceneReference gameScene;
        
        private static DatetectiveSettings _instance;
        public const string AssetPath = "Assets/Resources/DatetectiveSettings.asset";

        public static DatetectiveSettings instance {
            get {
                if (_instance != null ||
                    (_instance = Resources.Load<DatetectiveSettings>("DatetectiveSettings")) != null) 
                    return _instance;
                
                // create if missing (in Editor)
        #if UNITY_EDITOR
                _instance = CreateInstance<DatetectiveSettings>();
                AssetDatabase.CreateAsset(_instance, AssetPath);
                AssetDatabase.SaveAssets();
        #endif

                return _instance;
            }
        }
    }

    // Unity apparently provides a class for custom settings assets :D
}
