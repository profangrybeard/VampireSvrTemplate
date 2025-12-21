using System;
using VampireSurvivor.Events;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Central event dispatcher for decoupled system communication.
    /// Uses strong-typed events to prevent runtime errors.
    /// </summary>
    public static class EventBus
    {
        // Enemy events
        public static event Action<EnemyKilledEvent> OnEnemyKilled;
        public static event Action<EnemySpawnedEvent> OnEnemySpawned;

        // Player events
        public static event Action<PlayerDamagedEvent> OnPlayerDamaged;
        public static event Action<PlayerDeathEvent> OnPlayerDeath;

        // Wave events
        public static event Action<WaveStartedEvent> OnWaveStarted;
        public static event Action<WaveCompletedEvent> OnWaveCompleted;

        // Game state events
        public static event Action<GameOverEvent> OnGameOver;
        public static event Action<GamePausedEvent> OnGamePaused;

        // Publish methods with null-safety
        public static void Publish(EnemyKilledEvent evt) => OnEnemyKilled?.Invoke(evt);
        public static void Publish(EnemySpawnedEvent evt) => OnEnemySpawned?.Invoke(evt);
        public static void Publish(PlayerDamagedEvent evt) => OnPlayerDamaged?.Invoke(evt);
        public static void Publish(PlayerDeathEvent evt) => OnPlayerDeath?.Invoke(evt);
        public static void Publish(WaveStartedEvent evt) => OnWaveStarted?.Invoke(evt);
        public static void Publish(WaveCompletedEvent evt) => OnWaveCompleted?.Invoke(evt);
        public static void Publish(GameOverEvent evt) => OnGameOver?.Invoke(evt);
        public static void Publish(GamePausedEvent evt) => OnGamePaused?.Invoke(evt);

        /// <summary>
        /// Clear all subscriptions. Call on scene reload to prevent memory leaks.
        /// </summary>
        public static void ClearAll()
        {
            OnEnemyKilled = null;
            OnEnemySpawned = null;
            OnPlayerDamaged = null;
            OnPlayerDeath = null;
            OnWaveStarted = null;
            OnWaveCompleted = null;
            OnGameOver = null;
            OnGamePaused = null;
        }
    }
}
