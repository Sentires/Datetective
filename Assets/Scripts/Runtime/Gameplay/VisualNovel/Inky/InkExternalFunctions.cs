using Ink.Runtime;
using UnityEngine;

namespace TheDates.Runtime.Dialogue
{
    public class InkExternalFunctions
    {
        public void Bind(Story story) {
            story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
            story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
            story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
            
            // Position is basically the index. 0 is the primary, 1 is the secondary. 
            story.BindExternalFunction("Speaker", (int positionIndex) => SetSpeaker(positionIndex));
            story.BindExternalFunction("Character", (string character, int positionIndex) => SetCharacter(character, positionIndex));
            story.BindExternalFunction("Portrait", (int positionIndex, int moodIndex) => SetPortrait(positionIndex, moodIndex));
        }
        public void Unbind(Story story) {
            story.UnbindExternalFunction("StartQuest");
            story.UnbindExternalFunction("AdvanceQuest");
            story.UnbindExternalFunction("FinishQuest");
            
            story.UnbindExternalFunction("Speaker");
            story.UnbindExternalFunction("Character");
            story.UnbindExternalFunction("Portrait");
        }
        
        private void StartQuest(string questID) {
            if (!GameEventsManager.Instance.QuestEvents.currentManager.TryGetQuestIdentifier(questID, out var hashID)) return;
            GameEventsManager.Instance.QuestEvents.StartQuest(hashID);
        }
        private void AdvanceQuest(string questID) {
            if (!GameEventsManager.Instance.QuestEvents.currentManager.TryGetQuestIdentifier(questID, out var hashID)) return;
            GameEventsManager.Instance.QuestEvents.AdvanceQuest(hashID);
        }
        private void FinishQuest(string questID) {
            if (!GameEventsManager.Instance.QuestEvents.currentManager.TryGetQuestIdentifier(questID, out var hashID)) return;
            GameEventsManager.Instance.QuestEvents.FinishQuest(hashID);
        }

        private void SetSpeaker(int positionIndex) {
            GameEventsManager.Instance.DialogueEvents.currentManager.SetCurrentSpeaker(positionIndex);
        }
        
        private void SetCharacter(string name, int positionIndex) {
            //Debug.Log("Speaker set to: " + name + ", index: " + index);
            //var profile = GameEventsManager.Instance.DialogueEvents.currentManager.FindCharacter(name);
            //if (!profile) return;
            
            //GameEventsManager.Instance.DialogueEvents.SetSpeaker(profile, index);
            GameEventsManager.Instance.DialogueEvents.currentManager.SetCharacterAt(name, positionIndex);
        }
        
        private void SetPortrait(int positionIndex, int moodIndex) {
            //Debug.Log($"Mood for '{name}' set to: {moodID}");
            //var profile = GameEventsManager.Instance.DialogueEvents.currentManager.FindCharacter(name);
            //if (!profile) return;
            
            //GameEventsManager.Instance.DialogueEvents.AdjustSpeaker(profile, moodID);
            GameEventsManager.Instance.DialogueEvents.currentManager.SetPortraitAt(positionIndex, moodIndex);
        }
    }
}
