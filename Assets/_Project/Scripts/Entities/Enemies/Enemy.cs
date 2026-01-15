using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Data;
using VampireSurvivor.Events;
using VampireSurvivor.Interfaces;
using VampireSurvivor.Systems.Pooling;

namespace VampireSurvivor.Entities.Enemies
{
    // Base enemy class. Handles health, damage, and pooling lifecycle.
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IDamageable, IPoolable, IKillable
    {
        [SerializeField] private EnemyData _data;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _currentHealth;
        private Rigidbody2D _rb;

        public EnemyData Data => _data;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Initialize this enemy with data. Called after spawning from pool.
        public void Initialize(EnemyData data)
        {
            _data = data;
            _currentHealth = data.MaxHealth;

            // Apply visuals from data
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = data.Color;
            }
            transform.localScale = new Vector3(data.Size.x, data.Size.y, 1f);
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            _currentHealth -= damage;

            // Visual feedback: brief flash
            if (_spriteRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }

            if (_currentHealth <= 0)
            {
                Kill();
            }
        }

        private System.Collections.IEnumerator DamageFlash()
        {
            if (_spriteRenderer == null) yield break;

            Color originalColor = _data != null ? _data.Color : _spriteRenderer.color;
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            if (_spriteRenderer != null && IsAlive)
                _spriteRenderer.color = originalColor;
        }

        public void Kill()
        {
            _currentHealth = 0;

            EventBus.Publish(new EnemyKilledEvent(
                transform.position,
                _data != null ? _data.XPValue : 1,
                _data
            ));

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            var poolManager = ServiceLocator.Get<PoolManager>();
            if (poolManager != null && _data != null)
            {
                poolManager.Return(_data.PoolKey, this);
            }
            else
            {
                // Fallback: just deactivate
                gameObject.SetActive(false);
            }
        }

        // IPoolable implementation
        public void OnSpawnFromPool()
        {
            if (_data != null)
            {
                _currentHealth = _data.MaxHealth;
                if (_spriteRenderer != null)
                    _spriteRenderer.color = _data.Color;
            }
        }

        public void OnReturnToPool()
        {
            StopAllCoroutines();
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
            }

            // Reset movement state to prevent state carryover
            var movement = GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.ResetMovementState();
            }
        }
    }
}
