using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TheDates
{
    public class ObservableTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D, Collider2D> onTriggerEnter = new();
        [SerializeField] private UnityEvent<Collider2D, Collider2D> onTriggerExit  = new();
        
        private Collider2D _collider2D;
        private bool _isInitialised;
        public event ColliderEvent triggerEnter = delegate{ };
        public event ColliderEvent triggerExit = delegate{ };
        
        public delegate void ColliderEvent(Collider2D target, Collider2D source);

        private void Awake() {
            var colliders = GetComponents<Collider2D>();
            if (colliders.Length < 1) return;
            
            _collider2D = colliders[0];
            _collider2D.isTrigger = true;
            // Just exits if length == 1
            for (var i = colliders.Length - 1; i >= 1; i--) {
                Destroy(colliders[i]);
            }
            
        }
        private void Start() {
            _isInitialised = _collider2D.AssertNull(this, nameof(_collider2D), $"{nameof(ObservableTrigger)} requires {nameof(_collider2D)} to function.");
            triggerEnter += onTriggerEnter.Invoke;
            triggerExit += onTriggerExit.Invoke;
        }

        private void OnTriggerEnter2D(Collider2D other) => RaiseColliderEvent(triggerEnter, other);
        private void OnTriggerExit2D(Collider2D other) => RaiseColliderEvent(triggerExit, other);
        

        private void RaiseColliderEvent(ColliderEvent triggerEvent, Collider2D target) {
            if (!_isInitialised) return;
            triggerEvent(target, _collider2D);
        }
        
        // I thought it would be fun to make this, but decided it was a waste of time when I could be doing... other stuff... oops!
        /*public enum ColliderType { Null, Box, Sphere, Capsule }
        public ColliderType triggerShape { get; private set; }
        
        private Collider2D _collider2D;
        private bool isInitialised;
        private delegate void ColliderEvent(Collider2D target);
        private ColliderEvent _triggerEnter = delegate { };
        private ColliderEvent _triggerExit = delegate { };
        
        [SerializeField] private UnityEvent<Collider2D> onTriggerEnter = new();
        [SerializeField] private UnityEvent<Collider2D> onTriggerExit  = new();
        
        private void Awake() {
            // Repeat my inspector logic (with some changes)
            //_collider2D = null;
            Collider2D thisCollider = null;
            var desiredType = GetColliderType(triggerShape);
            
            var colliders = GetComponents<Collider2D>();
            if (colliders.Length == 1) {
                var col = colliders[0];
                if (triggerShape == ColliderType.Null || thisCollider || col.GetType() != desiredType) {
                    Destroy(col);
                }
            }
            if (colliders.Length != 0) {
                for (var i = colliders.Length - 1; i >= 0; i--) {
                    var col = colliders[i];
                    if (triggerShape == ColliderType.Null || thisCollider || col.GetType() != desiredType) {
                        Destroy(col);
                        continue;
                    }
                    thisCollider = col;
                }
            }
            
            if (!thisCollider) thisCollider = AddCollider(gameObject, triggerShape);
            if (triggerShape == ColliderType.Null || !thisCollider) return;
            // And set it to be trigger
            _collider2D = thisCollider;
            _collider2D.isTrigger = true;
            isInitialised = true;

            //return;

            //bool IsColliderInvalid(ColliderType cT, Collider2D c, Type t) => cT == ColliderType.Null || c || c.GetType() != t;
        }

        private void Start() {
            //if (_collider2D) return;

            _triggerEnter = GetDelegate(onTriggerEnter);
            _triggerExit = GetDelegate(onTriggerExit);
            
            return;
            ColliderEvent GetDelegate(UnityEvent<Collider2D> evt) {
                return isInitialised && (evt?.GetPersistentEventCount() ?? 0) != 0 // If not null or (null is 0) length != 0
                    ? t => evt?.Invoke(t)
                    : delegate {};
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            _triggerEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other) {
            _triggerExit(other);
        }
        
        private static Type GetColliderType(ColliderType type)
        {
            return type switch {
                ColliderType.Box => typeof(BoxCollider2D),
                ColliderType.Sphere => typeof(CircleCollider2D),
                ColliderType.Capsule => typeof(CapsuleCollider2D),
                _ => null // ColliderType.Null => null
            };
        }

        private static Collider2D AddCollider(GameObject go, ColliderType type) {
            return type switch {
                ColliderType.Box => go.AddComponent<BoxCollider2D>(),
                ColliderType.Sphere => go.AddComponent<CircleCollider2D>(),
                ColliderType.Capsule => go.AddComponent<CapsuleCollider2D>(),
                _ => null // ColliderType.Null => null
            };
        }
        
        // Editor mumbo jumbo
    #if UNITY_EDITOR
        [SerializeField] private ColliderType colliderType = ColliderType.Null;
        private Type _desiredType;
        
        // https://discussions.unity.com/t/editor-destroyimmediate-a-component-on-targetobject-causes-missing-reference-exception/541446/6
        // https://discussions.unity.com/t/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress/705805/13
        private void OnValidate() { EditorApplication.delayCall += _OnValidate; }
        private void _OnValidate()
        {
            if (this == null) return;
            var type = GetColliderType(colliderType);
            if (_desiredType == type) return;
            _desiredType = type;
            triggerShape = colliderType;
            
            var colliders = GetComponents<Collider2D>();
            if (colliders.Length == 0) {
                if (colliderType == ColliderType.Null) return;
            }
            else {
                Collider2D match = null;
                for (var i = colliders.Length - 1; i >= 0; i--) {
                    if (colliderType == ColliderType.Null|| match || _desiredType != colliders[i].GetType()) {
                        var col = colliders[i];
                        col.hideFlags = HideFlags.HideInInspector;
                        //EditorApplication.delayCall += () => DestroyImmediate(col);
                        DestroyImmediate(col);
                        continue;
                        //DestroyImmediate(colliders[i]);
                    }
                    match = colliders[i];
                }
                
                if (match) return;
            }
            
            AddCollider(gameObject, colliderType);
        }

        
    #endif*/
    }
}
