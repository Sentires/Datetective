

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheDates.Runtime;
using TheDates.Runtime.Dialogue;
using TheDates.Runtime.Experimental;
using TheDates.Runtime.Experimental.MinigameCore;
using TheDates.Runtime.General;
using TheDates.Runtime.Input;
using TheDates.Runtime.PlayerCore;
using TheDates.Runtime.Quests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static TheDates.WhoopsieDaisies;
using Object = UnityEngine.Object;

namespace TheDates
{
    public class WhoopsieDaisies : MonoBehaviour
    {
        // Serialised Private
        public Test<Transform> thang = new Test<Transform>();
        [SerializeField] private PlayerController player;
        [SerializeField] private List<PlayerWarp> playerWarps = new();
        [SerializeField] private List<QuestObject> questObjects = new();
        [SerializeField] private List<QuestInteractionObserver> interactionObservers = new();

        [SerializeField] private MiniGameAssetReference puzzleOfficePaper;
        [SerializeField] private MiniGameAssetReference puzzleStorageMaze;
        [SerializeField] private MiniGameAssetReference puzzleAquariumSliding;
        [SerializeField] private MiniGameAssetReference puzzleOfficeEvidence;
        // Private
        private readonly Dictionary<string, List<QuestObject>> _questObjectDict = new();
        private readonly Dictionary<string, List<QuestInteractionObserver>> _questInteractionDict = new();
        
        
        // Other
        private static readonly string[] QuestKeysArray = {
            "CaseFile", "CorkBoard", "PC", "Bin", "Cabinet", "ExitDoor", 
            "Clown", "BakeryFront", "AquariumFront",
            "Counter", "Shelf", "Table", "BakerInitial",
            "Fence", "Breaker", "Dumpster", "CardboardBox", "BakeryAlley", "AquariumAlley",
            "Locker", "Flour", "MazePuzzle",
            "Corpse", "PoliceTape", "FishTank", "Stage", "ClownCostume", "WetFootprints",
            "BoxCutter", "Landlord", "YachtClubFront", "CafeFront", "BriarDate"
        };
        // Discards
        //, "LostAndFound"

        private HashSet<string> _questKeys;
        private bool _dialogueActive;
        private bool _puzzleActive => miniGameManager.isPuzzleActive;
        private static InputEvents inputEvents => GameEventsManager.Instance?.InputEvents;
        private static DialogueEvents dialogueEvents => GameEventsManager.Instance?.DialogueEvents;
        private static QuestEvents questEvents => GameEventsManager.Instance?.QuestEvents;
        private static MiniGameManager miniGameManager => MiniGameManager.Instance;
        
        private void Awake() {
            _questKeys = new HashSet<string>(QuestKeysArray);

            player.AssertNull(this, name);
            puzzleOfficePaper.AssertNull(this, name);
            puzzleStorageMaze.AssertNull(this, name);
            puzzleAquariumSliding.AssertNull(this, name);
            puzzleOfficeEvidence.AssertNull(this, name);

            //foreach (var obs in interactionObservers) {
            //    
            //}
            
            foreach (var qO in questObjects) {
                if (!qO.questObject) {
                    Debug.LogWarning("Quest object not found for " + qO.reference);
                    continue;
                }
                
                if (!_questObjectDict.TryGetValue(qO.reference, out var collection)) {
                    collection = new List<QuestObject>();
                    _questObjectDict.Add(qO.reference, collection);
                }
                qO.Init();
                collection.Add(qO);
            }
            
            foreach (var iO in interactionObservers) {
                if (!_questInteractionDict.TryGetValue(iO.questKey, out var collection)) {
                    collection = new List<QuestInteractionObserver>();
                    _questInteractionDict.Add(iO.questKey, collection);
                }
                iO.Init(TriggerUpdate);
                collection.Add(iO);
            }
            
            miniGameManager.OnMiniGameProcessed += MiniGameProcess;
            inputEvents.onInteract += InteractionInput;
            dialogueEvents.onDialogueStarted += DialogueStarted;
            dialogueEvents.onDialogueFinished += DialogueEnded;

        }
        
        /*
        public void OnEnable() {
            if (MiniGameManager.HasInstance) miniGameManager.OnMiniGameProcessed += MiniGameProcess;
            if (GameEventsManager.HasInstance) {
                inputEvents.onInteract += InteractionInput;
                dialogueEvents.onDialogueStarted += DialogueStarted;
                dialogueEvents.onDialogueFinished += DialogueEnded;
            }
        }

        public void OnDisable() {
            if (MiniGameManager.HasInstance) miniGameManager.OnMiniGameProcessed -= MiniGameProcess;
            if (GameEventsManager.HasInstance) {
                inputEvents.onInteract -= InteractionInput;
                dialogueEvents.onDialogueStarted -= DialogueStarted;
                dialogueEvents.onDialogueFinished -= DialogueEnded;
            }
        }*/
        
