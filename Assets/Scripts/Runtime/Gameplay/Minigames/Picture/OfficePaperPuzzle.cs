using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class OfficePaperPuzzle : MiniGame
    {
        public class TileGrid
        {
            public GameObject Parent ;
            public Vector2Int GridSize = new(3, 3);
            
            //private GridComponent[,] _tiles;
            private float _width;
            private float _height;
            private HashSet<int> _registeredTiles;
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
        
        [Header("Scene Dependencies")] 
        [SerializeField] private Transform tileParent;
        [SerializeField] private TextMeshProUGUI scoreText;
        
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
        private Transform _guide;
        private HashSet<int> _pieceIds;
        private LineRenderer _lineRenderer;
        private Transform _draggedPiece;
        private Vector3 _clickOffset;
        private int _score;
        #endregion
        
        #region { Event Handlers }
        private void OnClickState(bool input) {
            if (input || !_draggedPiece) return;
            Debug.Log($"Put Down {_draggedPiece.name}");
            TryGridSnap();
            
            _draggedPiece = null;
        }

        private void OnClickTarget(RaycastHit2D hit) {
            if (!hit || !_pieceIds.Contains(hit.transform.GetInstanceID())) return;
            
            _draggedPiece = hit.transform;
            SetSiblingPriority(); // It'll be drawn in front of its siblings
            _clickOffset = _draggedPiece.position - (Vector3)hit.point;
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
            
            _dimensions = GetDimensions(tileSource, multiplier);
            CreatePieces(tileSource);
            ScatterPieces();
            UpdateBorder();

            _score = 0;
            UpdateScoreDisplay();
        }
        private void ClearProgress() {
            if (_pieces.Count <= 0) return;
            for (var i = _pieces.Count - 1; i >= 0; i--) {
                Destroy(_pieces[i]?.gameObject);
            }
            _pieces.Clear();
            _pieceIds.Clear();
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
            scoreText.text = $"Score: {_score} / {_pieces.Count}";
        }
        
        private void UpdateBorder() {
            _lineRenderer.useWorldSpace = false;
            
            // We need to scale the border thickness by the local scale for clean edges
            // If this appears very small, check the line renderer component and debug it first
            var halfWidth = (((_width * _dimensions.x) + borderThickness / tileParent.localScale.x) / 2);
            var halfHeight = (((_height * _dimensions.y) + borderThickness / tileParent.localScale.y) / 2);
            
            // Clockwise
            _lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, 0));
            _lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, 0));
            
            _lineRenderer.enabled = true;
        }

        private void ScatterPieces() {
            // Get the camera's bounds
            var rangeHeight = CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera.orthographicSize;// Camera.main.orthographicSize;
            var screenAspect = (float)Screen.width / Screen.height;
            var rangeWidth = screenAspect * rangeHeight;
            
            // scale the base piece size by the gameHolder's local size
            tileParent.localScale = new Vector3(gameScale.x, gameScale.y, tileParent.localScale.z);
            var pieceWidth = _width * tileParent.localScale.x;
            var pieceHeight = _height * tileParent.localScale.y;
            
            // Scale it based on scatterRange, then subtract piece's height/width
            rangeHeight = rangeHeight * scatterRange.y - pieceHeight;
            rangeWidth = rangeWidth * scatterRange.x - pieceWidth;
            
            // Randomise their positions
            foreach (var piece in _pieces) {
                var x = Random.Range(-rangeWidth, rangeWidth);
                var y = Random.Range(-rangeHeight, rangeHeight);
                piece.position = new Vector3(x, y, zPosition);
            }
            
        }

        private Vector2Int GetDimensions(Texture2D texture, int divisions) {
            var dimensions = Vector2Int.zero;

            if (texture.width < texture.height) {
                dimensions.x = divisions;
                dimensions.y = (divisions * texture.height) / texture.width;
            } else {
                dimensions.x = (divisions * texture.width) / texture.height;
                dimensions.y = divisions;
            }
            
            
            return dimensions;
        }

        private void CreatePieces(Texture2D texture) {
            _height = 1f / _dimensions.y;
            var aspect = (float)texture.width / texture.height;
            _width = aspect / _dimensions.x;

            for (var row = 0; row < _dimensions.y; row++) {
                for (var col = 0; col < _dimensions.x; col++) {
                    // Make & add the piece
                    var piece = CreatePiece(col, row, texture);
                    _pieces.Add(piece);
                    _pieceIds.Add(piece.GetInstanceID());
                }
            }
        }

        private Transform CreatePiece(int col, int row, Texture2D pieceTexture) {
            var piece = Instantiate(tilePrefab, tileParent);
            piece.name = $"Piece {(row * _dimensions.x) + col}";
            
            AssignPieceTransform(piece, row, col);
            AssignPieceTexture(piece, row, col, pieceTexture);
            
            return piece;
        }

        private void AssignPieceTransform(Transform piece, int row, int col) {
            piece.localPosition = new Vector3(
                GetAlignment(_width, _dimensions.x, col),
                GetAlignment(_height, _dimensions.y, row),
                zPosition);
            piece.localScale = new Vector3(_width, _height, 1);
            return;
            
            float GetAlignment(float length, int count, int index) => -length * count / 2 + length * index + length / 2;
        }
        

        private void AssignPieceTexture(Transform piece, int row, int col, Texture2D pieceTexture) {
            // Calculate the dimensions
            var width = 1f / _dimensions.x;
            var height = 1f / _dimensions.y;
            
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

        private void TryGridSnap() {
            var index = _pieces.IndexOf(_draggedPiece);

            var col = index % _dimensions.x;
            var row = index / _dimensions.x;

            var targetPosition = new Vector2(
                (-_width * _dimensions.x / 2) + (_width * col) + (_width / 2),
                (-_height * _dimensions.y / 2) + (_height * row) + (_height / 2)
            );

            if (Vector2.Distance(_draggedPiece.localPosition, targetPosition) < _width / gridSnapFactor) {
                _draggedPiece.localPosition = targetPosition; 
                //_draggedPiece.GetComponent<BoxCollider2D>().enabled = false; // no longer clickable
                _score++;
                UpdateScoreDisplay();
                if (_score == _pieces.Count) {
                    SendCommand(MiniGameCommand.Win);
                }
            }
        }
        
        private Vector3 GetWorldSpacePosition(Vector2 input, float zPos) {
            var worldPos = targetCamera.ScreenToWorldPoint(input);
            worldPos.z = zPos;
            return worldPos;
        }
        
        private void SetSiblingPriority() {
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
