using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.Quests;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheDates
{
    public class MiniGameGoal : QuestGoal
    {
        //private int _interactions = 0;
        //private int _interactionsRequired = 4;
        private MiniGameState _minigameState;
        [FormerlySerializedAs("MiniGamePrefab")] public GameObject miniGamePrefab;
        
        protected override void SetState(QuestGoalStatus status, string goalState) {
            // try catch?
            if (!string.IsNullOrEmpty(goalState)) _minigameState = Enum.Parse<MiniGameState>(goalState);
            UpdateState(status);
        }

        protected override string GetState() => _minigameState.ToString();

        //public void Interact(GameObject target) => Interaction(target);

        private void OnEnable()
        {
            if (!MiniGameManager.HasInstance) return;
            MiniGameManager.Instance.OnMiniGameProcessed += CheckStatus;
        }

        private void CheckStatus(MiniGameContext context) {
            if (!context.Source.Equals(miniGamePrefab)) return;
            if (context.State == MiniGameState.Completed && GameEventsManager.Instance.QuestEvents.currentManager.QuestMap[questID].state == QuestState.Active) {
                Finalise();
                return;
            }
            
            UpdateState(QuestGoalStatus.Active);
        }

        private void OnDisable()
        {
            if (!MiniGameManager.HasInstance) return;
            MiniGameManager.Instance.OnMiniGameProcessed -= CheckStatus;
        }
        
        // listen to minigame event updates and interpret based on the referenced minigame prefab?


    }
}
