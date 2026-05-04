using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
        
        public enum Context { Restricted, UI, World} // Uninitialised, RPG Generic, RPG Minigame, VN Interactions, Menu
        public Context inputEventContext { get; private set; } = Context.Restricted;
        public event Action<Context> onInputContextChange = delegate { };

        public void ChangeInputEventContext(Context newContext) 
        {
            if (inputEventContext == newContext) return;
            //inputEventContext = newContext;
            
            // World uses RPG inputs specifically
            
            if (newContext is Context.Restricted) {
                ClearUIControls();
                ClearWorldControls();
                _testInputs.Player.Disable();
                _testInputs.UI.Disable();
            } else if (newContext is Context.World) {
                ClearUIControls();
                _testInputs.Player.Enable();
                _testInputs.UI.Disable();
            } else { // Menu, Puzzle, Dialogue
                ClearWorldControls();
                _testInputs.UI.Enable();
                _testInputs.Player.Disable();
            }
            
            inputEventContext = newContext;
            onInputContextChange.Invoke(inputEventContext);
        }
        
        private void ClearWorldControls()
        {
            onPoint.Invoke(default);
            onClick.Invoke(false);
            onMove.Invoke(default);
            onToggleSpecialMenu.Invoke(false);
            onToggleMenu.Invoke(false);
            onInteract.Invoke(false);
        }

        private void ClearUIControls()
        {
            onPosition.Invoke(default);
            onNavigate.Invoke(default);
            onSubmit.Invoke(false);
            onCancel.Invoke(false);
            onScrollWheel.Invoke(default);
            onLeftClick.Invoke(false);
            onRightClick.Invoke(false);
            onMiddleClick.Invoke(false);
        }

        private InputAction FindContextualAction(string actionName) {
            return inputEventContext switch {
                Context.World => _testInputs.Player.Get()[actionName],
                Context.UI => _testInputs.UI.Get()[actionName],
                _ => null // Context.Restricted => null
            };
            
        }
        
        // RPG //
        public event Action<Vector2> onPoint = delegate { };
        public Vector2 readPoint => _testInputs.Player.Point.ReadValue<Vector2>();
        public void OnPoint(InputAction.CallbackContext context) => onPoint.Invoke(context.ReadValue<Vector2>());
        //
        public event Action<bool> onClick = delegate { };
        public bool readClick => _testInputs.Player.Click.IsPressed();
        public void OnClick(InputAction.CallbackContext context) => onClick.Invoke(context.ReadValueAsButton());
        //
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
        public event Action<Vector2> onPosition = delegate { };
        public Vector2 readPosition => _testInputs.UI.Position.ReadValue<Vector2>();
        public void OnPosition(InputAction.CallbackContext context) => onPosition.Invoke(context.ReadValue<Vector2>());
        //
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
        public event Action<bool> onLeftClick = delegate { };
        public bool readLeftClick => _testInputs.UI.LeftClick.IsPressed();
        public void OnLeftClick(InputAction.CallbackContext context) => onLeftClick.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onMiddleClick = delegate { };
        public bool readMiddleClick => _testInputs.UI.MiddleClick.IsPressed();
        public void OnMiddleClick(InputAction.CallbackContext context) => onMiddleClick.Invoke(context.ReadValueAsButton());
        //
        public event Action<bool> onRightClick = delegate { };
        public bool readRightClick => _testInputs.UI.RightClick.IsPressed();
        public void OnRightClick(InputAction.CallbackContext context) => onRightClick.Invoke(context.ReadValueAsButton());

        private bool EvaluateButtonPress(InputAction.CallbackContext context) {
            return context.ReadValue<float>() > ((ButtonControl)context.control).pressPointOrDefault;
        }


    }
}
