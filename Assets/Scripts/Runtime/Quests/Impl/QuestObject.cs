using TheDates.Runtime.Quests;
using UnityEngine;

namespace TheDates.Runtime.Quests.Impl
{
    public class QuestObject : QuestPoint
    {
        //public int counter;
        
        private bool _isEnabled;
        
        private void Start()
        {
            _isEnabled = true;
            //counter = 0;
        }
        
        public void UpdateQuest()
        {
            // This is just a simple test implementation lmao
            if (!_isEnabled) return;
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
            
            //if (counter is >= 3 or < 0) counter = 0;
            //counter++;

            /*switch (counter) {
                case 1:
                    if (_currentQuestState == QuestState.Available && questRole.HasFlag(Role.StartPoint)) { //_currentQuestState == QuestState.Available && questRole.HasFlag(Role.StartPoint)
                        GameEventsManager.Instance.QuestEvents.StartQuest(_questID);
                        break;
                    }
                    Debug.Log($"QuestObject '{name}' cannot start the quest.\nIts state is '{_currentQuestState}' while expecting '{QuestState.Available}'\nIts role is '{questRole}' while expecting '{Role.StartPoint}'");
                    break;
                case 2:
                    GameEventsManager.Instance.QuestEvents.AdvanceQuest(_questID);
                    break;
                case 3:
                    if (_currentQuestState == QuestState.Achieved && questRole.HasFlag(Role.EndPoint)) { //_currentQuestState == QuestState.Achieved && questRole.HasFlag(Role.EndPoint)
                        GameEventsManager.Instance.QuestEvents.FinishQuest(_questID);
                        break;
                    }
                    Debug.Log($"QuestObject '{name}' cannot conclude the quest.\nIts state is '{_currentQuestState}' while expecting '{QuestState.Achieved}'\nIts role is '{questRole}' while expecting '{Role.EndPoint}'");
                    break;
                default:
                    counter = 0;
                    break;
            }*/
            
        }

        public override bool isAvailable => _isEnabled;
    }
}
