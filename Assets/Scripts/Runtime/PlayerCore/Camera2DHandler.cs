using System;
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
        private bool _followTarget => pairedCamera != null;

        [field: SerializeField, ReadOnly] public Camera pairedCamera { get; private set; }
        //[field: SerializeField, ReadOnly] public float defaultScale { get; private set; }
        
        public bool isFollowingTarget => _followTarget && target != null;

        private void Awake() {
            pairedCamera = GetComponent<Camera>();
            //defaultScale = pairedCamera.orthographicSize;
        }

        private void Start() {
            
            _totalOffset = new Vector3(offset.x, offset.y, transform.position.z);
            //InputContextChanged(GameEventsManager.Instance.InputEvents.inputEventContext); // double check now

            var startPosition = isFollowingTarget && snapOnEnable ? target.position : transform.position;
            
            startPosition.z = 0f;
            transform.position = startPosition + _totalOffset;
        }

        private void OnEnable() {
            pairedCamera ??= GetComponent<Camera>();
            pairedCamera.enabled = true;
            
            
            //if (!GameEventsManager.HasInstance) return;
            //GameEventsManager.Instance.InputEvents.onInputContextChange += InputContextChanged;
            
        }

        private void OnDisable() {
            pairedCamera ??= GetComponent<Camera>();
            pairedCamera.enabled = false;
            
            //if (!GameEventsManager.HasInstance) return;
            //GameEventsManager.Instance.InputEvents.onInputContextChange -= InputContextChanged;
        }
        
        

        //private void InputContextChanged(InputEvents.Context contextType) {
        //        _followTarget = contextType == InputEvents.Context.World;
        //}
        
        public void SetOffset(Vector2 newOffset) {
            offset = newOffset;
            _totalOffset = new Vector3(offset.x, offset.y, _totalOffset.z);
        }

        public void SnapToPosition(Vector2 newPosition)
        {
            pairedCamera.transform.position = new Vector3(newPosition.x + _totalOffset.x, newPosition.y + _totalOffset.y, pairedCamera.transform.position.z);
            //pairedCamera.transform.position = pos;
            
            _velocity = Vector3.zero;
        }

        private void LateUpdate() {
            if (!isFollowingTarget) return;
            
            //var cameraPos = PixelSnap(transform.position);
            //var targetPos = PixelSnap(target.position + _totalOffset);
            var cameraPos = transform.position;
            var targetPos = target.position + _totalOffset;
            targetPos.z = _totalOffset.z;
            
            
            if (IsWithinDeadZone(targetPos, cameraPos)) {
                transform.position = Vector3.SmoothDamp(cameraPos, targetPos, ref _velocity, damping);
                //transform.position = PixelSnap(Vector3.SmoothDamp(cameraPos, targetPos, ref _velocity, damping));
                //Vector3Int.
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
