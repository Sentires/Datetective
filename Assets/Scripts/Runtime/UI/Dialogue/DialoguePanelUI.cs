using System;
using System.Collections.Generic;
using Ink.Runtime;
using TheDates.Runtime.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheDates.Runtime.UI
{
    public class DialoguePanelUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject contentParent;
        [SerializeField] private GameObject dialogueBoxPrimary;
        [SerializeField] private GameObject dialogueBoxSecondary;
        [SerializeField] private TextMeshProUGUI dialogueTextPrimary;
        [SerializeField] private TextMeshProUGUI dialogueTextSecondary;
        [FormerlySerializedAs("speakerPrimary")] [SerializeField] private TextMeshProUGUI labelPrimary;
        [FormerlySerializedAs("speakerSecondary")] [SerializeField] private TextMeshProUGUI labelSecondary;
        [SerializeField] private Image portraitPrimary;
        [SerializeField] private Image portraitSecondary;
        [SerializeField] private DialogueChoiceButton[] choicesButtons;

        private int _currentWindowIndex;
        
        
        private DialogueEvents dialogueEvents => GameEventsManager.Instance?.DialogueEvents;

        private void Awake() {
            contentParent.SetActive(false);
            ResetPanel();
        }

        private void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.onDialogueStarted += DialogueStarted;
            dialogueEvents.onDialogueFinished += DialogueFinished;
            dialogueEvents.onDialogueDisplay += DisplayDialogue;
            dialogueEvents.onUpdateChoiceIndex += OnChoiceSelect;
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.onDialogueStarted -= DialogueStarted;
            dialogueEvents.onDialogueFinished -= DialogueFinished;
            dialogueEvents.onDialogueDisplay -= DisplayDialogue;
            dialogueEvents.onUpdateChoiceIndex -= OnChoiceSelect;
        }

        private void DialogueStarted() {
            contentParent.SetActive(true);
        }

        private void DialogueFinished() {
            contentParent.SetActive(false);
            ResetPanel();
        }

        private void OnChoiceSelect(int index) {
            foreach (var button in choicesButtons) {
                button.SetSelectionPointer(index == button.choiceIndex);
            }
        }

        private void TempAdjustUI(string dialogueLine)
        {
            var manager = dialogueEvents.currentManager;
            var primarySpeaker = manager.currentRoster[0];
            var secondarySpeaker = manager.currentRoster[1];
            
            portraitPrimary.sprite = primarySpeaker.GetCurrentPortrait();
            portraitSecondary.sprite = secondarySpeaker.GetCurrentPortrait();
            portraitPrimary.gameObject.SetActive(!primarySpeaker.isEmpty);
            portraitSecondary.gameObject.SetActive(!secondarySpeaker.isEmpty);
            
            switch (manager.currentSpeakerIndex) {
                case 0:
                    dialogueTextPrimary.text = dialogueLine;
                    //dialogueTextSecondary.text = string.Empty;
                    labelPrimary.text = primarySpeaker.GetCurrentName();
                    dialogueBoxPrimary.SetActive(true);
                    dialogueBoxSecondary.SetActive(false);
                    break;
                case 1:
                    dialogueTextSecondary.text = dialogueLine;
                    //dialogueTextPrimary.text = string.Empty;
                    labelSecondary.text = secondarySpeaker.GetCurrentName();
                    dialogueBoxSecondary.SetActive(true);
                    dialogueBoxPrimary.SetActive(false);
                    break;
                default: // 'no direct speaker'
                    dialogueTextPrimary.text = dialogueLine;
                    labelPrimary.text = string.Empty;
                    dialogueBoxPrimary.SetActive(true);
                    dialogueBoxSecondary.SetActive(false);
                    break;
            }
            
            
            
            
        }

        //private void UpdateDialogueBox(bool toggle, string text) {
        //    
        //}

        private void DisplayDialogue(string dialogueLine, List<Choice> choices) {
            TempAdjustUI(dialogueLine);
            //dialogueTextPrimary.text = dialogueLine;

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
                choiceButton.SetSelectionPointer(false);

                if (inkIndex == 0) {
                    choiceButton.SelectButton();
                    GameEventsManager.Instance.DialogueEvents.UpdateChoiceIndex(0);
                }
                
                choiceButtonIndex--;
            }
        }

        private void ResetPanel() {
            dialogueTextPrimary.text = string.Empty;
            dialogueTextSecondary.text = string.Empty;
            labelPrimary.text = string.Empty;
            labelSecondary.text = string.Empty;
            portraitPrimary.sprite = null;
            portraitSecondary.sprite = null;
        }
        
        
    }
}
