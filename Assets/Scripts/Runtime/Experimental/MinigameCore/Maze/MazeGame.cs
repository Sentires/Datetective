using System;
using System.Collections;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace TheDates.Runtime.Experimental.Puzzle.Maze
{
    public class MazeGame : MiniGame
    {
        [Header("Config")]
        [SerializeField] private float moveSpeed = 2f;
        
        [Header("Scene Dependencies")]
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject maskPrefab;
        
        [Header("Prefab Dependencies")]
        [SerializeField] private GameObject mazePrefab;
        [SerializeField] private Vector3 expectedLocalScale = Vector3.one;
        
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;

        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        
        private GameObject _mazeObject;
        private Transform _mazeStart;
        private Transform _mazeEnd;
        private Rigidbody2D _player;
        private bool _isMoving;
        private Vector3 _desiredPosition;
        
        private const string MazePointParent = "MazePoints";
        private const string MazePointStart = "Start";
        private const string MazePointEnd = "End";
        
        public override void Init(GameObject sourcePrefab) {
            //manager = MiniGameManager.Instance;
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
        }

        private void Init() {
            if (!contentParent || !mazePrefab) return;

            InitAssets();
            state = MiniGameState.Inactive;
            initialised = true;
        }

        private void InitAssets() {
            _mazeObject = Instantiate(mazePrefab, contentParent.transform, false);
            
            GetMazePoints();
            _player = Instantiate(playerPrefab, contentParent.transform, false).GetComponent<Rigidbody2D>();
            SetPlayer();
            var playerInteractions = _player.GetComponent<ObservableCollision>();
            playerInteractions.collisionEnter += ObservedCollision;
            maskPrefab.transform.SetParent(_player.transform, false);
            
            contentParent.gameObject.SetActive(false);
        }

        private void OnEnabled() {
            contentParent.gameObject.SetActive(true);
            _isMoving = false;
            _desiredPosition = _player.position;
            miniGameManager.OnClickPosition += OnPointPosition;
            miniGameManager.OnClickState += OnClick;
        }
        
        private void OnDisabled() {
            miniGameManager.OnClickPosition -= OnPointPosition;
            miniGameManager.OnClickState -= OnClick;
            _isMoving = false;
            _desiredPosition = _player.position;
            contentParent.gameObject.SetActive(false);
        }

        private void FixedUpdate() {
            var movement = (_desiredPosition - _player.transform.position).normalized;
            movement.z = 0;
            _player.velocity = _isMoving ? movement * moveSpeed : Vector3.zero;
        }

        private void OnClick(bool input) {
            _isMoving = input;
        }

        private void OnPointPosition(Vector2 input) {
            if (!_isMoving) return;
            _desiredPosition = miniGameManager.GetWorldSpacePosition(input, -2);
            
        }

        private void SetPlayer() {
            var pos = _mazeStart.position;
            pos.z = -2;
            var tr = _player.transform;
            tr.position = pos;
            tr.localScale = new Vector3(.8f, .8f, 1);
            tr.rotation = Quaternion.identity;
            
        }

        private void ClearAssets() {
            Destroy(_mazeObject);
        }

        private void GetMazePoints() {
            var parent = _mazeObject?.transform.Find(MazePointParent);
            _mazeStart = parent?.Find(MazePointStart);
            _mazeEnd = parent?.Find(MazePointEnd);

            if (!parent || !_mazeStart || !_mazeEnd) {
                Debug.LogWarning($"MazePoints not found. Ensure the correct names are used:\n(parent)'{MazePointParent}' = {parent?.name ?? "null"}\n'{MazePointStart}' = {_mazeStart?.name ?? "null"}\n'{MazePointEnd}' = {_mazeEnd?.name ?? "null"}");
                return;
            }
        }

        private void ObservedCollision(Collider2D target, Collider2D origin) {
            if (target.CompareTag("Hazard")) {
                AcceptCommand(MiniGameCommand.Lose);
            }
            else if (target.transform == _mazeEnd) {
                AcceptCommand(MiniGameCommand.Win);
            }
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
        
        private MiniGameState WinGame() {
            Debug.Log("Yippee I found my way there!");
            QuitGame();
            return MiniGameState.Completed;
        }

        private MiniGameState LoseGame() {
            Debug.Log("Aw shucks, wrong move. Let me try again!");
            return ResetGame();
        }

        private MiniGameState StartGame() {
            OnEnabled();
            contentParent.gameObject.SetActive(true);
            SetPlayer();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            SetPlayer();
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            OnDisabled();
            return MiniGameState.Inactive;
        }
        
        private void SetState(MiniGameState newState, bool notifyManager = true) {
            if (state == newState) return;
            state = newState;
            if (notifyManager) MiniGameManager.Instance.NotifyMiniGameState(source, state);
        }
    }
}
