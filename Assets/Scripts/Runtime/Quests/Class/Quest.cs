using TheDates.Runtime.Quests;
using UnityEngine;

namespace TheDates.Runtime.Quests
{
    public class Quest
    {
        public QuestInfo details;
        public QuestState state;
        
        private int currentGoalIndex;
        private QuestGoalState[] goalStates;

        public Quest(QuestInfo source)
        {
            this.details = source;
            state = QuestState.Unavailable;
            currentGoalIndex = 0;
            
            goalStates = new QuestGoalState[details.stepPrefabs.Length];
            for (var i = 0; i < goalStates.Length; i++) {
                goalStates[i] = new QuestGoalState();
            }
        }

        public Quest(QuestInfo source, QuestState state, int currentGoalIndex, QuestGoalState[] goalStates)
        {
            this.details = source;
            this.state = state;
            this.currentGoalIndex = currentGoalIndex;
            this.goalStates = goalStates;
            if (this.goalStates.Length != details.stepPrefabs.Length) {
                Debug.LogWarning("Quest goal prefabs and goal states do not match. Reset your data. Refer to: " + details.namedID);
            }
        }

        public void NextGoal()
        {
            currentGoalIndex++;
        }

        public void InstantiateCurrentGoal(Transform parent)
        {
            var prefab = GetGoalPrefab(currentGoalIndex);
            if (prefab) { // If the game were big enough, pooling could be worthwhile for performance
                var goal = Object.Instantiate(prefab, parent).GetComponent<QuestGoal>();
                goal.Initialise(details.hashID, currentGoalIndex, goalStates[currentGoalIndex].state);
            }
        }

        private GameObject GetGoalPrefab(int index)
        {
            GameObject prefab = null;
            if (HasGoal(index)) {
                prefab = details.stepPrefabs[index];
            }
            else {
                Debug.LogWarning($"No goal found for '{details.namedID}' at index [{index}].\nPlease check ScriptableObject '{details.namedID}' at {details.path}.");
            }
            
            return prefab;
        }
        
        public bool HasGoal(int index) => details.stepPrefabs.Length > index && index >= 0;
        
        public bool goalExists => currentGoalIndex < details.stepPrefabs.Length;
        public int hashID => details?.hashID ?? 0; // Shorthand access, 0 means details was null
        public string namedID => details?.namedID ?? string.Empty; // Shorthand access, string.Empty means details was null

        public void StoreGoalState(QuestGoalState goalState, int goalIndex) {
            if (goalIndex < goalStates.Length) {
                goalStates[goalIndex].state = goalState.state;
            }
            else {
                Debug.LogWarning($"Cannot access goal state data for '{details.namedID}' at index [{goalIndex}].\nThis is out of range.");
            }
        }

        public QuestData GetQuestData()
        {
            return new QuestData(state, currentGoalIndex, goalStates);
        }
    }
}