        void Start() {
            
        }
        
        private void DialogueStarted() => _dialogueActive = true;

        private void DialogueEnded() => _dialogueActive = false;

        private void InteractionInput(bool input) {
            if (!input) return;
            if (_dialogueActive) {
                dialogueEvents.currentManager.ProcessDialogue();
            }
        }

        private IEnumerator AwaitDialogue(Action callback) {
            //if (hasCooldown) yield break;
                
            while (_dialogueActive) {
                yield return null; // Skip next frame
            }
            
            callback?.Invoke();
        }

        
        private void MiniGameProcess(MiniGameContext context) {
            if (context.State == MiniGameState.Completed)
            {
                if (context.Source.Equals(puzzleOfficePaper.miniGamePrefab)) {
                    puzzleWinOfficePaper = true;
                }
                else if (context.Source.Equals(puzzleStorageMaze.miniGamePrefab)) {
                    puzzleWinStorageMaze = true;
                }
                else if (context.Source.Equals(puzzleAquariumSliding.miniGamePrefab)) {
                    puzzleWinAquariumSliding = true;
                }
                else if (context.Source.Equals(puzzleOfficeEvidence.miniGamePrefab)) {
                    puzzleWinOfficeEvidence = true;
                }
            }
            
            //Debug.Log(context.Source + context.State.ToString());
        }

        public void TriggerDialogue(string knotName) {
            if (!GameEventsManager.HasInstance || string.IsNullOrEmpty(knotName)) return;

            if (_dialogueActive) return;
            dialogueEvents.EnterDialogue(knotName);
            //return;
            //dialogueEvents.currentManager.ProcessDialogue();
        }

        private void ToggleObservers(bool toggle, params string[] keys) {
            foreach (var key in keys) {
                foreach (var o in _questInteractionDict[key]) {
                    o.ToggleObserver(toggle);
                }
            }
            
        }
        private void ToggleObjects(bool toggle, params string[] keys) {
            foreach (var key in keys) {
                foreach (var o in _questObjectDict[key]) {
                    o.SetActive(toggle);
                }
            }
        }
        
