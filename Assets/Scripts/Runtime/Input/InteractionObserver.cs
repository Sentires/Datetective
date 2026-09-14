using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheDates.Runtime;
using TheDates.Runtime.General;
using TheDates.Runtime.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

namespace TheDates
{
    public class InteractionObserver : MonoBehaviour {
        [SerializeField] private UnityEvent<bool> onInteract = new();
        [SerializeField] private UnityEvent onActivate = new();
        [SerializeField] private UnityEvent onDeactivate = new();
        
        private InputEvents dialogueEvents => GameEventsManager.Instance?.InputEvents;
        private bool canInteract;
        
        //[StringDropdown(nameof(tags))]
        //public string tagSelect;
        
        //[StringDropdown(nameof(tags))]
        //public string[] tagSelectArray;
        
        //private static string[] tags => UnityEditorInternal.InternalEditorUtility.tags.Prepend("<None>").ToArray();
        private void Start() {
            canInteract = false;
        }
        
        private void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.onInteract += OnInteraction;
        }

        private void OnInteraction(bool input) {
            if (input == false || canInteract == false) return;
            onInteract?.Invoke(true);
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.onInteract -= OnInteraction;
        }

        public void SetInteractionEnter(Collider2D target, Collider2D source) {
            if (!target.CompareTag("Player")) return;
            canInteract = true;
            onActivate?.Invoke();
        }
        public void SetInteractionExit(Collider2D target, Collider2D source) {
            if (!target.CompareTag("Player")) return;
            canInteract = false;
            onDeactivate?.Invoke();
        }
    }
}
