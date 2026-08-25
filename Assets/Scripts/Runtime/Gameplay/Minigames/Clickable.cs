using UnityEngine;
using UnityEngine.Events;

namespace TheDates.Runtime.Experimental.MinigameCore
{
    public class Clickable : MonoBehaviour, IClickable
    {
        //[SerializeField] float  OnClickRelease = new();
        [SerializeField] private UnityEvent OnClickStart = new();
        [SerializeField] private UnityEvent OnClickHold = new();
        [SerializeField] private UnityEvent OnClickRelease = new();

        private bool _hasStartEvent;
        private bool _hasHoldEvent;
        private bool _hasReleaseEvent;

        private bool _isHeld;
        
        void Start() {
            _hasStartEvent = OnClickStart.GetPersistentEventCount() > 0;
            _hasHoldEvent = OnClickHold.GetPersistentEventCount() > 0;
            _hasReleaseEvent = OnClickRelease.GetPersistentEventCount() > 0;
            
            
        }

        public void LinkParent(Transform parent)
        {
            this.contentParent = parent;
        }

        // Update is called once per frame
        void Update() {
            OnHold();
        }

        public Transform contentParent { get; private set; }

        public void OnStart() {
            if (_hasStartEvent) OnClickStart.Invoke();
        }

        public void OnHold() {
            if (!_hasHoldEvent) OnClickHold.Invoke();
        }

        public void OnRelease() {
            if (_hasReleaseEvent) OnClickRelease.Invoke();
        }
    }
}
