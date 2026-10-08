using System;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class PuzzleTemplate : MiniGame
    {
        #region [ Inspector Fields ]
        //[Header("Mechanic Config")]
        //[Header("Visual Config")]
        //[Header("Scene Dependencies")] 
        //[Header("Prefab Dependencies")]
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        #endregion
        
        #region [ Hidden Fields ]
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        private Camera targetCamera => !CameraManager.HasInstance ? null : CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera;
        #endregion
        
        #region { Event Handlers }
        private void OnClickState(bool input) {
        }

        private void OnClickTarget(RaycastHit2D hit) {
        }

        private void OnClickPosition(Vector2 input) {
        }
        #endregion
        
        #region { MonoBehaviour Lifecycle }
        //private void Awake() { }
        //private void Start() { }
        //private void OnDestroy() { }
        private void OnEnabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickState += OnClickState;
            MiniGameManager.Instance.OnClickPosition += OnClickPosition;
            MiniGameManager.Instance.OnClickTarget += OnClickTarget;
        }
        
        private void OnDisabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickState -= OnClickState;
            MiniGameManager.Instance.OnClickPosition -= OnClickPosition;
            MiniGameManager.Instance.OnClickTarget -= OnClickTarget;
        }
        #endregion
        
        #region { MiniGame Lifecycle }
        private void PrepareGame() {
        }
        private void ClearProgress() {
        }
       
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
            InitCommon();
        }

        private void Init() {
        }

        public override void AcceptCommand(MiniGameCommand command) {
            var newState = command switch {
                MiniGameCommand.Start => StartGame(),
                MiniGameCommand.Reset => ResetGame(),
                MiniGameCommand.Win => WinGame(),
                MiniGameCommand.Lose => LoseGame(),
                _ => QuitGame() // ForceQuit here
            };
            
            SetState(newState);
        }
        
        private void SetState(MiniGameState newState, bool notifyManager = true) {
            if (state == newState) return;
            state = newState;
            if (notifyManager) MiniGameManager.Instance.NotifyMiniGameState(source, state);
        }
        
        private MiniGameState WinGame() {
            if (HasWon)
            {
                Debug.Log("Yippee I finished!");
                QuitGame();
                return MiniGameState.Completed;
            }
            
            OpenWinPrompt();
            return MiniGameState.Active;
        }

        private MiniGameState LoseGame() {
            return ResetGame();
        }

        private MiniGameState StartGame() {
            OnEnabled();
            
            PrepareGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            ClearProgress();
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            ClearProgress();
            OnDisabled();

            return MiniGameState.Inactive;
        }
        #endregion
        
        
    }
}
