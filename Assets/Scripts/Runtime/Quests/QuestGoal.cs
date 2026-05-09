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
            //if (!string.IsNullOrEmpty(goalState)) {
            SetState(QuestGoalStatus.Starting, goalState);
            //}
            // If we want to notify others when it becomes active
        }

        private void ChangeState(QuestGoalStatus status, string state) => GameEventsManager.Instance.QuestEvents.QuestGoalStateChange(questID, goalIndex, new QuestGoalState(status, state));

        protected void Finalise()
        {
            if (!_isFinished)
            {
                _isFinished = true;
                // If we want to notify others when it becomes inactive
                UpdateState(QuestGoalStatus.Concluding);
                GameEventsManager.Instance.QuestEvents.AdvanceQuest(questID);
                
                Destroy(gameObject);
            }
        }
        
        // Each child implements its own GetState(), so any core logic can go here
        protected void UpdateState(QuestGoalStatus status) {
            var state = GetState();
            ChangeState(status, state);
        }
        protected abstract string GetState();

        //protected abstract void AdvanceQuest(); // TODO - Advancement logic
        protected abstract void SetState(QuestGoalStatus status, string goalState);

    }
}
