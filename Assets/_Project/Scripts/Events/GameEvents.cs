using UnityEngine;
using VampireSurvivor.Data;

namespace VampireSurvivor.Events
{
    // Readonly struct events for zero-allocation event passing.

    public readonly struct EnemyKilledEvent
    {
        public readonly Vector2 Position;
        public readonly int XPValue;
        public readonly EnemyData EnemyType;

        public EnemyKilledEvent(Vector2 position, int xpValue, EnemyData enemyType)
        {
            Position = position;
            XPValue = xpValue;
            EnemyType = enemyType;
        }
    }

    public readonly struct EnemySpawnedEvent
    {
        public readonly Vector2 Position;
        public readonly EnemyData EnemyType;

        public EnemySpawnedEvent(Vector2 position, EnemyData enemyType)
        {
            Position = position;
            EnemyType = enemyType;
        }
    }

    public readonly struct PlayerDamagedEvent
    {
        public readonly float Damage;
        public readonly float CurrentHealth;
        public readonly float MaxHealth;

        public PlayerDamagedEvent(float damage, float currentHealth, float maxHealth)
        {
            Damage = damage;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }

    public readonly struct PlayerDeathEvent { }

    public readonly struct WaveStartedEvent
    {
        public readonly int WaveNumber;
        public WaveStartedEvent(int waveNumber) => WaveNumber = waveNumber;
    }

    public readonly struct WaveCompletedEvent
    {
        public readonly int WaveNumber;
        public WaveCompletedEvent(int waveNumber) => WaveNumber = waveNumber;
    }

    public readonly struct GameOverEvent { }

    public readonly struct GamePausedEvent
    {
        public readonly bool IsPaused;
        public GamePausedEvent(bool isPaused) => IsPaused = isPaused;
    }
}
