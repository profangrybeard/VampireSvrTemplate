using UnityEngine;
using UnityEngine.InputSystem;
using VampireSurvivor.Data;

namespace VampireSurvivor.Entities.Player
{
    /// <summary>
    /// Handles player movement using the new Input System.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private PlayerInput _playerInput;
        private InputAction _moveAction;

        public Vector2 MoveDirection => _moveInput;
        public Vector2 FacingDirection { get; private set; } = Vector2.right;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput != null && _playerInput.actions != null)
            {
                _moveAction = _playerInput.actions["Move"];
            }
        }

        private void Update()
        {
            if (_moveAction != null)
            {
                _moveInput = _moveAction.ReadValue<Vector2>();

                // Update facing direction when moving
                if (_moveInput.sqrMagnitude > 0.01f)
                {
                    FacingDirection = _moveInput.normalized;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_config == null) return;

            Vector2 velocity = _moveInput * _config.PlayerMoveSpeed;
            _rb.linearVelocity = velocity;
        }

        /// <summary>
        /// External method for AI or abilities to move the player.
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            _moveInput = Vector2.ClampMagnitude(input, 1f);
        }
    }
}
