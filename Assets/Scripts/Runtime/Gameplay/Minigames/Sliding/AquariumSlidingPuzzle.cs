using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace TheDates.Runtime.Gameplay.Minigames
{
    public class AquariumSlidingPuzzle : MiniGame
    {
        [Serializable]
        public class SlidingPuzzleLevel
        {
            [field: SerializeField] public float borderThickness {get; private set;} = 0.1f;
            [field: SerializeField] public Color borderColour {get; private set;} = Color.white;
            [field: SerializeField] public TileGridAsset gridConfig {get; private set;} = new();
            [field: SerializeField] public GameObject levelParent {get; private set;}
            [field: SerializeField] public LineRenderer lineRenderer {get; private set;}
            public bool isAchieved {get; private set;}
            
            [SerializeField] private Vector2 gameScale = new(10, 10); 
            
            public GridTile currentTile;

            private Vector2Int _currentIndex;
            private TileGrid _tileGrid;
            private BorderRenderer _borderRenderer;
            
            public void CreateGrid() {
                _tileGrid = new TileGrid(gridConfig);
                _tileGrid.Create(levelParent.transform);
                
                var parent = _tileGrid.parent;
                parent.localScale = new Vector3(gameScale.x, gameScale.y, parent.localScale.z);
                
                Random rand = new();
                
                int cols = _tileGrid.dimensions.x;
                int rows = _tileGrid.dimensions.y;
                
                var shuffled = _tileGrid.orderedTiles.Cast<GridTile>().OrderBy(x => rand.Next()).ToArray();
                
                for (var i = 0; i < cols * rows; i++) {
                    var index = new Vector2Int(i / rows, i % rows);
                    _tileGrid.SetTile(index, shuffled[i]);
                    shuffled[i].clickableArea.enabled = true;
                    shuffled[i].tileLocalPosition = _tileGrid.GetTileAnchor(index);
                }
            }
            
            public void UpdateBorder() {
                var parent = _tileGrid.parent;
                var width = _tileGrid.tileWidth;
                var height = _tileGrid.tileHeight;
                var dimensions = _tileGrid.dimensions;

                var fullWidth = width * dimensions.x;
                var fullHeight = height * dimensions.y;
                
                _borderRenderer.TotalHeight = fullHeight;
                _borderRenderer.TotalWidth = fullWidth;
                _borderRenderer.LineThickness = borderThickness;
                _borderRenderer.LineColour = borderColour;
                
                _borderRenderer.Init(parent.transform, lineRenderer);
                _borderRenderer.SetActive(true);
            }

            public bool HasTarget(GridTile target) {
                return _tileGrid.HasTile(target);
            }

            public bool ClickTarget(GridTile target) {
                if (!target || !_tileGrid.HasTile(target)) return false;
                Debug.Log($"Clicked {target.name}");
                if (!currentTile) SelectTile(target);
                else {
                    SwapTile(target);
                    //if (!CheckScore()) return true;
                    if (_tileGrid.score < _tileGrid.maxScore) return true;
                    DeselectTile();
                    SetFinished(true);
                }
                
                return true;
            }

            public void DeselectTile() {
                if (!currentTile) return;
                Debug.Log($"Unclicked {currentTile.name}");
                currentTile.transform.localScale += SelectedScale;
                currentTile = null;
            }
            
            public void SetFinished(bool toggle)
            {
                isAchieved = toggle;
                foreach (var tile in _tileGrid.orderedTiles) {
                    tile.clickableArea.enabled = !toggle;
                }
            }

            public void ClearLevel() {
                _tileGrid.Purge();
                isAchieved = false;
                currentTile = null;
                //_borderRenderer.Clear();
                // TODO _borderRenderer.Clear();
                //SetFinished(false);
            }
            
            public void SetActive(bool toggle) {
                levelParent.gameObject.SetActive(toggle);
            }
            
            private static readonly Vector3 SelectedScale = new(0.05f, 0.05f, 0f);
            
            private void SelectTile(GridTile target) {
                if (!_tileGrid.IndexFromTile(target, out var index)) return;
                currentTile = target;
                _currentIndex = index;
                currentTile.transform.localScale -= SelectedScale;
            }
        
            private void SwapTile(GridTile target) {
                if (!_tileGrid.IndexFromTile(target, out var newIndex)) return;
                // This took me a hot minute, but I remembered you can use absolute to do these checks.
                // Since we only want to allow arbitrary movement if it doesnt exceed 1 'index' past its 2D neighbours, we just need to check for that.
                // Zero counts as invalid for the same outcome in this case, at least for now.
                var difference = _currentIndex - newIndex;
                var absoluteDifference = new Vector2Int(Mathf.Abs(difference.x), Mathf.Abs(difference.y));
                
                // XOR is niche, but here we don't want players moving parallel. If only 1 is true, it passes the logic check.
                if (absoluteDifference is { x: <= 1, y: <= 1 } 
                    && absoluteDifference.x == 1 ^ absoluteDifference.y == 1)  {
                    // Jetbrains suggested deconstruction instead of x = y, y = z, z = x, but it looks cursed lmao
                    (currentTile.transform.localPosition, target.transform.localPosition) = (target.transform.localPosition, currentTile.transform.localPosition);
                
                    _tileGrid.RemoveTile(_currentIndex);
                    _tileGrid.RemoveTile(newIndex);
                
                    _tileGrid.SetTile(newIndex, currentTile);
                    _tileGrid.SetTile(_currentIndex, target);
        
                    _currentIndex = newIndex;
                    //Debug.Log($"New Tile Layout for {_tileGrid.TileFromIndex(_currentIndex)} : Left [{_tileGrid.TileFromIndex(_currentIndex + Vector2Int.left)?.name}], Right [{_tileGrid.TileFromIndex(_currentIndex + Vector2Int.right)?.name}], Up [{_tileGrid.TileFromIndex(_currentIndex + Vector2Int.up)?.name}], Down [{_tileGrid.TileFromIndex(_currentIndex + Vector2Int.down)?.name}]");
                    return;
                }
                Debug.Log($"Target {target.gameObject.name} is out of range.");
                DeselectTile();


            }
        }
        
        #region [ Inspector Fields ]
        //[Header("Mechanic Config")]
        //[Header("Visual Config")]
        //[Header("Scene Dependencies")] 
        //[Header("Prefab Dependencies")]
        
        
        public SlidingPuzzleLevel[] slidingPuzzleLevels = Array.Empty<SlidingPuzzleLevel>();
        [SerializeField] private TextMeshProUGUI phaseText;
        
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
        #endregion
        
        #region { Event Handlers }
        private void OnClickState(bool input) {
            //if (input || !_draggedPiece) return;
            //_currentSlidingPuzzleLevel.ClearTarget();
        }

        private void OnClickTarget(RaycastHit2D hit) {
            var tile = hit.transform?.GetComponent<GridTile>();
            slidingPuzzleLevels[_currentBoardIndex].ClickTarget(tile);
            
            if (slidingPuzzleLevels[_currentBoardIndex].isAchieved) {
                StartCoroutine(NextPuzzle());
            }
        }

        private void OnClickPosition(Vector2 input) {
            //if (!_draggedPiece) return;
            
            //var worldPos = GetWorldSpacePosition(input, 0) + _clickOffset;
            //_draggedPiece.tilePosition = worldPos;
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
            _phaseTime = new WaitForSeconds(phaseDelay);
            _currentBoardIndex = 0;
            UpdatePhaseText($"[Phases {_currentBoardIndex +1}/{slidingPuzzleLevels.Length} active]");

            foreach (var level in slidingPuzzleLevels) {
                level.CreateGrid();
                level.SetActive(false);
            }
            
            //slidingPuzzleLevels[_currentBoardIndex] = slidingPuzzleLevels[0];
            slidingPuzzleLevels[_currentBoardIndex].SetActive(true);
        }
        private void ClearProgress() {
            foreach (var level in slidingPuzzleLevels) {
                level.ClearLevel();
            }
        }
       
        public override void Init(GameObject sourcePrefab) {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            prefab = sourcePrefab;
            Init();
            InitCommon();
        }

        private void Init() {
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
                Debug.Log("Yippee I finished!");
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
        
        public float phaseDelay = 2f;

        private int _currentBoardIndex;
        private WaitForSeconds _phaseTime;
        
        private IEnumerator NextPuzzle() {
            UpdatePhaseText($"[Phases {_currentBoardIndex +1}/{slidingPuzzleLevels.Length} completed]");
            yield return _phaseTime;
            if (_currentBoardIndex + 1 < slidingPuzzleLevels.Length) {
                slidingPuzzleLevels[_currentBoardIndex].SetActive(false);
                slidingPuzzleLevels[_currentBoardIndex].ClearLevel();
                
                _currentBoardIndex++;
                slidingPuzzleLevels[_currentBoardIndex].SetActive(true);
                UpdatePhaseText($"[Phases {_currentBoardIndex +1}/{slidingPuzzleLevels.Length} active]");
            }
            else {
                UpdatePhaseText("[All phases finished]");
                SendCommand(MiniGameCommand.Win);
            }
        }

        private void UpdatePhaseText(string text) {
            phaseText.text = text;
        }
        
    }
}