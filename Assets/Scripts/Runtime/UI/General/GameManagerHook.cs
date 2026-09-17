using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using TheDates.Runtime;
using UnityEngine;

namespace TheDates
{
    // Tries to dynamically find and access GameManager methods/properties (yippee)
    public class GameManagerHook : MonoBehaviour
    {
        public GameManager gameManager => _gameManager ??= GameManager.Instance;
        private GameManager _gameManager;
        //void Awake() {
        //
        //}

        public void LoadGameplay() => gameManager?.LoadScene(DatetectiveSettings.instance.gameScene);
        public void LoadMainMenu() => gameManager?.LoadScene(DatetectiveSettings.instance.mainMenuScene);
        public void QuitGame() => gameManager?.QuitGame();
    }
}
