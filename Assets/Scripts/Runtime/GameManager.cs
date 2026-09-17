using System;
using Eflatun.SceneReference;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheDates.Runtime
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        // Probably dont need to be this complex, leaving this for now
        /*[Flags]
        public enum GameUpdate
        {
            Freeze = 0,
            Physics = 1 << 0,
            Animation = 1 << 1,
            Time = 1 << 2,
            Default = ~0
        }*/
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LazyInstance() {
            if (!TryCreateInstance()) {
                Debug.LogWarning($"Creating new {nameof(GameManager)}");
            }
        }

        public bool isPaused;
        
        public void Start()
        {
            
        }

        public void LoadScene(SceneReference sceneName) {
            SceneManager.LoadScene(sceneName.Path, LoadSceneMode.Single);
        }
        
        public void QuitGame() {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }
}
