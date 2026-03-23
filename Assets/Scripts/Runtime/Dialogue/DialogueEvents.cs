using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace TheDates.Runtime.Dialogue
{
    public class DialogueEvents : EventHandler
    {
        public DialogueEvents() : base()
        {
            _handlers.Add(nameof(onEnterDialogue), onEnterDialogue);
        }
        
        public DialogueManager currentManager { get; private set; }
        public void BindManager(DialogueManager dialogueManager) {
            currentManager = dialogueManager;
        }
        
        public void UnbindManager(DialogueManager dialogueManager) {
            if (currentManager != dialogueManager) return;
            currentManager = null;
        }
        
        public event Action<string> onEnterDialogue = delegate { };
        public void EnterDialogue(string dialogue) => onEnterDialogue?.Invoke(dialogue);
        
        //public event Action<string> onExitDialogue;
        //public void ExitDialogue(string dialogue) => onExitDialogue?.Invoke(dialogue);

        public event Action onDialogueStarted;
        public void DialogueStarted() => onDialogueStarted?.Invoke();
        
        public event Action onDialogueFinished;
        public void DialogueFinished() => onDialogueFinished?.Invoke();

        public event Action<string, List<Choice>> onDialogueDisplay;
        public void DisplayDialogue(string dialogueLine, List<Choice> choices) => onDialogueDisplay?.Invoke(dialogueLine, choices);
        
        public event Action<int> onUpdateChoiceIndex;
        public void UpdateChoiceIndex(int index) => onUpdateChoiceIndex?.Invoke(index);

        public event Action<string, Ink.Runtime.Object> onUpdateInkVariable;
        public void UpdateInkVariable(string name, Ink.Runtime.Object value) => onUpdateInkVariable?.Invoke(name, value);
    }
}
