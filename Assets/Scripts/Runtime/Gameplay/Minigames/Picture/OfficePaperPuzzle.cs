using System;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class OfficePaperPuzzle : MiniGame
    {
        [Serializable]
        public class TileGridAsset
        {
            public Texture2D texture;
            public GameObject tilePrefab;
            public GameObject tileGridPrefab;
            
            public Vector2Int tiling = new(2, 2);
            public float gridLineThickness = 0.01f;
            public Color gridLineColor = Color.white;
        }
        
        public class TileGrid
        {
            public Transform Parent;
            public GameObject Grid;
            public GridDisplay GridDisplay;
            public int Score;
            public int MaxScore;
            
            public float Width;
            public float Height;
            public Vector2Int Dimensions;
            public  List<Transform> Tiles;
            
            private HashSet<int> _registeredTiles;
            private readonly TileGridAsset _tileGridAsset;
            private Dictionary<Vector2Int, Vector2> _tileAnchors;
            private Transform[,] _tileMapping;
            private Dictionary<Transform, Vector2Int> _desiredMapping;
            

            
            

            public TileGrid(TileGridAsset tileGridAsset) {
                _tileGridAsset = tileGridAsset;
            }

            public void Create(Transform parent) {
                Parent = parent;
                
                Tiles = new List<Transform>();
                _registeredTiles = new HashSet<int>();
                _tileAnchors = new Dictionary<Vector2Int, Vector2>();

                Dimensions = _tileGridAsset.tiling;
                
                CreatePieces(_tileGridAsset.texture);
                CreateGrid();

                Score = 0;
                MaxScore = Tiles.Count;
            }

            public bool HasTile(Transform target) => _registeredTiles.Contains(target.GetInstanceID());

            public void Purge() {
                if (Tiles.Count <= 0) return;
                for (var i = Tiles.Count - 1; i >= 0; i--) {
                    Destroy(Tiles[i]?.gameObject);
                }
                Tiles.Clear();
                _registeredTiles.Clear();
                _tileAnchors.Clear();
                _tileMapping = null;
                _desiredMapping.Clear();
            }
            
            private void CreateGrid()
            {
                Grid = Instantiate(_tileGridAsset.tileGridPrefab, Parent);
                Grid.name = "Grid";
                var grid = Grid.transform;
            
                grid.localPosition = new Vector3(0, 0, 0);
                grid.localScale = new Vector3(Width * Dimensions.x, Height * Dimensions.y, 1);

                GridDisplay = new GridDisplay(Grid.GetComponent<MeshRenderer>());
                
                GridDisplay.SetLineColour(_tileGridAsset.gridLineColor);
                GridDisplay.SetLineThickness(_tileGridAsset.gridLineThickness);
                GridDisplay.SetGridTiling(Dimensions);

                Grid.GetComponent<Collider>().enabled = false;
            }
            
            private Vector2Int GetDimensions(Texture2D texture, int divisions) {
                //var dimensions = Vector2Int.zero;
                var div = _tileGridAsset.tiling;
                var dimensions = Vector2Int.zero;
                if (texture.width < texture.height) {
                    //dimensions.x = divisions;
                    //dimensions.y = (divisions * texture.height) / texture.width;
                    dimensions.x = div.x;
                    dimensions.y = (div.y * texture.height) / texture.width;
                } else {
                    //dimensions.x = (divisions * texture.width) / texture.height;
                    //dimensions.y = divisions;
                    dimensions.x = (div.x * texture.width) / texture.height;
                    dimensions.y = div.y;
                }

                return div; //dimensions;
            }
            
            private void CreatePieces(Texture2D texture) {
                Height = 1f / Dimensions.y;
                var aspect = (float)texture.width / texture.height;
                Width = aspect / Dimensions.x;
                
                _tileMapping = new Transform[Dimensions.x, Dimensions.y];
                _desiredMapping = new Dictionary<Transform, Vector2Int>();

                for (var row = 0; row < Dimensions.y; row++) {
                    for (var col = 0; col < Dimensions.x; col++) {
                        // Make & add the piece
                        var piece = CreatePiece(col, row, texture);
                        Tiles.Add(piece);
                        _registeredTiles.Add(piece.GetInstanceID());

                        var anchorIndex = new Vector2Int(col, row);
                        var anchor = GetPositionAt(col, row);
                        _tileAnchors.Add(anchorIndex, anchor);
                        _desiredMapping[piece] = anchorIndex;
                    }
                }
            }

            public bool SetTile(Vector2Int index, Transform tile) {
                // If the new tile is null, no record of that tile exists, or the index is already filled, then return false
                if (!tile || !_desiredMapping.ContainsKey(tile) || _tileMapping[index.x, index.y]) return false;
                _tileMapping[index.x, index.y] = tile;
                if (_desiredMapping[tile] == index) Score++;
                
                Debug.Log($"Added tile {index}, Completion: {Score} / {_desiredMapping.Count}");
                return true;
            }

            public bool RemoveTile(Vector2Int index) {
                var tile = _tileMapping[index.x, index.y];
                if (!tile) return false;
                
                if (_desiredMapping[tile] == index) Score--;
                _tileMapping[index.x, index.y] = null;
                
                Debug.Log($"Removed tile {index}, Completion: {Score} / {_desiredMapping.Count}");
                return true;
            }

            public bool IndexFromTile(Transform target, out Vector2Int index) {
                index = default;
                for (var row = 0; row < Dimensions.y; row++) {
                    for (var col = 0; col < Dimensions.x; col++) {
                        if (!_tileMapping[col, row] || target != _tileMapping[col, row]) continue;
                        index = new Vector2Int(col, row);
                        return true;
                    }
                }
                
                return false;
            }
            
            private Transform CreatePiece(int col, int row, Texture2D pieceTexture) {
                var piece = Instantiate(_tileGridAsset.tilePrefab.transform, Parent.transform);
                piece.name = $"Piece {(row * Dimensions.x) + col}";

                AssignPieceTransform(piece, row, col);
                AssignPieceTexture(piece, row, col, pieceTexture);

                return piece;
            }
            
            private void AssignPieceTransform(Transform piece, int row, int col) {
                piece.localPosition = new Vector3(
                    GetAlignment(Width, Dimensions.x, col),
                    GetAlignment(Height, Dimensions.y, row),
                    0);
                piece.localScale = new Vector3(Width, Height, 1);
                return;

                float GetAlignment(float length, int count, int index) => -length * count / 2 + length * index + length / 2;
            }


            private void AssignPieceTexture(Transform piece, int row, int col, Texture2D pieceTexture) {
                // Calculate the dimensions
                var width = 1f / Dimensions.x;
                var height = 1f / Dimensions.y;

                // Create the new UV
                var uv = new Vector2[] {
                    new (width * col, height * row), // 1
                    new (width * (col + 1), height * row), // 2
                    new (width * col, height * (row + 1)), // 3
                    new (width * (col + 1), height * (row + 1)) // 4
                };

                // Get the MeshFilter component & assign the new UV
                var mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;

                // Get the MeshRenderer component & assign the texture
                piece.GetComponent<MeshRenderer>().material.SetTexture(MatTextureID, pieceTexture);
            }

            private Vector2 GetPositionAt(int col, int row) {
                var targetPosition = new Vector2(
                    (-Width * Dimensions.x / 2) + (Width * col) + (Width / 2),
                    (-Height * Dimensions.y / 2) + (Height * row) + (Height / 2)
                );
                Debug.Log("Target position for tile is: " + targetPosition);
                return targetPosition;
            }

            public Vector2 GetTileAnchor(Vector2Int index) {
                return _tileAnchors[index];
            }

            public bool FindTileAnchor(Vector2 localPosition, float sensitivity, out Vector2Int nearestAnchor) {
                nearestAnchor = Vector2Int.zero;
                var currentDistance = float.PositiveInfinity;
                var matchFactor = Width / sensitivity;
                
                foreach (var pair in _tileAnchors) {
                    var anchorDistance = Vector2.Distance(localPosition, pair.Value);
                    if (anchorDistance < matchFactor)
                    {
                        if (anchorDistance > currentDistance) continue;
                        currentDistance = anchorDistance;
                        nearestAnchor = pair.Key;
                    }
                    
                }
                
                return !float.IsInfinity(currentDistance);
            }
        }
        
        #region [ Inspector Fields ]
        [Header("Mechanic Config")]
        [SerializeField] private float gridSnapFactor = 2;
        [SerializeField] private Vector2 scatterRange = new (1, 1);
        [SerializeField] private int multiplier = 4;
        
        [Header("Visual Config")]
        [SerializeField] private float borderThickness = 0.1f;
        [SerializeField] private float zPosition = -2;
        [SerializeField] private Vector2 gameScale = new(3, 3);
        [SerializeField] private TileGridAsset gridConfig = new();
        
        [Header("Scene Dependencies")] 
        [SerializeField] private Transform tileParent;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject gridPrefab;
        
        [Header("Prefab Dependencies")]
        [SerializeField] private Transform tilePrefab;
        [SerializeField] private Texture2D tileSource;
        
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        #endregion
        
        #region [ Hidden Fields ]
        private static readonly int MatTextureID = Shader.PropertyToID("_BaseMap");
        private static readonly int MatColourID = Shader.PropertyToID("_BaseColor");
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        private Camera targetCamera => !CameraManager.HasInstance ? null : CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera;
        
        private float _width;
        private float _height;
        private Vector2Int _dimensions;
        private List<Transform> _pieces;
        private HashSet<int> _pieceIds;
        private LineRenderer _lineRenderer;
        private Transform _draggedPiece;
        private Vector3 _clickOffset;
        private int _score;
        private GridDisplay _gridDisplay;
        private GameObject _gridObject;
        
        private TileGrid _tileGrid;
        #endregion
        
        #region { Event Handlers }
        private void OnClickState(bool input) {
            if (input || !_draggedPiece) return;
            Debug.Log($"Put Down {_draggedPiece.name}");
            
            TryGridSnap();
            
            UpdateScoreDisplay();
            if (_tileGrid.Score >= _tileGrid.MaxScore) {
                 SendCommand(MiniGameCommand.Win);
            }
            
            _draggedPiece = null;
        }

        private void OnClickTarget(RaycastHit2D hit) {
            if (!hit || !_tileGrid.HasTile(hit.transform)) return;
            
            _draggedPiece = hit.transform;
            SetSiblingPriority(); // It'll be drawn in front of its siblings
            _clickOffset = _draggedPiece.position - (Vector3)hit.point;
            if (_tileGrid.IndexFromTile(_draggedPiece, out var index)) _tileGrid.RemoveTile(index);
            
            Debug.Log($"Picked Up {_draggedPiece.name}");
        }

        private void OnClickPosition(Vector2 input) {
            if (!_draggedPiece) return;
            
            var worldPos = GetWorldSpacePosition(input, 0) + _clickOffset;
            _draggedPiece.position = worldPos;
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
            _tileGrid.Create(tileParent);
            ScatterPieces();
            UpdateBorder();
            
            _score = 0;
            UpdateScoreDisplay();
        }
        private void ClearProgress() {
            _tileGrid.Purge();
        }
       
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
            InitCommon();
        }

        private void Init() {
            if (!tileParent) return;
            
            // Setup Collections
            _pieces = new List<Transform>();
            _pieceIds = new HashSet<int>();
            
            // Setup Line Renderer 
            _lineRenderer = tileParent.GetComponent<LineRenderer>();
            if (_lineRenderer.positionCount != 4) _lineRenderer.positionCount = 4;
            _lineRenderer.startWidth = 1;
            _lineRenderer.endWidth = 1;
            _lineRenderer.widthMultiplier = borderThickness;
            
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
            if (HasWon)
            {
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
            tileParent.gameObject.SetActive(true);
            
            PrepareGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            ClearProgress();
            _lineRenderer.enabled = true;
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            ClearProgress();
            _lineRenderer.enabled = false;
            OnDisabled();

            return MiniGameState.Inactive;
        }
        #endregion
        
        #region { Remaining Logic }
        
        private void UpdateScoreDisplay() {
            scoreText.text = $"Score: {_tileGrid.Score} / {_tileGrid.MaxScore}";
        }
        
        private void UpdateBorder() {
            var parent = _tileGrid.Parent;
            var width = _tileGrid.Width;
            var height = _tileGrid.Height;
            var dimensions = _tileGrid.Dimensions;
            
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
        }

        private void ScatterPieces() {
            var parent = _tileGrid.Parent;
            var width = _tileGrid.Width;
            var height = _tileGrid.Height;
            var tiles = _tileGrid.Tiles;
            
            // Get the camera's bounds
            var rangeHeight = CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera.orthographicSize;// Camera.main.orthographicSize;
            var screenAspect = (float)Screen.width / Screen.height;
            var rangeWidth = screenAspect * rangeHeight;
            
            // scale the base piece size by the gameHolder's local size
            parent.localScale = new Vector3(gameScale.x, gameScale.y, parent.localScale.z);
            var pieceWidth = width * parent.localScale.x;
            var pieceHeight = height * parent.localScale.y;
            
            // Scale it based on scatterRange, then subtract piece's height/width
            rangeHeight = rangeHeight * scatterRange.y - pieceHeight;
            rangeWidth = rangeWidth * scatterRange.x - pieceWidth;
            
            // Randomise their positions
            foreach (var piece in tiles) {
                var x = Random.Range(-rangeWidth, rangeWidth);
                var y = Random.Range(-rangeHeight, rangeHeight);
                piece.position = new Vector3(x, y, zPosition);
            }
            
        }

        private void TryGridSnap() {
            if (_tileGrid.FindTileAnchor(_draggedPiece.localPosition, gridSnapFactor, out var anchor)) {
                if (!_tileGrid.SetTile(anchor, _draggedPiece)) return;
                var targetPosition = _tileGrid.GetTileAnchor(anchor);
                _draggedPiece.localPosition = targetPosition;
            }
        }
        
        private Vector3 GetWorldSpacePosition(Vector2 input, float zPos) {
            var worldPos = targetCamera.ScreenToWorldPoint(input);
            worldPos.z = zPos;
            return worldPos;
        }
        
        private void SetSiblingPriority() {
            // This aids targeting, but currently doesn't sync well with our rendering pipeline. I should add some direct material render priority logic.
            _draggedPiece.SetAsLastSibling();
            foreach (Transform child in tileParent) {
                var newPos = child.position;
                newPos.z = zPosition - child.GetSiblingIndex() * 0.01f;
                child.position = newPos;
            }
            
            _draggedPiece.parent = tileParent;
        }
        #endregion
    }
}
