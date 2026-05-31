using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates
{
    public class WireGame2 : MiniGame
    {
        // Index is determined by the collection order!
        [Serializable]
        public class WireIdentity
        {
            private const string StartPointReference = "LeftPoint";
            private const string EndPointReference = "RightPoint";
            
            [Header("Editor")]
            public string name;
            public GameObject prefab;
            
            [Header("Runtime")]
            [SerializeField, ReadOnly] private GameObject wireParent;
            //[SerializeField, ReadOnly] private int index = -1;
            [SerializeField, ReadOnly] private LineRenderer trail;
            //[SerializeField, ReadOnly] private bool isConnected;

            [field: SerializeField, ReadOnly] public Controller startController {get; private set;}
            [field: SerializeField, ReadOnly] public Controller endController {get; private set;}
            public Dictionary<Collider2D, Controller> Matches = new();
            //private Vector3 _startPoint;
            
            

            public void Init(Transform parent, Collider2D first, Collider2D second)
            {
                // TODO Instantiate from a prefab
                wireParent = Instantiate(prefab, parent);
                wireParent.transform.localPosition = Vector3.zero;
                trail = wireParent.GetComponent<LineRenderer>();
                trail.positionCount = 4;
                trail.useWorldSpace = true;
                //UpdateTrail();
                //wireParent.transform.SetParent(parent);
                //_startPoint = wireParent.transform.position;
                startController = new Controller();
                startController.Init(wireParent.transform.Find(StartPointReference)?.GetComponent<Collider2D>(), first);
                endController = new Controller();
                endController.Init(wireParent.transform.Find(EndPointReference)?.GetComponent<Collider2D>(), second);
                
                Matches.Add(startController.point, startController);
                Matches.Add(startController.goal, startController);
                Matches.Add(endController.point, endController);
                Matches.Add(endController.goal, endController);
                
            }

            private void UpdateTrail()
            {
                //trail.SetPosition(0, startController.pointTransform.position);
                //trail.SetPosition(1, endController.pointTransform.position);
            }

            public void SetActive(bool toggle) {
                wireParent.SetActive(toggle);
                //UpdateTrail();
            }

            public void Reset() {
                startController.Connect(false);
                endController.Connect(false);
                MoveWire(startController.point, startController.startPosition);
                MoveWire(endController.point, endController.startPosition);
                //isConnected = false;
            }
            
            public Vector3 MoveWire(Collider2D target, Vector3 input) {
                if(!Matches.TryGetValue(target, out var controller)) return Vector3.zero;
                controller.pointTransform.position = input;
                controller.pointTransform.localPosition = target.transform.localPosition.With(z: 0f);
                var pos = controller.pointTransform.position;
                
                // TODO line
                var startPos = startController.pointTransform.position;
                var endPos = endController.pointTransform.position;
                trail.SetPosition(0, startPos);
                trail.SetPosition(1, startPos.With(x: startPos.x + 1f));
                trail.SetPosition(2, endPos.With(x: endPos.x - 1f));
                trail.SetPosition(3, endPos);
                // trail.SetPosition(2, pos.With(x: pos.x - 0.3f));
                // trail.SetPosition(3, pos);
                return pos;
            }

            public bool TrySnapWire(Collider2D target)
            {
                if(!Matches.TryGetValue(target, out var controller)) return false;
                if (controller.IsOverlapping()) {
                    //isConnected = true;
                    MoveWire(target, controller.endPosition);
                    controller.Connect(true);
                    //controller.SetActive(false);
                    return true;
                }
                // TODO fix
                MoveWire(target, controller.startPosition);
                return false;
            }

            [Serializable]
            public class Controller
            {
                [field: SerializeField, ReadOnly] public Collider2D point {get; private set;}
                [field: SerializeField, ReadOnly] public Collider2D goal {get; private set;}
                [field: SerializeField, ReadOnly] public Vector3 startPosition {get; private set;}
                [field: SerializeField, ReadOnly] public Vector3 endPosition {get; private set;}
                [field: SerializeField, ReadOnly] public int[] trailIndices {get; private set;}
                [field: SerializeField, ReadOnly] public bool IsConnected {get; private set;}
                
                public Transform pointTransform => point.transform;
                public Transform goalTransform => goal.transform;

                public void Init(Collider2D wirePoint, Collider2D wireGoal, params int[] indices) {
                    point = wirePoint;
                    goal = wireGoal;
                    startPosition = wirePoint.transform.position;
                    endPosition = wireGoal.transform.position;
                    trailIndices = indices;
                }

                //public void SetActive(bool active) {
                    //point.enabled = active;
                    //goal.enabled = active;
                    //IsConnected = active;
                //}

                public void Connect(bool toggle) {
                    pointTransform.position = toggle ? endPosition : startPosition;
                    point.enabled = !toggle;
                    goal.enabled = !toggle;
                    IsConnected = toggle;
                }

                public bool IsOverlapping() {
                    return goal.bounds.Intersects(point.bounds);
                }
            }
        }
        
        private enum Stage { Cables, Switches }
        
        [Header("Mechanics Config")]
        // The index of these will be matched to startGoals and endGoals.
        public List<WireIdentity> wires = new();

        public Transform contentParent;
        
        [Header("Mechanics Runtime")]
        // start and end goals are found at runtime, indexed based on sibling order.
        [SerializeField, ReadOnly] private Collider2D[] startGoals = Array.Empty<Collider2D>();
        [SerializeField, ReadOnly] private Collider2D[] endGoals = Array.Empty<Collider2D>();
        [SerializeField, ReadOnly] private Collider2D[] switches = Array.Empty<Collider2D>();

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
        private WireIdentity.Controller _currentController;
        private int _score;
        private int _maxScore;
        private Vector3 _movePosition;
        private Stage _currentStage;
        private int[] _currentSwitchOrder;
        private int _currentSwitchStep;
        
        public override void Init(GameObject gamePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = gamePrefab;
            initialised = InitObjectives();
            
            InitCommon();
        }

        public bool InitObjectives()
        {
            var mechanics = contentParent?.Find("Mechanics");
            if (!SearchForGoals(mechanics)) return false;
            
            var count = wires.Count;
            if (count != startGoals?.Length || count != endGoals?.Length) {
                Debug.LogWarning("Check that there's an equal amount of start goals, end goals and wires.");
                return false;
            }
            
            _collidables.Clear();
            var wireParent = mechanics?.Find("Wires");
            var switchParent = mechanics?.Find("Switches");
            for (var i = 0; i < count; i++) {
                wires[i].Init(wireParent, startGoals[i], endGoals[i]);
                _collidables.Add(wires[i].startController.point.GetInstanceID(), wires[i]);
                _collidables.Add(wires[i].startController.goal.GetInstanceID(), wires[i]);
                _collidables.Add(wires[i].endController.point.GetInstanceID(), wires[i]);
                _collidables.Add(wires[i].endController.goal.GetInstanceID(), wires[i]);
                
                // TODO refine
                _collidables.Add(switches[i].GetInstanceID(), wires[i]);
            }
            _currentSwitchOrder = new int[switches.Length];
            return true;
        }

        private bool SearchForGoals(Transform parent) {
            var tempColliders = new HashSet<Collider2D>();
            var scope = parent?.Find("LeftSockets")?.Children();
            if (scope == null) return false;
            
            foreach (var child in scope) {
                tempColliders.Add(child.GetComponentInChildren<Collider2D>());
            }
            startGoals = tempColliders.ToArray();
            
            tempColliders.Clear();
            scope = parent.Find("RightSockets")?.Children();
            if (scope == null) return false;
            
            foreach (var child in scope) {
                tempColliders.Add(child.GetComponentInChildren<Collider2D>());
            }
            endGoals = tempColliders.ToArray();
            
            tempColliders.Clear();
            scope = parent.Find("Switches")?.Children();
            if (scope == null) return false;
            
            foreach (var child in scope) {
                tempColliders.Add(child.GetComponentInChildren<Collider2D>());
            }
            switches = tempColliders.ToArray();
            
            return true;
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
                MiniGameCommand.Start => StartGame(),
                MiniGameCommand.Reset => ResetGame(),
                MiniGameCommand.Win => WinGame(),
                MiniGameCommand.Lose => LoseGame(),
                _ => QuitGame() // ForceQuit here
            };
            
            SetState(newState);
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
            Debug.Log("Aw shucks, wrong move. Let me try again!");
            return ResetGame();
        }

        private MiniGameState StartGame() {
            OnEnabled();
            SetupGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            SetupGame();
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            OnDisabled();
            return MiniGameState.Inactive;
        }

        private void SetupGame() {
            //foreach (var wire in wires) {
            //    wire.Reset();
            //    wire.SetActive(false);
            //}
            HasWon = false;
            winScreen.SetActive(false);
            
            _currentStage = Stage.Cables;
            for (var i = 0; i < wires.Count; i++) {
                wires[i].Reset();
                wires[i].SetActive(false);
                // TODO refine
                switches[i].transform.GetChild(0).gameObject.SetActive(true);
                switches[i].transform.GetChild(1).gameObject.SetActive(false);
                switches[i].enabled = false;

                _currentSwitchOrder[i] = -1;
            }
            _maxScore = wires.Count;
            _currentSwitchStep = 0;
            SetScore(0);
        }

        private void SetScore(int score) {
            _score = score;
            if (wires.IsWithinBounds(_score)) {
                _currentWire = wires[_score];
                _currentWire.SetActive(true);
            };
        }
        
        private void SetState(MiniGameState newState, bool notifyManager = true) {
            if (state == newState) return;
            state = newState;
            if (notifyManager) MiniGameManager.Instance.NotifyMiniGameState(source, state);
        }

        private void OnClickTarget(RaycastHit2D input) {
            Debug.Log($"OnClickTarget called with {input.collider.name} : {_currentWire.name}");
            if (_currentStage == Stage.Cables) {
                _currentWire.Matches.TryGetValue(input.collider, out _currentController);
            } else {
                if(!_collidables.ContainsKey(input.collider.GetInstanceID())) return;
                input.transform.GetChild(0).gameObject.SetActive(false);
                input.transform.GetChild(1).gameObject.SetActive(true);
                input.collider.enabled = false;

                //var step = _currentSwitchStep;
                _currentSwitchOrder[_currentSwitchStep] = input.transform.GetSiblingIndex();
                
                if (_currentSwitchStep >= _currentSwitchOrder.Length-1)
                {
                    var check = true;
                    for (int i = 0; i < _currentSwitchOrder.Length; i++) {
                        var switchIndex = _currentSwitchOrder[i];
                        Debug.Log($"{switches[switchIndex]} : s-{switches[i]}");
                        if (switches[switchIndex] == switches[i]) continue;
                        check = false;
                        break;
                        
                    }
                    //var check = !switches.Where((t, i) => _currentSwitchOrder[i] != t.transform.GetSiblingIndex()).Any();
                    if (check) SendCommand(MiniGameCommand.Win);
                    else {
                        _currentSwitchStep = 0;
                        for (var i = 0; i < switches.Length; i++) {
                            switches[i].transform.GetChild(0).gameObject.SetActive(true);
                            switches[i].transform.GetChild(1).gameObject.SetActive(false);
                            switches[i].enabled = true;

                            _currentSwitchOrder[i] = -1;
                        }
                        return;
                    }
                }
                _currentSwitchStep++;
                
            }
            
            
        }

        private void OnClickState(bool input) {
            if (_currentStage == Stage.Switches) return;
            if (_currentController == null) return;
            //var wire = _collidables[_currentPoint.GetInstanceID()];
            if (!input && _currentWire.TrySnapWire(_currentController.point)) {
                if (_currentWire.startController.IsConnected && _currentWire.endController.IsConnected) {
                    if (_score < _maxScore - 1) {
                        //_currentWire.SetActive(false);
                        SetScore(_score + 1);
                    }
                    else
                    {
                        _currentStage = Stage.Switches;
                        foreach (var sw in switches) {
                            sw.enabled = true;
                        }
                        //foreach (var sw in switches)
                        //{
                        //    if (sw.transform.GetChild().gameObject.activeSelf == true)
                        //}
                        //SendCommand(MiniGameCommand.Win);
                    }
                }
            }
            
            _currentController = null;
        }
        
        private void OnClickPosition(Vector2 input) {
            if (_currentStage == Stage.Switches) return;
            if (_currentController == null) return;
            _movePosition = miniGameManager.GetWorldSpacePosition(input, -0.3f);
            _currentWire.MoveWire(_currentController.point, _movePosition);
        }
    }
}
