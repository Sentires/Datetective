using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheDates
{
    //[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    // We'll try fetch them if they weren't specified instead. This might be more versatile.
    public class GridTile : MonoBehaviour
    {
        public GameObject gridParent { get; private set; }
        public MaterialInstanceWrapper materialDisplay { get; private set; }
        public BoxCollider2D clickableArea => hitBox;
        public bool isSelected => _selected;
        public Vector3 tilePosition { get => transform.position; set => transform.position = value; }
        public Vector3 tileLocalPosition { get => transform.localPosition; set => transform.localPosition = value; }

        [SerializeField] private BoxCollider2D hitBox;
        [SerializeField] private Renderer materialRenderer;
        
        private int _sortingLayerID;
        private bool _selected;

        public void Awake() {
            _sortingLayerID = SortingLayer.NameToID("Minigame");
            
            hitBox ??= GetComponent<BoxCollider2D>();
            materialRenderer ??= GetComponent<Renderer>();
            
            //if (!materialRenderer) return;
            //materialDisplay = new MaterialInstanceWrapper(materialRenderer);
        }

        public void Init(GameObject newParent) {
            gridParent = newParent;
            // probs should set as transform parent, too
            
            materialDisplay = new MaterialInstanceWrapper(materialRenderer) {
                currentSortingLayerID = _sortingLayerID
            };
        }
        
        
    }
}
