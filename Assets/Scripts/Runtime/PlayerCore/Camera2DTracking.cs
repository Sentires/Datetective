using UnityEngine;

namespace TheDates.Runtime.PlayerCore
{
    public class Camera2DTracking : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private bool snapOnStart = true;
        
        public Transform target;
        public Vector2 trackingDeadZone;
        
        private Vector3 _velocity;
        private Vector3 _totalOffset;

        private void Start() {
            _totalOffset = new Vector3(offset.x, offset.y, transform.position.z);
            if (target == null || !snapOnStart) return;
            
            var startPosition = target.position;
            startPosition.z = 0f;
            transform.position = startPosition + _totalOffset;
        }
        
        public void SetOffset(Vector2 newOffset) {
            offset = newOffset;
            _totalOffset = new Vector3(offset.x, offset.y, _totalOffset.z);
        }

        private void LateUpdate() {
            if (target == null) return;
            
            var cameraPos = transform.position;
            var targetPos = target.position + _totalOffset;
            targetPos.z = _totalOffset.z;
            
            if (IsWithinDeadZone(targetPos, cameraPos)) {
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, damping);
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
