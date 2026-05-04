using System;
using UnityEngine;
using UnityEngine.Events;

namespace TheDates.Runtime.Quests
{
    public class QuestHandler : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField] protected QuestInfo questContext;
        
        [Header("Config")]
        [SerializeField] protected Role questRole = Role.Both;
        
        protected int _questID;
        protected QuestState _currentQuestState;
        
        public virtual bool isAvailable { get; }

        public UnityEvent OnQuestAvailable = new UnityEvent();
        public UnityEvent OnQuestUnavailable = new UnityEvent();
        public UnityEvent OnQuestActive = new UnityEvent();
        public UnityEvent OnQuestAchieved = new UnityEvent();
        public UnityEvent OnQuestConcluded = new UnityEvent();

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

        public virtual void UpdateQuest() {
            switch (_currentQuestState) {
                case QuestState.Available: {
                    if (questRole.HasFlag(Role.StartPoint)) {
                        GameEventsManager.Instance.QuestEvents.StartQuest(_questID);
                    }
                    break;
                }
                case QuestState.Achieved: {
                    if (questRole.HasFlag(Role.EndPoint)) {
                        GameEventsManager.Instance.QuestEvents.FinishQuest(_questID);
                    }
                    break;
                }
                case QuestState.Unavailable:
                case QuestState.Active:
                case QuestState.Concluded:
                default:
                    break;
            }
            /*
            switch (_currentQuestState)
            {
                case QuestState.Available:
                    if (questRole.HasFlag(Role.StartPoint)) {
                        GameEventsManager.Instance.QuestEvents.StartQuest(_questID);
                        break;
                    }
                    Debug.Log("YOU: There must be something that can give me this quest");
                    break;
                case QuestState.Achieved:
                    if (questRole.HasFlag(Role.EndPoint)) {
                        GameEventsManager.Instance.QuestEvents.FinishQuest(_questID);
                        break;
                    }
                    Debug.Log("YOU: There must be something that can end this quest for me");
                    break;
                case QuestState.Unavailable:
                    Debug.Log("YOU: I haven't unlocked this quest yet!");
                    break;
                case QuestState.Active:
                    Debug.Log("YOU: This quest is active!");
                    break;
                case QuestState.Concluded:
                    Debug.Log("YOU: I've already completed this quest!");
                    break;
                default:
                    Debug.Log("..wtf? how");
                    break;
            }
            */
        }

        private void QuestStateChange(Quest quest)
        {
            if (quest.details.hashID == _questID) {
                _currentQuestState = quest.state;
                var ev = _currentQuestState switch {
                    QuestState.Available => OnQuestAvailable,
                    QuestState.Unavailable => OnQuestUnavailable,
                    QuestState.Active => OnQuestActive,
                    QuestState.Achieved => OnQuestAchieved,
                    QuestState.Concluded => OnQuestConcluded,
                    _ => throw new ArgumentOutOfRangeException()
                };
                ev.Invoke();
                //InvokeQuestEvents();
                
                Debug.Log($"Quest state changed to {_currentQuestState} for '{quest.namedID}'");
            }
        }

        private void InvokeQuestEvents()
        {
            //string debugMessage;
            switch (_currentQuestState)
            {
                case QuestState.Available:
                    OnQuestAvailable.Invoke();
                    //debugMessage = "Quest Event: Available";
                    break;
                case QuestState.Unavailable:
                    OnQuestUnavailable.Invoke();
                    //debugMessage = "Quest Event: Unavailable";
                    break;
                case QuestState.Active:
                    OnQuestActive.Invoke();
                    //debugMessage = "Quest Event: Active";
                    break;
                case QuestState.Achieved:
                    OnQuestAchieved.Invoke();
                    //debugMessage = "Quest Event: Achieved";
                    break;
                case QuestState.Concluded:
                    OnQuestConcluded.Invoke();
                    //debugMessage = "Quest Event: Concluded";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //Debug.Log(debugMessage);
        }

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
