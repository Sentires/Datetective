using System;
using UnityEngine;

namespace TheDates.Runtime.Quests
{
    
    public abstract class QuestPoint : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField] protected QuestInfo questContext;
        
        [Header("Config")]
        [SerializeField] protected Role questRole = Role.Both;
        
        protected int _questID;
        protected QuestState _currentQuestState;
        
        public abstract bool isAvailable { get; }

        private void OnEnable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange += QuestStateChange;
        }

        private void OnDisable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange += QuestStateChange;
        }

        private void QuestStateChange(Quest quest)
        {
            if (quest.details.hashID == _questID) {
                _currentQuestState = quest.state;
                Debug.Log($"Quest state changed to {_currentQuestState} for '{quest.namedID}'");
            }
        }
        
        //public static void 

        private void Awake()
        {
            if (!questContext.AssertValidation(this)) {
                return;
            }
            
            _questID = questContext.hashID;
        }
        
        [Flags] // This is an alternative to using 2 booleans.
        public enum Role
        {
            Neither = 0, // this disables them
            StartPoint = 1 << 0,
            EndPoint = 1 << 1,
            Both = ~0, // this enables them
        }
    }
}
