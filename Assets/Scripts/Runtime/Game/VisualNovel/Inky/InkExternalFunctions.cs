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
            
            story.BindExternalFunction("SetSpeaker", (string characterName) => SetSpeaker(characterName));
            story.BindExternalFunction("SetMood", (string characterName, int moodId) => SetMood(characterName, moodId));
        }
        public void Unbind(Story story) {
            story.UnbindExternalFunction("StartQuest");
            story.UnbindExternalFunction("AdvanceQuest");
            story.UnbindExternalFunction("FinishQuest");
            
            story.UnbindExternalFunction("SetSpeaker");
            story.UnbindExternalFunction("SetMood");
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

        private void SetSpeaker(string name)
        {
            Debug.Log("Speaker set to: " + name);
        }
        
        private void SetMood(string name, int mood)
        {
            Debug.Log($"Mood for '{name}' set to: {mood}");
        }
    }
}
