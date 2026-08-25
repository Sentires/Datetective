using UnityEngine;

namespace TheDates.Runtime.General
{
    // Inherit to simplify singleton pattern objects
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        public bool autoUnparentOnAwake = true;

        protected static T instance;

        public static bool HasInstance => instance != null && Application.isPlaying; //&& !isQuitting;

        public static T TryGetInstance() => HasInstance ? instance : null;
        //public static bool isQuitting { get; private set; }

        public static T Instance {
            get {
                if (!HasInstance)
                {
                    instance = FindObjectOfType<T>();
                    // instance = FindAnyObjectByType<T>();
                    // TryCreateInstance()
                }

                return instance;
            }
        }

        protected static bool TryCreateInstance()
        {
            if (!HasInstance) {
                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                instance = go.AddComponent<T>();
            }
            
            return instance != null;
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

        /*protected virtual void OnDisable() {
            instance = null;
        }
        
        protected virtual void OnEnable() {
            instance ??= Instance;
        }*/
        
        // Ensure that this is destroyed when you exit runtime
        /*protected virtual void OnApplicationQuit() {
            isQuitting = true;
            Destroy(gameObject);
        }*/
    }
    
    public class BasicSingleton<T> : MonoBehaviour where T : Component {
        protected static T instance;

        public static bool HasInstance => instance != null && Application.isPlaying;

        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance {
            get {
                if (!HasInstance)
                {
                    instance = FindObjectOfType<T>();
                }

                return instance;
            }
        }

        protected static bool TryCreateInstance()
        {
            if (!HasInstance) {
                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                instance = go.AddComponent<T>();
            }
            
            return instance != null;
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake() {
            InitialiseSingleton();
        }
        protected virtual void InitialiseSingleton() {
            if (!Application.isPlaying) return;

            if (instance == null) {
                instance = this as T;
            } else {
                if (instance != this) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
