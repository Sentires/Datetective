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
            public int suspectIndex;
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
            
            private const float ResetDuration = 0.5f;
            
            public EvidenceData(PuzzlePiece source, EvidencePreset preset) {
                piece = source;
                this.preset = preset;
                
                colEvents = source.GetComponent<ObservableCollision>();
                rb = source.GetComponent<Rigidbody2D>();
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

            public void SetActive(bool toggle) {
                piece.gameObject.SetActive(toggle);
                
                if (toggle) return;
                currentPin.SetAttached(null);
                piece.transform.localPosition = Vector3.zero;
            }
        }

        public class StringData
        {
            public LineRenderer Trail;
            public List<Transform> Points;

            public bool IsInitialised;
            public int count => Points.Count;

            private HashSet<Transform> _uniquePoints;

            public StringData(LineRenderer renderer) {
                Trail = renderer;
                
                Points = new List<Transform>();
                _uniquePoints = new HashSet<Transform>();
                
                Trail.useWorldSpace = true;
                Trail.positionCount = 0;
                
                IsInitialised = false;
            }

            public void Init(Transform startPoint) {
                Points.Clear();
                Trail.transform.localPosition = Vector3.zero;
                
                Points.Add(startPoint);
                _uniquePoints.Add(startPoint);
                Trail.positionCount = 1; //2;
                Trail.SetPosition(0, startPoint.position);
                
                IsInitialised = true;
            }
            
            public void Reset() {
                Points.Clear();
                _uniquePoints.Clear();
                Trail.positionCount = 0;
                
                IsInitialised = false;
            }

            public enum LinkedOffset {
                Before, After
            }

            public void UpdatePoint(int index) {
                if (!Points.IsWithinBounds(index)) return;
                Trail.SetPosition(index, Points[index].position);
            }

            private void UpdateTrailPoints() {
                for (var i = 0; i < count; i++) {
                    Trail.SetPosition(i, Points[i].position);
                }
            }
            
            public void RemovePoint(int index) {
                if (!Points.IsWithinBounds(index)) return;
                
                _uniquePoints.Remove(Points[index]);
                Points.RemoveAt(index);
                Trail.positionCount = count;
                
                UpdateTrailPoints();
            }
            
            public void AddPoint(Transform pointSource, int index) {
                if (!pointSource) return;
                
                if(!_uniquePoints.Add(pointSource)) return;
                Points.Insert(index, pointSource);
                Trail.positionCount = count;

                UpdateTrailPoints();
            }

            public bool ContainsPoint(Transform pointSource) => _uniquePoints.Contains(pointSource);
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
        [SerializeField] private int correctSuspectIndex;
        [SerializeField] private float phaseDelay = 1f;
        
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
        private Transform _currentStringPointer;
        private WaitForSeconds _phaseTime;
        
        // Negative is towards the camera
        private const float ZClickOffset = -0.5f;

        #endregion
        
        #region { Event Handlers }
        private void OnLeftClickState(bool input) {
            if (!MiniGameManager.HasInstance) return;
            switch (_currentPhase) {
                case Phase.Connecting: {
                    // TODO
                    return;
                }
                case Phase.Arranging: {
                    if (input || !_draggedPiece) return;

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
        private void OnRightClickState(bool input) {
            if (!MiniGameManager.HasInstance) return;
            switch (_currentPhase) {
                case Phase.Connecting: {
                    if (!input || _currentSuspectIndex == -1) return;
                    if (_currentStringData.count >= 3) {
                        _currentStringData.RemovePoint(+_currentStringData.count - 2);
                    }
                    else {
                        SetSuspectTarget(-1);
                    }
                    
                    return;
                }
                case Phase.Arranging:
                case Phase.Inactive:
                default:
                    return;
            }
        }

        

        private void OnClickTarget(RaycastHit2D hit, MiniGameManager.ClickType clickType) {
            if (!MiniGameManager.HasInstance) return;
            switch (_currentPhase) {
                case Phase.Connecting: {
                    if (clickType != MiniGameManager.ClickType.Left) return;
                    
                    var piece = hit.transform?.GetComponent<PuzzlePiece>();
                    if (!piece) return;
                    
                    if (!_evidenceDataDictionary.TryGetValue(piece, out var data)) {
                        if (_currentSuspectIndex != -1) return;
                        for (var i = 0; i < _suspects.Length; i++) {
                            if (piece != _suspects[i]) continue;
                            SetSuspectTarget(i);
                            break;
                        }
                    }
                    else {
                        if (_currentSuspectIndex == -1) return;
                        _currentStringData.AddPoint(data.currentPin.transform, _currentStringData.count - 1);
                    }
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
            if (!MiniGameManager.HasInstance) return;
            switch (_currentPhase) {
                case Phase.Connecting: {
                    if (_currentSuspectIndex == -1) return;

                    _currentStringPointer.position = ClampToBounds(miniGameManager.GetWorldSpacePosition(input, 0), boardMain.bounds);
                    _currentStringData.UpdatePoint(_currentStringData.count - 1);
                    return;
                }
                case Phase.Arranging: {
                    if (!_draggedPiece) return;
            
                    var worldPos = miniGameManager.GetWorldSpacePosition(input, ZClickOffset) + _clickOffset;
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
            
            if (!targetPiece || !_evidenceDataDictionary.TryGetValue(targetPiece, out var targetData) || targetData.currentPin == null) return;
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
                ModifyArrangeScore(target, null);
                
                ResetPiece(target);
            }
        }

        private void OnButtonSubmit() {
            switch (_currentPhase) {
                case Phase.Connecting:
                {
                    var suspectIndex = _currentSuspectIndex;
                    if (EvaluateScore()) {
                        if (suspectIndex == correctSuspectIndex) {
                            _currentStringData.RemovePoint(_currentStringData.count - 1);
                            
                            StartCoroutine(DelayedWin());
                            return;
                        }
                        
                        _suspects[suspectIndex].gameObject.SetActive(false);
                        foreach (var pair in _evidenceDataDictionary) {
                            if (pair.Value.preset.suspectIndex != suspectIndex) continue;
                            pair.Value.SetActive(false);
                        }
                    }
                    SetSuspectTarget(-1);
                    return;
                }
                case Phase.Arranging: {
                    _currentPhase = Phase.Connecting;
                    SetSuspectTarget(-1);
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
            
            miniGameManager.OnLeftClickState += OnLeftClickState;
            miniGameManager.OnRightClickState += OnRightClickState;
            miniGameManager.OnClickPosition += OnPointerPosition;
            miniGameManager.OnClickTarget += OnClickTarget;
        }
        
        private void OnDisabled() {
            if (!MiniGameManager.HasInstance) return;
            
            miniGameManager.OnLeftClickState -= OnLeftClickState;
            miniGameManager.OnRightClickState -= OnRightClickState;
            miniGameManager.OnClickPosition -= OnPointerPosition;
            miniGameManager.OnClickTarget -= OnClickTarget;
        }
        #endregion
        
        #region { MiniGame Lifecycle }
        private void PrepareGame() {
            ToggleWin(false);
            
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
            _currentStringPointer.localPosition = Vector3.zero;
            
            SetSuspectTarget(-1);
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
            _hits = new List<Collider2D>();
            _contactFilter = new ContactFilter2D() {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("OverlayWorld"),
            };
            _currentStringPointer = new GameObject("String Pointer").transform;
            _currentStringPointer.parent = lineRenderer.transform;
            _phaseTime = new WaitForSeconds(phaseDelay);
            

            _suspectPins = GetPinData(suspectPinParent);
            _evidencePins = GetPinData(evidencePinParent);
            
            confirmButton.onClick.AddListener(OnButtonSubmit);
            confirmButton.interactable = false;
            if (lineRenderer.AssertNull(this, "String LineRenderer")) _currentStringData = new StringData(lineRenderer);
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
            if (notifyManager) miniGameManager.NotifyMiniGameState(source, state);
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

        private void ToggleWin(bool toggle) {
            HasWon = toggle;
            winScreen.SetActive(toggle);
        }
        #endregion
        
        private void SetSuspectTarget(int index) {
            if (index == -1) {
                _currentSuspectIndex = -1;
                _currentStringData.Reset();
                
                confirmButton.interactable = false;
            }
            else {
                _currentSuspectIndex = index;
                _currentStringData.Init(_suspectPins[index].transform);

                _currentStringPointer.position = ClampToBounds(miniGameManager.GetWorldSpacePosition(miniGameManager.mousePosition, 0), boardMain.bounds);
                _currentStringData.AddPoint(_currentStringPointer, 1);
                
                confirmButton.interactable = true;
            }
        }

        private Vector3 ClampToBounds(Vector3 input, Bounds bounds) {
            var min = bounds.min;
            var max = bounds.max;
            
            input.x = Mathf.Clamp(input.x, min.x, max.x);
            input.y = Mathf.Clamp(input.y, min.y, max.y);
            input.z = Mathf.Clamp(input.z, min.z, max.z);
            return input;
        }

        private bool EvaluateScore() {
            if (_currentSuspectIndex == -1) return false;
            
            foreach (var pair in _evidenceDataDictionary) {
                var isRequired = pair.Value.preset.suspectIndex == _currentSuspectIndex;
                var isUsed = pair.Value.currentPin != null && _currentStringData.ContainsPoint(pair.Value.currentPin.transform);
                
                // Only continue if both true or both false
                if ((isRequired && isUsed) || (!isRequired && !isUsed)) continue;
                Debug.Log($"Evidence for suspect '{_currentSuspectIndex}' contains incorrect choices");
                return false;
            }
            
            Debug.Log($"Evidence for suspect '{_currentSuspectIndex}' is all correct");
            return true;
        }

        private void ModifyArrangeScore(EvidenceData evidenceData, PinData pinData) {
            if (evidenceData.currentPin != null && pinData == null) _arrangeCount--;
            else if (evidenceData.currentPin == null && pinData != null) _arrangeCount++;
            
            evidenceData.SetPin(pinData);
            confirmButton.interactable = _arrangeCount >= _evidenceDataDictionary.Count;
        }

        private void ResetPiece(EvidenceData target) {
            if (!target.piece.gameObject.activeSelf) return;
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
        
        private IEnumerator DelayedWin() {
            confirmButton.interactable = false;
            
            yield return _phaseTime;
            SendCommand(MiniGameCommand.Win);
            _currentPhase = Phase.Inactive;
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
