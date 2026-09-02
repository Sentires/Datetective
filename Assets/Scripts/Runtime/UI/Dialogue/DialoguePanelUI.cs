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
            //dialogueEvents.onSetSpeaker += SetActiveSpeaker;
            //dialogueEvents.onAdjustSpeaker += SetPortraitVariant;
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.onDialogueStarted -= DialogueStarted;
            dialogueEvents.onDialogueFinished -= DialogueFinished;
            dialogueEvents.onDialogueDisplay -= DisplayDialogue;
            dialogueEvents.onUpdateChoiceIndex -= OnChoiceSelect;
            //dialogueEvents.onSetSpeaker -= SetActiveSpeaker;
            //dialogueEvents.onAdjustSpeaker -= SetPortraitVariant;
        }
        
        private void SetActiveSpeaker(CharacterProfile profile, int index) {
            portraitSecondary.sprite = profile?.GetPortrait(1);
            //Debug.Log($"Active portrait {0} for {profile?.name}");
        }
        
        private void SetPortraitVariant(CharacterProfile profile, int mood, int variant) {
            portraitSecondary.sprite = profile?.GetPortrait(mood);
            //Debug.Log($"Active portrait {mood} for {profile?.name}");
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
            
            if (manager.currentSpeakerIndex == 0) {
                dialogueTextPrimary.text = dialogueLine;
                dialogueTextSecondary.text = string.Empty;
                dialogueBoxPrimary.SetActive(true);
                dialogueBoxSecondary.SetActive(false);
            }
            else {
                dialogueTextSecondary.text = dialogueLine;
                dialogueTextPrimary.text = string.Empty;
                dialogueBoxSecondary.SetActive(true);
                dialogueBoxPrimary.SetActive(false);
            }
            
            var primarySpeaker = manager.currentRoster[0];
            var secondarySpeaker = manager.currentRoster[1];

            labelPrimary.text = primarySpeaker.Name;
            labelSecondary.text = secondarySpeaker.Name;
            portraitPrimary.sprite = primarySpeaker.Profile.GetPortrait(primarySpeaker.MoodIndex);
            portraitSecondary.sprite = secondarySpeaker.Profile.GetPortrait(secondarySpeaker.MoodIndex);
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
        }
        
        
    }
}
