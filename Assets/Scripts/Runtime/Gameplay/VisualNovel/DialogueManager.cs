using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TheDates.Runtime.General;
using TheDates.Runtime.Quests;
using UnityEngine;


namespace TheDates.Runtime.Dialogue
{
    public class DialogueManager : BasicSingleton<DialogueManager>
    {
        [Header("Ink Scripting")]
        [SerializeField] private TextAsset inkJson;

        [field: SerializeField, ReadOnly] public bool isRunning { get; private set; }
        [field: SerializeField, ReadOnly] public int currentChoiceIndex { get; private set; } = -1;
        [field: SerializeField, ReadOnly] public string currentKnotName { get; private set; } = string.Empty;
        [field: SerializeField] public CharacterProfile[] characters { get; private set; } = Array.Empty<CharacterProfile>();
        
        private Story _story;
        private InkExternalFunctions _inkExternalFunctions;
        private InkDialogueVariables _inkDialogueVariables;
        private Dictionary<string, int> _characterDictionary;
        
        public CharacterData[] currentRoster { get; private set; }
        public int currentSpeakerIndex { get; private set; } = -1;

        private DialogueEvents dialogueEvents => GameEventsManager.Instance?.DialogueEvents;
        private static CharacterData _characterEmpty = new();
        private CharacterData _characterFallback;

        public struct CharacterData
        {
            public readonly CharacterProfile Profile;
            public string Name => Profile?.CharacterName ?? string.Empty;
            public int MoodIndex;
            public string Alias;

            public bool isEmpty => !Profile;
            
            public CharacterData(CharacterProfile profile) {
                Profile = profile;
                //Name = profile.CharacterName;
                MoodIndex = 0;
                Alias = string.Empty;
            }

            public Sprite GetCurrentPortrait() {
                return isEmpty ? null : Profile.GetPortrait(MoodIndex);
            }

            public string GetCurrentName() {
                return string.IsNullOrEmpty(Alias) ? Name : Alias;
            }

            public bool isValid => Profile;

        }


        protected override void Awake() {
            base.Awake();
            _story = new Story(inkJson.text);
            _inkExternalFunctions = new InkExternalFunctions();
            _inkExternalFunctions.Bind(_story);
            _inkDialogueVariables = new InkDialogueVariables(_story);
            
            _characterDictionary = new Dictionary<string, int>();
            for (var i = 0; i < characters.Length; i++) {
                _characterDictionary.TryAdd(characters[i].CharacterName, i);
                Debug.Log($"Character {characters[i].CharacterName} has been added to Story at position {i}");
            }
            
            currentRoster = new CharacterData[2];
            _characterFallback = characters.IsNullOrEmpty() ? _characterEmpty : new CharacterData(characters[0]);

        }

        private void OnDestroy() {
            _inkExternalFunctions.Unbind(_story);
        }

