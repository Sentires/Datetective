using System.Collections;
using System.Collections.Generic;
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

    public static class Vector3Extensions
    {
        // Shorthand methods
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) {
            return new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }
        public static Vector3 Subtract(this Vector3 vector, float x = 0, float y = 0, float z = 0) {
            return new Vector3(vector.x - x, vector.y - y, vector.z - z);
        }
        public static Vector3 Multiply(this Vector3 vector, float x = 0, float y = 0, float z = 0) {
            return new Vector3(vector.x * x, vector.y * y, vector.z * z);
        }
        
        public static Vector3 Divide(this Vector3 vector, float x = 0, float y = 0, float z = 0) {
            return new Vector3(Divide(vector.x, x), Divide(vector.y, y), Divide(vector.z, z));
        }
        
        // Avoid errors
        private static float Divide(float dividend, float divisor) {
            return dividend == 0 && divisor == 0 ? 0 : dividend / divisor;
        }
    }
    
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> Children(this Transform parent) {
            foreach (Transform child in parent) {
                yield return child;
            }
        }
    }

    //public static class Vector2Extensions
    //{
    //    
    //}

    public static class QuestExtensions {
        public static bool AssertValidation(this QuestInfo quest, object sender, string bonusMessage = "") {
            return quest.AssertNull(sender, "QuestInfo", bonusMessage);
        }
    }
    
    public static class CollectionExtensions
    {
        // General extensions for shorthand
        public static bool IsWithinBounds(this ICollection collection, int index) => index > 0 || index <= collection.Count;
        public static bool IsEmpty(this ICollection collection) => collection.Count == 0;
        public static bool IsNullOrEmpty(this ICollection collection) => collection?.IsEmpty() ?? true;
    }

#if UNITY_EDITOR
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
#endif
    
}
