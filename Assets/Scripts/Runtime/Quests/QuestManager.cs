using System;
using System.Collections.Generic;
using TheDates.Runtime.Quests;
using UnityEngine;

namespace TheDates.Runtime.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool serialiseQuests = true;
        
        public const string QuestResourcePath = "Quests";
        public Dictionary<int, Quest> QuestMap;
        private Dictionary<string, int> _questIdentifiers; // Cases where you NEED a string reference (like Ink)
        private QuestEvents _questEvents; // Convenience only

        private QuestEvents questEvents => _questEvents ?? GameEventsManager.Instance?.QuestEvents; // Shorthand access & reassignment
        private bool hasRequirementUpdate;
        
        //private QuestEvents QuestEvents()
        
        //private Dictionary<int, string> questNames; // perhaps later
        public bool TryGetQuestIdentifier(string questName, out int questIdentifier) {
            return _questIdentifiers.TryGetValue(questName, out questIdentifier);
        }

        private void Awake() {
            QuestMap = MapQuests(out _questIdentifiers);
            foreach (var val in QuestMap.Values)
            {
                Debug.Log($"Quest: {val.namedID} has been loaded");
            }
        }

        private void Start() {
            if (!GameEventsManager.HasInstance) return;
            foreach (var quest in QuestMap.Values) {
                if (quest.state == QuestState.Active) {
                    quest.InstantiateCurrentGoal(transform);
                }
                GameEventsManager.Instance.QuestEvents.QuestStateChange(quest);
            }
        }

        public void UpdateQuestRequirements() {
            if (hasRequirementUpdate) return;
            hasRequirementUpdate = true;
            //Debug.Log($"QuestManager is awaiting a requirement update");
        }
        
        // Later on, it'd be wise to have an event system for any global game conditions/requirements.
        // We could hook this general functionality up to that rather than Update()
        private void Update() {
            if (!hasRequirementUpdate) return;
            foreach (var quest in QuestMap.Values) {
                if (quest.state == QuestState.Unavailable && CheckRequirementsMet(quest)) {
                    ChangeQuestState(quest.hashID, QuestState.Available);
                }
            }
            
            hasRequirementUpdate = false;
        }

        private void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            questEvents.BindManager(this);
            questEvents.onStartQuest += StartQuest;
            questEvents.onAdvanceQuest += AdvanceQuest;
            questEvents.onFinishQuest += FinishQuest;
            questEvents.onQuestStateChange += QuestStateChange;
            questEvents.onQuestGoalStateChange += QuestGoalStateChange;
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            questEvents.UnbindManager(this);
            questEvents.onStartQuest -= StartQuest;
            questEvents.onAdvanceQuest -= AdvanceQuest;
            questEvents.onFinishQuest -= FinishQuest;
            questEvents.onQuestStateChange -= QuestStateChange;
            questEvents.onQuestGoalStateChange -= QuestGoalStateChange;
        }

        private void OnApplicationQuit()
        {
            foreach (var quest in QuestMap.Values) {
                SaveQuest(quest);
            }
        }

        private void SaveQuest(Quest quest)
        {
            try {
                if (!serialiseQuests) return;
                var data = quest.GetQuestData();
                var serialisedData = JsonUtility.ToJson(data);
                Debug.Log(serialisedData);
                //Debug.Log($"SAVE DATA FOR {quest.namedID}\nState: {data.state}\nGoal Index: {data.goalIndex}\n{data.questGoalStates.Length} Goal States: {string.Join(", ", data.questGoalStates.Select(a => a.state))}");
                // we dont rlly wanna do that, but its an example
                //PlayerPrefs.SetString(quest.hashID, serializedData);
            }
            catch (Exception e) {
                Debug.LogError("Failed to save quest '" + quest.namedID + "' with id " + quest.hashID + ": " + e);
            }
        }
        
        private Quest LoadQuest(QuestInfo questInfo)
        {
            Quest quest = null;
            try {
                if (PlayerPrefs.HasKey(questInfo.namedID) && serialiseQuests) {
                    var serialisedData = "";//PlayerPrefs.GetString(questInfo.namedID);
                    var data = JsonUtility.FromJson<QuestData>(serialisedData);
                    quest = new Quest(questInfo, data.state, data.goalIndex, data.questGoalStates);
                }
                else {
                    quest = new Quest(questInfo);
                }
            }
            catch (Exception e) {
                Debug.LogError("Failed to load quest '" + questInfo.namedID + "' with id " + questInfo.hashID + ": " + e);
            }
            return quest;
        }
        
        private void QuestGoalStateChange(int id, int goalIndex, QuestGoalState goalState)
        {
            var quest = QuestMap[id];
            quest.StoreGoalState(goalState, goalIndex);
            ChangeQuestState(quest.hashID, quest.state);
        }

        private void QuestStateChange(Quest quest) => UpdateQuestRequirements();

        private void StartQuest(int id) {
            //Debug.Log($"Quest started: {questMap[id].namedID}");
            var quest = GetQuest(id);
            quest.InstantiateCurrentGoal(transform);
            ChangeQuestState(quest.hashID, QuestState.Active);
        }

        private void AdvanceQuest(int id) {
            //Debug.Log($"Quest advanced: {questMap[id].namedID}");
            var quest = GetQuest(id);
            quest.NextGoal();

            if (quest.goalExists) {
                quest.InstantiateCurrentGoal(transform);
                Debug.Log("Instantiated goal by: " + quest.namedID);
            }
            else {
                ChangeQuestState(quest.hashID, QuestState.Achieved);
            }
        }

        private void FinishQuest(int id) {
            //Debug.Log($"Quest finished: {questMap[id].namedID}");
            var quest = GetQuest(id);
            ClaimRewards(quest);
            ChangeQuestState(quest.hashID, QuestState.Concluded);
        }

        private void ClaimRewards(Quest quest) {
            Debug.Log($"Rewards claimed: {quest.namedID}");
        }

        private void ChangeQuestState(int id, QuestState state)
        {
            var quest = GetQuest(id);
            quest.state = state;
            
            questEvents.QuestStateChange(quest);
        }

        private bool CheckRequirementsMet(Quest quest)
        {
            var permitted = true;
            
            // TODO add more requirement options

            foreach (var prerequisite in quest.details.prerequisites)
            {
                if (GetQuest(prerequisite.hashID).state != QuestState.Concluded) {
                    permitted = false;
                }
            }
            return permitted;
        }

        private Dictionary<int, Quest> MapQuests(out Dictionary<string, int> questIdentities) {
            var allQuests = Resources.LoadAll<QuestInfo>(QuestResourcePath);
            var toMap = new Dictionary<int, Quest>();
            questIdentities = new Dictionary<string, int>();
            
            foreach (var quest in allQuests) {
                if (toMap.ContainsKey(quest.hashID)) {
                    Debug.LogWarning($"Duplicate quest key '{quest.hashID}' found for quest (n)'{quest.namedID}' and (o)'{toMap[quest.hashID].namedID}'");
                    continue;
                }
                
                toMap.Add(quest.hashID, LoadQuest(quest));
                if (!questIdentities.TryAdd(quest.namedID, quest.hashID)) {
                    Debug.LogWarning($"Duplicate quest named-identifier '{quest.namedID}' found. Please ensure unique names are given.");
                }
            }
            return toMap;
        }

        private Quest GetQuest(int hashID) {
            if (!QuestMap.TryGetValue(hashID, out var quest)) {
                Debug.LogError($"No quest found for quest hashID '{hashID}'");
            }
            
            return quest;
        }

        
        
        
    }
}
