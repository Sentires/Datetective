using System.Collections.Generic;
using TheDates.Runtime.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheDates.Runtime.PlayerCore
{
    // https://www.youtube.com/watch?v=DQY62meLVCk
    // I can probably split this class later if necessary.
    // It transforms inputs, applies physics and manages the animator.
    // CharacterAnimator, CharacterRigidbody, PlayerController (CharacterController for NPCs)
    public class PlayerController : MonoBehaviour
    {
        public float speed = 1f;
        public float horizontalInputBias = 1.1f; // Affects how dominant left/right velocity is in the animator
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        
        private Direction _currentDirection;
        private Vector2 _currentMovementInput;
        private bool _isMoving;
        private static bool canMove => GameEventsManager.Instance.InputEvents.inputEventContext == InputEvents.Context.World;
        
        // Cache Animator Parameters
        private static readonly Vector2Int ParamFacingHash = new(Animator.StringToHash("FacingX"), Animator.StringToHash("FacingY"));
        private static readonly Vector2Int ParamMovingHash = new(Animator.StringToHash("MovingX"), Animator.StringToHash("MovingY"));
        private static readonly int ParamIsMovingHash = Animator.StringToHash("IsMoving");
        
        // If needed by other scripts, I can make this public and in a static class
        private enum Direction { Down, Up, Left, Right }
        private static readonly Dictionary<Direction, Vector2> DirectionToVector = new() {
            { Direction.Down, Vector2.down },
            { Direction.Up, Vector2.up },
            { Direction.Left, Vector2.left },
            { Direction.Right, Vector2.right }
        };
        
        private void Start() {
            rb.AssertNull(this, "RigidBody2D", "Please assign the RB2D component in the editor");
            animator.AssertNull(this, "Animator", "Please assign the Animator component in the editor");

            UpdateAnimator(false, Direction.Down);
        }

        public void OnEnable() {
            GameEventsManager.Instance.InputEvents.onMove += Move;
            //GameEventsManager.Instance.InputEvents.onClick += Click;
            //GameEventsManager.Instance.InputEvents.onPoint += Point;
        }
        
        public void OnDisable() {
            GameEventsManager.Instance.InputEvents.onMove -= Move;
            //GameEventsManager.Instance.InputEvents.onClick -= Click;
            //GameEventsManager.Instance.InputEvents.onPoint -= Point;
        }
        
        

        private void FixedUpdate() {
            if (!canMove) return;
            UpdateDirection();
            SetVelocity();
            UpdateAnimator(_isMoving, _currentDirection);
        }
        
        private void Move(Vector2 input) {
            _currentMovementInput = input;
        }
        
        //private void Click(bool input) {
        //    Debug.Log($"Clicked: {input}");
        //}
        
        //private void Point(Vector2 input) {
        //    Debug.Log($"Pointed to: {input}");
        //}
        
        private void SetVelocity() {
            rb.velocity = _currentMovementInput * speed;
        }
        
        private void UpdateDirection() {
            // I just like using an enum in case other scripts need easy access later. Reads better.
            // Arguably could just use Vector2 without the dictionary.
            _isMoving = _currentMovementInput != Vector2.zero;
            if (_isMoving) {
                _currentDirection = Mathf.Abs(_currentMovementInput.x) * horizontalInputBias >= Mathf.Abs(_currentMovementInput.y)
                    ? _currentMovementInput.x > 0 ? Direction.Right : Direction.Left
                    : _currentMovementInput.y > 0 ? Direction.Up : Direction.Down;
            }
        }

        private void UpdateAnimator(bool hasMoved, Direction direction) {
            animator.SetBool(ParamIsMovingHash, hasMoved);
            
            var facing = DirectionToVector[direction];
            animator.SetFloat(ParamFacingHash.x, facing.x);
            animator.SetFloat(ParamFacingHash.y, facing.y);
            animator.SetFloat(ParamMovingHash.x, facing.x);
            animator.SetFloat(ParamMovingHash.y, facing.y);
        }

        

        
    }
}