        public void TriggerUpdate(string reference) {
            if (_puzzleActive || _dialogueActive) return;
            
            
            if (!_questKeys.Contains(reference)) {
                Debug.LogWarning("Quest key '" + reference + "' doesn't exist in the static HashSet");
                return;
            }
            Action action = delegate { };
            // I formally apologise to anybody who opened this file lmao
            switch (reference) {
                #region { Office }
                case "CaseFile": {
                    TriggerDialogue("Case_file");
                    checkCaseFile = true;
                    break;
                }
                case "CorkBoard": {
                    // TODO - trigger puzzle
                    break;
                }
                case "PC": {
                    if (puzzleWinOfficePaper) {
                        TriggerDialogue("PC_Unlocked");
                        checkPCUnlocked = true;
                        ToggleObservers(false, reference);
                    }
                    else TriggerDialogue("PC_Locked");
                    break;
                }
                case "Bin": {
                    if (checkCaseFile) {
                        TriggerDialogue("Bin_After_Files");
                        
                        action += () => miniGameManager.SendCommandStart(puzzleOfficePaper);
                        action += () => ToggleObservers(false, reference, "Cabinet", "CaseFile");
                        StartCoroutine(AwaitDialogue(action));
                    }
                    else TriggerDialogue("Bin_No_Files");
                    break;
                }
                case "Cabinet": {
                    TriggerDialogue("Filing_cabinet");
                    break;
                }
                case "ExitDoor": {
                    if (checkPCUnlocked) {
                        TriggerDialogue("Door_Complete");
                        ToggleObjects(true, reference);
                        ToggleObservers(false, reference);
                        // TODO - enable door
                    }
                    else TriggerDialogue("Door_No_Goal");
                    break;
                }
                #endregion
                #region { Lafayette }
                case "BakeryFront": {
                    TriggerDialogue("Bakery_Front");
                    //ToggleObservers(false, reference);
                    break;
                }
                case "AquariumFront": {
                    TriggerDialogue("Aquarium_Front");
                    //ToggleObservers(false, reference);
                    break;
                }
                case "Clown": {
                    if (fetchClownOutfit && meetClown) {
                        if (!checkClownRequest) {
                            checkClownRequest = true;
                            TriggerDialogue("Jestar_Uniform");
                        }
                        else {
                            TriggerDialogue("Jestar_Ask");
                        }
                    }
                    else {
                        meetClown = true;
                        TriggerDialogue("Jestar_Meet");
                    }
                    break;
                }
                #endregion
                #region { Bakery }
                case "Counter": {
                    if (!meetBaker) {
                        checkLostAndFound = true;
                        TriggerDialogue("Bakery_Box_Briar");
                    }
                    else {
                        TriggerDialogue(checkLostAndFound ? "Bakery_Box_Alone" : "Bakery_Box_Alone_New");
                    }
                    break;
                }
                case "Shelf": {
                    TriggerDialogue("Bakery_Cabinet");
                    break;
                }
                case "Table": {
                    TriggerDialogue("Bakery_Table");
                    // TODO - spawn order form?
                    break;
                }
                case "BakerInitial": {
                    // date segment, a lot of prereq
                    //if (meetLandlord && fetchGateKey && meetBaker && checkFixBreaker) {
                    //    TriggerDialogue("Briar_Date");
                    //}
                    meetBaker = true;
                    TriggerDialogue("Briar_Meet");
                    ToggleObservers(false, "BakerInitial");
                    ToggleObjects(false, "BakerInitial");
                    break;
                }
                #endregion
                #region { Alley }
                case "BakeryAlley": {
                    TriggerDialogue("Bakery_Backdoor"); // Need to enable elsewhere
                    break;
                }
                case "AquariumAlley": {
                    if (puzzleStorageMaze) {
                        ToggleObservers(false, "AquariumAlley");
                        ToggleObjects(true, "AquariumAlley");
                    }
                    else {
                        TriggerDialogue("Aquarium_Backdoor");
                        action += () => miniGameManager.SendCommandStart(puzzleStorageMaze);
                        StartCoroutine(AwaitDialogue(action));
                    }
                    
                    break;
                }
                case "Fence": {
                    if (fetchGateKey) {
                        TriggerDialogue("Fence_Unlocked");
                        ToggleObservers(false, reference);
                        ToggleObjects(false, reference);
                        
                        ToggleObservers(true, "Breaker");
                    }
                    else {
                        TriggerDialogue("Fence_Locked");
                        checkFence = true;
                    }
                    break;
                }
                case "Breaker": {
                    ToggleObservers(false, reference);
                    ToggleObjects(false, reference);
                    checkFixBreaker = true;
                    
                    //ToggleObservers(true, "AquariumAlley");
                    break;
                }
                case "Dumpster": {
                    if (puzzleWinAquariumSliding) {
                        TriggerDialogue("Rubbish");
                        ToggleObservers(false, reference);
                    }
                    else {
                        miniGameManager.SendCommandStart(puzzleAquariumSliding);
                    }
                    break;
                }
                case "CardboardBox": {
                    TriggerDialogue("Delivery_Box");
                    fetchBox = true;
                    break;
                }
                #endregion
                #region { Storage }
                case "Locker": {
                    // idk what goes here rn
                    break;
                }
                case "Flour": {
                    TriggerDialogue("Flour_Box");
                    // TODO - fetch
                    break;
                }
                case "MazePuzzle": {
                    // TODO - trigger puzzle
                    break;
                }
                #endregion
                #region { Androne }
                case "BoxCutter": {
                    TriggerDialogue("Box_Cutter");
                    fetchBoxCutter = true;
                    // TODO - hide the object after
                    break;
                }
                
                case "YachtClubFront": {
                    TriggerDialogue("Yacht_Club_Front");
                    break;
                }
                case "CafeFront": {
                    TriggerDialogue("Cafe");
                    break;
                }
                case "Landlord": {
                    if (meetLandlord) {
                        if (checkFence && !fetchGateKey) {
                            TriggerDialogue("Dakota_Key");
                            fetchGateKey = true;
                        }
                        else TriggerDialogue("Dakota_Ask");
                    }
                    else {
                        TriggerDialogue("Dakota_Meet");
                        meetLandlord = true;
                        if (checkFence && !fetchGateKey) {
                            action += () => TriggerDialogue("Dakota_Key");
                            action += () => fetchGateKey = true;
                            StartCoroutine(AwaitDialogue(action));
                        }
                        
                    }
                    
                    break;
                }
                case "BriarDate": { // this one will need heavy testing
                    TriggerDialogue("Briar_Date");
                    checkDateSegment = true;
                    break;
                }

                #endregion
                #region { Crime }
                case "Corpse": {
                    TriggerDialogue("Corpse");
                    checkCorpse = true;
                    break;
                }
                case "PoliceTape": {
                    TriggerDialogue("Caution_Tape");
                    break;
                }
                case "FishTank": {
                    TriggerDialogue("Fish_Tank");
                    break;
                }
                #endregion
                #region { Lobby }
                case "Stage": {
                    TriggerDialogue("Stage");
                    break;
                }
                case "ClownCostume": {
                    TriggerDialogue("Clown_Costume");
                    fetchClownOutfit = true;
                    break;
                }
                case "WetFootprints": {
                    TriggerDialogue("Wet_Footprints");
                    // TODO - fetch?
                    break;
                }
                #endregion
                
                
                
                default:
                    Debug.LogWarning("Tried using undefined key: " + reference);
                    break;
            }
        }
        
        
        
