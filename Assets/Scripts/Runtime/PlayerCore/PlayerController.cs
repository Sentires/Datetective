using System.Collections;
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
        public float movementLockoutTime = 0.25f;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform cameraTarget;
        
        private Direction _currentDirection;
        private Vector2 _currentMovementInput;
        private bool _isMoving;
        private bool _canMove;
        private WaitForSeconds _movementLockout;
        private static bool ignoreInputs => GameEventsManager.Instance.InputEvents.inputEventContext != InputEvents.Context.World;
        
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
            cameraTarget.AssertNull(this, "CameraTarget", "Please assign the camera target in the editor");
            UpdateAnimator(false, Direction.Down);
            
            _movementLockout = new WaitForSeconds(movementLockoutTime);
            StartCoroutine(WarpLockout()); // this sets _canMove in the process
        }

        public void OnEnable() {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.InputEvents.onMove += Move;
        }
        
        public void OnDisable() {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.InputEvents.onMove -= Move;
        }

        public void Warp(Vector2 position, Vector2 direction) {
            _currentDirection = Mathf.Abs(direction.x) * horizontalInputBias >= Mathf.Abs(direction.y)
                ? direction.x > 0 ? Direction.Right : Direction.Left
                : direction.y > 0 ? Direction.Up : Direction.Down;
            //_currentMovementInput = Vector2.zero;
            
            transform.position = position;
            rb.velocity = Vector2.zero;
            _isMoving = false;
            CameraManager.Instance.SnapToPosition(cameraTarget.position);
            
            if (!_canMove) StopCoroutine(WarpLockout());
            StartCoroutine(WarpLockout());
        }

        private IEnumerator WarpLockout() {
            _canMove = false;
            UpdateAnimator(_isMoving, _currentDirection);
            
            yield return _movementLockout;
            _canMove = true;
        }

        

        private void FixedUpdate() {
            if (!_canMove) return;
            UpdateDirection();
            SetVelocity();
            UpdateAnimator(_isMoving, _currentDirection);
        }
        
        private void Move(Vector2 input) {
            _currentMovementInput = ignoreInputs ? Vector2.zero : input;
        }
        
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
