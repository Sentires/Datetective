using System;
using TheDates.Runtime.Quests;
using UnityEngine;
using UnityEngine.Events;

namespace TheDates.Runtime.Experimental.EventBridges
{
    public class QuestListener : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField] protected QuestInfo questContext;
        
        [Header("Listeners")]
        public QuestEvent[] onQuestStateChange = Array.Empty<QuestEvent>();
        public QuestGoalEvent[] onQuestGoalChange = Array.Empty<QuestGoalEvent>();

        private bool _isInitialised;
        
        // Start is called before the first frame update

        public void Awake() {
            _isInitialised = questContext != null;
        }

        private void OnEnable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange += QuestStateChanged;
            GameEventsManager.Instance.QuestEvents.onQuestGoalStateChange += QuestGoalChanged;
        }
        
        private void OnDisable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange -= QuestStateChanged;
            GameEventsManager.Instance.QuestEvents.onQuestGoalStateChange -= QuestGoalChanged;
        }

        private void QuestStateChanged(Quest quest) {
            if (!_isInitialised || quest.hashID != questContext.hashID) return;
            foreach (var ev in onQuestStateChange) {
                ev.Invoke(quest.state);
            }
            
        }

        private void QuestGoalChanged(int id, int index, QuestGoalState state) {
            if (!_isInitialised || id != questContext.hashID) return;
            // We currently only want to know when they're initialised
            //if (state.status != QuestGoalStatus.Starting) return;
            foreach (var ev in onQuestGoalChange) {
                ev.Invoke(index, state.status);
            }
        }
        
        public void DebugQuestState(QuestState state) {
            Debug.Log($"Changed to State '{state}' for {questContext.name}");
        }
        
        public void DebugQuestGoal(int index, QuestGoalStatus status) {
            Debug.Log($"Changed to Goal '{index}':'{status}' for {questContext.name}");
        }
        
        
        [Serializable]
        public class QuestGoalEvent {
            public int targetIndex;
            public bool ignoreIndexConstraint;
            public StatusFilter filter = StatusFilter.Start;
            public UnityEvent<int, QuestGoalStatus> listeners;

            
            
            public void Invoke(int index, QuestGoalStatus status) {
                if (!ignoreIndexConstraint && index != targetIndex) return;
                if (!ValidateFlags(status)) return;
                listeners.Invoke(index, status);
            }

            private bool ValidateFlags(QuestGoalStatus status)
            {
                return status switch {
                    QuestGoalStatus.Starting => filter.HasFlag(StatusFilter.Start),
                    QuestGoalStatus.Concluding => filter.HasFlag(StatusFilter.Finish),
                    _ => false
                };
                //return (filter.HasFlag(StatusFilter.Start) && status == QuestGoalStatus.Starting) 
                //       || (filter.HasFlag(StatusFilter.Finish) && status == QuestGoalStatus.Concluding);
            }
            
            [Flags]
            public enum StatusFilter {
                None = 0,
                Start = 1,
                Finish = 2
            }
        }
        
        [Serializable]
        public class QuestEvent {
            public QuestState targetState;
            public bool ignoreStateConstraint;
            public UnityEvent<QuestState> listeners;
            
            public void Invoke(QuestState state) {
                if (!ignoreStateConstraint && state != targetState) return;
                listeners.Invoke(state);
            }
        }
    }
}
