using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.Quests;
using UnityEngine;

namespace TheDates
{
    public class MiniGameGoal : QuestGoal
    {
        //private int _interactions = 0;
        //private int _interactionsRequired = 4;
        private MiniGameState minigameState;
        public GameObject MiniGamePrefab;
        
        protected override void SetState(string goalState)
        {
            // try catch?
            minigameState = Enum.Parse<MiniGameState>(goalState);
            UpdateState();
        }

        protected override string GetState() => minigameState.ToString();

        //public void Interact(GameObject target) => Interaction(target);

        private void OnEnable()
        {
            if (!MiniGameManager.HasInstance) return;
            MiniGameManager.Instance.OnMiniGameProcessed += CheckStatus;
        }

        private void CheckStatus(MiniGameContext context) {
            if (!context.Source.Equals(MiniGamePrefab)) return;
            if (context.State == MiniGameState.Completed && GameEventsManager.Instance.QuestEvents.currentManager.QuestMap[questID].state == QuestState.Active) {
                Finalise();
                return;
            }
            
            UpdateState();
            
        }

        private void OnDisable()
        {
            if (!MiniGameManager.HasInstance) return;
            MiniGameManager.Instance.OnMiniGameProcessed -= CheckStatus;
        }
        
        // listen to minigame event updates and interpret based on the referenced minigame prefab?


    }
}
