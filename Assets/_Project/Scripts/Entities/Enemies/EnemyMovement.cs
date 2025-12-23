using UnityEngine;
using VampireSurvivor.Core.Constants;

namespace VampireSurvivor.Entities.Enemies
{
    /// <summary>
    /// Simple chase-player movement for enemies.
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        private Enemy _enemy;
        private Rigidbody2D _rb;
        private Transform _target;

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
        }

        private void FixedUpdate()
        {
            if (_target == null || _enemy == null || !_enemy.IsAlive) return;
            if (_enemy.Data == null) return;

            Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            _rb.linearVelocity = direction * _enemy.Data.MoveSpeed;
        }

        /// <summary>
        /// Set the target to chase (usually the player).
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}
