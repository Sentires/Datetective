using System;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace TheDates.Runtime.UI
{
    public class DialoguePanelUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject contentParent;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private DialogueChoiceButton[] choicesButtons;

        private void Awake() {
            contentParent.SetActive(false);
            ResetPanel();
        }

        private void OnEnable() {
            GameEventsManager.Instance.DialogueEvents.onDialogueStarted += DialogueStarted;
            GameEventsManager.Instance.DialogueEvents.onDialogueFinished += DialogueFinished;
            GameEventsManager.Instance.DialogueEvents.onDialogueDisplay += DisplayDialogue;
        }

        private void OnDisable() {
            GameEventsManager.Instance.DialogueEvents.onDialogueStarted -= DialogueStarted;
            GameEventsManager.Instance.DialogueEvents.onDialogueFinished -= DialogueFinished;
            GameEventsManager.Instance.DialogueEvents.onDialogueDisplay -= DisplayDialogue;
        }

        private void DialogueStarted() {
            contentParent.SetActive(true);
        }

        private void DialogueFinished() {
            contentParent.SetActive(false);
            ResetPanel();
        }

        private void DisplayDialogue(string dialogueLine, List<Choice> choices) {
            dialogueText.text = dialogueLine;

            if (choices.Count > choicesButtons.Length) {
                Debug.LogWarning("Too many choices on the dialogue line");
            }

            foreach (var choiceButton in choicesButtons) {
                choiceButton.gameObject.SetActive(false);
            }

            var choiceButtonIndex = choices.Count - 1;
            for (var inkIndex = 0; inkIndex < choices.Count; inkIndex++) {
                var dialogueChoice = choices[inkIndex];
                var choiceButton = choicesButtons[choiceButtonIndex];
                
                choiceButton.gameObject.SetActive(true);
                choiceButton.SetChoiceText(dialogueChoice.text);
                choiceButton.SetChoiceIndex(inkIndex);

                if (inkIndex == 0) {
                    choiceButton.SelectButton();
                    GameEventsManager.Instance.DialogueEvents.UpdateChoiceIndex(0);
                }
                
                choiceButtonIndex--;
            }
        }

        private void ResetPanel() {
            dialogueText.text = string.Empty;
        }
        
        
    }
}
