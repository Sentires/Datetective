using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TMPro;
using UnityEngine;

namespace TheDates
{
    public class SlidingGame : MonoBehaviour//, IMiniGame
    {
        [Header("Exposed Fields")]
        [SerializeField, ReadOnly] private MiniGameManager miniGameManager;
        [field: SerializeField, ReadOnly] public bool isInitialised { get; private set; }
        [field: SerializeField, ReadOnly] public MiniGameState gameState { get; private set; }
        
        private HashSet<int> _collidables = new();
        private Transform[,] tiles;
        
        public List<GameObject> pieces = new();
        public Transform pieceParent;
        public Vector2Int gridSize = new Vector2Int(3, 3);

        private Vector2Int currentIndex;
        private Transform currentTile;

        private void Start()
        {
            //var length = Mathf.CeilToInt((float)pieces.Count / 2);
            tiles = new Transform[gridSize.x, gridSize.y];
            //for (var i = 0; i < pieces.Count; i++)
            //{
                //var id = pieces[i].gameObject.GetInstanceID();
                
                
                //_collidables.Add(id);
                //Debug.Log($"ADDED piece {pieces[i].name}({id}) at index [{i}], sibling index is [{pieces[i].transform.GetSiblingIndex()}]");
                //CreatePiece(i + 1, pieces[i]);
                //ArrangePiece()
            //}
    
            var index = 0;
            //for (var y = 0; y < gridSize.y; y++)
            for (var y = gridSize.y - 1; y >= 0; y--)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    //if (!pieces.IsWithinBounds(index)) break;
                    GameObject piece;
                    var isWithinBounds = index < pieces.Count;
                    //int id;
                    if (!isWithinBounds || pieces[index] == null) {
                        piece = new GameObject($"Empty Tile");
                        piece.transform.SetParent(pieceParent);
                        //piece.transform.SetAsLastSibling();
                        //piece.transform.SetSiblingIndex(index);
                        
                        if (isWithinBounds) {
                            pieces[index] = piece;
                        }
                        else {
                            pieces.Add(piece);
                        }
                        
                    }
                    else {
                        piece = pieces[index];
                        if (!piece.transform.IsChildOf(pieceParent))
                        {
                            Debug.Log($"IGNORED piece {piece.name} at index [{index}], sibling index is [{piece.transform.GetSiblingIndex()}]");
                            pieces[index].SetActive(false);
                            continue;
                        }
                        piece.name = $"Tile";
                        
                    }
                    
                    CreatePiece(piece.transform.GetSiblingIndex(), piece);
                    tiles[x,y] = piece.transform;
                    _collidables.Add(piece.GetInstanceID());
                    
                    Debug.Log($"ADDED piece {piece?.name}({x}, {y}) at index [{index}], sibling index is [{piece?.transform.GetSiblingIndex()}]");
                    
                    ArrangePiece(new Vector3(x, y, -1), piece);
                    index++;
                }
            }
        }

        private void OnEnabled()
        {
            miniGameManager.OnClickPosition += OnPointPosition;
            miniGameManager.OnClickState += OnClickState;
            miniGameManager.OnClickTarget += OnClickTarget;
        }

        private void OnDisabled()
        {
            miniGameManager.OnClickPosition -= OnPointPosition;
            miniGameManager.OnClickState -= OnClickState;
            miniGameManager.OnClickTarget -= OnClickTarget;
        }

        private void CreatePiece(int index, GameObject piece)
        {
            var text = piece.GetComponentInChildren<TextMeshPro>();
            if (text != null) {
                text.text = index.ToString();
            }

            piece.name += $" ({index})";
            piece.transform.localScale = Vector3.one;
            piece.SetActive(false);
        }

        private void ArrangePiece(Vector3 position, GameObject piece)
        {
            piece.transform.localPosition = position;
            piece.SetActive(true);
        }
        
        private void OnClickTarget(RaycastHit2D input)
        {
            //throw new NotImplementedException();
            var target = input.transform;
            if (!_collidables.TryGetValue(target.gameObject.GetInstanceID(), out var wire)) return;
            Debug.Log($"Clicked {input.collider?.gameObject.name}");
            
            
            //var index = Vector2Int.zero;
            //for (var y = 0; y < gridSize.y; y++)
            if (currentTile == null) SelectTile(target);
            else
            {
                SwapTile(target);
                
            }
            //Debug.Log($"OnClickTarget Any called with {input.collider.name}");
            //if ( != null)
            //if (!_collidables.TryGetValue(input.collider.GetInstanceID(), out var wire)) return;
            //if (wire.clickPoint != input.collider) return;
            //_currentWire = wire;
            //Debug.Log($"OnClickTarget called with {input.collider.name} : {wire.name}");
        }

        private void SelectTile(Transform target)
        {
            for (var y = gridSize.y - 1; y >= 0; y--)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    if (tiles[x, y] == target)
                    {
                        currentTile = target;
                        currentIndex = new Vector2Int(x, y);
                        currentTile.localScale = SelectedScale;
                    }
                }
            }
        }

        private static readonly Vector3 SelectedScale = new(0.8f, 0.8f, 1f);

        private void SwapTile(Transform target)
        {
            //var canSwap = true;
            Vector2Int newIndex;
            if (currentIndex.x - 1 >= 0 && target == tiles[currentIndex.x - 1, currentIndex.y]) {
                newIndex = new Vector2Int(currentIndex.x - 1, currentIndex.y);
            }
            else if (currentIndex.x + 1 < gridSize.x && target == tiles[currentIndex.x + 1, currentIndex.y]) {
                newIndex = new Vector2Int(currentIndex.x + 1, currentIndex.y);
            }
            else if (currentIndex.y - 1 >= 0 && target == tiles[currentIndex.x, currentIndex.y - 1]) {
                newIndex = new Vector2Int(currentIndex.x, currentIndex.y - 1);
            }
            else if (currentIndex.y + 1 < gridSize.y && target == tiles[currentIndex.x, currentIndex.y + 1]) {
                newIndex = new Vector2Int(currentIndex.x, currentIndex.y + 1);
            }
            else
            {
                Debug.Log($"Target {target.gameObject.name} is out of range.");
                currentTile.localScale = Vector3.one;
                currentTile = null;
                return;
            }

            // Jetbrains suggested deconstruction instead of x = y, y = z, z = x, but it looks cursed lmao
            (currentTile.localPosition, target.localPosition) = (target.localPosition, currentTile.localPosition);
            
            //currentTile.localScale = Vector3.one;
            tiles[newIndex.x, newIndex.y] = currentTile;
            //target.localScale = SelectedScale;
            tiles[currentIndex.x, currentIndex.y] = target;
            
            //currentTile = target;
            currentIndex = newIndex;

            //if ((currentIndex.x - 1 >= 0 && target == tiles[currentIndex.x - 1, currentIndex.y])
            //    || (currentIndex.x + 1 < gridSize.x && target == tiles[currentIndex.x + 1, currentIndex.y])
            //    || (currentIndex.y - 1 >= 0 && target == tiles[currentIndex.x, currentIndex.y - 1])
            //    || (currentIndex.y + 1 < gridSize.y && target == tiles[currentIndex.x, currentIndex.y + 1]))
            //{
            //    Debug.Log($"Swapped {target?.name}");
            //}
        }

        private void OnClickState(bool input)
        {
            //if (_currentWire == null) return;
            
            //_currentWire = null;
        }
        private void OnPointPosition(Vector2 input)
        {
            //if (_currentWire == null) return;
            //_movePosition = miniGameManager.GetWorldSpacePosition(input, -0.3f);
            //_currentWire.clickPoint.transform.position = _movePosition;
            //_currentWire.MoveWire(_movePosition);
        }
        [field: SerializeField, ReadOnly] public GameObject source { get; private set; }
        public void Init(GameObject prefab)
        {
            if (!MiniGameManager.HasInstance || MiniGameManager.Instance == miniGameManager) return;
            miniGameManager = MiniGameManager.Instance;
            source = prefab;
            
        }

        public void StartGame()
        {
            OnEnabled();
        }

        public void ResetGame()
        {
        }

        public void StopGame()
        {
            OnDisabled();
        }
    }
}