        private void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.BindManager(this);
            dialogueEvents.onEnterDialogue += EnterDialogue;
            dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
            dialogueEvents.onUpdateInkVariable += UpdateInkVariable;
            //dialogueEvents.onSetSpeaker += SetCharacterAt;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange += QuestStateChange;
        }

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            dialogueEvents.UnbindManager(this);
            dialogueEvents.onEnterDialogue -= EnterDialogue;
            dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
            dialogueEvents.onUpdateInkVariable -= UpdateInkVariable;
            //dialogueEvents.onSetSpeaker -= SetCharacterAt;
            GameEventsManager.Instance.QuestEvents.onQuestStateChange -= QuestStateChange;
        }

        public void SetCharacterAt(string characterName, int positionIndex) {
            //Debug.Log($"SetCharacterAt: {characterName} at position {positionIndex}");
            if (characterName == string.Empty) {
                currentRoster[positionIndex] = _characterEmpty;
                return;
            }
            
            if (!currentRoster.IsWithinBounds(positionIndex)) return;
            //currentRoster[positionIndex].Profile != characters[profileIndex]
            currentRoster[positionIndex] = !_characterDictionary.TryGetValue(characterName, out var profileIndex) 
                ? _characterFallback
                : currentRoster[positionIndex].Profile != characters[profileIndex] 
                    ? new CharacterData(characters[profileIndex]) 
                    : currentRoster[positionIndex];
        }
        
        public void SetPortraitAt(int positionIndex, int moodIndex) {
            if (currentRoster[positionIndex].isEmpty) return;
            
            if (!currentRoster.IsWithinBounds(positionIndex)) return;
            //currentRoster[positionIndex].MoodIndex == moodIndex
            currentRoster[positionIndex].MoodIndex = moodIndex;
            //currentCharacters[index] = character;
        }

        public void SetCurrentSpeaker(int positionIndex) {
            //Debug.Log($"SetSpeaker: at position {positionIndex}");
            if (!currentRoster.IsWithinBounds(positionIndex) && positionIndex != -1) return; //|| currentSpeakerIndex == positionIndex
            currentSpeakerIndex = positionIndex;
        }
        
        public void SetSpeakerAlias(int positionIndex, string alias) {
            //Debug.Log($"SetSpeaker: at position {positionIndex}");
            if (!currentRoster.IsWithinBounds(positionIndex)) return; //|| currentSpeakerIndex == positionIndex
            currentRoster[positionIndex].Alias = string.IsNullOrEmpty(alias) ? string.Empty : alias;
        }

        //public CharacterProfile FindCharacter(string characterName) {
        //    return _characterDictionary.TryGetValue(characterName, out var index) ? characters[index] : null;
        //}
        
        //public CharacterProfile GetCharacterAt(int index) {
        //    return currentRoster.IsWithinBounds(index) ? currentRoster[index].Profile : null;
        //}
        
        //public void UpdateCharacterPortrait()

        private void QuestStateChange(Quest quest) {
            GameEventsManager.Instance.DialogueEvents.UpdateInkVariable(quest.namedID + "State", new StringValue(quest.state.ToString())
                
                );
        }

        private void UpdateInkVariable(string variableName, Ink.Runtime.Object value) {
            _inkDialogueVariables.UpdateVariableState(variableName, value);
        }

        private void UpdateChoiceIndex(int choiceIndex) {
            currentChoiceIndex = choiceIndex;
            //ProcessDialogue(); // We can change this later?
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
                // Default to 'none'
                //currentRoster[0] = _characterEmpty;
                //currentRoster[1] = _characterEmpty;
                SetCharacterAt(string.Empty, 0);
                SetCharacterAt(string.Empty, 1);
                SetCurrentSpeaker(0);
                GameEventsManager.Instance.DialogueEvents.DialogueStarted();
            }
            else {
                Debug.LogWarning("Dialogue Knot was empty and cannot be entered.");
            }
            _inkDialogueVariables.StartListening(_story);
            RunStory();
            Debug.Log($"Entering Dialogue: {knotName}");
        }

        private void RunStory() {
            //Debug.Log($"Roster: {currentRoster[0].Name} is {currentRoster[0].isEmpty} and {currentRoster[1].Name} is {currentRoster[1].isEmpty}");
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
            Debug.Log("Dialogue Knot has been exited.");
            
            isRunning = false;
            currentKnotName = string.Empty;
            // Default to 'none'
            SetCharacterAt(string.Empty, 0);
            SetCharacterAt(string.Empty, 1);
            SetCurrentSpeaker(0);
            GameEventsManager.Instance.DialogueEvents.DialogueFinished();
            
            _story.ResetState();
        }
        
        private void ExitDialogue2() {
            Debug.Log("Dialogue Knot has been exited.");
            isRunning = false;
            _inkDialogueVariables.StopListening(_story);
            _story.ResetState();
        }

        private bool IsLineEmpty(string dialogueLine) => dialogueLine.Trim().Equals(string.Empty) || dialogueLine.Trim().Equals("\n");
    }
}
