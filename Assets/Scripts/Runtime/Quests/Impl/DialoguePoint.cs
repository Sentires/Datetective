using TheDates.Runtime.Dialogue;
using UnityEngine;

namespace TheDates.Runtime.Quests.Impl
{
    public class DialoguePoint: QuestPoint
    {
        [Header("Config")] 
        [SerializeField] private string dialogueKnot;
        
        private bool _isEnabled;
        private DialogueEvents dialogueEvents => GameEventsManager.Instance?.DialogueEvents; // Shorthand access
        private DialogueManager dialogueManager => dialogueEvents?.currentManager; // Shorthand access
        
        private void Start()
        {
            _isEnabled = true;
        }

        public void SetQuestActive()
        {
            
        }
        
        public void UpdateQuest()
        {
            // This is just a simple test implementation lmao
            if (!GameEventsManager.HasInstance || string.IsNullOrEmpty(dialogueKnot)) return;

            if (!dialogueEvents.currentManager.isRunning) {
                dialogueEvents.EnterDialogue(dialogueKnot);
                return;
            }
            //if (!dialogueManager.currentKnotName.Equals(dialogueKnot)) return;
            dialogueManager.ProcessDialogue();
            
            
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

        public override bool isAvailable => _isEnabled;
    }
}
