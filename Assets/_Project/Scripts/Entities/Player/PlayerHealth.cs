using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Data;
using VampireSurvivor.Events;
using VampireSurvivor.Interfaces;

namespace VampireSurvivor.Entities.Player
{
    /// <summary>
    /// Manages player health, damage, and death.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable, IKillable
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _currentHealth;
        private float _invincibilityTimer;
        private bool _isInvincible;
        private Color _originalColor;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _config != null ? _config.PlayerMaxHealth : 100f;
        public float HealthPercent => _currentHealth / MaxHealth;
        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
        }

        private void Start()
        {
            _currentHealth = MaxHealth;
        }

        private void Update()
        {
            if (_isInvincible)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    _isInvincible = false;
                    if (_spriteRenderer != null)
                        _spriteRenderer.color = _originalColor;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive || _isInvincible) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);

            EventBus.Publish(new PlayerDamagedEvent(damage, _currentHealth, MaxHealth));

            if (_currentHealth <= 0)
            {
                Kill();
            }
            else
            {
                StartInvincibility();
            }
        }

        private void StartInvincibility()
        {
            if (_config == null) return;

            _isInvincible = true;
            _invincibilityTimer = _config.InvincibilityDuration;

            // Visual feedback: flash white
            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.white;
        }

        public void Kill()
        {
            _currentHealth = 0;
            EventBus.Publish(new PlayerDeathEvent());
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
        }

        /// <summary>
        /// Called when player touches an enemy.
        /// </summary>
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.gameObject.GetComponent<Enemies.Enemy>();
                if (enemy != null && enemy.Data != null)
                {
                    TakeDamage(enemy.Data.ContactDamage * Time.deltaTime);
                }
            }
        }
    }
}
