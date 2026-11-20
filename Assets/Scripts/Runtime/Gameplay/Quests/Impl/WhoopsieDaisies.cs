

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
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static TheDates.WhoopsieDaisies;
using Object = UnityEngine.Object;

namespace TheDates
{
    public class WhoopsieDaisies : MonoBehaviour
    {
        // 0 = juno spawn, 1 = storage from alley, 2 = alley from storage
        
        // Serialised Private
        //public Test<Transform> thang = new Test<Transform>();
        [SerializeField] private TextMeshProUGUI helpText;
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform[] playerWarps; // 0 will be spawnpoint
        [SerializeField] private GameObject winScreen;
        [SerializeField] private List<QuestObject> questObjects = new();
        [SerializeField] private List<QuestInteractionObserver> interactionObservers = new();

        [SerializeField] private MiniGameAssetReference puzzleOfficePaper;
        [SerializeField] private MiniGameAssetReference puzzleStorageMaze;
        [SerializeField] private MiniGameAssetReference puzzleAquariumSliding;
        [SerializeField] private MiniGameAssetReference puzzleOfficeEvidence;
        // Private
        private readonly Dictionary<string, List<QuestObject>> _questObjectDict = new();
        private readonly Dictionary<string, List<QuestInteractionObserver>> _questInteractionDict = new();

        public GameObject tempPopup;
        
        
        // Other
        private static readonly string[] QuestKeysArray = {
            "CaseFile", "CorkBoard", "PC", "Bin", "Cabinet", "ExitDoor", // Office
            "Clown", "BakeryFront", "AquariumFront", // Lafayette Street
            "Counter", "Shelf", "Table", "BakerInitial", // Bakery
            "Fence", "Breaker", "Dumpster", "CardboardBox", "BakeryAlley", "AquariumAlley", // Back Alley
            "LightsOff", "Flour", "MazePuzzle", // Storage Room
            "Corpse", "PoliceTape", "FishTank", // Crime Scene
            "Stage", "ClownCostume", "WetFootprints", // Aquarium Lobby
            "BoxCutter", "Landlord", "YachtClubFront", "CafeFront", "BakerDate" // Androne Street
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
            winScreen.AssertNull(this, name);
            puzzleOfficePaper.AssertNull(this, name);
            puzzleStorageMaze.AssertNull(this, name);
            puzzleAquariumSliding.AssertNull(this, name);
            puzzleOfficeEvidence.AssertNull(this, name);
            winScreen.SetActive(false);
            
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
            inputEvents.onSubmit += InteractionInput;
            dialogueEvents.onDialogueStarted += DialogueStarted;
            dialogueEvents.onDialogueFinished += DialogueEnded;

        }

        public void SetTempPopup(bool toggle) {
            tempPopup.SetActive(toggle);
            GameEventsManager.Instance.InputEvents.ChangeInputEventContext(toggle ? InputEvents.Context.UI : InputEvents.Context.World);
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
            helpText.text = "Use [W][A][S][D]/Arrows to move around. [Space] will continue active dialogue and interact with clipboard-marked locations.";
            GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.World);
            CameraManager.Instance.SetState(CameraManager.RenderState.PixelWorld);
            SetTempPopup(false);
        }

