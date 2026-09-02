using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Dialogue;
using UnityEngine;

namespace TheDates.Runtime
{
    public class DialogueSource : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private string dialogueKnot;
        private DialogueEvents dialogueEvents => GameEventsManager.Instance?.DialogueEvents; // Shorthand access
        private DialogueManager dialogueManager => dialogueEvents?.currentManager; // Shorthand access

        public void TriggerDialogue() {
            // This is just a simple test implementation lmao
            if (!GameEventsManager.HasInstance || string.IsNullOrEmpty(dialogueKnot)) return;

            if (!dialogueEvents.currentManager.isRunning) {
                dialogueEvents.EnterDialogue(dialogueKnot);
                return;
            }
            //if (!dialogueManager.currentKnotName.Equals(dialogueKnot)) return;
            dialogueManager.ProcessDialogue();

        }
    }
}
