using UnityEngine;

namespace TheDates.Runtime.Quests
{
    public abstract class QuestGoal : MonoBehaviour
    {
        protected bool _isFinished = false;
        protected int questID;
        protected int goalIndex;

        public void Initialise(int id, int index, string goalState) {
            questID = id;
            goalIndex = index;
            if (!string.IsNullOrEmpty(goalState)) {
                SetState(goalState);
            }
        }

        private void ChangeState(string state) => GameEventsManager.Instance.QuestEvents.QuestGoalStateChange(questID, goalIndex, new QuestGoalState(state));

        protected void Finalise()
        {
            if (!_isFinished)
            {
                _isFinished = true;
                //AdvanceQuest();
                GameEventsManager.Instance.QuestEvents.AdvanceQuest(questID);
                
                Destroy(gameObject);
            }
        }
        
        // Each child implements its own GetState(), so any core logic can go here
        protected void UpdateState() {
            var state = GetState();
            ChangeState(state);
        }
        protected abstract string GetState();

        //protected abstract void AdvanceQuest(); // TODO - Advancement logic
        protected abstract void SetState(string goalState);

    }
}
