using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TheDates.Runtime;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheDates
{
    public class SlidingGame : MiniGame
    {
        [Serializable]
        public class TileBoard
        {
            [Serializable]
            public class TileData
            {
                public BoxCollider2D tile;
                public Sprite sprite;
                
                public int correctIndex => tile.transform.GetSiblingIndex();
            }
            
            public Transform stepParent;
            public Vector2Int gridSize = new(3, 3);
            
            private Transform[,] _tiles;
            
            private Vector2Int _currentIndex;
            private Transform _currentTile;
            
            [FormerlySerializedAs("_shuffledTiles")] public List<TileData> shuffledTiles = new();
            private HashSet<int> _collidables;

            public bool isAchieved;

            public void Init(Transform parent)
            {
                _collidables = new HashSet<int>();
                var root = stepParent.transform.Find("Root");
                //var root = new GameObject("Tiles").transform;
                //root.SetParent(stepParent);
                
                GenerateTiles();
            }

            public void Reset()
            {
                _currentIndex = Vector2Int.zero;
                _currentTile = null;
                isAchieved = false;
                
                SetupTiles();
                SetFinished(false);
            }

            public void SetActive(bool toggle) {
                stepParent.gameObject.SetActive(toggle);
            }

            public bool TargetTile(Collider2D collider)
            {
                var target = collider.transform;
                if (!_collidables.TryGetValue(target.gameObject.GetInstanceID(), out var wire)) return false;
                Debug.Log($"Clicked {collider?.gameObject.name}");
            
                if (_currentTile == null) SelectTile(target);
                else {
                    SwapTile(target);
                    if (!CheckScore()) return true;
                    if (_currentTile) _currentTile.localScale = Vector3.one;
                    _currentTile = null;
                    SetFinished(true);
                    //SendCommand(MiniGameCommand.Win);
                }
                return true;
            }

            public void SetFinished(bool toggle)
            {
                isAchieved = toggle;
                foreach (var tile in shuffledTiles) {
                    tile.tile.enabled = !toggle;
                }
            }
            
            
            
            private void GenerateTiles() {
                _tiles = new Transform[gridSize.x, gridSize.y];
            
                var index = 0;
                for (var y = gridSize.y - 1; y >= 0; y--) {
                    for (var x = 0; x < gridSize.x; x++) {
                        GameObject piece;
                        var isWithinBounds = index < shuffledTiles.Count;
                        if (!isWithinBounds || shuffledTiles[index] == null) {
                            Debug.Log("Error for tiles idk: " + stepParent?.name);
                            return;
                        }
                        
                        piece = shuffledTiles[index].tile.gameObject;
                        if (!piece.transform.IsChildOf(stepParent)) {
                            //Debug.Log($"IGNORED piece {piece.name} at index [{index}], sibling index is [{piece.transform.GetSiblingIndex()}]");
                            piece.SetActive(false);
                            continue;
                        }
                        piece.name = $"Tile";
                        /*
                        if (!isWithinBounds || pieces[index] == null) {
                            piece = new GameObject($"Empty Tile");
                            piece.transform.SetParent(pieceParent);

                            if (isWithinBounds) {
                                pieces[index] = piece;
                            }
                            else {
                                pieces.Add(piece);
                            }
                        } else {
                            piece = pieces[index];
                            if (!piece.transform.IsChildOf(pieceParent)) {
                                //Debug.Log($"IGNORED piece {piece.name} at index [{index}], sibling index is [{piece.transform.GetSiblingIndex()}]");
                                pieces[index].SetActive(false);
                                continue;
                            }
                            piece.name = $"Tile";
                        }
                        */
                        var siblingIndex = piece.transform.GetSiblingIndex();
                        CreatePiece(siblingIndex, piece, shuffledTiles[index].sprite);
                        _collidables.Add(piece.GetInstanceID());
                        index++;
                    }
                }
            }
            
            private void SetupTiles() {
                var index = 0;
                for (var y = gridSize.y - 1; y >= 0; y--) {
                    for (var x = 0; x < gridSize.x; x++) {
                        var piece = shuffledTiles[index].tile;
                        _tiles[x,y] = piece.transform;
                        ArrangePiece(new Vector3(x, y, -1), piece.gameObject);
                        index++;
                    }
                }
            }
            
            private void CreatePiece(int index, GameObject piece, Sprite sprite) {
                var text = piece.GetComponentInChildren<TextMeshPro>();
                if (text != null) {
                    text.text = index.ToString();
                }

                piece.name += $" ({index})";
                piece.transform.localScale = Vector3.one;
                var renderer = piece.transform.Find("Border").GetComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                //renderer.localBounds = new Bounds(Vector3.zero, Vector3.one);
                piece.SetActive(false);
            }
            
            private void ArrangePiece(Vector3 position, GameObject piece) {
                //var renderer = piece.transform.Find("Border").GetComponent<SpriteRenderer>();
                //piece.transform.localPosition = new Vector3(position.x * renderer.sprite., position.y, piece.transform.localPosition.z);
                piece.transform.localPosition = position;
                piece.transform.localScale = Vector3.one;
                piece.SetActive(true);
            }
            
            private void SelectTile(Transform target) {
                for (var y = gridSize.y - 1; y >= 0; y--) {
                    for (var x = 0; x < gridSize.x; x++) {
                        if (_tiles[x, y] != target) continue;
                    
                        _currentTile = target;
                        _currentIndex = new Vector2Int(x, y);
                        _currentTile.localScale = SelectedScale;
                    }
                }
            }
            
            private void SwapTile(Transform target) {
                //var canSwap = true;
                Vector2Int newIndex;
                if (_currentIndex.x - 1 >= 0 && target == _tiles[_currentIndex.x - 1, _currentIndex.y]) {
                    newIndex = new Vector2Int(_currentIndex.x - 1, _currentIndex.y);
                } else if (_currentIndex.x + 1 < gridSize.x && target == _tiles[_currentIndex.x + 1, _currentIndex.y]) {
                    newIndex = new Vector2Int(_currentIndex.x + 1, _currentIndex.y);
                } else if (_currentIndex.y - 1 >= 0 && target == _tiles[_currentIndex.x, _currentIndex.y - 1]) {
                    newIndex = new Vector2Int(_currentIndex.x, _currentIndex.y - 1);
                } else if (_currentIndex.y + 1 < gridSize.y && target == _tiles[_currentIndex.x, _currentIndex.y + 1]) {
                    newIndex = new Vector2Int(_currentIndex.x, _currentIndex.y + 1);
                } else {
                    Debug.Log($"Target {target.gameObject.name} is out of range.");
                    _currentTile.localScale = Vector3.one;
                    _currentTile = null;
                    return;
                }

                // Jetbrains suggested deconstruction instead of x = y, y = z, z = x, but it looks cursed lmao
                (_currentTile.localPosition, target.localPosition) = (target.localPosition, _currentTile.localPosition);
            
                _tiles[newIndex.x, newIndex.y] = _currentTile;
                _tiles[_currentIndex.x, _currentIndex.y] = target;
            
                _currentIndex = newIndex;
            }

            private static readonly Vector3 SelectedScale = new(0.8f, 0.8f, 1f);
            
            private bool CheckScore() {
                var index = 0;
                for (var y = gridSize.y - 1; y >= 0; y--) {
                    for (var x = 0; x < gridSize.x; x++) {
                        if (_tiles[x, y].GetSiblingIndex() != index) {
                            return false;
                        }
                        index++;
                    }
                }
                Debug.Log("You win!!!");
                return true;
            
            }
        }
        [Header("Mechanic Config")]
        //public List<GameObject> pieces = new();
        //public Transform pieceParent;
        //public Vector2Int gridSize = new(3, 3);
        
        //public GameObject tilePrefab;
        public TileBoard[] tileBoards = Array.Empty<TileBoard>();
        public Transform boardParent;
        //[SerializeField, ReadOnly] private MiniGameState state;
        
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [SerializeField, ReadOnly] private bool initialised;
        [SerializeField, ReadOnly] private GameObject prefab;
        [SerializeField, ReadOnly] private MiniGameState state;
        public override bool isInitialised => initialised;
        public override GameObject source => prefab;
        public override MiniGameState gameState => state;
        
        //private readonly HashSet<int> _collidables = new();
        //private Transform[,] _tiles;
        //private Vector2Int _currentIndex;
        //private Transform _currentTile;

        private int _currentBoardIndex;

        private void SetupGame() {
            _currentBoardIndex = 0;
            foreach (var board in tileBoards) {
                board.Reset();
                board.SetActive(false);
            }
            
            tileBoards[_currentBoardIndex].SetActive(true);
            //_currentIndex = Vector2Int.zero;
            //_currentTile = null;
            //SetupTiles();
        }
        
        /*private void SetupTiles() {
            var index = 0;
            for (var y = gridSize.y - 1; y >= 0; y--) {
                for (var x = 0; x < gridSize.x; x++) {
                    var piece = pieces[index];
                    _tiles[x,y] = piece.transform;
                    ArrangePiece(new Vector3(x, y, -1), piece);
                    index++;
                }
            }
        }

        private void GenerateTiles() {
            _tiles = new Transform[gridSize.x, gridSize.y];
            
            var index = 0;
            for (var y = gridSize.y - 1; y >= 0; y--) {
                for (var x = 0; x < gridSize.x; x++) {
                    GameObject piece;
                    var isWithinBounds = index < pieces.Count;

                    if (!isWithinBounds || pieces[index] == null) {
                        piece = new GameObject($"Empty Tile");
                        piece.transform.SetParent(pieceParent);
                        
                        if (isWithinBounds) {
                            pieces[index] = piece;
                        }
                        else {
                            pieces.Add(piece);
                        }
                    } else {
                        piece = pieces[index];
                        if (!piece.transform.IsChildOf(pieceParent)) {
                            //Debug.Log($"IGNORED piece {piece.name} at index [{index}], sibling index is [{piece.transform.GetSiblingIndex()}]");
                            pieces[index].SetActive(false);
                            continue;
                        }
                        piece.name = $"Tile";
                    }
                    var siblingIndex = piece.transform.GetSiblingIndex();
                    CreatePiece(siblingIndex, piece);
                    _collidables.Add(piece.GetInstanceID());
                    index++;
                }
            }
        }*/

        private void OnEnabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickTarget += OnClickTarget;
        }

        private void OnDisabled() {
            if (!MiniGameManager.HasInstance) return;
            
            MiniGameManager.Instance.OnClickTarget -= OnClickTarget;
        }

        /*private void CreatePiece(int index, GameObject piece) {
            var text = piece.GetComponentInChildren<TextMeshPro>();
            if (text != null) {
                text.text = index.ToString();
            }

            piece.name += $" ({index})";
            piece.transform.localScale = Vector3.one;
            piece.SetActive(false);
        }

        private void ArrangePiece(Vector3 position, GameObject piece) {
            piece.transform.localPosition = position;
            piece.transform.localScale = Vector3.one;
            piece.SetActive(true);
        }*/
        
        private void OnClickTarget(RaycastHit2D input) {
            tileBoards[_currentBoardIndex].TargetTile(input.collider);
            if (tileBoards[_currentBoardIndex].isAchieved)
            {
                tileBoards[_currentBoardIndex].SetActive(false);
                _currentBoardIndex++;
                if (_currentBoardIndex < tileBoards.Length) {
                    tileBoards[_currentBoardIndex].Reset();
                    tileBoards[_currentBoardIndex].SetActive(true);
                    return;
                }
                
                SendCommand(MiniGameCommand.Win);
            }
            /*
            var target = input.transform;
            if (!_collidables.TryGetValue(target.gameObject.GetInstanceID(), out var wire)) return;
            Debug.Log($"Clicked {input.collider?.gameObject.name}");
            
            if (_currentTile == null) SelectTile(target);
            else {
                SwapTile(target);
                if (!CheckScore()) return;
                if (_currentTile) _currentTile.localScale = Vector3.one;
                _currentTile = null;
                SendCommand(MiniGameCommand.Win);
            }
            */
        }

        /*private void SelectTile(Transform target) {
            for (var y = gridSize.y - 1; y >= 0; y--) {
                for (var x = 0; x < gridSize.x; x++) {
                    if (_tiles[x, y] != target) continue;
                    
                    _currentTile = target;
                    _currentIndex = new Vector2Int(x, y);
                    _currentTile.localScale = SelectedScale;
                }
            }
        }

        private static readonly Vector3 SelectedScale = new(0.8f, 0.8f, 1f);

        private void SwapTile(Transform target) {
            //var canSwap = true;
            Vector2Int newIndex;
            if (_currentIndex.x - 1 >= 0 && target == _tiles[_currentIndex.x - 1, _currentIndex.y]) {
                newIndex = new Vector2Int(_currentIndex.x - 1, _currentIndex.y);
            } else if (_currentIndex.x + 1 < gridSize.x && target == _tiles[_currentIndex.x + 1, _currentIndex.y]) {
                newIndex = new Vector2Int(_currentIndex.x + 1, _currentIndex.y);
            } else if (_currentIndex.y - 1 >= 0 && target == _tiles[_currentIndex.x, _currentIndex.y - 1]) {
                newIndex = new Vector2Int(_currentIndex.x, _currentIndex.y - 1);
            } else if (_currentIndex.y + 1 < gridSize.y && target == _tiles[_currentIndex.x, _currentIndex.y + 1]) {
                newIndex = new Vector2Int(_currentIndex.x, _currentIndex.y + 1);
            } else {
                Debug.Log($"Target {target.gameObject.name} is out of range.");
                _currentTile.localScale = Vector3.one;
                _currentTile = null;
                return;
            }

            // Jetbrains suggested deconstruction instead of x = y, y = z, z = x, but it looks cursed lmao
            (_currentTile.localPosition, target.localPosition) = (target.localPosition, _currentTile.localPosition);
            
            _tiles[newIndex.x, newIndex.y] = _currentTile;
            _tiles[_currentIndex.x, _currentIndex.y] = target;
            
            _currentIndex = newIndex;
        }

        private bool CheckScore() {
            var index = 0;
            for (var y = gridSize.y - 1; y >= 0; y--) {
                for (var x = 0; x < gridSize.x; x++) {
                    if (_tiles[x, y].GetSiblingIndex() != index) {
                        return false;
                    }
                    index++;
                }
            }
            Debug.Log("You win!!!");
            return true;
            
        }*/
        
        public override void Init(GameObject gamePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = gamePrefab;

            foreach (var board in tileBoards) {
                board.Init(boardParent);
            }
            //GenerateTiles();
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
    }
}
