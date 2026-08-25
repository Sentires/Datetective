using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.General;
using TheDates.Runtime.Input;
using UnityEngine;

namespace TheDates.Runtime.PlayerCore
{
    [RequireComponent(typeof(Camera))]
    public class Camera2DHandler : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private bool snapOnEnable = true;
        
        public Transform target;
        public Vector2 trackingDeadZone;
        
        private Vector3 _velocity;
        private Vector3 _totalOffset;
        private bool _followTarget;
        private Transform _pairedTransform;

        [field: SerializeField, ReadOnly] public Camera pairedCamera { get; private set; }

        private bool isFollowingTarget => _followTarget && pairedCamera != null && target != null;

        private void Awake() {
            pairedCamera = GetComponent<Camera>();
            _pairedTransform = pairedCamera.transform;
        }

        private void Start() {
            _totalOffset = new Vector3(offset.x, offset.y, pairedCamera.transform.position.z);

            _followTarget = true;
            var startPosition = isFollowingTarget && snapOnEnable ? target.position : pairedCamera.transform.position;
            pairedCamera.transform.position = startPosition.With(z: 0f) + _totalOffset;
        }

        private void OnEnable() {
            pairedCamera ??= GetComponent<Camera>();
            pairedCamera.enabled = true;
            _pairedTransform = pairedCamera?.transform;
        }

        private void OnDisable() {
            pairedCamera ??= GetComponent<Camera>();
            pairedCamera.enabled = false;
            _pairedTransform = null;
        }
        
        public void SetOffset(Vector2 newOffset) {
            offset = newOffset;
            _totalOffset = new Vector3(offset.x, offset.y, _totalOffset.z);
        }

        public void SnapToPosition(Vector2 newPosition) {
            StartCoroutine(SnapPosition(newPosition));
        }

        private IEnumerator SnapPosition(Vector2 newPosition) {
            _followTarget = false;
            _velocity = Vector3.zero;
            _pairedTransform.position = new Vector3(newPosition.x + _totalOffset.x, newPosition.y + _totalOffset.y, _totalOffset.z);
            yield return null;
            _followTarget = true;
        }

        private void LateUpdate() {
            if (!isFollowingTarget) return;

            var cameraPos = _pairedTransform.position;
            var targetPos = (target.position + _totalOffset).With(z: _totalOffset.z);
            
            if (IsWithinDeadZone(targetPos, cameraPos)) {
                _pairedTransform.position = Vector3.SmoothDamp(cameraPos, targetPos, ref _velocity, damping);
            }
        }

        private bool IsWithinDeadZone(Vector3 to, Vector3 from) {
            return trackingDeadZone == Vector2.zero || 
                   Mathf.Abs(to.x - from.x) > trackingDeadZone.x ||
                   Mathf.Abs(to.y - from.y) > trackingDeadZone.y;
        }
        
        // This just visualises the offset and deadzone for us
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            var position = transform.position;
            
            if (trackingDeadZone != Vector2.zero) {
                Gizmos.color = Color.blue;
                
                var gizmoArea = new Vector3(trackingDeadZone.x, trackingDeadZone.y, position.z);
                Gizmos.DrawWireCube(position, gizmoArea);
            }
            if (offset != Vector2.zero) {
                Gizmos.color = Color.green;
            
                var gizmoOffset = new Vector3(offset.x, offset.y, 0f);
                gizmoOffset += position;
                Gizmos.DrawLine(position, gizmoOffset);
            }
        }
        #endif
    }
}
