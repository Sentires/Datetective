using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheDates
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class GridComponent : MonoBehaviour
    {
        //public GameObject controller { get; private set; }
        public BoxCollider2D hitBox { get; private set; }
        public SpriteRenderer sprite { get; private set; }
        
        public Vector2Int desiredIndex;

        public void Awake() {
            hitBox = GetComponent<BoxCollider2D>();
            sprite = GetComponent<SpriteRenderer>();
        }
        
        
    }
}
