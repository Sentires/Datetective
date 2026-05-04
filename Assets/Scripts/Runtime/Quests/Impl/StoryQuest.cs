namespace TheDates.Runtime.Quests.Impl
{
    public class StoryQuest : QuestGoal
    {
        private string _placeholder;
        
        protected override string GetState() => _placeholder;

        protected override void SetState(string goalState) {
            _placeholder = goalState;
            UpdateState();
        }
    }
}
