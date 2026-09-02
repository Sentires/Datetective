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
            story.BindExternalFunction("Alias", (int positionIndex, string alias) => SetAlias(positionIndex, alias));
        }
        public void Unbind(Story story) {
            story.UnbindExternalFunction("StartQuest");
            story.UnbindExternalFunction("AdvanceQuest");
            story.UnbindExternalFunction("FinishQuest");
            
            story.UnbindExternalFunction("Speaker");
            story.UnbindExternalFunction("Character");
            story.UnbindExternalFunction("Portrait");
            story.UnbindExternalFunction("Alias");
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
            GameEventsManager.Instance.DialogueEvents.currentManager.SetCharacterAt(name, positionIndex);
        }
        
        private void SetPortrait(int positionIndex, int moodIndex) {
            GameEventsManager.Instance.DialogueEvents.currentManager.SetPortraitAt(positionIndex, moodIndex);
        }
        
        private void SetAlias(int positionIndex, string alias) {
            GameEventsManager.Instance.DialogueEvents.currentManager.SetSpeakerAlias(positionIndex, alias);
        }
    }
}
