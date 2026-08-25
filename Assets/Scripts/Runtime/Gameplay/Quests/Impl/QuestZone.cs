using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates.Runtime.Quests.Impl
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class QuestZone : QuestPoint
    {
        private bool _playerInZone;

        private void Start()
        {
            _playerInZone = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.IsPlayer()) {
                _playerInZone = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.IsPlayer()) {
                _playerInZone = false;
            }
        }

        public override bool isAvailable => _playerInZone;
    }
}
