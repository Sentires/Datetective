using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class OfficeEvidencePuzzle : MiniGame
    {
        [Serializable]
        public class EvidencePreset {
            public string name = "Piece_Default";
            public PuzzlePiece prefab = null;
            [Range(0, 1)] public float pinOffset = 0.95f;
            public int weight;
        }

        public class PinData {
            public Transform transform { get; }
            public Collider2D collider { get; }
            
            public PuzzlePiece attachedPiece { get; private set; }
            
            public Vector3 position => transform.position;
            
            
            
            public PinData(Transform source) {
                transform = source;
                collider = source.GetComponent<Collider2D>();
            }

            public void Init() {
                attachedPiece = null;
                // Could be used to reset data
            }

            public void SetAttached(PuzzlePiece piece) {
                attachedPiece = piece;
            }
        }
        
        private class EvidenceData {
            public PuzzlePiece piece { get; }
            public EvidencePreset preset { get; }
            public Rigidbody2D rb { get; }
            public ObservableCollision colEvents { get; }
            public PinData currentPin {get; private set;}
            
            public bool isResetting { get; private set; }
            
            //private IEnumerator resetRoutine;
            //private WaitForSeconds resetWait;
            private const float ResetDuration = 0.5f;
            
            public EvidenceData(PuzzlePiece source, EvidencePreset preset) {
                piece = source;
                this.preset = preset;
                
                colEvents = source.GetComponent<ObservableCollision>();
                rb = source.GetComponent<Rigidbody2D>();
                //resetWait = new WaitForSeconds(1f);
            }

            public void Init(Transform parent) {
                piece.Init(parent);
                
                colEvents.AssertNull(parent, nameof(colEvents));
                rb.AssertNull(parent, nameof(rb));
                isResetting = false;
                // Could be used to reset data
            }
            
            public void SetPin(PinData pin) {
                if (isResetting) return;
                if (pin?.attachedPiece) return;
                
                // If null, it should treat this as a 'reset' to the used.
                if (currentPin != null && currentPin.attachedPiece == piece) {
                    currentPin.SetAttached(null);
                }
                
                currentPin = pin;
                currentPin?.SetAttached(piece);
                SetPinnedOffset();
            }
            
            private void SetPinnedOffset() {
                if (currentPin == null) return;
                var halfHeight = piece.GetScaledHeight(piece.contentParent.localScale.y) / 2;
                var pinOffset = Mathf.Lerp(halfHeight, -halfHeight, preset.pinOffset);
            
                var newPosition = currentPin.position;
                newPosition.y += pinOffset;
            
                piece.piecePosition = newPosition;
            }
            
            public IEnumerator Reset(Vector3 endPosition) {
                if (isResetting) yield break;
                
                isResetting = true;
                piece.clickableArea.enabled = false;
                var elapsed = 0f;
                var velocity = Vector3.zero;
                
                while (elapsed < ResetDuration)
                {
                    if (!isResetting) break; // Finish up below
                    var deltaTime = Time.deltaTime;
                    
                    elapsed += deltaTime;
                    velocity += new Vector3(0f, -1f * deltaTime, 0f);
                    piece.piecePosition += velocity;
                    
                    yield return null; // Skip next frame
                }

                piece.piecePosition = endPosition;
                isResetting = false;
                piece.clickableArea.enabled = true;

            }
        }

        public class StringData
        {
            public LineRenderer Trail;
            //public List<PinData> Points;
            public LinkedList<Transform> Points;

            public bool IsInitialised;
            //public PinData startPin;

            public StringData(LineRenderer renderer) {
                Trail = renderer;
                
                //Points = new List<PinData>();
                Points = new LinkedList<Transform>();
                Trail.useWorldSpace = true;
                Trail.positionCount = 0;
                IsInitialised = false;
            }

            public void Init(Transform startPoint) {
                //startPin = startPoint;
                Points.Clear();
                Trail.transform.localPosition = Vector3.zero;
                
                Points.AddFirst(startPoint);
                Trail.positionCount = 1; //2;
                var startPosition = startPoint.position;
                Trail.SetPosition(0, startPosition);
                //Trail.SetPosition(1, startPosition);
                
                IsInitialised = true;
            }
            
            public void Reset() {
                Points.Clear();
                Trail.positionCount = 0;
                
                IsInitialised = false;
            }

            public void AddPoint(Transform target) {
                //var hasEntry = false;
                for (var n = Points.First; n != null; n = n.Next) {
                    if (n.Value == target) return;
                }
                
                var node = Points.AddLast(target);
                
                Trail.positionCount++;
                Trail.SetPosition(Trail.positionCount - 1, node.Value.position);
            }

            public void UpdatePoint(int point) {
                
            }

            public void RemovePoint(Transform target) {
                var hasEntry = false;
                var i = 0;
                for (var n = Points.First; n != null; n = n.Next) {
                    if (n.Value == target) {
                        Points.Remove(n);
                        hasEntry = true;
                        i++;
                        continue;
                    }
                    
                    
                    i++;
                }
                //Points.RemoveAt(point);
                //for (var i = 0; i < Points.Count -1; i++) {
                //    
                //}
                //Trail.po
                //for (var node = Points.First; node != null; node = node.Next) {
                //}
            }
            
            //private Vector3[] 
        }
        
        // Inactive = not playing or completed cycle
        // Arranging = dragging n dropping pieces onto pins
        // Connecting = linking pins with string to guess the perp
        public enum Phase { Inactive, Arranging, Connecting }
        
        #region [ Inspector Fields ]
        //[Header("Mechanic Config")]
        //[Header("Visual Config")]
        //[Header("Scene Dependencies")] 
        //[Header("Prefab Dependencies")]
        
        [Header("Prefab Dependencies")]
        [SerializeField] private Transform contentParent;
        [SerializeField] private Transform evidencePinParent;
        [SerializeField] private Transform suspectPinParent;
        [SerializeField] private MeshRenderer boardMain;
        [SerializeField] private MeshRenderer boardSegmentA;
        [SerializeField] private MeshRenderer boardSegmentB;
        [SerializeField] private Button confirmButton;
        [SerializeField] private LineRenderer lineRenderer;
        
        [Header("Visual Config")]
        [SerializeField] private Vector2 gameScale = new(4, 4);
        [SerializeField, Range(0, 1)] private float suspectPinOffset = 0.95f;
        
        [Header("Mechanic Config")]
        [SerializeField] private EvidencePreset[] evidencePresets;
        [SerializeField] private PuzzlePiece[] suspects = new PuzzlePiece[3];
        
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        
        //[SerializeField, ReadOnly] private Transform[] pinLocations;
        #endregion
        
        #region [ Hidden Fields ]
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        private Camera targetCamera => !CameraManager.HasInstance ? null : CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera;
        
        private Phase _currentPhase;
        private Dictionary<PuzzlePiece, EvidenceData> _evidenceDataDictionary;
        private Dictionary<Transform, PinData> _pinDataDictionary;
        private PuzzlePiece[] _suspects;
        private Transform _evidenceParent;
        private Transform _suspectParent;
        private PinData[] _suspectPins;
        private PinData[] _evidencePins;
        private int _suspectCount;
        private PuzzlePiece _draggedPiece;
        private Vector3 _clickOffset;
        private int _arrangeCount;
        private StringData _currentStringData;
        private int _currentSuspectIndex;
        
        // Negative is towards the camera
        private const float ZClickOffset = -0.5f;

        #endregion
        
        #region { Event Handlers }
        private void OnLeftClickState(bool input) {
            switch (_currentPhase) {
                case Phase.Connecting: {
                    // TODO
                    return;
                }
                case Phase.Arranging: {
                    if (input || !_draggedPiece) return;
                    //Debug.Log($"Put Down {_draggedPiece.name}");
            
                    //SetSiblingPriority(_draggedPiece, null); // It'll be drawn like its other siblings
                    //TryGridSnap();
            
                    //UpdateScoreDisplay();
                    //if (_tileGrid.score >= _tileGrid.maxScore) {
                    //    SendCommand(MiniGameCommand.Win);
                    //}

                    var data = _evidenceDataDictionary[_draggedPiece];
            
                    if (!TrySnapToPin(_draggedPiece)) {
                        var containerBounds = GetEvidenceBounds(data.preset.weight);
                        // If we failed to snap, check if we're outside of the assigned container bounds
                        if (!IsWithinBounds(containerBounds, _draggedPiece.clickableArea.bounds)) {
                            // If we're outside said bounds, we reset it to its container
                            ResetPiece(data);
                        }
                    }
            
                    _draggedPiece.materialDisplay.currentSortingOrder = 11;
                    _draggedPiece.piecePosition = _draggedPiece.piecePosition.With(z: 0);
                    _draggedPiece = null;
                    return;
                }
                case Phase.Inactive:
                default:
                    return;
            }
        }
        //private void OnRightClickState(bool input) {
        //}

        

        private void OnClickTarget(RaycastHit2D hit, MiniGameManager.ClickType clickType) {
            switch (_currentPhase) {
                case Phase.Connecting: {
                    var piece = hit.transform?.GetComponent<PuzzlePiece>();
                    if (!piece) return;
                    if (!_evidenceDataDictionary.TryGetValue(piece, out var data)) {
                        Debug.Log("ello 1: " + _currentSuspectIndex);
                        if (_currentSuspectIndex != -1) return;
                        Debug.Log("ello 2: " + _currentSuspectIndex);
                        for (var i = 0; i < _suspects.Length; i++) {
                            if (piece != _suspects[i]) continue;
                            Debug.Log("ello 3: " + _currentSuspectIndex);
                            _currentSuspectIndex = i;
                            Debug.Log(_suspectPins[i].transform);
                            _currentStringData.Init(_suspectPins[i].transform);
                            //Debug.Log($"Suspect string attache to {_suspects[_currentSuspectIndex].name}]");
                        }
                    }
                    else {
                        if (_currentSuspectIndex == -1) return;
                        _currentStringData.AddPoint(data?.currentPin?.transform);
                    }
                    
                    // TODO
                    return;
                }
                case Phase.Arranging: {
                    var piece = hit.transform?.GetComponent<PuzzlePiece>();
                    if (!piece || !_evidenceDataDictionary.TryGetValue(piece, out var data)) return;
                    var pin = FindAttachedPin(piece, _evidencePins);

                    if (clickType == MiniGameManager.ClickType.Left) {
                        // If we have no pinned piece, left-clicking will start dragging a piece
                        if (pin?.attachedPiece)
                            return; // we cannot interact with left click if its assigned already. If it's null, we don't return

                        _draggedPiece = piece;

                        _clickOffset = _draggedPiece.piecePosition - (Vector3)hit.point;
                        
                        // It'll be drawn in front of its siblings
                        _draggedPiece.materialDisplay.currentSortingOrder = 12;
                        _draggedPiece.piecePosition = _draggedPiece.piecePosition.With(z: ZClickOffset);
                        _draggedPiece.transform.SetAsLastSibling();
                    }
                    else {
                        // If there's a pinned piece, right-clicking will release it to your pile
                        if (pin == null) return;

                        //if (data.currentPin != null) _arrangeCount--;
                        //data.SetPin(null);
                        ModifyArrangeScore(data, null);
                        ResetPiece(data);
                    }
                    return;
                }
                case Phase.Inactive:
                default:
                    return;
            }
        }
        
        private void OnPointerPosition(Vector2 input) {
            switch (_currentPhase) {
                case Phase.Connecting: {
                    if (_currentSuspectIndex == -1) return;
                    //_currentStringData
                    return;
                }
                case Phase.Arranging: {
                    if (!_draggedPiece) return;
            
                    var worldPos = MiniGameManager.Instance.GetWorldSpacePosition(input, ZClickOffset) + _clickOffset;
                    _draggedPiece.piecePosition = worldPos;
                    
                    return;
                }
                case Phase.Inactive:
                default:
                    return;
            }
        }

        private void OnDraggingCollision(Collider2D target, Collider2D origin) {
            if (_currentPhase != Phase.Arranging) return;
            if (!_draggedPiece || origin.transform != _draggedPiece.transform) return;
            var targetPiece = target.GetComponent<PuzzlePiece>();
            //var originPiece = origin.GetComponent<PuzzlePiece>();
            
            if (!targetPiece || !_evidenceDataDictionary.TryGetValue(targetPiece, out var targetData) || targetData.currentPin == null) return; //|| !originPiece || !_evidenceDataDictionary.TryGetValue(originPiece, out var originData)) return;
            ApplyCollision(targetData, _evidenceDataDictionary[_draggedPiece]);
        }

        private void ApplyCollision(EvidenceData target, EvidenceData origin) {
            if (origin.preset.weight > 1) {
                ResetTarget();

                if (target.preset.weight > 1) {
                    ResetOrigin();
                }
            }
            else {
                if (target.preset.weight == 1) {
                    ResetTarget();
                }
                
                ResetOrigin();
            }

            return;
            void ResetOrigin() {
                origin.piece.materialDisplay.currentSortingOrder = 11;
                origin.piece.piecePosition = origin.piece.piecePosition.With(z: 0);
                _draggedPiece = null;
                
                ResetPiece(origin);
            }
            
            void ResetTarget() {
                target.piece.materialDisplay.currentSortingOrder = 11;
                target.piece.piecePosition = target.piece.piecePosition.With(z: 0);
                //if (target.currentPin != null) _arrangeCount--;
                //target.SetPin(null);
                ModifyArrangeScore(target, null);
                
                ResetPiece(target);
            }
        }

        private void OnButtonSubmit() {
            switch (_currentPhase) {
                case Phase.Connecting: {
                    // TODO
                    return;
                }
                case Phase.Arranging: {
                    _currentPhase = Phase.Connecting;
                    confirmButton.interactable = false;
                    _currentSuspectIndex = -1;
                    return;
                }
                case Phase.Inactive:
                default:
                    return;
                
            }
        }
        
        #endregion
        
        #region { MonoBehaviour Lifecycle }
        //private void Awake() { }
        //private void Start() { }
        //private void OnDestroy() { }
        private void OnEnabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnLeftClickState += OnLeftClickState;
            MiniGameManager.Instance.OnClickPosition += OnPointerPosition;
            MiniGameManager.Instance.OnClickTarget += OnClickTarget;
        }
        
        private void OnDisabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnLeftClickState -= OnLeftClickState;
            MiniGameManager.Instance.OnClickPosition -= OnPointerPosition;
            MiniGameManager.Instance.OnClickTarget -= OnClickTarget;
        }
        #endregion
        
        #region { MiniGame Lifecycle }
        private void PrepareGame() {
            _currentPhase = Phase.Arranging;
            confirmButton.interactable = false;
            _arrangeCount = 0;
            
            CreateSuspects();
            CreateEvidence();
            
            contentParent.localScale = new Vector3(gameScale.x, gameScale.y, 1);
            
        }
        
        
        private void ClearProgress() {
            _currentPhase = Phase.Inactive;
            
            Destroy(_suspectParent.gameObject);
            Destroy(_evidenceParent.gameObject);
            for (var i = 0; i < _suspectCount; i++) {
                _suspects[i] = null;
            }
            
            _suspectParent = null;
            _evidenceParent = null;
            _evidenceDataDictionary.Clear();
            
            _clickOffset = Vector3.zero;
            _draggedPiece = null;
            
            confirmButton.interactable = false;
            _currentStringData.Reset();
            _currentSuspectIndex = -1;
        }
       
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
            InitCommon();
        }

        private void Init() {
            _suspectCount = suspects.Length;
            _currentPhase = Phase.Inactive;
                
            _evidenceDataDictionary = new Dictionary<PuzzlePiece, EvidenceData>();
            _pinDataDictionary = new Dictionary<Transform, PinData>();
            _suspects = new PuzzlePiece[_suspectCount];

            _suspectPins = GetPinData(suspectPinParent);
            _evidencePins = GetPinData(evidencePinParent);

            _hits = new List<Collider2D>();
            _contactFilter = new ContactFilter2D() {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("OverlayWorld"),
            };
            
            confirmButton.onClick.AddListener(OnButtonSubmit);
            confirmButton.interactable = false;

            if (lineRenderer.AssertNull(this, "String LineRenderer")) _currentStringData = new StringData(lineRenderer);

            //Debug.Log($"Init for {name}");
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
                //Debug.Log("Yippee I finished!");
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

        private void ModifyArrangeScore(EvidenceData evidenceData, PinData pinData) {
            if (evidenceData.currentPin != null && pinData == null) _arrangeCount--;
            else if (evidenceData.currentPin == null && pinData != null) _arrangeCount++;
            
            //Debug.Log($"Arrange Count: {_arrangeCount}");
            
            evidenceData.SetPin(pinData);
            confirmButton.interactable = true; // TODO - _arrangeCount >= _evidenceDataDictionary.Count;
            
            //if (_arrangeCount >= evidencePresets.Length) confirmButton.interactable = true;
        }

        private void ResetPiece(EvidenceData target) {
            var pos = GetScatteredPosition(target.piece, GetEvidenceBounds(target.preset.weight));
            StartCoroutine(target.Reset(pos));
        }

        private List<Collider2D> _hits;
        private ContactFilter2D _contactFilter;
        private bool TrySnapToPin(PuzzlePiece piece) {
            _hits.Clear();

            var count = piece.clickableArea.OverlapCollider(_contactFilter, _hits);
            if (count == 0) return false;
            
            
            PinData pin = null;
            var currentDistance = float.PositiveInfinity;
            
            for (var i = 0; i < count; i++) {
                if (!_pinDataDictionary.TryGetValue(_hits[i].transform, out var pinData) // Must be recorded in the pin dict
                    || _hits[i].transform.parent != evidencePinParent // Must be an evidence pin (under the same parent)
                    || pinData.attachedPiece) // Must lack an attached piece
                    continue;
                
                var distance = piece.clickableArea.Distance(_hits[i]).distance;
                if (distance >= currentDistance) continue;
                
                currentDistance = distance;
                pin = pinData;
            }
            
            if (pin == null) return false;
            ModifyArrangeScore(_evidenceDataDictionary[piece], pin);
            //_evidenceDataDictionary[piece].SetPin(pin);
            //if (_evidenceDataDictionary[piece].currentPin == pin) _arrangeCount++;
            
            //SetPinnedOffset(piece, suspectPinOffset, pin.position);
            //pin.SetAttached(piece);
            //Debug.Log($"Closest {pin.transform.name}, child of {pin.transform.parent.name}");
            return true;
        }

        private PinData FindAttachedPin(PuzzlePiece piece, PinData[] pins) {
            foreach (var pin in pins) {
                if (pin.attachedPiece != piece) continue;
                return pin;
            }
            
            return null;
        }

        private void CreateSuspects() {
            _suspectParent = CreateTransform("SuspectRoot", contentParent);

            var count = suspects.Length;
            _suspects = new PuzzlePiece[count];

            for (var i = 0; i < count; i++) {
                _suspectPins[i].Init();
                
                if (!suspects[i]) continue;
                _suspects[i] = CreatePiece(suspects[i], _suspectParent);
                SetPinnedOffset(_suspects[i], suspectPinOffset, _suspectPins[i].position);

                _suspects[i].materialDisplay.currentSortingOrder = 2;
            }
        }

        private void SetPinnedOffset(PuzzlePiece piece, float offset, Vector3 position) {
            var halfHeight = piece.GetScaledHeight(contentParent.localScale.y) / 2;
            var pinOffset = Mathf.Lerp(halfHeight, -halfHeight, offset);
            
            var newPosition = position;
            newPosition.y += pinOffset;
            
            piece.piecePosition = newPosition;
        }
        

        private void CreateEvidence() {
            _evidenceParent = CreateTransform("EvidenceRoot", contentParent);

            for (var i = 0; i < evidencePresets.Length; i++) {
                _evidencePins[i].Init();
                
                var preset = evidencePresets[i];
                if (!preset.prefab) continue;
                
                var piece = CreatePiece(preset.prefab, _evidenceParent);
                _evidenceDataDictionary.Add(piece, new EvidenceData(piece, preset));
                _evidenceDataDictionary[piece].colEvents.collisionEnter += OnDraggingCollision;
                    
                piece.materialDisplay.currentSortingOrder = 3;

                piece.piecePosition = GetScatteredPosition(piece, GetEvidenceBounds(preset.weight));
            }
        }
        
        
        
        private PinData[] GetPinData(Transform parent) {
            var count = parent.childCount;
            var pins = new PinData[count];
            
            for (var i = 0; i < count; i++) {
                pins[i] = new PinData(parent.GetChild(i));
                //var hash = pins[i].transform.gameObject.GetHashCode();
                _pinDataDictionary.Add(pins[i].transform, pins[i]);
            }
            
            return pins;
        }
        
        private Vector3 GetScatteredPosition(PuzzlePiece piece, Bounds area) {
            var parentDimensions = contentParent.localScale;
            var pieceWidth = piece.GetScaledWidth(parentDimensions.x);
            var pieceHeight = piece.GetScaledWidth(parentDimensions.y);
            
            // We can add more math to this later if needed
            var rangeWidth = 1f * area.extents.x - pieceWidth;
            var rangeHeight = 1f * area.extents.y - pieceHeight;
            
            
            var x = Random.Range(-rangeWidth, rangeWidth);
            var y = Random.Range(-rangeHeight, rangeHeight);

            //piece.piecePosition = area.center + new Vector3(x, y, 0);
            return area.center + new Vector3(x, y, 0);
        }
        
        private PuzzlePiece CreatePiece(PuzzlePiece piecePrefab, Transform parent) {
            var piece = Instantiate(piecePrefab, parent);
            piece.Init(contentParent);
            
            return piece;
        }
        
        // Move to helper class
        private static Transform CreateTransform(string objectName = "Generated Object", Transform parent = null) {
            var t  = new GameObject(objectName).transform;
            t.SetParent(parent);
            ResetTransform(t);
            
            return t;
        }
        // Move to helper class
        private static void ResetTransform(Transform t) {
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        }

        private Bounds GetEvidenceBounds(int weight) => (weight > 1 ? boardSegmentA : boardSegmentB).bounds;

        private bool IsWithinBounds(Bounds container, Bounds target) => container.min.x <= target.min.x && container.min.y <= target.min.y && container.max.x >= target.max.x && container.max.y >= target.max.y;

    }
}
