using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


// Gotta refine this further
namespace TheDates.Runtime.Gameplay.Minigames
{
    public class BorderRenderer
    {
        public Transform parent { get; private set; }
        public bool isInitialised {get; private set;}
        public bool isActive => _lineObject?.activeSelf ?? false;
        
        private GameObject _lineObject;
        private LineRenderer _lineRenderer;

        public float TotalWidth = 10f;
        public float TotalHeight = 10f;
        public float LineThickness = 0.1f;
        public Color LineColour = Color.white;
        public int MaterialSortingOrder = 0;

        private const int PointCount = 4;
        private const int LineWidth = 1;

        public void Init(Transform parentObject, LineRenderer lineObject) {
            parent = parentObject;
            _lineObject = lineObject.gameObject;
            _lineRenderer = lineObject;
            isInitialised = true;
            
            _lineRenderer.useWorldSpace = false;
            if (_lineRenderer.positionCount != PointCount) _lineRenderer.positionCount = PointCount;
            
            //_lineRenderer.startWidth = LineWidth;
            //_lineRenderer.endWidth = LineWidth;
            _lineRenderer.widthMultiplier = LineThickness;
            
            _lineRenderer.material.color = LineColour;
            _lineRenderer.sortingOrder = MaterialSortingOrder;
            
            SetDimensions();
            
            _lineRenderer.enabled = true;
        }

        public void SetMaterialSortingOrder(int order) {
            if (!_lineRenderer) return;
            _lineRenderer.sortingOrder = order;
        }

        public void Clear() {
            parent = null;
            _lineRenderer = null;
            isInitialised = false;
        }

        //public void SetDimensions(float width, float height, float lineThickness) {
        //    TotalWidth = width;
        //    TotalHeight = height;
        //    LineThickness = lineThickness;
        //}

        public void SetActive(bool toggle) {
            _lineObject?.SetActive(toggle);
        }

