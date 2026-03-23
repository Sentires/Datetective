using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace TheDates.Runtime.Dialogue
{
    public class InkDialogueVariables
    {
        private Dictionary<string, Ink.Runtime.Object> _variables;

        public InkDialogueVariables(Story story)
        {
            _variables = new Dictionary<string, Ink.Runtime.Object>();
            foreach (var name in story.variablesState)
            {
                var value = story.variablesState.GetVariableWithName(name);
                _variables.Add(name, value);
                Debug.Log($"Initialised global Ink variable: {name} = {value}");
            }
        }

        public void StartListening(Story story)
        {
            SyncToStory(story);
            story.variablesState.variableChangedEvent += UpdateVariableState;
        }

        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= UpdateVariableState;
        }

        public void UpdateVariableState(string name, Ink.Runtime.Object value)
        {
            if (!_variables.ContainsKey(name)) return;
            _variables[name] = value;
            Debug.Log($"Updating global variable: {name} = {value}");
        }

        private void SyncToStory(Story story)
        {
            foreach (var variable in _variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }
    }
}
