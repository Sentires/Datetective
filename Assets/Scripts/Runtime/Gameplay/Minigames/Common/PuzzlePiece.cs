using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheDates
{
    public class PuzzlePiece : MonoBehaviour {
        public Transform contentParent { get; private set; }
        public PuzzlePieceDisplay materialDisplay { get; private set; }
        public Vector3 piecePosition { get => transform.position; set => transform.position = value; }
        public Vector3 pieceLocalPosition { get => transform.localPosition; set => transform.localPosition = value; }
        public float pieceWidth { get; private set; }
        public float pieceHeight { get; private set; }
        
        public BoxCollider2D clickableArea => hitBox;
        //public bool isSelected => _selected; // Probs better to rework this into a hover/select/normal enum state machine or smth

        [SerializeField] private BoxCollider2D hitBox;
        [SerializeField] private Renderer materialRenderer;
        [SerializeField] private Texture2D presetTexture;
        [SerializeField] private Vector2 sizeModifier = Vector2.one;
        
        
        private int _sortingLayerID;
        //private bool _selected;

        public void Awake() {
            _sortingLayerID = SortingLayer.NameToID("Minigame");
            
            hitBox ??= GetComponent<BoxCollider2D>();
            materialRenderer ??= GetComponent<Renderer>();
        }

        public void Init(Transform newParent) {
            contentParent = newParent;
            // probs should set as transform parent, too
            
            materialDisplay = new PuzzlePieceDisplay(materialRenderer) {
                currentSortingLayerID = _sortingLayerID
            };
            if (presetTexture) {
                materialDisplay.SetAlbedoMap(presetTexture);
                
                pieceHeight = 1f;
                var aspect = (float)presetTexture.width / presetTexture.height;
                pieceWidth = aspect;
                transform.localScale = new Vector3(sizeModifier.x * pieceWidth, sizeModifier.y * pieceHeight, 1);
            }
        }

        public Vector3 GetScaledDimensions(Vector3 factor) => new Vector3(GetScaledWidth(factor.x), GetScaledHeight(factor.y), factor.z);
        
        public float GetScaledWidth(float factor) => sizeModifier.x * pieceWidth * factor;
        public float GetScaledHeight(float factor) => sizeModifier.y * pieceHeight * factor;
        
        

        //public void AdjustScale(Vector3 factor){
        //    transform.localScale = new Vector3(pieceWidth, pieceHeight, 1);
        //}
    }
}
