using UnityEngine;
using VampireSurvivor.Core.Constants;

namespace VampireSurvivor.Entities.Enemies
{
    // Simple chase-player movement. Enemies move directly toward the player.
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        private Enemy _enemy;
        private Rigidbody2D _rb;
        private Transform _target;

        // Zigzag movement state
        private float _zigzagTime = 0f;
        private const float ZIGZAG_FREQUENCY = 3f;  // How fast to zigzag
        private const float ZIGZAG_AMPLITUDE = 0.5f;  // How wide the zigzag

        // Wander movement state
        private Vector2 _wanderDirection;
        private float _wanderChangeTime = 0f;
        private const float WANDER_CHANGE_INTERVAL = 2f;  // Change direction every 2 seconds
        private const float PLAYER_SEEK_CHANCE = 0.3f;    // 30% chance to move toward player

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Find player
            var player = GameObject.FindGameObjectWithTag(Tags.Player);
            if (player != null)
            {
                _target = player.transform;
            }

            // Initialize wander with random direction
            InitializeWander();
        }

        private void FixedUpdate()
        {
            if (_target == null || _enemy == null || !_enemy.IsAlive) return;
            if (_enemy.Data == null) return;

            // Execute movement based on type
            switch (_enemy.Data.MovementType)
            {
                case Data.MovementType.Chase:
                    MoveChase();
                    break;
                case Data.MovementType.Zigzag:
                    MoveZigzag();
                    break;
                case Data.MovementType.Wander:
                    MoveWander();
                    break;
            }
        }

        // Set the target to chase.
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void MoveChase()
        {
            Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            _rb.linearVelocity = direction * _enemy.Data.MoveSpeed;
        }

        private void MoveZigzag()
        {
            Vector2 toPlayer = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x);  // Rotate 90 degrees

            _zigzagTime += Time.fixedDeltaTime;
            float zigzagOffset = Mathf.Sin(_zigzagTime * ZIGZAG_FREQUENCY) * ZIGZAG_AMPLITUDE;

            Vector2 direction = (toPlayer + perpendicular * zigzagOffset).normalized;
            _rb.linearVelocity = direction * _enemy.Data.MoveSpeed;
        }

        private void MoveWander()
        {
            _wanderChangeTime += Time.fixedDeltaTime;

            if (_wanderChangeTime >= WANDER_CHANGE_INTERVAL)
            {
                _wanderChangeTime = 0f;

                // 30% chance to move toward player, 70% chance random direction
                if (Random.value < PLAYER_SEEK_CHANCE)
                {
                    _wanderDirection = ((Vector2)_target.position - (Vector2)transform.position).normalized;
                }
                else
                {
                    float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    _wanderDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
                }
            }

            _rb.linearVelocity = _wanderDirection * _enemy.Data.MoveSpeed;
        }

        private void InitializeWander()
        {
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _wanderDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        }

        public void ResetMovementState()
        {
            _zigzagTime = 0f;
            _wanderChangeTime = 0f;
            InitializeWander();
        }
    }
}
