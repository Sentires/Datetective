using System.Collections;
using Ink.Runtime;
using TheDates.Runtime.General;
using TheDates.Runtime.Quests;
using UnityEngine;


namespace TheDates.Runtime.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Ink Scripting")]
        [SerializeField] private TextAsset inkJson;

        [field: SerializeField, ReadOnly] public bool isRunning { get; private set; }
        [field: SerializeField, ReadOnly] public int currentChoiceIndex { get; private set; } = -1;
        [field: SerializeField, ReadOnly] public string currentKnotName { get; private set; } = string.Empty;
        
        private Story _story;
        private InkExternalFunctions _inkExternalFunctions;
        private InkDialogueVariables _inkDialogueVariables;


        private void Awake() {
            _story = new Story(inkJson.text);
            _inkExternalFunctions = new InkExternalFunctions();
            _inkExternalFunctions.Bind(_story);
            _inkDialogueVariables = new InkDialogueVariables(_story);
        }

        private void OnDestroy() {
            _inkExternalFunctions.Unbind(_story);
        }

        private void OnEnable() {
            if (!GameEventsManager.HasInstance()) return;
            GameEventsManager.Instance.DialogueEvents.BindManager(this);
            GameEventsManager.Instance.DialogueEvents.onEnterDialogue += EnterDialogue;
            GameEventsManager.Instance.DialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
            GameEventsManager.Instance.DialogueEvents.onUpdateInkVariable += UpdateInkVariable;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange += QuestStateChange;
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance()) return;
            GameEventsManager.Instance.DialogueEvents.UnbindManager(this);
            GameEventsManager.Instance.DialogueEvents.onEnterDialogue -= EnterDialogue;
            GameEventsManager.Instance.DialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
            GameEventsManager.Instance.DialogueEvents.onUpdateInkVariable -= UpdateInkVariable;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange -= QuestStateChange;
        }

        private void QuestStateChange(Quest quest) {
            GameEventsManager.Instance.DialogueEvents.UpdateInkVariable(quest.namedID + "State", new StringValue(quest.state.ToString())
                
                );
        }

        private void UpdateInkVariable(string variableName, Ink.Runtime.Object value) {
            _inkDialogueVariables.UpdateVariableState(variableName, value);
        }

        private void UpdateChoiceIndex(int choiceIndex) {
            currentChoiceIndex = choiceIndex;
        }

        public void ProcessDialogue() {
            if (!isRunning) return;
            RunStory();
        }

        private void EnterDialogue(string knotName) {
            if (isRunning) return;
            
            if (!string.IsNullOrEmpty(knotName)) {
                isRunning = true;
                currentKnotName = knotName;
                _story.ChoosePathString(knotName);
                GameEventsManager.Instance.DialogueEvents.DialogueStarted();
            }
            else {
                Debug.LogWarning("Dialogue Knot was empty and cannot be entered.");
            }
            _inkDialogueVariables.StartListening(_story);
            RunStory();
            //Debug.Log($"Entering Dialogue: {dialogueKnot}");
        }

        private void RunStory() {
            if (_story.currentChoices.Count > 0 && currentChoiceIndex != -1)
            {
                _story.ChooseChoiceIndex(currentChoiceIndex);
                // Reset it for next time
                currentChoiceIndex = -1;
            }
            
            if (_story.canContinue) {
                var dialogueLine = _story.Continue();

                while (IsLineEmpty(dialogueLine) && _story.canContinue) {
                    dialogueLine = _story.Continue();
                }

                if (IsLineEmpty(dialogueLine) && !_story.canContinue) {
                    StartCoroutine(ExitDialogue());
                    return;
                }
                
                GameEventsManager.Instance.DialogueEvents.DisplayDialogue(dialogueLine, _story.currentChoices);
            }
            else if (_story.currentChoices.Count == 0)
            {
                StartCoroutine(ExitDialogue());
            }
        }
        
        private IEnumerator ExitDialogue() {
            yield return null;
            //Debug.Log("Dialogue Knot has been exited.");
            
            isRunning = false;
            currentKnotName = string.Empty;
            GameEventsManager.Instance.DialogueEvents.DialogueFinished();
            
            _story.ResetState();
        }
        
        private void ExitDialogue2() {
            //Debug.Log("Dialogue Knot has been exited.");
            isRunning = false;
            _inkDialogueVariables.StopListening(_story);
            _story.ResetState();
        }

        private bool IsLineEmpty(string dialogueLine) => dialogueLine.Trim().Equals(string.Empty) || dialogueLine.Trim().Equals("\n");
    }
}
