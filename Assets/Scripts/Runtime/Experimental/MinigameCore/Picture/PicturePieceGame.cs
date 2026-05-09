using System;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TheDates.Runtime.Experimental.Puzzle.eugh
{
    // https://www.youtube.com/watch?v=lC_AMRKGpwg
    // https://www.youtube.com/watch?v=OFC_UUaS4gs
    // https://www.youtube.com/watch?v=2ZH0GaYbrc8
    // https://www.youtube.com/watch?v=bNBS8ZuzgZo
    public class PicturePieceGame : MiniGame
    {
        private static readonly int MatTextureID = Shader.PropertyToID("_BaseMap");
        private static readonly int MatColourID = Shader.PropertyToID("_BaseColor");
        
        [Header("Mechanic Config")]
        [SerializeField] private int multiplier = 4;
        [SerializeField] private float gridSnapFactor = 2;
        [SerializeField] private Vector2 scatterRange = new (1, 1);
        
        [Header("Visual Config")]
        [SerializeField] private float borderThickness = 0.1f;
        [SerializeField] private float zPosition = -2;
        [SerializeField] private Vector2 gameScale = new(3, 3);

        [Header("Scene Dependencies")] 
        [SerializeField] private Transform contentParent;
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private Transform levelSelectPanel;
        [SerializeField] private GameObject winInterface;
        
        [Header("Prefab Dependencies")]
        [SerializeField] private Image levelSelectPrefab;
        [SerializeField] private Transform piecePrefab;
        //[SerializeField] private List<Texture2D> textures = new();
        [SerializeField] private Texture2D slicedPicture;
        [SerializeField] private Texture2D guidePicture;
        
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        
        
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

        private void PrepareGame() {
            levelSelectPanel.gameObject.SetActive(false);
            //_pieces.Clear();
            //_pieceIds.Clear();
            _dimensions = GetDimensions(slicedPicture, multiplier);
            CreatePieces(slicedPicture);
            CreateGuide(guidePicture);
            ScatterPieces();
            UpdateBorder();

            _score = 0;
        }

        private void UpdateBorder() {
            _lineRenderer.useWorldSpace = false;
            
            // We need to scale the border thickness by the local scale for clean edges
            // If this appears very small, check the line renderer component and debug it first
            var halfWidth = (((_width * _dimensions.x) + borderThickness / contentParent.localScale.x) / 2);
            var halfHeight = (((_height * _dimensions.y) + borderThickness / contentParent.localScale.y) / 2);
            
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
            contentParent.localScale = new Vector3(gameScale.x, gameScale.y, contentParent.localScale.z);
            var pieceWidth = _width * contentParent.localScale.x;
            var pieceHeight = _height * contentParent.localScale.y;
            
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
            var piece = Instantiate(piecePrefab, contentParent);
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

        private void CreateGuide(Texture2D texture)
        {
            _guide = Instantiate(piecePrefab, contentParent);
            _guide.name = $"Guide";
            
            _guide.localPosition = new Vector3(0, 0, zPosition);
            _guide.localScale = new Vector3(_width * _dimensions.x, _height * _dimensions.y, 1);
            
            var mat = _guide.GetComponent<MeshRenderer>().material;
            mat.SetTexture(MatTextureID, texture);
            var colour = mat.color;
            mat.color = new Color(colour.r, colour.g, colour.b, 0f);

            _guide.GetComponent<BoxCollider2D>().enabled = false;
            
            //_guide = guide;
        }

        private void OnClickState(bool input) {
            if (input || !_draggedPiece) return;
            Debug.Log($"Put Down {_draggedPiece.name}");
            TryGridSnap();
            
            _draggedPiece = null;
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
                _draggedPiece.GetComponent<BoxCollider2D>().enabled = false; // no longer clickable
                _score++;
                if (_score == _pieces.Count) {
                    SendCommand(MiniGameCommand.Win);
                }
            }
        }

        private void ClearProgress() {
            Destroy(_guide.gameObject);
            if (_pieces.Count <= 0) return;
            for (var i = _pieces.Count - 1; i >= 0; i--) {
                Destroy(_pieces[i]?.gameObject);
            }
            _pieces.Clear();
            _pieceIds.Clear();
        }

        private void OnClickPosition(Vector2 input) {
            if (!_draggedPiece) return;
            
            var worldPos = GetWorldSpacePosition(input, 0) + _clickOffset;
            _draggedPiece.position = worldPos;
        }

        private Camera GetCamera() {
            return !CameraManager.HasInstance ? null : CameraManager.Instance.cameraMap[CameraManager.RenderState.HighDefWorld].pairedCamera;
        }

        private Vector3 GetWorldSpacePosition(Vector2 input, float zPos) {
            var worldPos = GetCamera().ScreenToWorldPoint(input);
            worldPos.z = zPos;
            return worldPos;
        }

        private void SetSiblingPriority() {
            _draggedPiece.SetAsLastSibling();
            foreach (Transform child in contentParent) {
                var newPos = child.position;
                newPos.z = zPosition - child.GetSiblingIndex() * 0.01f;
                child.position = newPos;
            }
            
            _draggedPiece.parent = contentParent;
        }

        private void OnClickTarget(RaycastHit2D hit) {
            if (!hit || !_pieceIds.Contains(hit.transform.GetInstanceID())) return;
            
            _draggedPiece = hit.transform;
            SetSiblingPriority(); // It'll be drawn in front of its siblings
            _clickOffset = _draggedPiece.position - (Vector3)hit.point; //+ Vector3.back;
            //_clickOffset += Vector3.back;
            Debug.Log($"Picked Up {_draggedPiece.name}");
        }
        
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
        }

        private void Init() {
            if (!contentParent) return;
            
            // Setup Collections
            _pieces = new List<Transform>();
            _pieceIds = new HashSet<int>();
            
            // Setup Line Renderer 
            _lineRenderer = contentParent.GetComponent<LineRenderer>();
            if (_lineRenderer.positionCount != 4) _lineRenderer.positionCount = 4;
            _lineRenderer.startWidth = 1;
            _lineRenderer.endWidth = 1;
            _lineRenderer.widthMultiplier = borderThickness;
            
            //InitAssets();
            state = MiniGameState.Inactive;
            initialised = true;
        }
        
        private void OnEnabled() {
            contentParent.gameObject.SetActive(true);
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickState += OnClickState;
            MiniGameManager.Instance.OnClickPosition += OnClickPosition;
            MiniGameManager.Instance.OnClickTarget += OnClickTarget;
        }
        
        private void OnDisabled() {
            contentParent.gameObject.SetActive(false);
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
        
        private void SetState(MiniGameState newState, bool notifyManager = true) {
            if (state == newState) return;
            state = newState;
            if (notifyManager) MiniGameManager.Instance.NotifyMiniGameState(source, state);
        }
        
        private MiniGameState WinGame() {
            Debug.Log("Yippee I finished!");
            QuitGame();
            return MiniGameState.Completed;
        }

        private MiniGameState LoseGame() {
            Debug.Log("Aw shucks, got it wrong. Let me try again!");
            return ResetGame();
        }

        private MiniGameState StartGame() {
            OnEnabled();
            contentParent.gameObject.SetActive(true);
            //foreach (var tex in textures) {
            //    var image = Instantiate(levelSelectPrefab, levelSelectPanel);
            //    image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            //    
            //    // Assign to button
            //    image.GetComponent<Button>().onClick.AddListener(() => StartGame(tex));
            //}
            //levelSelectPanel.gameObject.SetActive(true);
            
            PrepareGame();
            return MiniGameState.Active;
        }

        private MiniGameState ResetGame() {
            ClearProgress();
            _lineRenderer.enabled = false;
            //levelSelectPanel.gameObject.SetActive(true);
            
            return MiniGameState.Active;
        }

        private MiniGameState QuitGame() {
            ClearProgress();
            _lineRenderer.enabled = false;
            OnDisabled();

            return MiniGameState.Inactive;
        }
    }
}
