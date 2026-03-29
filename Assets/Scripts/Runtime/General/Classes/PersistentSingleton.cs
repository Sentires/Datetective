using UnityEngine;

namespace TheDates.Runtime.General
{
    // Inherit to simplify singleton pattern objects
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        public bool autoUnparentOnAwake = true;

        protected static T instance;

        public static bool HasInstance => instance && Application.isPlaying;

        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance {
            get {
                if (!instance)
                {
                    instance = FindObjectOfType<T>();
                    //instance = FindAnyObjectByType<T>();
                    if (!HasInstance) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake() {
            InitialiseSingleton();
        }
        protected virtual void InitialiseSingleton() {
            if (!Application.isPlaying) return;

            if (autoUnparentOnAwake) {
                transform.SetParent(null);
            }

            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else {
                if (instance != this) {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnDisable() {
            instance = null;
        }
        
        // Ensure that this is destroyed when you exit runtime
        protected virtual void OnApplicationQuit() {
            Destroy(gameObject);
        }
    }
}
