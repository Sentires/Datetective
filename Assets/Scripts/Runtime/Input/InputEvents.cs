using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheDates.Runtime.Input
{
    public class InputEvents : EventHandler, TestInputs.IPlayerActions, TestInputs.IUIActions
    {
        
        private readonly TestInputs _testInputs;
        
        public InputEvents() {
            _testInputs = new TestInputs();
            _testInputs.Player.SetCallbacks(this);
            _testInputs.UI.SetCallbacks(this);
        }
        
        public enum Context { Restricted, Menu, Puzzle, Dialogue, World} // Uninitialised, RPG Generic, RPG Minigame, VN Interactions, Menu
        public Context inputEventContext { get; private set; } = Context.Restricted;

        public void ChangeInputEventContext(Context newContext) 
        {
            if (inputEventContext == newContext) return;
            inputEventContext = newContext;
            
            // World uses RPG inputs specifically
            
            if (inputEventContext is Context.Restricted) {
                _testInputs.Player.Disable();
                _testInputs.UI.Disable();
            } else if (inputEventContext is Context.World) {
                _testInputs.Player.Enable();
                _testInputs.UI.Disable();
            } else { // Menu, Puzzle, Dialogue
                _testInputs.UI.Enable();
                _testInputs.Player.Disable();
            }
        }

        private InputAction FindContextualAction(string actionName) {
            return inputEventContext switch {
                Context.World => _testInputs.Player.Get()[actionName],
                Context.Menu or Context.Puzzle or Context.Dialogue => _testInputs.UI.Get()[actionName],
                _ => null // Context.Restricted => null
            };
            
        }
        
        // Shared //
        public event Action<Vector2> onPoint = delegate { };
        public Vector2 readPoint => FindContextualAction("Point")?.ReadValue<Vector2>() ?? Vector2.zero;
        public void OnPoint(InputAction.CallbackContext context) => onPoint.Invoke(context.ReadValue<Vector2>());
        //
        public event Action<bool> onClick = delegate { };
        public bool readClick => FindContextualAction("Click")?.IsPressed() ?? false;
        public void OnClick(InputAction.CallbackContext context) => onClick.Invoke(context.ReadValueAsButton());
        
        // RPG //
        public event Action<Vector2> onMove = delegate { };
        public Vector2 readMove => _testInputs.Player.Move.ReadValue<Vector2>();
        public void OnMove(InputAction.CallbackContext context) => onMove.Invoke(context.ReadValue<Vector2>());
        //
        public event Action<bool> onToggleSpecialMenu = delegate { };
        public bool readToggleSpecialMenu => _testInputs.Player.ToggleSpecialMenu.IsPressed();
        public void OnToggleSpecialMenu(InputAction.CallbackContext context) => onToggleSpecialMenu.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onToggleMenu = delegate { };
        public bool readToggleMenu => _testInputs.Player.ToggleMenu.IsPressed();
        public void OnToggleMenu(InputAction.CallbackContext context) => onToggleMenu.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onInteract = delegate { };
        public bool readInteract => _testInputs.Player.Interact.IsPressed();
        public void OnInteract(InputAction.CallbackContext context) => onInteract.Invoke(context.ReadValueAsButton());
        
        // UI //
        public event Action<Vector2> onNavigate = delegate { };
        public Vector2 readNavigate => _testInputs.UI.Navigate.ReadValue<Vector2>();
        public void OnNavigate(InputAction.CallbackContext context) => onNavigate.Invoke(context.ReadValue<Vector2>());
        //
        public event Action<bool> onSubmit = delegate { };
        public bool readSubmit => _testInputs.UI.Submit.IsPressed();
        public void OnSubmit(InputAction.CallbackContext context) => onSubmit.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onCancel = delegate { };
        public bool readCancel => _testInputs.UI.Cancel.IsPressed();
        public void OnCancel(InputAction.CallbackContext context) => onCancel.Invoke(context.ReadValueAsButton());
        //
        public event Action<Vector2> onScrollWheel = delegate { };
        public Vector2 readScrollWheel => _testInputs.UI.ScrollWheel.ReadValue<Vector2>();
        public void OnScrollWheel(InputAction.CallbackContext context) => onScrollWheel.Invoke(context.ReadValue<Vector2>());
        //
        public event Action<bool> onMiddleClick = delegate { };
        public bool readMiddleClick => _testInputs.UI.MiddleClick.IsPressed();
        public void OnMiddleClick(InputAction.CallbackContext context) => onMiddleClick.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onRightClick = delegate { };
        public bool readRightClick => _testInputs.UI.RightClick.IsPressed();
        public void OnRightClick(InputAction.CallbackContext context) => onRightClick.Invoke(context.ReadValueAsButton());
        
        

        
    }
}
