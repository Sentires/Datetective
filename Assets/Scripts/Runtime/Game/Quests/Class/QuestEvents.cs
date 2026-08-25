using System;
using UnityEngine;

namespace TheDates.Runtime.Quests
{
    public class QuestEvents : EventHandler
    {
        public QuestManager currentManager { get; private set; }
        public void BindManager(QuestManager questManager) {
            currentManager = questManager;
        }
        
        public void UnbindManager(QuestManager questManager) {
            if (currentManager != questManager) return;
            currentManager = null;
        }
        
        public event Action<int> onStartQuest;
        public void StartQuest(int id) => onStartQuest?.Invoke(id);
        
        public event Action<int> onAdvanceQuest;
        public void AdvanceQuest(int id) => onAdvanceQuest?.Invoke(id);
        
        public event Action<int> onFinishQuest;
        public void FinishQuest(int id) => onFinishQuest?.Invoke(id);
        
        public event Action<Quest> onQuestStateChange;
        public void QuestStateChange(Quest quest) => onQuestStateChange?.Invoke(quest);
        
        public event Action<int, int, QuestGoalState> onQuestGoalStateChange;
        public void QuestGoalStateChange(int id, int index, QuestGoalState state) => onQuestGoalStateChange?.Invoke(id, index, state);
    }
    
    public class InteractableEvents : EventHandler
    {
        public event Action<GameObject> onInteracted;

        public void Interact(GameObject gameObject) {
            onInteracted?.Invoke(gameObject);
        }
    }
}
