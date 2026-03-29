using System.Collections;
using System.IO;
using System.Linq;
using TheDates.Runtime.Quests;
using UnityEditor;
using UnityEngine;

namespace TheDates.Runtime.General
{
    public static class GameExtensions {
        public const string TeamName = "The Dates";
        public const string AssetCreationRoot = TeamName + "/";
        
        // Encompasses Colliders, Transforms, GameObjects, MonoBehaviours, etc.
        // Seems more effective than separate methods for each
        public static bool IsPlayer(this Behaviour target) => target != null && target.CompareTag("Player");
    }
}

namespace TheDates
{
    public static class DebugExtensions {
        public static void LogObjectError(string message, Object sender) {
            if (sender) {
                Debug.LogError(message, sender);
                return;
            }
            Debug.LogError(message);
        }
        
        public static bool AssertNull(this object target, object sender, string identifier = "Object", string bonusMessage = "") {
            if (target is Object targetObj) {
                if (targetObj) return true; // Check as a Unity Object
            }
            else {
                if (target != null) return true; // Check as a standard object
            }
            
            // If null, LogObjectError just ignores it
            var senderObj = sender as Object;
            bonusMessage = string.IsNullOrEmpty(bonusMessage) ? "No further context provided." : bonusMessage;
            LogObjectError($"{identifier} ({nameof(target)}) has been declared null by '{nameof(sender)}'.\n{bonusMessage}", senderObj);
            
            return false;
        }
    }

    
    public static class CollectionExtensions
    {
        // General extensions for shorthand
        public static bool IsWithinBounds(this ICollection collection, int index) => index > 0 || index <= collection.Count;
        public static bool IsEmpty(this ICollection collection) => collection.Count == 0;
        public static bool IsNullOrEmpty(this ICollection collection) => collection?.IsEmpty() ?? true;
    }

    public static class SceneHelpers
    {
        // Return all the build's scenes
        public static string[] GetBuildScenes(bool filterEnabled = true) {
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
