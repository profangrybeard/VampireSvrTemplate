using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Data;
using VampireSurvivor.Events;
using VampireSurvivor.Interfaces;

namespace VampireSurvivor.Entities.Player
{
    /// <summary>
    /// Manages player health, damage, and death.
    /// Uses continuous damage model (DPS) for enemy contact.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable, IKillable
    {
        [SerializeField] private GameConfig _config;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _config != null ? _config.PlayerMaxHealth : 100f;
        public float HealthPercent => _currentHealth / MaxHealth;
        public bool IsAlive => _currentHealth > 0;

        private void Start()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);

            EventBus.Publish(new PlayerDamagedEvent(damage, _currentHealth, MaxHealth));

            if (_currentHealth <= 0)
            {
                Kill();
            }
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
        /// Called every physics frame while player touches an enemy.
        /// ContactDamage is treated as damage per second (DPS).
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