        private void DialogueStarted() {
            _dialogueActive = true;
            GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.UI);
        }

        private void DialogueEnded() {
            _dialogueActive = false;
            if (_puzzleActive) return;
            GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.World);
        }

        private void InteractionInput(bool input) {
            if (!input) return;
            if (_dialogueActive) {
                dialogueEvents.currentManager.ProcessDialogue();
            }
        }

        private IEnumerator AwaitDialogue(Action callback) {
            while (_dialogueActive) {
                yield return null; // Skip next frame
            }
            
            callback?.Invoke();
        }
        
        private IEnumerator AwaitPuzzle(Action callback) {
            while (_puzzleActive) {
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
                    winScreen.SetActive(puzzleWinOfficeEvidence);
                    player.enabled = false;
                    // Essentially disable this script

                    miniGameManager.OnMiniGameProcessed -= MiniGameProcess;
                    inputEvents.onInteract -= InteractionInput;
                    dialogueEvents.onDialogueStarted -= DialogueStarted;
                    dialogueEvents.onDialogueFinished -= DialogueEnded;
                    gameObject.SetActive(false);
                }
            }
        }

        public void TriggerDialogue(string knotName) {
            if (!GameEventsManager.HasInstance || string.IsNullOrEmpty(knotName)) return;

            if (_dialogueActive) return;
            dialogueEvents.EnterDialogue(knotName);
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
                    helpText.text = "When encountering a puzzle, a MiniGame will overlay on your screen. These primarily use [Left/Right Click], but check their help-box for more details.";
                    action += () => SetTempPopup(true);
                    StartCoroutine(AwaitDialogue(action));
                    break;
                }
                case "CorkBoard": {
                    // TODO - trigger puzzle
                    if (checkEvidenceReady) {
                        TriggerDialogue("Corkboard_End");
                        action += () => miniGameManager.SendCommandStart(puzzleOfficeEvidence);
                        action += () => ToggleObservers(false, reference);
                        
                        StartCoroutine(AwaitDialogue(action));
                    }
                    else {
                        TriggerDialogue("Corkboard_Start");
                    }
                    
                    break;
                }
                case "PC": {
                    if (puzzleWinOfficePaper) {
                        TriggerDialogue("PC_Unlocked");
                        checkPCUnlocked = true;
                        ToggleObservers(false, reference);
                        ToggleObservers(true, "CorkBoard");
                        helpText.text = "Follow the red triangles to move between locations. Your office connects to Lafayette Street via the bus stop.";
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
                        helpText.text = "You're tasked to investigate the assigned case. Find evidence, solve puzzles and speak to characters!";
                        // TODO - enable door
                    }
                    else TriggerDialogue("Door_No_Goal");
                    break;
                }
                #endregion
                #region { Lafayette }
                case "BakeryFront": {
                    TriggerDialogue("Bakery_Front");
                    ToggleObservers(false, "BakeryFront");
                    ToggleObjects(true, "BakeryFront");
                    //ToggleObservers(false, reference);
                    break;
                }
                case "AquariumFront": {
                    TriggerDialogue("Aquarium_Front");
                    ToggleObservers(false, reference);
                    //ToggleObservers(false, reference);
                    break;
                }
                case "Clown": {
                    if (findClownOutfit && meetClown) {
                        if (!checkClownRequest) {
                            checkClownRequest = true;
                            TriggerDialogue("Jestar_Uniform");
                            ToggleObjects(false, reference);
                        }
                        else {
                            TriggerDialogue("Jestar_Ask");
                        }
                    }
                    else {
                        meetClown = true;
                        TriggerDialogue("Jestar_Meet");
                        ToggleObservers(true, "ClownCostume");
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
                        ToggleObservers(false, reference);
                        ToggleObjects(false, reference);
                        findPhone = true;
                    }
                    break;
                }
                case "Shelf": {
                    TriggerDialogue("Bakery_Cabinet");
                    ToggleObservers(false, reference);
                    break;
                }
                case "Table": {
                    TriggerDialogue("Bakery_Table");
                    ToggleObservers(false, reference);
                    ToggleObjects(false, reference);
                    findList = true;
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
                    ToggleObservers(false, reference);
                    break;
                }
                case "AquariumAlley": {
                    TriggerDialogue("Aquarium_Backdoor");
                    player.Warp(playerWarps[1].position, Vector2.down);
                    break;
                }
                case "Fence": {
                    if (findGateKey) {
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
                    TriggerDialogue("Breaker");
                    ToggleObservers(false, reference);
                    //ToggleObjects(false, reference);
                    checkFixBreaker = true;
                    
                    _questObjectDict["Breaker"][0].SetActive(false);
                    _questObjectDict["Breaker"][1].SetActive(false);
                    _questObjectDict["Breaker"][2].SetActive(true);
                    
                    ToggleObjects(true, "AquariumAlley");
                    ToggleObjects(true, "LightsOff");
                    ToggleObservers(false, "LightsOff");
                    ToggleObservers(true, "CardboardBox");
                    
                    //ToggleObservers(true, "AquariumAlley");
                    break;
                }
                case "Dumpster": {
                    miniGameManager.SendCommandStart(puzzleAquariumSliding);
                    action += () => ToggleObservers(false, reference);
                    action += () => ToggleObjects(false, reference);
                    //action += () => ToggleObservers(true, "CardboardBox");
                    action += () => TriggerDialogue("Rubbish");
                    findTicket = true;
                    findTissue = true;
                    
                    StartCoroutine(AwaitPuzzle(action));
                    break;
                }
                case "CardboardBox": {
                    TriggerDialogue("Delivery_Box");
                    ToggleObservers(false, reference);
                    findBox = true;
                    // TODO - world object
                    break;
                }
                #endregion
                #region { Storage }
                case "LightsOff": {
                    TriggerDialogue("No_Lights");

                    action += () => miniGameManager.SendCommandStart(puzzleStorageMaze);
                    action += () => StartCoroutine(AwaitPuzzle(
                        () => player.Warp(playerWarps[2].position, Vector2.down))
                    );
                    //checkOutage = true;
                    
                    _questObjectDict["Breaker"][0].SetActive(false);
                    _questObjectDict["Breaker"][1].SetActive(true);
                    _questObjectDict["Breaker"][2].SetActive(false);
                    
                    
                    ToggleObservers(true, "Fence");
                    ToggleObservers(false, "AquariumAlley");
                    ToggleObjects(false, "AquariumAlley");
                    
                    StartCoroutine(AwaitDialogue(action));
                    break;
                }
                case "Flour": {
                    TriggerDialogue("Flour");
                    ToggleObservers(false, "Flour");
                    findFlour = true;
                    break;
                }
                //case "MazePuzzle": {
                //    // TODO - trigger puzzle
                //    break;
                //}
                #endregion
                #region { Androne }
                case "BoxCutter": {
                    TriggerDialogue("Box_Cutter");
                    findBoxCutter = true;
                    ToggleObservers(false, reference);
                    ToggleObjects(false, reference);
                    break;
                }
                
                case "YachtClubFront": {
                    TriggerDialogue("Yacht_Club_Front");
                    ToggleObservers(false, reference);
                    break;
                }
                case "CafeFront": {
                    TriggerDialogue("Cafe");
                    ToggleObservers(false, reference);
                    break;
                }
                case "Landlord": {
                    if (meetLandlord) {
                        if (checkFence && !findGateKey) {
                            TriggerDialogue("Dakota_Key");
                            findGateKey = true;
                            ToggleObjects(false, reference);
                        }
                        else TriggerDialogue("Dakota_Ask");
                    }
                    else {
                        TriggerDialogue("Dakota_Meet");
                        meetLandlord = true;
                        if (checkFence && !findGateKey) {
                            action += () => TriggerDialogue("Dakota_Key");
                            action += () => findGateKey = true;
                            action += () => ToggleObjects(false, reference);
                            StartCoroutine(AwaitDialogue(action));
                        }
                        
                    }
                    
                    break;
                }
                case "BakerDate": { // this one will need heavy testing
                    TriggerDialogue("Briar_Date");
                    checkDateSegment = true;
                    ToggleObservers(false, reference);
                    helpText.text = "You're tasked to investigate the assigned case. Find evidence, solve puzzles and speak to characters!";
                    break;
                }

                #endregion
                #region { Crime }
                case "Corpse": {
                    TriggerDialogue("Corpse");
                    //ToggleObservers(false, reference);
                    findCorpse = true;
                    break;
                }
                case "PoliceTape": {
                    TriggerDialogue("Caution_Tape");
                    ToggleObservers(false, reference);
                    break;
                }
                case "FishTank": {
                    TriggerDialogue("Fish_Tank");
                    ToggleObservers(false, reference);
                    break;
                }
                #endregion
                #region { Lobby }
                case "Stage": {
                    TriggerDialogue("Stage");
                    ToggleObservers(false, reference);
                    break;
                }
                case "ClownCostume": {
                    TriggerDialogue("Clown_Costume");
                    findClownOutfit = true;
                    ToggleObservers(false, reference);
                    ToggleObjects(false, reference);
                    break;
                }
                case "WetFootprints": {
                    TriggerDialogue("Wet_Footprints");
                    ToggleObservers(false, reference);
                    findFootprints = true;
                    break;
                }
                #endregion
                
                
                
                default:
                    Debug.LogWarning("Tried using undefined key: " + reference);
                    break;
            }
            
            // Complex checks go here
            if (!checkDateSegment) {
                // Breaker means met Dakota, gotten key, done maze puzzle, turned lights on.
                // Meeting briar just means doing the initial intro.
                var canDateBriar = meetBaker && checkFixBreaker;
                if (canDateBriar) {
                    ToggleObjects(true, "BakerDate");
                    ToggleObservers(true, "BakerDate");
                    Debug.Log("Player can now rizz Briar");

                    helpText.text = "Meet Briar to ask further questions. She owns a Cafe on Androne Street.";
                }
            }
            else {
                var canDoEvidencePuzzle = 
                    checkDateSegment && checkClownRequest && // all characters
                    puzzleWinOfficePaper && puzzleWinAquariumSliding && puzzleWinStorageMaze && // all puzzles
                    findCorpse && findClownOutfit && findGateKey && findBox && findBoxCutter && findPhone && findList && findTicket && findTissue && findFlour && findFootprints; // all 'evidence'
                
                if (canDoEvidencePuzzle) {
                    ToggleObjects(false, "CorkBoard");
                    //ToggleObservers(true, "CorkBoard");
                    checkEvidenceReady = true;
                    Debug.Log("Player can now do the evidence puzzle");
                    
                    helpText.text = "You've got enough evidence for today. Head back to the office!";
                }
            }
        }
        
        
        
        // Quest vars
        [Header("Quest Values")]
        //[SerializeField, ReadOnly] private bool checkDesk;
        [SerializeField, ReadOnly] private bool checkCaseFile;
        [SerializeField, ReadOnly] private bool checkPCUnlocked;
        [SerializeField, ReadOnly] private bool checkLostAndFound;
        [SerializeField, ReadOnly] private bool checkFence;
        [SerializeField, ReadOnly] private bool checkClownRequest;
        //[SerializeField, ReadOnly] private bool checkOutage;
        [SerializeField, ReadOnly] private bool checkDateSegment;
        [SerializeField, ReadOnly] private bool checkFixBreaker;
        [SerializeField, ReadOnly] private bool checkEvidenceReady;
        
        [SerializeField, ReadOnly] private bool meetClown;
        [SerializeField, ReadOnly] private bool meetBaker;
        [SerializeField, ReadOnly] private bool meetLandlord;
        
        [SerializeField, ReadOnly] private bool findCorpse;
        [SerializeField, ReadOnly] private bool findClownOutfit;
        [SerializeField, ReadOnly] private bool findGateKey;
        [SerializeField, ReadOnly] private bool findBox;
        [SerializeField, ReadOnly] private bool findBoxCutter;
        [SerializeField, ReadOnly] private bool findPhone;
        [SerializeField, ReadOnly] private bool findList;
        [SerializeField, ReadOnly] private bool findTicket;
        [SerializeField, ReadOnly] private bool findTissue;
        [SerializeField, ReadOnly] private bool findFlour;
        [SerializeField, ReadOnly] private bool findFootprints;
        //[SerializeField, ReadOnly] private bool fetch;
        
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