        private void SetDimensions() {
            // We need to scale the border thickness by the local scale for clean edges
            // If this appears very small, check the line renderer component and debug it first
            var halfWidth = ((TotalWidth + LineThickness / parent.localScale.x) / 2);
            var halfHeight = ((TotalHeight + LineThickness / parent.localScale.y) / 2);
            
            // Clockwise
            _lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, 0));
            _lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, 0));
            _lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, 0));
        }
        
    }
    
    
    [Serializable]
    public class TileGridAsset
    {
        public Texture2D texture;
        public GridTile tilePrefab;
        public GameObject tileGridPrefab;

        public Vector2Int tiling = new(2, 2);
        public float gridLineThickness = 0.01f;
        public Color gridLineColor = Color.white;
    }

    public class TileGrid
    {
        private static readonly int MatTextureID = Shader.PropertyToID("_BaseMap");
        private static readonly int MatColourID = Shader.PropertyToID("_BaseColor");
        
        public Transform parent { get; private set; }
        public int score { get; private set; }
        public int maxScore { get; private set; }

        public float tileWidth { get; private set; }
        public float tileHeight { get; private set; }
        public Vector2Int dimensions { get; private set; }
        //public List<GridTile> tiles { get; private set; }
        public GridTile[,] orderedTiles { get; private set; }
        
        private readonly TileGridAsset _tileGridAsset;
        
        private HashSet<int> _registeredTiles;
        private Dictionary<Vector2Int, Vector2> _tileAnchors;
        private GridTile[,] _tileMapping;
        private Dictionary<GridTile, Vector2Int> _desiredTileMapping;
        
        private GameObject _grid;
        private Transform _tileParent;
        private GridDisplay _gridDisplay;


        public TileGrid(TileGridAsset tileGridAsset)
        {
            _tileGridAsset = tileGridAsset;
        }

        public void Create(Transform parentObject)
        {
            parent = parentObject;
            
            _registeredTiles = new HashSet<int>();
            _tileAnchors = new Dictionary<Vector2Int, Vector2>();

            dimensions = _tileGridAsset.tiling;
            orderedTiles = new GridTile[dimensions.x, dimensions.y];

            CreatePieces(_tileGridAsset.texture);
            CreateGrid();

            score = 0;
            maxScore = orderedTiles.Length;
        }

        public bool HasTile(GridTile target) => _registeredTiles.Contains(target.GetInstanceID());

        public void Purge()
        {
            if (orderedTiles.IsNullOrEmpty()) return;
            for (var row = 0; row < dimensions.y; row++) {
                for (var col = 0; col < dimensions.x; col++) {
                    Object.Destroy(orderedTiles[col, row]?.gameObject);
                }
            }
            Object.Destroy(_grid);
            Object.Destroy(_tileParent.gameObject);

            orderedTiles = null;
            _registeredTiles.Clear();
            _tileAnchors.Clear();
            _desiredTileMapping.Clear();
            
            _tileMapping = null;
            _gridDisplay = null;
            parent = null;

        }

        private void CreateGrid()
        {
            _grid = Object.Instantiate(_tileGridAsset.tileGridPrefab, parent);
            _grid.name = "Grid";
            var grid = _grid.transform;

            grid.localPosition = new Vector3(0, 0, 0);
            grid.localScale = new Vector3(tileWidth * dimensions.x, tileHeight * dimensions.y, 1);

            _gridDisplay = new GridDisplay(_grid.GetComponent<MeshRenderer>());

            _gridDisplay.SetLineColour(_tileGridAsset.gridLineColor);
            _gridDisplay.SetLineThickness(_tileGridAsset.gridLineThickness);
            _gridDisplay.SetGridTiling(dimensions);

            _grid.GetComponent<Collider>().enabled = false;
        }

        private Vector2Int GetDimensions(Texture2D texture, int divisions)
        {
            //var dimensions = Vector2Int.zero;
            var div = _tileGridAsset.tiling;
            var dimensions = Vector2Int.zero;
            if (texture.width < texture.height)
            {
                //dimensions.x = divisions;
                //dimensions.y = (divisions * texture.height) / texture.width;
                dimensions.x = div.x;
                dimensions.y = (div.y * texture.height) / texture.width;
            }
            else
            {
                //dimensions.x = (divisions * texture.width) / texture.height;
                //dimensions.y = divisions;
                dimensions.x = (div.x * texture.width) / texture.height;
                dimensions.y = div.y;
            }

            return div; //dimensions;
        }

        private void CreatePieces(Texture2D texture)
        {
            tileHeight = 1f / dimensions.y;
            var aspect = (float)texture.width / texture.height;
            tileWidth = aspect / dimensions.x;
            
            _tileParent = new GameObject().transform;
            _tileParent.parent = parent;
            _tileParent.name = "Tiles";
            // Had a mysterious case of shrinkage, not sure why. This is a last ditch effort (AND IT WORKED YAY)
            _tileParent.localPosition = Vector3.zero;
            _tileParent.localScale = Vector3.one;

            _tileMapping = new GridTile[dimensions.x, dimensions.y];
            _desiredTileMapping = new Dictionary<GridTile, Vector2Int>();

            for (var row = 0; row < dimensions.y; row++) {
                for (var col = 0; col < dimensions.x; col++) {
                    // Make & add the piece
                    var piece = CreatePiece(col, row, texture);
                    piece.Init(_grid);
                    //piece.materialDisplay.currentSortingOrder = 1;
                    //.Add(piece)
                    orderedTiles[col, row] = piece;
                    _registeredTiles.Add(piece.GetInstanceID());

                    var anchorIndex = new Vector2Int(col, row);
                    var anchor = GetPositionAt(col, row);
                    _tileAnchors.Add(anchorIndex, anchor);
                    _desiredTileMapping[piece] = anchorIndex;
                    Debug.Log($"Tile Created: {piece.name}, index = {anchorIndex}, anchor = {anchor}");
                }
            }
        }

        /*public bool SwapTiles(Vector2Int index1, Vector2Int index2) {
            var tile1 = _tileMapping[index1.x, index1.y];
            var tile2 = _tileMapping[index2.x, index2.y];
            
            if (!tile1 || !tile2) return false;
            UpdateTileScore(tile1, index1, index2);
            UpdateTileScore(tile2, index2, index1);
            
            return true;
        }
        // Might be used in the older methods below at some point
        private void UpdateTileScore(GridTile tile, Vector2Int currentIndex, Vector2Int newIndex) {
            var desiredIndex = _desiredTileMapping[tile];
            if (desiredIndex == newIndex) score++;
            else if (desiredIndex == currentIndex) score--;
            
            _tileMapping[currentIndex.x, currentIndex.y] = null;
            _tileMapping[newIndex.x, newIndex.y] = tile;
        }*/

        public bool SetTile(Vector2Int index, GridTile tile)
        {
            // If the new tile is null, no record of that tile exists, or the index is already filled, then return false
            if (!tile || !_desiredTileMapping.ContainsKey(tile) || _tileMapping[index.x, index.y]) return false;
            _tileMapping[index.x, index.y] = tile;
            if (_desiredTileMapping[tile] == index) score++;

            Debug.Log($"Added tile {index}, Completion: {score} / {_desiredTileMapping.Count}");
            return true;
        }

        public bool RemoveTile(Vector2Int index)
        {
            var tile = _tileMapping[index.x, index.y];
            if (!tile) return false;

            if (_desiredTileMapping[tile] == index) score--;
            _tileMapping[index.x, index.y] = null;

            Debug.Log($"Removed tile {index}, Completion: {score} / {_desiredTileMapping.Count}");
            return true;
        }

        public bool IndexFromTile(GridTile target, out Vector2Int index)
        {
            index = default;
            for (var row = 0; row < dimensions.y; row++)
            {
                for (var col = 0; col < dimensions.x; col++)
                {
                    if (!_tileMapping[col, row] || target != _tileMapping[col, row]) continue;
                    index = new Vector2Int(col, row);
                    return true;
                }
            }

            return false;
        }
        public GridTile TileFromIndex(Vector2Int index) => TileFromIndex(index.x, index.y);
        public GridTile TileFromIndex(int row, int col) {
            //if (col >= (_tileMapping.GetUpperBound(0) - _tileMapping.GetLowerBound(0) + 1)
            //    || row >= (_tileMapping.GetUpperBound(1) - _tileMapping.GetLowerBound(1) + 1)) return null;
            //if (Mathf.Abs(row) >= dimensions.y || Mathf.Abs(col) >= dimensions.x) return null;
            if (col < 0 || col >= dimensions.x || row < 0 || row >= dimensions.y) return null;
            return _tileMapping[col, row];
        }

        private GridTile CreatePiece(int col, int row, Texture2D pieceTexture)
        {
            var piece = Object.Instantiate(_tileGridAsset.tilePrefab, _tileParent);
            piece.name = $"Piece {(row * dimensions.x) + col}";

            AssignPieceTransform(piece.transform, row, col);
            AssignPieceTexture(piece, row, col, pieceTexture);

            return piece;
        }

        private void AssignPieceTransform(Transform piece, int row, int col)
        {
            piece.localPosition = new Vector3(
                GetAlignment(tileWidth, dimensions.x, col),
                GetAlignment(tileHeight, dimensions.y, row),
                0);
            piece.localScale = new Vector3(tileWidth, tileHeight, 1);
            return;

            float GetAlignment(float length, int count, int index) =>
                -length * count / 2 + length * index + length / 2;
        }


        private void AssignPieceTexture(GridTile piece, int row, int col, Texture2D pieceTexture)
        {
            // Calculate the dimensions
            var width = 1f / dimensions.x;
            var height = 1f / dimensions.y;

            // Create the new UV
            var uv = new Vector2[]
            {
                new(width * col, height * row), // 1
                new(width * (col + 1), height * row), // 2
                new(width * col, height * (row + 1)), // 3
                new(width * (col + 1), height * (row + 1)) // 4
            };

            // Get the MeshFilter component & assign the new UV
            var mesh = piece.GetComponent<MeshFilter>().mesh;
            mesh.uv = uv;

            // Get the MeshRenderer component & assign the texture
            piece.GetComponent<MeshRenderer>().material.SetTexture(MatTextureID, pieceTexture);
        }

        private Vector2 GetPositionAt(int col, int row)
        {
            var targetPosition = new Vector2(
                (-tileWidth * dimensions.x / 2) + (tileWidth * col) + (tileWidth / 2),
                (-tileHeight * dimensions.y / 2) + (tileHeight * row) + (tileHeight / 2)
            );
            //Debug.Log("Target position for tile is: " + targetPosition);
            return targetPosition;
        }

        public Vector2 GetTileAnchor(Vector2Int index)
        {
            return _tileAnchors[index];
        }

        public bool FindTileAnchor(Vector2 localPosition, float sensitivity, out Vector2Int nearestAnchor)
        {
            nearestAnchor = Vector2Int.zero;
            var currentDistance = float.PositiveInfinity;
            var matchFactor = tileWidth / sensitivity;

            foreach (var pair in _tileAnchors)
            {
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
}