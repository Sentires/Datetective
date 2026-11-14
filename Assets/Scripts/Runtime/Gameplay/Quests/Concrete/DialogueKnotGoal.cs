using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime;
using TheDates.Runtime.Dialogue;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.Quests;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheDates
{
    public class DialogueKnotGoal : QuestGoal
    {
        private string _currentKnot;

        [SerializeField] private string desiredKnot;
        
        protected override void SetState(QuestGoalStatus status, string goalState) {
            // try catch?
            Debug.Log("Knot!1!1 " + goalState);
            if (!string.IsNullOrEmpty(goalState)) _currentKnot = goalState;
            UpdateState(status);
        }

        protected override string GetState() => _currentKnot;

        private void OnEnable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.DialogueEvents.onEnterDialogue += CheckStatus;
        }

        private void CheckStatus(string context) {
            //if (!context.Source.Equals(miniGamePrefab)) return;
            if (_currentKnot == desiredKnot) {
                Finalise();
                return;
            }
            
            UpdateState(QuestGoalStatus.Active);
        }

        private void OnDisable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.DialogueEvents.onEnterDialogue -= CheckStatus;
        }
        
        // listen to minigame event updates and interpret based on the referenced minigame prefab?


    }
}
