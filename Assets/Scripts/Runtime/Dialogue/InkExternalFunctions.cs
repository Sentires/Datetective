using Ink.Runtime;

namespace TheDates.Runtime.Dialogue
{
    public class InkExternalFunctions
    {
        public void Bind(Story story) {
            story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
            story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
            story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
        }
        public void Unbind(Story story) {
            story.UnbindExternalFunction("StartQuest");
            story.UnbindExternalFunction("AdvanceQuest");
            story.UnbindExternalFunction("FinishQuest");
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
    }
}
