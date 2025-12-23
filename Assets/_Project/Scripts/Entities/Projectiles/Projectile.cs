using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Core.Constants;
using VampireSurvivor.Data;
using VampireSurvivor.Interfaces;
using VampireSurvivor.Systems.Pooling;

namespace VampireSurvivor.Entities.Projectiles
{
    /// <summary>
    /// Base projectile class. Moves in a direction and damages enemies on contact.
    /// </summary>
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private WeaponData _weaponData;
        private Vector2 _direction;
        private float _lifetime;
        private int _pierceRemaining;
        private string _poolKey;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Initialize projectile after spawning.
        /// </summary>
        public void Initialize(WeaponData data, Vector2 direction, string poolKey)
        {
            _weaponData = data;
            _direction = direction.normalized;
            _poolKey = poolKey;
            _lifetime = data.ProjectileLifetime;
            _pierceRemaining = data.PierceCount;

            // Apply visuals
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = data.ProjectileColor;
            }
            transform.localScale = Vector3.one * data.ProjectileSize;

            // Rotate to face direction
            if (_direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void Update()
        {
            if (_weaponData == null) return;

            // Move
            transform.Translate(Vector3.right * _weaponData.ProjectileSpeed * Time.deltaTime, Space.Self);

            // Lifetime check
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_weaponData == null) return;
            if (!other.CompareTag(Tags.Enemy)) return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_weaponData.Damage);

                // Handle pierce
                if (_weaponData.PierceCount > 0) // 0 = infinite pierce
                {
                    _pierceRemaining--;
                    if (_pierceRemaining <= 0)
                    {
                        ReturnToPool();
                    }
                }
            }
        }

        private void ReturnToPool()
        {
            var poolManager = ServiceLocator.Get<PoolManager>();
            if (poolManager != null && !string.IsNullOrEmpty(_poolKey))
            {
                poolManager.Return(_poolKey, this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        // IPoolable implementation
        public void OnSpawnFromPool()
        {
            // Reset handled in Initialize
        }

        public void OnReturnToPool()
        {
            _weaponData = null;
            _lifetime = 0;
        }
    }
}