        // Quest vars
        [Header("Quest Values")]
        [SerializeField, ReadOnly] private bool checkDesk;
        [SerializeField, ReadOnly] private bool checkCaseFile;
        [SerializeField, ReadOnly] private bool checkPCUnlocked;
        [SerializeField, ReadOnly] private bool checkLostAndFound;
        [SerializeField, ReadOnly] private bool checkFence;
        [SerializeField, ReadOnly] private bool checkClownRequest;
        [SerializeField, ReadOnly] private bool checkOutage;
        [SerializeField, ReadOnly] private bool checkCorpse;
        [SerializeField, ReadOnly] private bool checkDateSegment;
        [SerializeField, ReadOnly] private bool checkFixBreaker;
        
        [SerializeField, ReadOnly] private bool meetClown;
        [SerializeField, ReadOnly] private bool meetBaker;
        [SerializeField, ReadOnly] private bool meetLandlord;
        
        
        [SerializeField, ReadOnly] private bool fetchClownOutfit;
        [SerializeField, ReadOnly] private bool fetchGateKey;
        [SerializeField, ReadOnly] private bool fetchBox;
        [SerializeField, ReadOnly] private bool fetchBoxCutter;
        
        [SerializeField, ReadOnly] private bool puzzleWinOfficePaper;
        [SerializeField, ReadOnly] private bool puzzleWinStorageMaze;
        [SerializeField, ReadOnly] private bool puzzleWinAquariumSliding;
        [SerializeField, ReadOnly] private bool puzzleWinOfficeEvidence;
        
        [Serializable]
        public class QuestObject {
            public GameObject questObject;
            //[StringDropdown(nameof(QuestKeysArray))]
            public string reference;
            [SerializeField] private bool activeOnInit = true;
            [SerializeField] private bool invertActivity;

            public void Init() {
                SetActive(activeOnInit);
            }

            public void SetActive(bool toggle) => questObject.SetActive(invertActivity ? !toggle : toggle);
        }
        
        // Shoulda done smth like this for QuestObject, maybe later
        [Serializable]
        public class Test<T> where T : Component {
            public GameObject questObject => questComponent.gameObject;
            public T questComponent;
            
            public string reference;
            
            [SerializeField] private bool activeOnInit = true;
            [SerializeField] private bool invertActivity;
            
            public void Init() {
                questComponent.AssertNull(this, nameof(Test<T>));
                SetActive(activeOnInit);
            }

            public void SetActive(bool toggle) => questObject.SetActive(invertActivity ? !toggle : toggle);

            public static Test<T> Convert(QuestObject old) {
                var result = new Test<T>();
                result.questComponent = old.questObject.GetComponent<T>();
                result.reference = old.reference;
                return result;
            }
        }
        
        [Serializable]
        public struct QuestKey : IEquatable<QuestKey>, IComparable<QuestKey>, IEquatable<string>, IComparable<string> {
            [StringDropdown(nameof(QuestKeysArray))]
            public string reference;
            
            // At this point I just went on a tangent with all this cuz I'm bored at 1 am
            // (for funsies)
            public static implicit operator string(QuestKey key) => key.reference;
            public static explicit operator QuestKey(string key) => new(){reference = key};
            
            public static bool operator ==(QuestKey left, QuestKey right) => left.Equals(right);
            public static bool operator !=(QuestKey left, QuestKey right) => !left.Equals(right);
            public override bool Equals(object obj) => obj is string other && Equals(other);
            public override int GetHashCode() => (reference != null ? reference.GetHashCode() : 0);
            
            public bool Equals(QuestKey other) => reference == other.reference;
            public bool Equals(string other) => reference == other;
            
            
            public int CompareTo(QuestKey other) => string.Compare(reference, other.reference, StringComparison.Ordinal);
            public int CompareTo(string other) => string.Compare(reference, other, StringComparison.Ordinal);
        }
        
        [Serializable]
        public class QuestInteractionObserver {
            [SerializeField] private InteractionObserver observer;
            [field: SerializeField] public string questKey {get; private set;} //, StringDropdown(nameof(QuestKeysArray))
            [SerializeField] private bool activeOnInit = true;
            [SerializeField] private bool invertActivity;
            
            private Action<string> _onTriggered;

            public void Init(Action<string> action) {
                if (!observer.AssertNull(this, "observer thang")) return;
                
                _onTriggered = action;
                observer.onInteract.AddListener(Trigger);
                
                ToggleObserver(activeOnInit);
            }

            public void ToggleObserver(bool toggle) => observer.gameObject.SetActive(invertActivity ? !toggle : toggle);

            private void Trigger(bool value) {
                if (!value || _onTriggered == null) return;
                _onTriggered(questKey);
            }
        }
    }
}
