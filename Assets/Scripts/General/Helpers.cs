using System.IO;
using System.Linq;
using UnityEditor;

namespace TheDates
{
    public static class Helpers
    {
        public const string TeamName = "The Dates";
        public const string AssetCreationRoot = TeamName + "/";
    }

    public static class SceneHelper
    {
        public static string[] GetBuildScenes(bool filterEnabled = true)
        {
            var scenes = filterEnabled
                ? EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                : EditorBuildSettings.scenes;
            
            var sceneNames = scenes
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();
                
            return sceneNames;
        }
    }
}
