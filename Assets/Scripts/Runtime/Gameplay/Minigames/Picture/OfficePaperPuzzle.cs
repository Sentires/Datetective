using System;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using MiniGame = TheDates.Runtime.Experimental.MinigameCore.MiniGame;
using Random = UnityEngine.Random;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class OfficePaperPuzzle : MiniGame
    {
        #region [ Inspector Fields ]
        [Header("Mechanic Config")]
        [SerializeField] private float gridSnapFactor = 2;
        [SerializeField] private Vector2 scatterRange = new (1, 1);
        //[SerializeField] private int multiplier = 4;
        
        [Header("Visual Config")]
        [SerializeField] private float borderThickness = 0.1f;
        [SerializeField] private Color borderColour = Color.white;
        [SerializeField] private float zPosition = -2;
        [SerializeField] private Vector2 gameScale = new(3, 3); // can probs be factored out
        [SerializeField] private TileGridAsset gridConfig = new();
        
        [FormerlySerializedAs("tileParent")]
        [Header("Scene Dependencies")] 
        [SerializeField] private Transform contentParent;
        [SerializeField] private TextMeshProUGUI scoreText;
        //[SerializeField] private GameObject gridPrefab;
        
        //[Header("Prefab Dependencies")]
        //[SerializeField] private Transform tilePrefab;
        //[SerializeField] private Texture2D tileSource;
        
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
        
        // Tabbing out for cleanup marking
        //private float _width;
        //private float _height;
        //private Vector2Int _dimensions;
        //private List<Transform> _pieces;
        //private HashSet<int> _pieceIds;
        private LineRenderer _lineRenderer;
        private GridTile _draggedPiece;
        private Vector3 _clickOffset;

        //private GridDisplay _gridDisplay;
        //private GameObject _gridObject;
        
        private TileGrid _tileGrid;
        private BorderRenderer _borderRenderer;
        #endregion
        
        #region { Event Handlers }
        private void OnClickState(bool input) {
            if (input || !_draggedPiece) return;
            Debug.Log($"Put Down {_draggedPiece.name}");
            
            //SetSiblingPriority(_draggedPiece, null); // It'll be drawn like its other siblings
            TryGridSnap();
            
            UpdateScoreDisplay();
            if (_tileGrid.score >= _tileGrid.maxScore) {
                 SendCommand(MiniGameCommand.Win);
            }
            
            
            _draggedPiece = null;
        }

        private void OnClickTarget(RaycastHit2D hit) {
            var tile = hit.transform?.GetComponent<GridTile>();
            if (!tile || !_tileGrid.HasTile(tile)) return;
            
            _draggedPiece = tile;
            
            
            //SetSiblingPriority(null, _draggedPiece); // It'll be drawn in front of its siblings
            _clickOffset = _draggedPiece.tilePosition - (Vector3)hit.point;
            if (_tileGrid.IndexFromTile(_draggedPiece, out var index)) _tileGrid.RemoveTile(index);
            
            Debug.Log($"Picked Up {_draggedPiece.name}");
            // It'll be drawn in front of its siblings
            _draggedPiece.materialDisplay.currentSortingOrder = 12;
            _draggedPiece.transform.SetAsLastSibling(); 
        }

        private void OnClickPosition(Vector2 input) {
            if (!_draggedPiece) return;
            
            var worldPos = GetWorldSpacePosition(input, 0) + _clickOffset;
            _draggedPiece.tilePosition = worldPos;
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
            HasWon = false;
            winScreen.SetActive(false);
            _tileGrid = new TileGrid(gridConfig);
            _tileGrid.Create(contentParent);
            
            
            // Calc screen sizing
            var screenRange = GetGameScreenSize();
            // Set game scale
            var parent = _tileGrid.parent;
            parent.localScale = new Vector3(gameScale.x, gameScale.y, parent.localScale.z);
            // Scatter tiles based on screen calc
            
            ScatterPieces(screenRange.x, screenRange.y);
            
            _borderRenderer = new BorderRenderer();
            UpdateBorder();
            
            UpdateScoreDisplay();
        }
        private void ClearProgress() {
            _tileGrid.Purge();
            _borderRenderer.Clear();
        }
       
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
            InitCommon();
        }

        private void Init() {
            if (!contentParent) return;
            
            // Setup Collections
            //_pieces = new List<Transform>();
            //_pieceIds = new HashSet<int>();
            
            // Setup Line Renderer 
            _lineRenderer = contentParent.GetComponent<LineRenderer>();
            
            // TODO if (_lineRenderer.positionCount != 4) _lineRenderer.positionCount = 4;
            // TODO _lineRenderer.startWidth = 1;
            // TODO _lineRenderer.endWidth = 1;
            // TODO _lineRenderer.widthMultiplier = borderThickness;
            
            state = MiniGameState.Inactive;
            initialised = true;
            
            
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
            if (HasWon) {
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
            contentParent.gameObject.SetActive(true);
            
            PrepareGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            ClearProgress();
            // TODO _lineRenderer.enabled = true;
            _borderRenderer.SetActive(true);
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            ClearProgress();
            // TODO _lineRenderer.enabled = false;
            _borderRenderer.SetActive(false);
            OnDisabled();

            return MiniGameState.Inactive;
        }
        #endregion
        
        #region { Remaining Logic }
        
        private void UpdateScoreDisplay() {
            scoreText.text = $"Score: {_tileGrid.score} / {_tileGrid.maxScore}";
        }
        
        private void UpdateBorder() {
            var parent = _tileGrid.parent;
            var width = _tileGrid.tileWidth;
            var height = _tileGrid.tileHeight;
            var dimensions = _tileGrid.dimensions;

            var fullWidth = width * dimensions.x;
            var fullHeight = height * dimensions.y;
            
            //_borderRenderer.SetDimensions(fullWidth, fullHeight, borderThickness);
            _borderRenderer.TotalHeight = fullHeight;
            _borderRenderer.TotalWidth = fullWidth;
            //_borderRenderer.LineThickness = borderThickness;
            _borderRenderer.LineThickness = borderThickness;
            _borderRenderer.LineColour = borderColour;
            _borderRenderer.MaterialSortingOrder = 8;
            
            
            _borderRenderer.Init(parent.transform, _lineRenderer);
            _borderRenderer.SetActive(true);
            
            /*
            _lineRenderer.useWorldSpace = false;
            
            // We need to scale the border thickness by the local scale for clean edges
            // If this appears very small, check the line renderer component and debug it first
            var halfWidth = (((width * dimensions.x) + borderThickness / parent.localScale.x) / 2);
            var halfHeight = (((height * dimensions.y) + borderThickness / parent.localScale.y) / 2);
            
            // Clockwise
            _lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, 0));
            _lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, 0));
            
            _lineRenderer.enabled = true;
            */
            // TODO
        }

        private Vector2 GetGameScreenSize() {
            // Get the camera's bounds
            var rangeHeight = CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera.orthographicSize;// Camera.main.orthographicSize;
            var screenAspect = (float)Screen.width / Screen.height;
            var rangeWidth = screenAspect * rangeHeight;
            
            // scale the base piece size by the gameHolder's local size
            return new Vector2(rangeWidth, rangeHeight); // Return the calculated range
        }
        
        private void ScatterPieces(float rangeWidth, float rangeHeight) {
            var parent = _tileGrid.parent;
            var width = _tileGrid.tileWidth;
            var height = _tileGrid.tileHeight;
            var tiles = _tileGrid.orderedTiles;
            
            // Get the camera's bounds
            // var rangeHeight = CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera.orthographicSize;// Camera.main.orthographicSize;
            // var screenAspect = (float)Screen.width / Screen.height;
            // var rangeWidth = screenAspect * rangeHeight;
            
            // scale the base piece size by the gameHolder's local size
            // parent.localScale = new Vector3(gameScale.x, gameScale.y, parent.localScale.z);
            var pieceWidth = width * parent.localScale.x;
            var pieceHeight = height * parent.localScale.y;
            
            // Scale it based on scatterRange, then subtract piece's height/width
            rangeHeight = rangeHeight * scatterRange.y - pieceHeight;
            rangeWidth = rangeWidth * scatterRange.x - pieceWidth;
            
            // Randomise their positions
            foreach (var piece in tiles) {
                var x = Random.Range(-rangeWidth, rangeWidth);
                var y = Random.Range(-rangeHeight, rangeHeight);
                piece.tilePosition = new Vector3(x, y, zPosition);

                piece.materialDisplay.currentSortingOrder = 6;
            }
            
        }

        private void TryGridSnap() {
            var newOrder = 6; // aka no snap occurred 
            if (_tileGrid.FindTileAnchor(_draggedPiece.tileLocalPosition, gridSnapFactor, out var anchor)) {
                if (_tileGrid.SetTile(anchor, _draggedPiece)) {
                    var targetPosition = _tileGrid.GetTileAnchor(anchor);
                    _draggedPiece.tileLocalPosition = targetPosition;
                    newOrder = 5;
                    _draggedPiece.transform.SetAsFirstSibling(); // shouldnt be easy to target over loose pieces 
                }
            }
            _draggedPiece.materialDisplay.currentSortingOrder = newOrder;
        }
        
        private Vector3 GetWorldSpacePosition(Vector2 input, float zPos) {
            var worldPos = targetCamera.ScreenToWorldPoint(input);
            worldPos.z = zPos;
            return worldPos;
        }
        
        private void SetSiblingPriority(GridTile oldTile, GridTile newTile) {
            // This aids targeting, but currently doesn't sync well with our rendering pipeline. I should add some direct material render priority logic.
            if (oldTile) oldTile.materialDisplay.currentSortingOrder = 6;
            
            if (newTile) {
                newTile.transform.SetAsLastSibling();
                newTile.materialDisplay.currentSortingOrder = 10;
            }
            
            
            //foreach (Transform child in tileParent) {
            //    var newPos = child.position;
            //    newPos.z = zPosition - child.GetSiblingIndex() * 0.01f;
            //    child.position = newPos;
            //}
            
            //_draggedPiece.transform.parent = _tileGrid.parent;
        }
        #endregion
    }
}
