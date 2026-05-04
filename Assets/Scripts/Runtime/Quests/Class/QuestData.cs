using System;

namespace TheDates.Runtime.Quests
{
    [Serializable]
    public class QuestData
    {
        // TODO implement this with the existing JSON persistent data architecture I did
        public QuestState state;
        public int goalIndex;
        public QuestGoalState[] questGoalStates;

        public QuestData(QuestState state, int goalIndex, QuestGoalState[] questGoalStates) {
            this.state = state;
            this.goalIndex = goalIndex;
            this.questGoalStates = questGoalStates;
        }
    }
    
    [Serializable]
    public class QuestGoalState {
        public string state;

        public QuestGoalState(string state) {
            this.state = state;
        }

        public QuestGoalState() {
            this.state = string.Empty;
        }
    }
}
