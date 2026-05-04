using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;
using UnityEngine.Events;

namespace TheDates
{
    public class WireGame : MonoBehaviour//, IMiniGame
    {
        public List<WireIdentity> wires = new();
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [field: SerializeField, ReadOnly] public bool isInitialised { get; private set; }
        [field: SerializeField, ReadOnly] public MiniGameState gameState { get; private set; }
        
        private Dictionary<int, WireIdentity> _collidables = new();
        private WireIdentity _currentWire;
        private int _score;

        private Vector3 _movePosition;
        // Start is called before the first frame update
        void Start()
        {
            
            foreach (var wire in wires)
            {
                _collidables.Add(wire.clickPoint.GetInstanceID(), wire);
                _collidables.Add(wire.goal.GetInstanceID(), wire);
                wire.Init();
                //wire.goal.transform.
                Debug.Log($"Added Wire: {wire.clickPoint.GetInstanceID()}, {wire.goal.GetInstanceID()}");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void OnEnabled()
        {
            miniGameManager.OnClickPosition += OnPointPosition;
            miniGameManager.OnClickState += OnClickState;
            miniGameManager.OnClickTarget += OnClickTarget;
        }

        

        private void OnDisabled()
        {
            miniGameManager.OnClickPosition -= OnPointPosition;
            miniGameManager.OnClickState -= OnClickState;
            miniGameManager.OnClickTarget -= OnClickTarget;
        }

        private void OnClickTarget(RaycastHit2D input)
        {
            //throw new NotImplementedException();
            
            //Debug.Log($"OnClickTarget Any called with {input.collider.name}");
            //if ( != null)
            if (!_collidables.TryGetValue(input.collider.GetInstanceID(), out var wire)) return;
            if (wire.clickPoint != input.collider) return;
            _currentWire = wire;
            Debug.Log($"OnClickTarget called with {input.collider.name} : {wire.name}");
        }

        private void OnClickState(bool input)
        {
            if (_currentWire == null) return;
            //_currentWire.clickPoint.bounds.Intersects(_currentWire.goal.bounds)
            //var fulfilled = miniGameManager.Raycast(_movePosition).collider != null;
            //if (_currentWire.clickPoint.bounds.Intersects(_currentWire.goal.bounds)) Debug.Log("Bounds hit");
            if (!input && _currentWire.TrySnapWire()) {
                Debug.Log("hit");
                //_currentWire.isConnected = true;
                //_currentWire.clickPoint.transform.position = _currentWire.goal.transform.position;
                //_currentWire.clickPoint.enabled = false;
                //_currentWire.goal.enabled = false;
                _score++;
                //return;
            }
            else {
                Debug.Log("not hit");
                //_currentWire.clickPoint.transform.position = _currentWire.startPoint.position;
            }
            
            _currentWire = null;
        }
        private void OnPointPosition(Vector2 input)
        {
            if (_currentWire == null) return;
            _movePosition = miniGameManager.GetWorldSpacePosition(input, -0.3f);
            //_currentWire.clickPoint.transform.position = _movePosition;
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

            public void Init()
            {
                //startPoint.localPosition = startPoint.localPosition.With(z: -0.3f);
                //clickPoint.transform.position = startPoint.position;
                var connectingPoint = trail.gameObject.transform.position;
                trail.SetPosition(0, connectingPoint.With(x: connectingPoint.x - 0.3f));
                trail.SetPosition(1, connectingPoint);
                MoveWire(startPoint.position);
                //trail.transform.localPosition = trail.transform.localPosition.With(z: -0.2f);
                //goal.transform.localPosition = goal.transform.localPosition.With(z: -0.1f);
                
                
                //trail.
                isConnected = false;
                movement = Vector3.zero;
                clickPoint.enabled = true;
                goal.enabled = true;
            }

            public bool CheckGoal() {
                return goal.bounds.Intersects(clickPoint.bounds); //clickPoint.IsTouching(goal);
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
                if (goal.bounds.Intersects(clickPoint.bounds))
                {
                    isConnected = true;
                    //clickPoint.transform.position = endPoint.position;
                    MoveWire(endPoint.position);
                    clickPoint.enabled = false;
                    goal.enabled = false;
                    return true;
                }

                MoveWire(startPoint.position);
                //clickPoint.transform.position = startPoint.position;
                return false;
            }
        }
        
        [field: SerializeField, ReadOnly] public GameObject source { get; private set; }
        public void Init(GameObject prefab)
        {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            source = prefab;
            
        }

        public void StartGame()
        {
            OnEnabled();
            //throw new NotImplementedException();
        }

        public void ResetGame()
        {
            //throw new NotImplementedException();
        }

        public void StopGame()
        {
            OnDisabled();
            //throw new NotImplementedException();
        }
    }
    
}
