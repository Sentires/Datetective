using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.General;
using TheDates.Runtime.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace TheDates.Runtime.Experimental.MinigameCore
{
    public class MiniGameManager : BasicSingleton<MiniGameManager>
    {
        private bool _isActive;
        private bool _isClicking;
        private int _layerMask;
        

        public Vector2 mousePosition { get; private set; }
        [FormerlySerializedAs("MiniGamePrefabs")] [FormerlySerializedAs("MiniGames")] public List<GameObject> miniGamePrefabs = new();
        [SerializeField] private Camera overlayCamera;
        public Dictionary<GameObject, MiniGame> miniGamesDict;

        public event Action<bool> OnClickState = delegate { };
        public event Action<Vector2> OnClickPosition = delegate { };
        public event Action<RaycastHit2D> OnClickTarget = delegate { };
        public event Action<MiniGameContext> OnMiniGameProcessed = delegate { };

        public Vector2 ScreenResolution;

        private void MiniGameProcessed(MiniGameContext context)
        {
            if (!miniGamesDict.TryGetValue(context.Source, out var game)) return;
            if (_currentMiniGame != game) return;

            //ToggleMiniGame(game, context.State == MiniGameState.Active);
            //Debug.Log($"MiniGameProcessed: {game.name}, is current game: {game == _currentMiniGame}");
        }


        protected override void Awake()
        {
            base.Awake();
            ScreenResolution = new Vector2(Screen.width, Screen.height);
            //ScaleToScreen();
            
            miniGamesDict = new Dictionary<GameObject, MiniGame>();
            var gameParent = transform.Find("Games");
            for (var i = miniGamePrefabs.Count - 1; i >= 0; i--)
            {
                var prefab = miniGamePrefabs[i];
                if (!prefab) continue;
                var gameObj = Instantiate(prefab, gameParent);
                if (gameObj.TryGetComponent<MiniGame>(out var game)) {
                    miniGamesDict.Add(prefab, game);
                    game.Init(prefab);
                    gameObj.SetActive(false);
                    continue;
                }
                
                Debug.Log("Removed MiniGame: " + prefab.name + ", this does not contain an IMiniGame inheritor component!");
                miniGamePrefabs.RemoveAt(i);
                Destroy(gameObj);
            }
            
            OnMiniGameProcessed += MiniGameProcessed;
        }
        //https://www.youtube.com/watch?v=YdCx82q3ONE&t=89s
        private void ScaleToScreen()
        {
            var planeHeightScale = 2f * overlayCamera.orthographicSize / 10f;
            var planeWidthScale = planeHeightScale * overlayCamera.aspect;
            transform.localScale = new Vector3(planeWidthScale, 1, planeHeightScale);
        }

        public Vector2 GetScale()
        {
            var planeHeightScale = 2f * overlayCamera.orthographicSize / 10f;
            var planeWidthScale = planeHeightScale * overlayCamera.aspect;
            return new Vector3(planeWidthScale, 1, planeHeightScale);
        }

        public void NotifyMiniGameState(GameObject reference, MiniGameState state)
        {
            //miniGamesDict.
            if (!miniGamesDict.TryGetValue(reference, out var game)) return;
            var context = new MiniGameContext(reference, state);
            Debug.Log($"{context.Source} : {context.State}");
            OnMiniGameProcessed(context);

            if (context.State == MiniGameState.Active) {
                if (_currentMiniGame && _currentMiniGame != game) {
                    //var current = miniGamesDict[_currentMiniGame.source];
                    _currentMiniGame.AcceptCommand(MiniGameCommand.ForceQuit);
                    //_currentMiniGame.gameObject.SetActive(false);
                }
            
                _currentMiniGame = game;
                
                ToggleMiniGame(game, true);
                return;
            }
            
            ToggleMiniGame(game, false);
            if (_currentMiniGame == game) _currentMiniGame = null;


        }

        private void Update()
        {
            if (!Mathf.Approximately(ScreenResolution.x, Screen.width) || !Mathf.Approximately(ScreenResolution.y, Screen.height))
            {
                //ScaleToScreen();
                ScreenResolution.x = Screen.width;
                ScreenResolution.y = Screen.height;
            }
        }

        private void Start() {
            _isActive = true;
            _layerMask = LayerMask.GetMask("OverlayWorld");
            overlayCamera.AssertNull(this, nameof(overlayCamera));
            //_clickListeners = GetComponentsInChildren<IClickListener>();
            //GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.UI);
            
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("OverlayWorld"));
        }

        private void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.InputEvents.onLeftClick += OnClick;
            GameEventsManager.Instance.InputEvents.onPosition += OnPoint;
            //GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.UI);
        }

        public void SendCommand(GameObject target, MiniGameCommand command) {
            if (!miniGamesDict.TryGetValue(target, out var game)) return;
            game.AcceptCommand(command);
        }
        
        // To call from UnityEvents
        public void SendCommandStart(GameObject target) => SendCommand(target, MiniGameCommand.Start);
        public void SendCommandReset(GameObject target) => SendCommand(target, MiniGameCommand.Reset);
        public void SendCommandWin(GameObject target) => SendCommand(target, MiniGameCommand.Win);
        public void SendCommandLose(GameObject target) => SendCommand(target, MiniGameCommand.Lose);
        public void SendCommandForceQuit(GameObject target) => SendCommand(target, MiniGameCommand.ForceQuit);
        
        /*
        public void StartGame(GameObject target) {
            if (target == _currentMiniGame?.source || !miniGamesDict.TryGetValue(target, out var game)) return;
            if (_currentMiniGame) {
                //var current = miniGamesDict[_currentMiniGame.source];
                _currentMiniGame.AcceptCommand(MiniGameCommand.ForceQuit);
                _currentMiniGame.gameObject.SetActive(false);
            }
            
            _currentMiniGame = game;
            ToggleMiniGame(true);
            game.AcceptCommand(MiniGameCommand.Start);
            
            //Debug.Log($"{_currentMiniGame.source.name} is now active!");
        }

        public void ResetGame() {
            //if (!_currentMiniGame?.source || miniGamesDict[_currentMiniGame.source] == null) return;
            //miniGamesDict[_currentMiniGame.source].ResetGame();
            _currentMiniGame?.AcceptCommand(MiniGameCommand.Reset);
        }
        
        public void StopGame() {
            //if (!_currentMiniGame?.source || miniGamesDict[_currentMiniGame.source] == null) return;
            //miniGamesDict[_currentMiniGame.source].StopGame();
            _currentMiniGame?.AcceptCommand(MiniGameCommand.ForceQuit);
            ToggleMiniGame(false);
            
            _currentMiniGame = null;
        }

        public void ForceGameOutcome(bool isWinner) {
            
        }*/
        
        private void ToggleMiniGame(MiniGame game, bool toggle)
        {
            if (toggle) {
                GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.UI);
                CameraManager.Instance.SetState(CameraManager.RenderState.HighDefWorld);
            }
            else {
                GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.World);
                CameraManager.Instance.SetState(CameraManager.RenderState.PixelWorld);
            }
            //GameEventsManager.Instance.InputEvents.ChangeInputEventContext(toggle
            //    ? InputEvents.Context.UI
            //    : InputEvents.Context.World);

            game?.gameObject.SetActive(toggle);
        }

        /*private void ToggleMiniGame(bool toggle)
        {
            if (toggle) {
                GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.UI);
                CameraManager.Instance.SetState(CameraManager.RenderState.HighDefWorld);
            }
            else {
                GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.World);
                CameraManager.Instance.SetState(CameraManager.RenderState.PixelWorld);
            }
            //GameEventsManager.Instance.InputEvents.ChangeInputEventContext(toggle
            //    ? InputEvents.Context.UI
            //    : InputEvents.Context.World);

            _currentMiniGame?.gameObject.SetActive(toggle);
        }*/
        
        private MiniGame _currentMiniGame;

        private void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.InputEvents.onLeftClick -= OnClick;
            GameEventsManager.Instance.InputEvents.onPosition -= OnPoint;
            //GameEventsManager.Instance.InputEvents.ChangeInputEventContext(InputEvents.Context.World);
        }
        
        private void OnPoint(Vector2 input) {
            if (!_isActive) return;
            mousePosition = input;
            OnClickPosition.Invoke(input);
        }

        private void OnClick(bool input) {
            if (!_isActive) return;
            _isClicking = input;
            OnClickState.Invoke(input);
            
            if (!_isClicking || !overlayCamera) return;
            var worldPos = overlayCamera.ScreenToWorldPoint(mousePosition);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, _layerMask);
            if (!hit) return;
            
            OnClickTarget.Invoke(hit);
            
            //var target = hit.transform;
            //Debug.Log($"Clicked On {hit.collider.name} at {worldPos}");

            //if (target.TryGetComponent<IClickable>(out var clickable)) {
            //    StartCoroutine(ProcessClick(clickable));
            //}
        }
        
        public Vector3 GetWorldSpacePosition(Vector2 input, float zPosition) {
            var worldPos = overlayCamera.ScreenToWorldPoint(input);
            worldPos.z = zPosition;
            return worldPos;
        }

        public RaycastHit2D Raycast(Vector3 position, bool isWorldSpace = true) {
            var pos = isWorldSpace ? position : overlayCamera.ScreenToWorldPoint(position);
            return Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, _layerMask);
        }

        private IEnumerator ProcessClick(IClickable clickable)
        {
            clickable?.OnStart();
            //Debug.Log("Button pressed");
            
            while (_isClicking) {
                // clickable?.OnHold();
                yield return null;
                if (!_isActive || clickable == null) break;
            }
            
            clickable?.OnRelease();
            //Debug.Log("Button released");
        }
        
    }
    
    [Serializable]
    public struct MiniGameCommandContext {
        public GameObject target;
        public MiniGameCommand command;

        public MiniGameCommandContext(GameObject target, MiniGameCommand command)
        {
            this.target = target;
            this.command = command;
        }
    }

    public interface IClickable {
        public Transform contentParent { get; }
        void OnStart();
        void OnHold();
        void OnRelease();
    }
    
    public interface IClickListener
    {
        MiniGameManager manager { get; }
        void StartListening(MiniGameManager manager);
        void StopListening(MiniGameManager manager);
        void OnClickState(bool input);
        void OnClickPosition(Vector2 input);
        void OnClickTarget(RaycastHit2D hit);
    }
    
    public enum MiniGameState { Uninitialised, Active, Inactive, Completed }
    public enum MiniGameCommand { Start, Reset, Win, Lose, ForceQuit}

    public readonly struct MiniGameContext
    {
        public readonly GameObject Source;
        public readonly MiniGameState State;
        

        public MiniGameContext(GameObject source, MiniGameState state)
        {
            Source = source;
            State = state;
        }
        
        
        //public IMiniGame MiniGame;
    }

    public abstract class MiniGame : MonoBehaviour
    {
        public abstract bool isInitialised { get; }
        public abstract GameObject source { get; }
        public abstract MiniGameState gameState { get; }
        public abstract void Init(GameObject prefab);

        public abstract void AcceptCommand(MiniGameCommand command);
        
        // To call from UnityEvents
        public void SendCommand(MiniGameCommand command) {
            if (!MiniGameManager.HasInstance) return;
            MiniGameManager.Instance.SendCommand(source, command);
        }
        public void SendCommandStart() => SendCommand(MiniGameCommand.Start);
        public void SendCommandReset() => SendCommand(MiniGameCommand.Reset);
        public void SendCommandWin() => SendCommand(MiniGameCommand.Win);
        public void SendCommandLose() => SendCommand(MiniGameCommand.Lose);
        public void SendCommandForceQuit() => SendCommand(MiniGameCommand.ForceQuit);
    }
}
