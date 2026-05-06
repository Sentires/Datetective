using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;
using UnityEngine.Events;

namespace TheDates
{
    public class WireGame : MiniGame
    {
        [Header("Mechanic Config")]
        public List<WireIdentity> wires = new();

        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        
        private readonly Dictionary<int, WireIdentity> _collidables = new();
        private WireIdentity _currentWire;
        private int _score;
        private int _maxScore;
        private Vector3 _movePosition;
        
        public override void Init(GameObject gamePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = gamePrefab;
            
            _collidables.Clear();
            foreach (var wire in wires) {
                _collidables.Add(wire.clickPoint.GetInstanceID(), wire);
                _collidables.Add(wire.goal.GetInstanceID(), wire);
                Debug.Log($"Added Wire: {wire.clickPoint.GetInstanceID()}, {wire.goal.GetInstanceID()}");
            }
            
        }
        
        private void OnEnabled() {
            //contentParent.gameObject.SetActive(true);
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickState += OnClickState;
            MiniGameManager.Instance.OnClickPosition += OnClickPosition;
            MiniGameManager.Instance.OnClickTarget += OnClickTarget;
        }

        private void OnDisabled() {
            //contentParent.gameObject.SetActive(false);
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickState -= OnClickState;
            MiniGameManager.Instance.OnClickPosition -= OnClickPosition;
            MiniGameManager.Instance.OnClickTarget -= OnClickTarget;
        }
        
        public override void AcceptCommand(MiniGameCommand command) {
            var newState = command switch {
                MiniGameCommand.Start => StartGame1(),
                MiniGameCommand.Reset => ResetGame1(),
                MiniGameCommand.Win => WinGame1(),
                MiniGameCommand.Lose => LoseGame1(),
                _ => QuitGame1() // ForceQuit here
            };
            
            SetState(newState);
        }
        
        private MiniGameState WinGame1() {
            Debug.Log("Yippee I found my way there!");
            QuitGame1();
            return MiniGameState.Completed;
        }

        private MiniGameState LoseGame1() {
            Debug.Log("Aw shucks, wrong move. Let me try again!");
            return ResetGame1();
        }

        private MiniGameState StartGame1() {
            OnEnabled();
            SetupGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame1() {
            SetupGame();
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame1() {
            OnDisabled();
            return MiniGameState.Inactive;
        }

        private void SetupGame() {
            _score = 0;
            _maxScore = wires.Count;
            foreach (var wire in wires) {
                wire.Init();
            }
        }
        
        private void SetState(MiniGameState newState, bool notifyManager = true) {
            if (state == newState) return;
            state = newState;
            if (notifyManager) MiniGameManager.Instance.NotifyMiniGameState(source, state);
        }

        private void OnClickTarget(RaycastHit2D input) {
            if (!_collidables.TryGetValue(input.collider.GetInstanceID(), out var wire)) return;
            if (wire.clickPoint != input.collider) return;
            _currentWire = wire;
            Debug.Log($"OnClickTarget called with {input.collider.name} : {wire.name}");
        }

        private void OnClickState(bool input) {
            if (_currentWire == null) return;
            if (!input && _currentWire.TrySnapWire()) {
                _score++;
                if (_score >= _maxScore) SendCommand(MiniGameCommand.Win);
            }
            
            _currentWire = null;
        }
        
        private void OnClickPosition(Vector2 input) {
            if (_currentWire == null) return;
            _movePosition = miniGameManager.GetWorldSpacePosition(input, -0.3f);
            _currentWire.MoveWire(_movePosition);
        }

        [Serializable]
        public class WireIdentity
        {
            public string name;
            public Collider2D goal;
            public Collider2D clickPoint;
            public LineRenderer trail;
            public Transform startPoint;
            public Transform endPoint;
            public bool isConnected;
            public Vector3 movement;
            
            private EdgeCollider2D _trailCollider;

            public void Init() {
                var connectingPoint = trail.gameObject.transform.position;
                trail.SetPosition(0, connectingPoint.With(x: connectingPoint.x - 0.3f));
                trail.SetPosition(1, connectingPoint);
                MoveWire(startPoint.position);
                
                isConnected = false;
                movement = Vector3.zero;
                clickPoint.enabled = true;
                goal.enabled = true;
            }

            public bool CheckGoal() {
                return goal.bounds.Intersects(clickPoint.bounds);
            }

            public Vector3 MoveWire(Vector3 input) {
                clickPoint.transform.position = input;
                clickPoint.transform.localPosition = clickPoint.transform.localPosition.With(z: 0f);
                var pos = clickPoint.transform.position;
                trail.SetPosition(2, pos.With(x: pos.x - 0.3f));
                trail.SetPosition(3, pos);
                return pos;
            }

            public bool TrySnapWire()
            {
                if (goal.bounds.Intersects(clickPoint.bounds)) {
                    isConnected = true;
                    MoveWire(endPoint.position);
                    clickPoint.enabled = false;
                    goal.enabled = false;
                    return true;
                }

                MoveWire(startPoint.position);
                return false;
            }
        }
        
    }
    
}
