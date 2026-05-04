using UnityEngine;
using UnityEngine.Events;

namespace TheDates
{
    public class ObservableCollision : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D, Collider2D> onCollisionEnter = new();
        [SerializeField] private UnityEvent<Collider2D, Collider2D> onCollisionExit = new();

        private Collider2D _collider2D;
        private bool _isInitialised;
        public event ColliderEvent collisionEnter = delegate { };
        public event ColliderEvent collisionExit = delegate { };

        public delegate void ColliderEvent(Collider2D target, Collider2D source);

        private void Awake()
        {
            var colliders = GetComponents<Collider2D>();
            if (colliders.Length < 1) return;

            _collider2D = colliders[0];
            _collider2D.isTrigger = false;
            // Just exits if length == 1
            for (var i = colliders.Length - 1; i >= 1; i--)
            {
                Destroy(colliders[i]);
            }

        }

        private void Start()
        {
            _isInitialised = _collider2D.AssertNull(this, nameof(_collider2D),
                $"{nameof(ObservableCollision)} requires {nameof(_collider2D)} to function.");
            collisionEnter += onCollisionEnter.Invoke;
            collisionExit += onCollisionExit.Invoke;
        }

        private void OnCollisionEnter2D(Collision2D other) => RaiseColliderEvent(collisionEnter, other.collider);
        private void OnCollisionExit2D(Collision2D other) => RaiseColliderEvent(collisionExit, other.collider);


        private void RaiseColliderEvent(ColliderEvent collisionEvent, Collider2D target)
        {
            if (!_isInitialised) return;
            collisionEvent(target, _collider2D);
        }
    }
}